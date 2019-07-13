#region INIT
using System;
using System.Collections;
using UnityEngine;

public class PlayerHandling : MonoBehaviour
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
    private float extraGravity = 100;
    private float jumpExtraGravity = 100f;
    private float climbExtraGravity = 50;

    private bool collidingWithGround = true;
    private bool collidingWithWall = true;
    private bool wallClimbingInProgress = false;

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
        if (this.active)
        {
            ///Increasing the gravity for snapier falls 
            this.rb.velocity -= new Vector3(0, this.extraGravity * Time.deltaTime, 0);

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

            if (this.isJumpInProgress == false && this.grounded == true)
            {
                this.rb.velocity = Vector3.zero;
                this.rb.angularVelocity = Vector3.zero;
            }

            return;
        }

        if (this.grounded == false)
        {
            speedDown = 0.4f;
        }

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
        if (this.isJumpInProgress == false)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log("JUMP ACTIVATION");
                this.isJumpInProgress = true;
                rb.AddForce(Vector3.up * this.ms.jumpForce);
                rb.AddForce(this.previousVelocity.normalized * 500);
                this.lastJumpTime = DateTime.Now;
                this.StartCoroutine("JumpCoroutine");
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

    #endregion

    #region COLLISIONS
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            ///Debug.Log("Grouns Collision Enter"); activates notmaly next to a wall
            this.grounded = true;
            this.isJumpInProgress = false;
            this.collidingWithGround = true;
            this.wallClimbingInProgress = false;
            this.extraGravity = this.jumpExtraGravity;
        }
        else if(other.gameObject.tag == "Wall")
        {
            this.GetDirection(other);
            
            this.collidingWithWall = true;
            if(rb.velocity.magnitude == 0)
            {
                Debug.Log("Just Wall Collision");
                return;
            }

            this.extraGravity = this.climbExtraGravity;
            var vel = rb.velocity;
            this.wallClimbingInProgress = true;

            //if(vel.y < 0) //TODO: TEMP
            //{
            //    vel.y = -vel.y;
            //}

            rb.AddForce(Vector3.up * this.ms.jumpForce);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            /// Debug.Log("Ground Collision Exit"); activates normaly next to a wall
            this.grounded = false;
            this.lastOnGroundExit = DateTime.Now;
            this.collidingWithGround = false;
        }
        else if (other.gameObject.tag == "Wall")
        {
            this.collidingWithWall = false;
        }
    }

    private void GetDirection(Collision col)
    {
        Vector3 hit = col.contacts[0].normal;
        float angle = Vector3.Angle(hit, Vector3.up);

        if (Mathf.Approximately(angle, 0))
        {
            //Down
            Debug.Log("Down");
        }
        if (Mathf.Approximately(angle, 180))
        {
            //Up
            Debug.Log("Up");
        }
        if (Mathf.Approximately(angle, 90))
        {
            // Sides
            Vector3 cross = Vector3.Cross(Vector3.forward, hit);
            if (cross.y > 0)
            { // left side of the player
                Debug.Log("Left");
            }
            else
            { // right side of the player
                Debug.Log("Right");
            }
        }
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
