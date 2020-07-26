#region INIT

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandling2 : MonoBehaviour
{
    private bool active;
    private Camera mainCamera;
    private TargetManagerBehaviour ms;
    private float speed;
    private Rigidbody rb;
    private float gravity = 100;
    private float noGravity = 0;
    private float jumpExtraGravity = 100f;
    private float climbExtraGravity = 50;
    private bool grounded;
    private bool isJumpInProgress = false;
    private Vector3 previousVelocity;
    private bool collidingWithWall = false;
    private bool wallClimbingInProgress = false;
    private Vector3 lastWallCollisionNormal = Vector3.zero;
    private Vector3 velocityBeforeCollision = Vector3.zero;
    private WallCollisionStatusManager collisionManager = new WallCollisionStatusManager();
    private int maxActionPoints = 3;
    private int actionPoints = 3;
    private float? firstPressOfW = null;
    private float doublePressInterval = 0.5f;
    private Image overlayImage;
    private Image image;
    private Text text;
    private Color imageOriginalColor;
    private float actionPointRechargeTime = 15f;
    private bool recharging = false;
    private delegate void RechargeDone();
    private event RechargeDone ActionPointRecharged;
    private int tapsCount = 0;
    private int tapsForActionPoint = 3;
    private bool holdingWall = false;
    private KeyCode rechargeWhileHangingKeyCode = KeyCode.V;

    private void Start()
    {
        //Debug.Log("Player Handling2");

        this.active = true;
        this.ms = GameObject.Find("Main").GetComponent<TargetManagerBehaviour>();
        this.speed = ReferenceBuffer.Instance.EditorInput.playerSpeed;
        this.mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        this.rb = gameObject.GetComponent<Rigidbody>();
        var actionPoints = GameObject.Find("ActionPoints");

        this.overlayImage = actionPoints.transform.Find("Image").GetComponent<Image>();
        this.image = actionPoints.GetComponent<Image>();
        this.text = actionPoints.GetComponentInChildren<Text>();

        this.ActionPointRecharged += TriggerActionPointRecharge;
        this.text.text = this.maxActionPoints.ToString();

        this.imageOriginalColor = image.color;

        //this.GroundPlayer();
    }

    #endregion

    #region ACTION_POINTS_DISPLAY_VISUALS

    private void APDFlash(int timeInMS, Color color)
    {
        this.image.color = color;
        this.APDEndFlash(timeInMS);
    }

    private async void APDEndFlash(int time)
    {
        await Task.Delay(time);
        this.image.color = this.imageOriginalColor;
    }

    #endregion

    #region V_ACTION_POINTS_RECHARGE

    public void TapSpaceToRechargePoint()
    {
        if (Input.GetKeyDown(this.rechargeWhileHangingKeyCode))
        {
            if (this.holdingWall)
            {
                this.tapsCount++;
                if (this.tapsCount == this.tapsForActionPoint && this.actionPoints < this.maxActionPoints)
                {
                    this.actionPoints++;
                    this.text.text = this.actionPoints.ToString();
                    this.APDFlash(300, Color.green);
                    this.tapsCount = 0;
                }
            }
        }
    }

    #endregion

    #region ACTION_POINT_RECHARGE

    private void TriggerActionPointRecharge()
    {
        if (this.actionPoints < this.maxActionPoints && this.recharging == false)
        {
            this.RechargeActionPointsAsync();
        }
        else
        {
            this.overlayImage.fillAmount = 0.0f;
        }
    }

    private async void RechargeActionPointsAsync()
    {
        this.recharging = true;

        const int stepsPerSecond = 5;
        var steps = actionPointRechargeTime * stepsPerSecond;

        for (int i = 0; i < steps; i++)
        {
            if (this.actionPoints == this.maxActionPoints)
            {
                this.overlayImage.fillAmount = 0.0f;
                return;
            }

            var fraction = i / steps;
            overlayImage.fillAmount = fraction;
            await Task.Delay(1000 / stepsPerSecond);
        }

        this.actionPoints++;
        this.text.text = this.actionPoints.ToString();
        this.recharging = false;
        ActionPointRecharged.Invoke();
    }

    #endregion

    #region ON_GUI 

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), this.rb.velocity.ToString());
    }

    #endregion

    #region VEHICLE
    public void EnterVehicle()
    {
        this.active = false;
        this.gameObject.SetActive(false);
    }

    public void ExitVehicle(Vector3 newPosition)
    {
        this.active = true;
        this.gameObject.SetActive(true);
        this.gameObject.transform.position = newPosition;
        this.grounded = false;
    }
    #endregion

    #region HOLD_WALL_DC_W 

    private void DoubleClickWAction()
    {
        if (this.holdingWall)
        {
            this.DoubleClickWUnfreeze("W->W");
        }
        else if (wallClimbingInProgress)
        {
            this.rb.velocity = Vector3.zero;
            this.gravity = this.noGravity;
            this.holdingWall = true;
            Debug.Log("Double Click W");
        }
    }

    private void DoubleClickWUnfreeze(string id = null)
    {
        if (id != null)
        {
            //Debug.Log(id);
        }

        this.holdingWall = false; ;
        this.gravity = this.climbExtraGravity;
    }

    private void DoubleClickWCheck()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            var time = Time.time;
            if (this.firstPressOfW == null)
            {
                this.firstPressOfW = time;
            }
            else
            {
                var deltaT = Math.Abs(this.firstPressOfW.Value - time);
                if (deltaT <= this.doublePressInterval)
                {
                    this.DoubleClickWAction();
                    this.firstPressOfW = null;
                }
                else
                {
                    this.firstPressOfW = time;
                }
            }
        }
    }

    #endregion

    #region UPDATE

    private void Update()
    {
        if (this.active == false) { return; }

        this.DoubleClickWCheck();

        this.SimulateGravity();

        this.Move();

        this.Jump();

        this.TapSpaceToRechargePoint();
    }

    private void FixedUpdate()
    {
        this.velocityBeforeCollision = this.rb.velocity;
    }

    #endregion

    #region MOVEMENT

    private void Move()
    {
        /// Not responding to movement commands while wall climbing  
        /// TODO: What about when jumping over a wall?
        if (this.wallClimbingInProgress)
        {
            return;
        }

        ///Input.GetAxis is incremental GetAxisRaw switches from 0 to 1
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        float speedDown = 1;

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            this.previousVelocity = Vector3.zero;
            return;
        }

        this.DoubleClickWUnfreeze("Move");

        Vector3 cameraForward = this.mainCamera.transform.forward.normalized;
        Vector3 forward2D = new Vector2(cameraForward.x, cameraForward.z).normalized;
        Vector3 forward3D = new Vector3(forward2D.x, 0, forward2D.y);
        Vector3 sideways3D = Quaternion.AngleAxis(-90, Vector3.up) * forward3D;
        Vector3 moveVector = ( - moveHorizontal * sideways3D + moveVertical * forward3D);
        float moveMultiplyer = speed * Time.deltaTime * speedDown; 
        Vector3 offset = moveVector.normalized * moveMultiplyer;
        this.rb.MovePosition(rb.position + offset);
        this.rb.MoveRotation(Quaternion.LookRotation(-offset));
        this.previousVelocity = offset;
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var normal = this.collisionManager.GetCurrenNormal();
            ///Wall Jump
            if (this.wallClimbingInProgress)
            {
                ///We do not have the normal of the wall we are climbing
                if (normal == null)
                {
                    Debug.Log("No Jump Normal!");
                    return;
                }

                ///You can only jump off a wall
                if (this.collidingWithWall == false)
                {
                    return;
                }

                var velocity = Vector3.zero;
                var directionCount = 0;
                var isDirectionalJump = false;

                var n = Util.GetNormalDerivatives(normal.Value, 45);

                if (Input.GetKey(KeyCode.W)) ///X UP
                {
                    directionCount++;
                    var upVelocity = n.Up * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    velocity += upVelocity;
                    isDirectionalJump = true;
                }
                if (Input.GetKey(KeyCode.S)) ///X DOWN
                {
                    directionCount++;
                    velocity += Vector3.up * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    velocity += n.Normal * ReferenceBuffer.Instance.EditorInput.jumpForce * 0.5f * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    isDirectionalJump = true;
                }
                if (Input.GetKey(KeyCode.A)) /// LEFT
                {
                    directionCount++;
                    var leftVelocity = n.LeftXDegrees * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    isDirectionalJump = true;
                    velocity += leftVelocity;
                }
                if (Input.GetKey(KeyCode.D))/// RIGHT
                {
                    directionCount++;
                    var rightVelocity = n.RightXDegrees * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    isDirectionalJump = true;
                    velocity += rightVelocity;
                }

                if (isDirectionalJump)
                {
                    if (this.actionPoints > 0)
                    {
                        this.actionPoints--;
                        this.text.text = this.actionPoints.ToString();
                        this.TriggerActionPointRecharge();
                        this.rb.velocity = velocity / directionCount;
                        this.DoubleClickWUnfreeze("Wall Jump");
                    }
                    else
                    {
                        this.APDFlash(300, Color.red);
                    }
                }
            }
            /// Regular Jump
            else if (this.isJumpInProgress == false)
            {
                if (this.collidingWithWall)
                {
                    ///TODO: Add directions
                    var resultJumpDirection = Vector3.up;
                    rb.velocity = resultJumpDirection * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    this.gravity = this.climbExtraGravity;
                    this.wallClimbingInProgress = true;
                }
                else
                {
                    this.isJumpInProgress = true;
                    this.wallClimbingInProgress = false;
                    this.grounded = false;
                    rb.velocity += Vector3.up * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    rb.velocity += this.previousVelocity.normalized * 500 * ReferenceBuffer.Instance.EditorInput.forceToVelocity;
                    this.DoubleClickWUnfreeze("Regular Jump");
                    ///This system resets the jump capability after a jump that did not get off the ground.
                    //this.lastJumpTime = DateTime.Now;
                    //this.StartCoroutine(nameof(this.JumpCoroutine));
                }
            }
            else
            {
                Debug.Log("isJumpInProgress = true");
            }
        }
    }

    private void SimulateGravity()
    {
        if (this.grounded == false)
        {
            this.rb.velocity -= new Vector3(0, this.gravity * Time.deltaTime, 0);
        }
    }

    #endregion

    #region COLLISIONS
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            this.GroundPlayer();
        }
        else if (col.gameObject.tag == "Wall")
        {
            var normal = col.GetContact(0).normal;
            var name = col.gameObject.name;

            if (normal == new Vector3(0f, 1f, 0f))
            {
                Debug.Log("Wall Up!");
                ///the up sides of a wall is considered ground;
                this.GroundPlayer();
                return;
            }

            ///We are moving from wall to wall
            if (collisionManager.IsColliding())
            {
                Debug.Log("Moving frow wall to wall!");
                var prevNormal = collisionManager.GetCurrenNormal().Value;
                ///ASSUMPTION: the walls are verticle 
                var angle = Vector3.SignedAngle(normal, prevNormal, Vector3.up);
                var otherAngle = Quaternion.FromToRotation(normal, prevNormal).eulerAngles.y;
                var alteredVelocity = (Quaternion.AngleAxis(-angle, Vector3.up) * this.velocityBeforeCollision);
                var nv = Util.GetNormalDerivatives(normal, 45);
                this.rb.velocity = alteredVelocity;
                this.collisionManager.RegisterCollisionEnter(name, normal, "Wall");
                return;
            }

            this.collisionManager.RegisterCollisionEnter(name, normal, "Wall");

            ///Avoiding collistios to the very edge of the walls preventing endless bouncing 
            if (gameObject.transform.position.y > col.gameObject.transform.localScale.y / 2)
            {
                Debug.Log("Collision with endge of the wall!");
                return;
            }

            ///visualising touching the wall
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            //Debug.Log($"Collision Enter on wall {name}");

            this.collidingWithWall = true;
            if (this.isJumpInProgress == false)
            {
                return;
            }

            Debug.Log("Jump from the wall!");
            this.JumpBounseFromWall(normal, col.gameObject.transform.rotation.y);
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            this.DeGroundPlayer();
        }
        else if (col.gameObject.tag == "Wall")
        {
            /// We are exiting top of wall, acting as ground 
            if (this.lastWallCollisionNormal == new Vector3(0f, 1f, 0f))
            {
                this.DeGroundPlayer();
            }
            /// Exiting actuall wall
            else
            {
                var name = col.gameObject.name;

                this.collisionManager.RegisterCollisionExit(name);

                if (this.collisionManager.IsColliding() == false)
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.white;

                    this.collidingWithWall = false;
                    this.wallClimbingInProgress = false;
                }
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Wall")
        {
            this.lastWallCollisionNormal = col.GetContact(0).normal;
        }
    }

    private void DeGroundPlayer()
    {
        this.grounded = false;
        //this.lastOnGroundExit = DateTime.Now;
        //this.collidingWithGround = false;
    }

    private void JumpBounseFromWall(Vector3 normal, float yRotation)
    {
        var nv = Util.GetNormalDerivatives(normal, 45);

        var velocity = rb.velocity;
        var projectedVelocity = Vector3.ProjectOnPlane(velocity, normal);

        var pvNorm = projectedVelocity.normalized;

        var angleFromLeft = Vector3.Angle(nv.Left, pvNorm);
        var angleFromRight = Vector3.Angle(nv.Right, pvNorm);

        var diff = (angleFromLeft - angleFromRight);

        Vector3 resultJumpDirection = Vector3.zero;
        if (Math.Abs(diff) > 20)
        {
            if (diff < 0)
            {
                resultJumpDirection = nv.LeftXDegrees;
            }
            else
            {
                resultJumpDirection = nv.RightXDegrees;
            }
        }
        else
        {
            resultJumpDirection = Vector3.up;
        }

        rb.velocity = resultJumpDirection * ReferenceBuffer.Instance.EditorInput.jumpForce * ReferenceBuffer.Instance.EditorInput.forceToVelocity;

        this.gravity = this.climbExtraGravity;
        this.wallClimbingInProgress = true;
    }

    private void GroundPlayer()
    {
        if (this.rb != null)
        {
            ///Reset vertival Velocity; 
            var vel = this.rb.velocity;
            vel.y = 0;
            this.rb.velocity = vel;
            ///...
        }

        this.grounded = true;
        this.isJumpInProgress = false;
        //this.collidingWithGround = true;
        this.wallClimbingInProgress = false;
        this.gravity = this.jumpExtraGravity;
    }
    #endregion

    #region GENERATE_FORWARD
    private Vector3 GenerateNormalisedForward(bool side = false)
    {
        var forward = this.mainCamera.transform.forward.normalized;
        var vectorTwo = new Vector2(forward.x, forward.z);
        vectorTwo.Normalize();
        forward.Set(vectorTwo.x, 0, vectorTwo.y);

        if (side)
        {
            forward = Quaternion.AngleAxis(-90, Vector3.up) * forward;
        }

        return forward;
    }

    private Vector3 GenerateNormalisedForwardRigidBody(bool side = false)
    {
        var forward = this.mainCamera.transform.forward.normalized;
        var vectorTwo = new Vector2(forward.x, forward.z);
        vectorTwo.Normalize();
        forward.Set(vectorTwo.x, 0, vectorTwo.y);

        if (side)
        {
            forward = Quaternion.AngleAxis(-90, Vector3.up) * forward;
        }

        return forward;
    }
    #endregion

    #region EXPOSED_VARIABLES
    public Vector3 GetCurrentPosition => this.gameObject.transform.position;
    #endregion

    #region HELPERS
    #endregion

    #region }
}
#endregion
