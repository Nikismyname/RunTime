#region INIT
using System;
using System.Collections;
using UnityEngine;

public class PlayerHandling2 : MonoBehaviour
{
    private bool active;
    private Camera mainCamera;
    private Main ms;
    private float speed;
    private Rigidbody rb;
    private bool grounded;
    private bool isJumpInProgress = false;
    private Vector3 previousVelocity;

    private float jumpHeight = 1200;
    private float gravity = 100;
    private float jumpExtraGravity = 100f;
    private float climbExtraGravity = 50;

    private bool collidingWithGround = true;
    private bool collidingWithWall = true;
    private bool wallClimbingInProgress = false;
    private bool wallJumpLock = false; 
    private Vector3? climbJumpNormal;

    private GameObject lastTouchedWall;
    private GameObject lastJumpedOffWall;

    private Vector3 lastWallCollisionNormal = Vector3.zero; 

    ///VelocitySpecifics 
    private float forceToVelocity = 0.05f;

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

    #region UPDATE
    private void Update()
    {
        if (this.active)
        {
            ///Simulation The Gravity;
            if (this.grounded == false)
            {
                this.rb.velocity -= new Vector3(0, this.gravity * Time.deltaTime, 0);
            }
            ///...
            
            this.MoveRigidBodyMovePosition();
            this.Jump();
        }
    }
    #endregion

    #region MOVEMENT
    private void MoveRigidBodyMovePosition()
    {
        if (this.wallClimbingInProgress)
        {
            return; 
        }

        ///Input.GetAxis is incremental GetAxisRaw switches from 0 to 1;
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        float speedDown = 1;

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            this.previousVelocity = Vector3.zero;

            //if (this.isJumpInProgress == false && this.grounded == true)
            //{
            //    this.rb.velocity = Vector3.zero;
            //    this.rb.angularVelocity = Vector3.zero;
            //}

            return;
        }

        ///letting the player move the character after jump;
        //if (this.isJumpInProgress)
        //{
        //    var vel = rb.velocity;
        //    vel.x = 0;
        //    vel.z = 0;
        //    rb.velocity = vel;
        //}

        var cameraForward = this.mainCamera.transform.forward.normalized;
        var forward2D = new Vector2(cameraForward.x, cameraForward.z);
        forward2D.Normalize();
        var forward3D = new Vector3(forward2D.x, 0, forward2D.y);
        var sideways3d = Quaternion.AngleAxis(-90, Vector3.up) * forward3D;

        var offset = (moveHorizontal * sideways3d * -1 + moveVertical * forward3D).normalized * speed * Time.deltaTime * speedDown;

        this.rb.MovePosition(rb.position + offset);
        this.rb.MoveRotation(Quaternion.LookRotation(-offset));

