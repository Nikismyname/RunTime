using UnityEngine;

public class SpellcraftCam : MonoBehaviour
{
    private Transform target;
    private GameObject mainTarget;
    private Vector3 targetOffset = Vector3.zero;
    private float distance = 35.0f;
    private float maxDistance = 50;
    private float minDistance = .6f;
    private float xSpeed = 200.0f;
    private float ySpeed = 200.0f;
    private int zoomRate = 40;
    private float panSpeed = 0.3f;
    private float zoomDampening = 10f;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private bool rotate;

    private float lastLongDist;
    private float lastShortDist;
    private float shortDist;

    void Start()
    {
        Init();

        
    }

    void OnEnable()
    {
        Init();
    }

    public void SetTarget(GameObject obj)
    {
        this.mainTarget = obj;
        this.target = obj.transform;
    }

    public void Init()
    {
        this.shortDist = 3f;
        this.lastShortDist = this.shortDist;
        this.lastLongDist = this.distance;

        this.currentDistance = this.distance;
        this.desiredDistance = this.distance;

        this.position = this.transform.position;
        this.rotation = this.transform.rotation;
        this.currentRotation = this.transform.rotation;
        this.desiredRotation = this.transform.rotation;

        this.xDeg = Vector3.Angle(Vector3.right, this.transform.right);
        this.yDeg = Vector3.Angle(Vector3.up, this.transform.up);
    }

    public void GetMouseButtonDownOne()
    {
        this.rotate = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GetMouseButtonUpOne()
    {
        this.rotate = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TriggerZoom(GameObject obj)
    {
        this.lastLongDist = this.desiredDistance;
        this.target = obj.transform;
        this.desiredDistance = this.lastShortDist;
    }

    public void UntriggerZoom()
    {
        this.lastShortDist = this.desiredDistance;
        this.target = this.mainTarget.transform;
        this.desiredDistance = this.lastLongDist;
    }

    void LateUpdate()
    {
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
}
