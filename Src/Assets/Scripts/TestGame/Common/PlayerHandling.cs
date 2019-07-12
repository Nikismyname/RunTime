#region INIT
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

    //private float jumpHeight = 1100f;
    //private float bonusGravity = 60f;

    private void Start()
    {
        this.active = true;
        this.ms = GameObject.Find("Main").GetComponent<Main>();
        this.speed = ms.playerSpeed;
        this.mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        this.rb = gameObject.GetComponent<Rigidbody>();
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
        if (this.active)
        {
            this.MoveRigidBodyMovePosition();
            this.Jump();
        }
    }
    #endregion

    #region MOVEMENT
    private void MoveRigidBodyMovePosition()
    {
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
        ///Increasing the gravity for smapier falls 
        this.rb.velocity -= new Vector3(0, this.ms.extraGravity * Time.deltaTime, 0);

        if (this.isJumpInProgress == false)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                this.isJumpInProgress = true;
                rb.AddForce(Vector3.up * this.ms.jumpForce);
                rb.AddForce(this.previousVelocity.normalized * 500);
            }
        }
    }
    #endregion

    #region COLLISIONS
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            this.grounded = true;
            this.isJumpInProgress = false;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            this.grounded = false;
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
