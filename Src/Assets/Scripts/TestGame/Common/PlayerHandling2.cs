#region INIT
using System;
using System.Collections;
using UnityEngine;

public class PlayerHandling2 : MonoBehaviour
{
    /// <summary>
    /// Means responds to movement commands. Not in a vahiche for example. 
    /// </summary>
    private bool active;
    private Camera mainCamera;
    private Main ms;
    private float speed;
    private Rigidbody rb;
    private float jumpHeight = 1200;
    private float gravity = 100;
    private float jumpExtraGravity = 100f;
    private float climbExtraGravity = 50;

    private bool grounded;
    private bool isJumpInProgress = false;
    private Vector3 previousVelocity;
    private bool collidingWithGround = true;
    private bool collidingWithWall = true;
    private bool wallClimbingInProgress = false;
    private Vector3? climbJumpNormal;
    private GameObject lastTouchedWall;
    private GameObject lastJumpedOffWall;
    /// <summary>
    /// Don't know about this too lazy to fix now 
    /// </summary>
    private Vector3 lastWallCollisionNormal = Vector3.zero;

    private float forceToVelocity = 0.05f;

    private Vector3 velocityBeforeCollision = Vector3.zero;

    private WallCollisionStatusManager collisionManager = new WallCollisionStatusManager();

    private void Start()
    {
        this.active = true;
        this.ms = GameObject.Find("Main").GetComponent<Main>();
        this.speed = ms.playerSpeed;
        this.mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        this.rb = gameObject.GetComponent<Rigidbody>();
    }

