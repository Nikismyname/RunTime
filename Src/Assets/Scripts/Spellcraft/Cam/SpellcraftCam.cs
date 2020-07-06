using UnityEngine;

public class SpellcraftCam : MonoBehaviour
{
    public Transform target;
    private Vector3 targetOffset = Vector3.zero;
    private float distance = 20.0f;
    private float maxDistance = 50;
    private float minDistance = .6f;
    private float xSpeed = 200.0f;
    private float ySpeed = 200.0f;
    private int zoomRate = 40;
    private float panSpeed = 0.3f;
    private float zoomDampening = 5.0f;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private bool rotate;

    void Start()
    {
        Init();
    }

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (this.rotate == false)
            {
                this.rotate = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (this.rotate == true)
            {
                this.rotate = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }
        else if (Input.GetMouseButton(2))
        {
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }

        if (this.rotate)
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, 1);
            transform.rotation = rotation;
        }

        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;

        Input.mousePosition.Set(0, 0, 0);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
