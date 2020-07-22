using UnityEngine;

public class SpellMenuDragCam : MonoBehaviour
{
    private int maxTicks = 40;
    private int currentTick = 0;
    private bool transitioning = false;
    private Quaternion targetRotation;
    private Vector3 targetPostion;
    private Quaternion startingRotation;
    private Vector3 startingPosition;
    private bool dragging = false;
    private bool transitioningBack = false;
    private GameObject otherCamera;

    public void SetTarget(Vector3 target, Quaternion startingRotation, Vector3 startingPosition)
    {
        this.startingPosition = startingPosition;
        this.startingRotation = startingRotation;

        this.transform.rotation = startingRotation;
        this.transform.position = startingPosition;

        this.targetPostion = target;
        this.targetRotation = Quaternion.Euler(0, 0, 0);

        this.transitioning = true;
        this.currentTick = 0;
    }

    public void UnsetTarget(GameObject camera)
    {
        this.transitioningBack = true;
        this.otherCamera = camera;

        this.targetPostion = this.startingPosition;
        this.targetRotation = this.startingRotation;

        this.startingPosition = gameObject.transform.position;
        this.startingRotation = gameObject.transform.rotation; 
        
        this.currentTick = 0;
    }

    private void Update()
    {
        if (this.transitioning)
        {
            this.currentTick++;

            float fraction = (float)this.currentTick / this.maxTicks;

            Quaternion intermidRotation = Quaternion.Lerp(this.startingRotation, this.targetRotation, fraction);
            Vector3 intermidPostion = Vector3.Lerp(this.startingPosition, this.targetPostion, fraction);

            this.transform.rotation = intermidRotation;
            this.transform.position = intermidPostion;

            if (this.currentTick == maxTicks + 1)
            {
                this.transitioning = false;
            }

            return;
        }

        if (this.transitioningBack)
        {
            this.currentTick++;

            float fraction = (float)this.currentTick / this.maxTicks;

            Quaternion intermidRotation = Quaternion.Lerp(this.startingRotation, this.targetRotation, fraction);
            Vector3 intermidPostion = Vector3.Lerp(this.startingPosition, this.targetPostion, fraction);

            this.transform.rotation = intermidRotation;
            this.transform.position = intermidPostion;

            if (this.currentTick == maxTicks + 1)
            {
                //this.otherCamera.SetActive(true);
                this.otherCamera.GetComponent<Camera>().enabled = true;
                this.gameObject.GetComponent<Camera>().enabled = false;
                this.transitioningBack = false;
            }

            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            this.dragging = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.dragging = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (this.dragging)
        {
            float multy = 7;
            var x  = -Input.GetAxis("Mouse X") * multy * 0.02f;
            var y = -Input.GetAxis("Mouse Y") * multy * 0.02f;

            this.transform.position += new Vector3(x,y,0);
        }
    }
}

