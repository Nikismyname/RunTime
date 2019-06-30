using UnityEngine;

public class NewtonianSpaceshipHandling : MonoBehaviour
{
    public GameObject myCamera;
    private GameObject mainCamera;
    private Camera cameraRef;

    private float maxSpeedCombat = 0.7f;
    private float rotationSpeedCombat = 150;
    private float slowDowntConstant = 0.6f;
    private float thrusterForceNewtonian = 2f;
    private float thrusterForceCombat = 3f;

    private GameObject shipBodyRef;
    private GameObject shipAnchorRef;
    private GameObject cameraTarget;

    private Vector3 shipVelocityNutonian;
    private float shipVelocityCombat;

    private bool nutonianMode = false;
    bool active = false;
    public bool IsActive => this.active;

    private PlayerHandling playerHandling;
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
        ///Adjusting the speed from the editor;
        var speedMultiplyer = main.GetComponent<Main>().NSSAllSpeedMultipyer;
        this.maxSpeedCombat *= speedMultiplyer;
        this.slowDowntConstant *= speedMultiplyer;
        this.thrusterForceNewtonian *= speedMultiplyer;
        this.thrusterForceCombat *= speedMultiplyer;

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
        this.shipVelocityNutonian = Vector3.zero;
        this.shipVelocityCombat = 0;
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

        Cursor.lockState = CursorLockMode.Locked;

        ///Invoking the HasActivated event
        this.HasActivated?.Invoke();
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
        this.shipVelocityNutonian = shipAnchorRef.transform.forward.normalized * shipVelocityCombat;
        this.shipBodyRef.GetComponent<Renderer>().material.color = Color.blue;
    }
    private void SwithToCombatMode()
    {
        //pssing the speed and direction 
        shipAnchorRef.transform.forward = shipVelocityNutonian;
        shipVelocityCombat = shipVelocityNutonian.magnitude;
        shipBodyRef.GetComponent<Renderer>().material.color = Color.red;
    }
    #endregion

    #region LATE_UPDATE
    private void LateUpdate()
    {
        if (this.active == false) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None; 
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        var targetRotation = cameraRef.transform.rotation;

        //FREE FLIGHT, Nutonian Mode 
        if (nutonianMode)
        {
            //moving the ship
            shipAnchorRef.transform.position += shipVelocityNutonian;
            //Chaging rotation, always in sink with the camera, might change it in future 
            shipAnchorRef.transform.rotation = targetRotation;

            if (Input.GetKey("w"))
            {
                shipVelocityNutonian += shipAnchorRef.transform.forward.normalized * thrusterForceNewtonian * Time.deltaTime;
            }

            if (Input.GetKeyDown("s"))
            {
                //full stop in nutionian Mode, temporary.
                shipVelocityNutonian = Vector3.zero;
            }
        }
        else //COMBAT MODE
        {
            //moving the ship
            shipAnchorRef.transform.position += shipAnchorRef.transform.forward.normalized * shipVelocityCombat;

            //ChangingTheOrientationTowardsTheCameraWithMaxDegrees 
            shipAnchorRef.transform.rotation = Quaternion.RotateTowards(shipAnchorRef.transform.rotation,
                                                      cameraRef.transform.rotation,
                                                      Time.deltaTime * rotationSpeedCombat);

            //GoingForward, Reaching Our Max Fast;
            if (Input.GetKey(KeyCode.W))
            {
                //If we till have palce to speed up, we do
                if (shipVelocityCombat < maxSpeedCombat)
                {
                    shipVelocityCombat += thrusterForceCombat * Time.deltaTime;
                }
                else //this is in case we are in transition from nutonian and Have Extra velocity to play with
                {
                    //getting the velocity of nutonian ship and decelerating slower than normal by a constant 
                    //making a check the velocity is not too close to an edge Speed(0,maxSpeed), if so we make it that speed, to avoid constantly gpoing over under it 
                    if (shipVelocityCombat < maxSpeedCombat + 0.01f)
                    {
                        //we are mooving at a constant max velocity, this is only the magnitude, because in this mode we can shift the velocity a certain angle with no penalty 
                        shipVelocityCombat = maxSpeedCombat;
                    }
                    else
                    {
                        //loosig the velocity wo got from nutonial mode
                        shipVelocityCombat -= thrusterForceCombat * Time.deltaTime * slowDowntConstant;
                    }
                }
            }
            else //Deaccelerating becasue no thrusters 
            {
                //same thing as before, to avoid jittering 
                if (shipVelocityCombat > 0.01)
                {
                    //still applying the slow deceleration here, not sure if I want to keep it
                    shipVelocityCombat -= thrusterForceCombat * Time.deltaTime * slowDowntConstant;
                }
                else
                {
                    //to avoid jittering over/under 0
                    shipVelocityCombat = 0;
                }
            }
        }

        //switches between combat and nutonian Mode
        if (Input.GetKeyDown(KeyCode.R))
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
            GUI.Label(new Rect(200, 30, 30, 30), shipVelocityNutonian.magnitude.ToString());
        }
        else
        {
            GUI.Label(new Rect(200, 30, 30, 30), shipVelocityCombat.ToString());
        }
    }
    #endregion
}
