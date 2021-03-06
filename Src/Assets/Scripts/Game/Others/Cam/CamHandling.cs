﻿using UnityEngine;

public class CamHandling : MonoBehaviour
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

    private InputFocusManager inputFocusManager;

    private bool rotate;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.None;
        var thing = Cursor.lockState;
        this.rotate = false;

        var main = GameObject.Find("Main");
        this.inputFocusManager = main.GetComponent<InputFocusManager>();
        Init();
    }

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1) && this.inputFocusManager.SafeToTrigger())
        {
            if (this.rotate == false)
            {
                this.rotate = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Input.GetMouseButtonUp(1) && this.inputFocusManager.SafeToTrigger())
        {
            if (this.rotate == true)
            {
                this.rotate = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(2))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }

        //always lock to mouse 

        // If middle mouse and left alt are selected? ORBIT
        //Replacing that with Right click alone 
        // else if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))


        xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        ////////OrbitAngle

        //Clamp the vertical axis for the orbit

        //test
        //yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
        // set camera rotation 
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;

        if (this.rotate)
        {
            //rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening)
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, 1);
            transform.rotation = rotation;
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;

        Input.mousePosition.Set(0, 0, 0);

        //getting the other player 

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