    private DateTime? lastJumpTime;
    private DateTime? lastOnGroundExit;
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
    }
    #endregion

    #region FIXED_UPDATE

    private void FixedUpdate()
    {
        this.velocityBeforeCollision = this.rb.velocity;
    }

    #endregion

    #region MOVEMENT

    private void Update()
    {
        if (this.active)
        {
            this.SimulateGravity();

            this.Move();

            this.Jump();
        }
    }

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

        var cameraForward = this.mainCamera.transform.forward.normalized;
        var forward2D = new Vector2(cameraForward.x, cameraForward.z);
        forward2D.Normalize();
        var forward3D = new Vector3(forward2D.x, 0, forward2D.y);
        var sideways3D = Quaternion.AngleAxis(-90, Vector3.up) * forward3D;

        var offset = (moveHorizontal * sideways3D * -1 + moveVertical * forward3D).normalized * speed * Time.deltaTime * speedDown;

        this.rb.MovePosition(rb.position + offset);
        this.rb.MoveRotation(Quaternion.LookRotation(-offset));

        this.previousVelocity = offset;
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ///Wall Jump
            if (this.wallClimbingInProgress)
            {
                ///We do not have the normal of the wall we are climbing
                if (this.climbJumpNormal == null)
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
                var n = Util.GetNormalDerivatives(this.climbJumpNormal.Value, 45);

                if (Input.GetKey(KeyCode.W)) ///X UP
                {
                    var upVelocity = n.Up * this.ms.jumpForce * this.ms.forceToVelocity;
                    velocity += upVelocity;
                }
                if (Input.GetKey(KeyCode.S)) ///X DOWN
                {
                    velocity += Vector3.up * this.ms.jumpForce * this.ms.forceToVelocity;
                    velocity += n.Normal * this.ms.jumpForce * 0.5f * this.ms.forceToVelocity;
                }
                if (Input.GetKey(KeyCode.A)) /// LEFT
                {
                    var leftVelocity = n.LeftXDegrees * this.ms.jumpForce * this.ms.forceToVelocity;
                    velocity += leftVelocity;
                }
                if (Input.GetKey(KeyCode.D))/// RIGHT
                {
                    var rightVelocity = n.RightXDegrees * this.ms.jumpForce * this.ms.forceToVelocity;
                    velocity += rightVelocity;
                }

                this.rb.velocity = velocity;
            }
            /// Regular Jump
            else if (this.isJumpInProgress == false)
            {
                this.isJumpInProgress = true;
                this.wallClimbingInProgress = false;
                this.grounded = false;
                rb.velocity += Vector3.up * this.ms.jumpForce * this.ms.forceToVelocity;
                rb.velocity += this.previousVelocity.normalized * 500 * this.ms.forceToVelocity;

                ///This system resets the jump capability after a jump that did not get off the ground.
                //this.lastJumpTime = DateTime.Now;
                //this.StartCoroutine(nameof(this.JumpCoroutine));
            }
        }
    }

    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        if (this.lastOnGroundExit == null || this.lastOnGroundExit.Value < this.lastJumpTime.Value)
        {
            this.grounded = true;
            this.isJumpInProgress = false;

            if (this.lastOnGroundExit == null)
            {
                Debug.Log("Player is at reset after jump! NULL");

            }
            else
            {
                if (this.lastOnGroundExit.Value < this.lastJumpTime.Value)
                {
                    Debug.Log("Player is at reset after jump! TIME");
                }
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

            ///We are moving from wall to wall
            if (collisionManager.IsColliding())
            {
                var prevNormal = collisionManager.GetPreviousNormal().Value;
                ///ASSUMPTION: the walls are verticle 
                var angle = Vector3.SignedAngle(normal, prevNormal, Vector3.up);
                var otherAngle = Quaternion.FromToRotation(normal, prevNormal).eulerAngles.y;
                Debug.Log(angle + " " + otherAngle);

                var alteredVelocity = (Quaternion.AngleAxis(-angle, Vector3.up) * this.velocityBeforeCollision);

                Debug.DrawLine(rb.transform.position, rb.transform.position + this.velocityBeforeCollision.normalized * 4, Color.green, 4);
                Debug.DrawLine(rb.transform.position, rb.transform.position + alteredVelocity.normalized * 4, Color.red, 4);

                var nv = Util.GetNormalDerivatives(normal, 45);

                this.rb.velocity = alteredVelocity;
                this.collisionManager.RegisterCollisionEnter(name, normal, "Wall");
                return;
            }

            this.collisionManager.RegisterCollisionEnter(name, normal, "Wall");

            if (normal == new Vector3(0f, 1f, 0f))
            {
                ///the up sides of a wall is considered ground;
                this.GroundPlayer();
                return;
            }

            ///Avoiding collistios to the very edge of the walls preventing endless bouncing 
            if (gameObject.transform.position.y > col.gameObject.transform.localScale.y / 2)
            {
                return;
            }

            ///visualising touching the wall
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            //Debug.Log($"Collision Enter on wall {name}");

            this.collidingWithWall = true;
            ///If the the collision is happened from walking into the wall ignore;
            if (this.isJumpInProgress == false)
            {
                return;
            }

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
                }
                this.collidingWithWall = false;
                this.wallClimbingInProgress = false;
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
        this.lastOnGroundExit = DateTime.Now;
        this.collidingWithGround = false;
    }

    private void JumpBounseFromWall(Vector3 normal, float yRotation)
    {
        var nv = Util.GetNormalDerivatives(normal, 45);

        var velocity = rb.velocity;
        var projectedVelocity = Vector3.ProjectOnPlane(velocity, normal); // works

        var pvNorm = projectedVelocity.normalized;

        float angleFromLeft = Vector3.Angle(nv.Left, pvNorm);
        float angleFromRight = Vector3.Angle(nv.Right, pvNorm);

        var diff = (angleFromLeft - angleFromRight);
        //Debug.Log($"{diff} {angleFromLeft} {angleFromRight}");

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
            Debug.Log("UP UP AND AWAY");
        }

        //Debug.DrawLine(rb.transform.position, rb.transform.position + normal.normalized * 4, Color.white, 4);
        //Debug.DrawLine(rb.transform.position, rb.transform.position + leftFromNormal.normalized * 4, Color.blue, 4);
        //Debug.DrawLine(rb.transform.position, rb.transform.position + upFromNormal.normalized * 4, Color.red, 4);
        //Debug.DrawLine(rb.transform.position, rb.transform.position + right45Degrees.normalized * 4, Color.blue, 4);
        //Debug.DrawLine(rb.transform.position, rb.transform.position + left45Degrees.normalized * 4, Color.red, 4);

        rb.velocity = resultJumpDirection * this.ms.jumpForce * this.ms.forceToVelocity;

        ///Reducing the gravity
        this.gravity = this.climbExtraGravity;
        ///Saving this so when another the player presses jump we know what direction to send him in;
        this.climbJumpNormal = normal;
        this.wallClimbingInProgress = true;
        ///This is a system where wall the player does not get automatically jumped of a wall 
        ///if he holds the jump key; this should be raplaced with system where holding the space does not 
        ///trigger it at all
        //this.wallJumpLock = true;
        //StartCoroutine(nameof(this.WallJumpTimeOut));
        ///...
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
        this.collidingWithGround = true;
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


#region TRASH_BIN
/// https://answers.unity.com/questions/163337/velocity-before-collision.html
//Vector3 ComputeIncidentVelocity(Rigidbody body, Collision collision, out Vector3 otherVelocity)
//{
//    Vector3 impulse = collision.impulse;
//    // Both participants of a collision see the same impulse, so we need to flip it for one of them.
//    if (Vector3.Dot(collision.GetContact(0).normal, impulse) < 0f)
//        impulse *= -1f;
//    otherVelocity = Vector3.zero;
//    // Static or kinematic colliders won't be affected by impulses.
//    var otherBody = collision.rigidbody;
//    if (otherBody != null)
//    {
//        otherVelocity = otherBody.velocity;
//        if (!otherBody.isKinematic)
//            otherVelocity += impulse / otherBody.mass;
//    }

//    var bodyVel = body.velocity;
//    var inpuls = impulse / body.mass;

//    Debug.DrawLine(rb.transform.position, rb.transform.position + bodyVel.normalized * 4, Color.green, 4);
//    Debug.DrawLine(rb.transform.position, rb.transform.position + inpuls.normalized * 5, Color.red, 4);

//    return bodyVel - inpuls;
//}
#endregion