        this.previousVelocity = offset;
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /// Wall Jump
            if (this.wallClimbingInProgress && this.wallJumpLock == false)
            {
                if (this.climbJumpNormal == null)
                {
                    Debug.Log("No Jump Normal!");
                    return;
                }

                this.rb.velocity = Vector3.zero;
                this.rb.velocity += Vector3.up * this.ms.jumpForce * this.ms.forceToVelocity;
                this.rb.velocity += this.climbJumpNormal.Value * this.ms.jumpForce * 0.5f * this.ms.forceToVelocity;
                this.wallClimbingInProgress = false;
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

        if(this.lastOnGroundExit == null || this.lastOnGroundExit.Value < this.lastJumpTime.Value)
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
    private IEnumerator WallJumpTimeOut()
    {
        yield return new WaitForSeconds(0.2f);
        this.wallJumpLock = false;
    }
    #endregion

    #region COLLISIONS
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            this.GroundPlayer();
        }
        else if(col.gameObject.tag == "Wall")
        {
            var normal = col.GetContact(0).normal;
            if (normal == new Vector3(0f, 1f, 0f))
            {
                ///the up sides of a wall is considered ground;
                this.GroundPlayer();
                return;
            }

            ///Avoiding collistios to the very edge of the walls preventing endless bouncing 
            if(gameObject.transform.position.y > col.gameObject.transform.localScale.y / 2)
            {
                return;
            } 

            gameObject.GetComponent<Renderer>().material.color = Color.red;

            this.collidingWithWall = true;
            ///If the the collision is happanse from walking into the wall ignore;
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
            ///We are on top of wall, acting as ground
            if (this.lastWallCollisionNormal == new Vector3(0f, 1f, 0f))
            {
                this.DeGroundPlayer();
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = Color.white;
                this.collidingWithWall = false;
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
        const float UpCheckModifier = 0.3f;
        var velocity = rb.velocity;
        var projectedVelocity = Vector3.ProjectOnPlane(velocity, normal); // works
        var rotatedProjectedVelocity = Quaternion.AngleAxis(yRotation, Vector3.up) * projectedVelocity;

        Vector3 jumpDirection = Vector3.zero;
        //Quaternion desiredRotation = Quaternion.AngleAxis(45, normal);
        //Quaternion currentRotation = Quaternion.LookRotation(projectedVelocity);
        jumpDirection = /*desiredRotation **/ projectedVelocity.normalized;
        //jumpDirection = (desiredRotation * currentRotation) * projectedVelocity.normalized;
        var leftFromNormal = Quaternion.AngleAxis(90, Vector3.up) * normal;
        var angle = Vector3.Angle(leftFromNormal, jumpDirection);

        if(rotatedProjectedVelocity.x < 0)
        {
            jumpDirection = Quaternion.AngleAxis(-angle + 135, normal) * jumpDirection;
        }
        else
        {
            jumpDirection = Quaternion.AngleAxis(-angle + 45, normal) * jumpDirection;
        }

        var angle2 = Vector3.Angle(leftFromNormal, jumpDirection);
        Debug.Log("Angle from right: " + angle2);

        Debug.DrawLine(rb.transform.position, rb.transform.position + jumpDirection * 4, Color.red, 4);

        //var go = new GameObject();
        //go.transform.rotation = currentRotation;


        //if (rotatedProjectedVelocity.x > 0)
        //{
        //    jumpDirection = (desiredRotation * currentRotation) * projectedVelocity.normalized; 
        //}
        //else
        //{
        //    jumpDirection = (desiredRotation * currentRotation) * projectedVelocity.normalized;
        //}

        if (rotatedProjectedVelocity.y < 0)
        {
            //Debug.Log("DOWN");
        }
        else
        {
            if (rotatedProjectedVelocity.y * UpCheckModifier > Math.Abs(rotatedProjectedVelocity.x))
            {
                Debug.Log("UP");
            }
            else
            {
                rb.velocity = jumpDirection * this.ms.jumpForce * this.ms.forceToVelocity;

                //Debug.DrawLine(rb.transform.position, rb.transform.position + jumpDirection * 4, Color.green,4);
                //Debug.Log(jumpDirection);

                if (rotatedProjectedVelocity.x < 0)
                {
                    Debug.Log("RIGHT");
                    
                }
                else if (rotatedProjectedVelocity.x > 0)
                {
                    Debug.Log("LEFT");
                }
            }
        }

        /////Reducing the gravity
        //this.gravity = this.climbExtraGravity;
        //rb.velocity = Vector3.up * this.ms.jumpForce * this.ms.forceToVelocity;
        /////Saving this so when another the player presses jump we know what direction to send him in;
        //this.climbJumpNormal = normal;
        //this.wallClimbingInProgress = true;
        /////This is a system where wall the player does not get automatically jumped of a wall 
        /////if he holds the jump key; this should be raplaced with system where holding the space does not 
        /////trigger it at all
        //this.wallJumpLock = true;
        //StartCoroutine(nameof(this.WallJumpTimeOut));
        /////...
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

    #region END_BRACKET
}
#endregion

#region OLD_CODE
//private void MoveRigidBodySetVelocity()
//{
//    float moveHorizontal = Input.GetAxis("Horizontal");
//    float moveVertical = Input.GetAxis("Vertical");

//    if (moveHorizontal == 0 && moveVertical == 0)
//    {
//        return;
//    }

//    if (this.grounded == false)
//    {
//        return;
//    }

//    var cameraForward = this.mainCamera.transform.forward.normalized;
//    var forward2D = new Vector2(cameraForward.x, cameraForward.z);
//    forward2D.Normalize();
//    var forward3D = new Vector3(forward2D.x, 0, forward2D.y);
//    var sideways3d = Quaternion.AngleAxis(-90, Vector3.up) * forward3D;

//    var force = (moveHorizontal * sideways3d * -1 + moveVertical * forward3D) * speed * Time.deltaTime;
//    this.rb.AddForce(force, ForceMode.VelocityChange); //if other forces are at play too;
//    //this.rb.velocity = (moveHorizontal * sideways3d * -1 + moveVertical * forward3D) * speed * Time.deltaTime * 100;
//}

//private void MoveTransform()
//{
//    if (Input.GetKey(KeyCode.W))
//    {
//        var direction = this.GenerateNormalisedForward();
//        transform.position += direction * speed * Time.deltaTime;
//        transform.rotation = Quaternion.LookRotation(-direction);
//    }

//    if (Input.GetKey(KeyCode.S))
//    {
//        var direction = this.GenerateNormalisedForward();
//        transform.position -= direction * speed * Time.deltaTime;
//        transform.rotation = Quaternion.LookRotation(direction);
//    }


//    if (Input.GetKey(KeyCode.D))
//    {
//        var direction = this.GenerateNormalisedForward(true);
//        transform.position -= direction * speed * Time.deltaTime;
//        transform.rotation = Quaternion.LookRotation(direction);
//    }

//    if (Input.GetKey(KeyCode.A))
//    {
//        var direction = this.GenerateNormalisedForward(true);
//        transform.position += direction * speed * Time.deltaTime;
//        transform.rotation = Quaternion.LookRotation(-direction);
//    }
//}

//private void MoveRigidBodyMovePositionFull()
//{
//    if (Input.GetKey(KeyCode.W))
//    {
//        var direction = this.GenerateNormalisedForwardRigidBody();
//        var offset = direction * speed * Time.deltaTime;
//        this.rb.MovePosition(rb.position + offset);
//        this.rb.MoveRotation(Quaternion.LookRotation(-direction));
//    }

//    if (Input.GetKey(KeyCode.S))
//    {
//        var direction = this.GenerateNormalisedForwardRigidBody();
//        var offset = -direction * speed * Time.deltaTime;
//        this.rb.MovePosition(rb.position + offset);
//        this.rb.MoveRotation(Quaternion.LookRotation(direction));
//    }

//    if (Input.GetKey(KeyCode.D))
//    {
//        var direction = this.GenerateNormalisedForwardRigidBody(true);
//        var offset = -direction * speed * Time.deltaTime;
//        this.rb.MovePosition(rb.position + offset);
//        this.rb.MoveRotation(Quaternion.LookRotation(direction));
//    }

//    if (Input.GetKey(KeyCode.A))
//    {
//        var direction = this.GenerateNormalisedForwardRigidBody(true);
//        var offset = direction * speed * Time.deltaTime;
//        this.rb.MovePosition(rb.position + offset);
//        this.rb.MoveRotation(Quaternion.LookRotation(-direction));
//    }
//}
#endregion
