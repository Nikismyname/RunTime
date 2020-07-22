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
    private bool isRotateToView = false;
    private GameObject rotateToViewTarget;
    private Quaternion preRotateToViewRotation;
    private int maxSteps = 1;
    private int currentStep = 0;
    private bool enabled = true;

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

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public void Setup(GameObject obj)
    {
        this.mainTarget = obj;
        this.target = obj.transform;
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
        if(this.enabled == false)
        {
            return;
        } 

        if (Input.GetKey(KeyCode.LeftShift) && this.rotate == true)
        {
            return;
        }

        if (this.isRotateToView)
        {
            this.currentRotation = transform.rotation;
            float fraction = (float)this.currentStep / this.maxSteps;
            this.desiredRotation = this.rotateToViewTarget.transform.rotation;
            this.rotation = Quaternion.Lerp(this.preRotateToViewRotation, desiredRotation, fraction);
            this.transform.rotation = rotation;

            if (this.currentStep == this.maxSteps)
            {
                Debug.Log("HERE");
                this.isRotateToView = false;
                this.xDeg = Vector3.Angle(Vector3.right, this.transform.right);
                this.yDeg = Vector3.Angle(Vector3.up, this.transform.up);
            }

            this.currentStep++;
        }
        else if (this.rotate) /// Rotating NORMAL
        {
            this.xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            this.yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            this.desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            this.currentRotation = transform.rotation;
            this.rotation = Quaternion.Lerp(currentRotation, desiredRotation, 1);
            this.transform.rotation = rotation;
        }

        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);

        transform.position = position;

        Input.mousePosition.Set(0, 0, 0);
        Debug.Log("old situation");
    }

    public void SetRotateToView(GameObject rtvTarget)
    {

        this.rotateToViewTarget = rtvTarget;
        this.rotateToViewTarget.transform.LookAt(this.target);
        this.rotateToViewTarget.transform.Rotate(new Vector3(0, 0, 1), 90);
        this.rotateToViewTarget.transform.Rotate(new Vector3(0, 0, 1), -90);
        //this.rotateToViewTarget.transform.Rotate(new Vector3(x,y,z), degree); 
        this.isRotateToView = true;
        this.desiredDistance = (rtvTarget.transform.position - this.target.position).magnitude;

        this.preRotateToViewRotation = transform.rotation;
        float angle = Quaternion.Angle(transform.rotation, this.rotateToViewTarget.transform.rotation);
        this.maxSteps = 25;
        this.currentStep = 1;
    }

    public Transform GetTarget()
    {
        return this.target;
    }

    public bool IsRotating()
    {
        return this.rotate;
    }
}
