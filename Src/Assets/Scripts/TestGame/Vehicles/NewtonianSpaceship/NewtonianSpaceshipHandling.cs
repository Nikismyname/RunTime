#region INIT

using UnityEngine;

public class NewtonianSpaceshipHandling : MonoBehaviour
{
    public GameObject myCamera;
    private GameObject mainCamera;
    private Camera cameraRef;

    private float maxCombatSpeed = 0.7f;
    private float newtonianThrusterForce = 2f;
    private float combatThrusterForce = 3f;
    private float slowDowntConstant = 0.6f;
    private float speedingSlowDownConstant = 0.4f;
    private float slowingSlowDownConstant = 0.9f;
    private float rotationSpeedCombat = 150;

    private GameObject shipBodyRef;
    private GameObject shipAnchorRef;
    private GameObject cameraTarget;

    private Vector3 nutonianVelocity;
    private float combatVelocityMagnitude;

    private bool nutonianMode = false;
    private bool active = false;

    private PlayerHandling2 playerHandling;
    private float MaxEntranceDistance = float.MaxValue;
    private ReferenceBuffer referenceBuffer;

    ///Event for when ship becomes active and inactive
    public delegate void HasActivatedAction();
    public event HasActivatedAction HasActivated;
    public delegate void HasDeactivatedAction();
    public event HasDeactivatedAction HasDeactivated;
    ///...
    
    private void Start()
    {
        var main = GameObject.Find("Main");
        this.referenceBuffer = main.GetComponent<ReferenceBuffer>();
        Main ms = main.GetComponent<Main>();

        ///Adjusting the speed from the editor;
        var speedMultiplyer = ms.NSSAllSpeedMultipyer;
        var slowMultiplayer = ms.NSSAllSlowConstantsMultiplyer; 

        this.maxCombatSpeed *= speedMultiplyer;
        this.newtonianThrusterForce *= speedMultiplyer;
        this.combatThrusterForce *= speedMultiplyer;

        ///Adjusting the slow down contants
        this.slowDowntConstant *= slowMultiplayer;
        this.speedingSlowDownConstant *= slowMultiplayer;
        this.slowingSlowDownConstant *= slowMultiplayer;

        ///Rotational Speed Stays constant for now; 

        ///The script is on the parent
        this.cameraRef = myCamera.GetComponent<Camera>();
        this.shipAnchorRef = gameObject;
        this.shipBodyRef = this.shipAnchorRef.transform.Find("Body").gameObject;

        this.cameraTarget = shipAnchorRef.transform.Find("CameraTarget").gameObject;
        this.cameraRef.GetComponent<NewtonianSpaceshipCamera>().target = cameraTarget.transform;
        var audioListener = myCamera.GetComponent<AudioListener>();
        this.myCamera.SetActive(false);
        audioListener.enabled = false;
        this.mainCamera = Camera.main.gameObject;
    }

    #endregion

    public void Drive()
    {
        ///acquiring the player handling script to get distance as well as SetUp the player for flight
        if (this.playerHandling == null)
        {
            this.playerHandling = this.referenceBuffer.PlayerHandling;

            if (this.playerHandling == null)
            {
                Debug.Log("Ship Can Not Find Player Handling!");
                return;
            }
        }
        ///...

        ///If too far do not engage
        var currentDistance = (playerHandling.GetCurrentPosition - gameObject.transform.position).magnitude;
        if (currentDistance > this.MaxEntranceDistance)
        {
            Debug.Log($"You are too far away to enter the vehicle\nMaxDistance: {this.MaxEntranceDistance} - your Distance: {currentDistance}");
            return;
        }
        ///...

        ///setting up the ship
        this.nutonianVelocity = Vector3.zero;
        this.combatVelocityMagnitude = 0;
        this.mainCamera.SetActive(false);
        this.myCamera.SetActive(true);
        this.active = true;
        ///...

        ///Clearing the UI
        this.referenceBuffer.ShowActions.Close();
        this.referenceBuffer.ShowAvailableCSFiles.Close();

        ///setting up the player
        this.playerHandling.EnterVehicle();
        ///...

        ///Invoking the HasActivated event
        this.HasActivated?.Invoke();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
        ///swithing the cameras
        this.mainCamera.SetActive(true);
        this.myCamera.SetActive(false);
        ///disabaling the inputs on the ship
        this.active = false;
        ///Releasing the player
        this.playerHandling.ExitVehicle(gameObject.transform.position);

        Cursor.lockState = CursorLockMode.None;

        ///Invoking the HasDeactivated event
        this.HasDeactivated?.Invoke();
    }

