using UnityEngine;

public class PlayerHandling : MonoBehaviour
{
    private bool active;
    private Camera mainCamera;
    private Main ms;
    private float speed;

    private void Start()
    {
        this.active = true;
        this.ms = GameObject.Find("Main").GetComponent<Main>();
        this.speed = ms.playerSpeed;
        this.mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

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

    private void Update()
    {
        if (this.active)
        {
            if (Input.GetKey(KeyCode.W))
            {
                var direction = this.GenerateNormalisedForward();
                transform.position += direction * speed / Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(-direction);
            }

            if (Input.GetKey(KeyCode.S))
            {
                var direction = this.GenerateNormalisedForward();
                transform.position -= direction * speed / Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction);
            }


            if (Input.GetKey(KeyCode.D))
            {
                var direction = this.GenerateNormalisedForward(true);
                transform.position -= direction * speed / Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (Input.GetKey(KeyCode.A))
            {
                var direction = this.GenerateNormalisedForward(true);
                transform.position += direction * speed / Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
    }

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
        //Debug.Log("M " + forward.magnitude);
        return forward;
    }

    public Vector3 GetCurrentPosition => this.gameObject.transform.position;
}