    #region SWITCHING_STANCES 
    private void SwitchToNewtonianMode()
    {
        this.nutonianVelocity = shipAnchorRef.transform.forward.normalized * combatVelocityMagnitude;
        this.shipBodyRef.GetComponent<Renderer>().material.color = Color.blue;
    }
    private void SwithToCombatMode()
    {
        //pssing the speed and direction 
        shipAnchorRef.transform.forward = nutonianVelocity;
        combatVelocityMagnitude = nutonianVelocity.magnitude;
        shipBodyRef.GetComponent<Renderer>().material.color = Color.red;
    }
    #endregion

    #region LATE_UPDATE
    private void LateUpdate()
    {
        if (this.active == false) return;

        var targetRotation = cameraRef.transform.rotation;

        //FREE FLIGHT, Nutonian Mode 
        if (nutonianMode)
        {
            //moving the ship
            shipAnchorRef.transform.position += nutonianVelocity;
            //Chaging rotation, always in sink with the camera, might change it in future 
            shipAnchorRef.transform.rotation = targetRotation;

            if (Input.GetKey("w"))
            {
                nutonianVelocity += shipAnchorRef.transform.forward.normalized * newtonianThrusterForce * Time.deltaTime;
            }

            ///full stop in nutionian Mode, temporary.
            //if (Input.GetKeyDown("s"))
            //{
            //    shipVelocityNutonian = Vector3.zero;
            //}
        }
        else //COMBAT MODE
        {
            //moving the ship
            shipAnchorRef.transform.position += shipAnchorRef.transform.forward.normalized * combatVelocityMagnitude;

            //ChangingTheOrientationTowardsTheCameraWithMaxDegrees 
            shipAnchorRef.transform.rotation =  
                Quaternion.RotateTowards(shipAnchorRef.transform.rotation,
                    cameraRef.transform.rotation,
                    Time.deltaTime * rotationSpeedCombat);

            //GoingForward, Reaching Our Max Fast;
            if (Input.GetKey(KeyCode.W))
            {
                //If we still have palce to speed up, we do
                if (combatVelocityMagnitude < maxCombatSpeed)
                {
                    combatVelocityMagnitude += combatThrusterForce * Time.deltaTime;
                }
                else //this is in case we are in transition from nutonian and Have Extra velocity to play with
                {
                    //getting the velocity of nutonian ship and decelerating slower than normal by a constant 
                    //making a check the velocity is not too close to an edge Speed(0,maxSpeed), if so we make it that speed, to avoid constantly gpoing over under it 
                    if (combatVelocityMagnitude < maxCombatSpeed + 0.01f)// the speed is between maxCombatSpeed and maxCombatSpeed + 0.01
                    {
                        //we are mooving at a constant max velocity, this is only the magnitude, because in this mode we can shift the velocity a certain angle with no penalty 
                        combatVelocityMagnitude = maxCombatSpeed;
                    }
                    else
                    {
                        //loosig the velocity wo got from nutonial mode
                        combatVelocityMagnitude -= combatThrusterForce * Time.deltaTime * speedingSlowDownConstant;
                    }
                }
            }
            else if(Input.GetKey(KeyCode.S))
            {
                if (combatVelocityMagnitude > 0.01)
                {
                    //still applying the slow deceleration here, not sure if I want to keep it
                    combatVelocityMagnitude -= combatThrusterForce * Time.deltaTime * slowingSlowDownConstant;
                }
                else
                {
                    //to avoid jittering over/under 0
                    combatVelocityMagnitude = 0;
                }
            }
            else //Deaccelerating becasue no thrusters 
            {
                //same thing as before, to avoid jittering 
                if (combatVelocityMagnitude > 0.01)
                {
                    //still applying the slow deceleration here, not sure if I want to keep it
                    combatVelocityMagnitude -= combatThrusterForce * Time.deltaTime * slowDowntConstant;
                }
                else
                {
                    //to avoid jittering over/under 0
                    combatVelocityMagnitude = 0;
                }
            }
        }

        //switches between combat and nutonian Mode
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nutonianMode)
            {
                this.nutonianMode = false;
                this.SwithToCombatMode();
            }
            else
            {
                this.nutonianMode = true;
                this.SwitchToNewtonianMode();
            }
        }
    }
    #endregion

    #region ON_GUI
    private void OnGUI()
    {
        if (this.active == false) return;

        if (nutonianMode)
        {
            GUI.Label(new Rect(200, 30, 30, 30), nutonianVelocity.magnitude.ToString());
        }
        else
        {
            GUI.Label(new Rect(200, 30, 30, 30), combatVelocityMagnitude.ToString());
        }
    }
    #endregion
}
