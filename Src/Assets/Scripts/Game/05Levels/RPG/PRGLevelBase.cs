using UnityEngine;

public class PRGLevelBase : LevelBase
{
    protected GameObject mainCamera;
    protected GameObject mainSpellCamera;
    protected GameObject secondSpellCamera;
    protected GameObject baseCylindcer;
    protected GameObject player;
    private WorldSpaceUI worldSpaceUI;
    private GameObject droneCamGO;

    protected virtual void Start()
    {
        // floor!
        GameObject baseCylindcer = ReferenceBuffer.Instance.gl.CylinderBasePrefab(new Vector3(40, 1, 40), true);

        // player and cam!
        this.player = ReferenceBuffer.Instance.gl.Player(new Vector3(0, 0, 10), true, true, true);
        this.mainCamera = GameObject.Find("MainCamera");
        this.mainSpellCamera = GameObject.Find("Camera"); /// second cam is Camera2
        this.secondSpellCamera = GameObject.Find("Camera2");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;

        // spellcraft
        SpellcraftProcUI procUI = gameObject.AddComponent<SpellcraftProcUI>();
        this.worldSpaceUI = gameObject.AddComponent<WorldSpaceUI>();
        this.worldSpaceUI.Setup(new Vector3(0, -40, 0));
        this.worldSpaceUI.LoadLevel = false;
        ReferenceBuffer.Instance.RegisterWorldSapceUI(this.worldSpaceUI);

        // drone setup
        this.droneCamGO = GameObject.Find("DroneCamera");
        this.droneCamGO.GetComponent<Camera>().enabled = false;
        this.droneCamGO.transform.position = new Vector3(5, 5, 5);
        this.droneCamGO.AddComponent<Drone>();
        this.droneCamGO.SetActive(false);
    }

    protected virtual void Update()
    {
        this.CameraSwitch();
    }

    #region CameraSwitch
    
    protected void CameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.F)  && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            // RPG Mode active
            if (this.mainCamera.GetComponent<Camera>().enabled == true)
            {
                this.SwithchToSpellCraft();
            }
            else
            {
                this.SwithchToRPG();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D)  && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.SwitchToDrone();
        }

        if (Input.GetKeyDown(KeyCode.C) && this.mainCamera.GetComponent<Camera>().enabled == false && ReferenceBuffer.Instance.focusManager.SafeToTrigger())
        {
            this.worldSpaceUI.SwitchToMenu();
            //Debug.Log("SPELL SWITCH");
        }
    }
    
    public void SwithchToSpellCraft()
    {
        this.mainCamera.GetComponent<Camera>().enabled = false;
        this.mainSpellCamera.GetComponent<Camera>().enabled = true;
        ReferenceBuffer.Instance.UI.SetActive(false);
        //Debug.Log("RPG OFF");
    }

    public void SwithchToRPG()
    {
        this.mainCamera.GetComponent<Camera>().enabled = true;
        this.mainSpellCamera.GetComponent<Camera>().enabled = false;
        this.secondSpellCamera.GetComponent<Camera>().enabled = false;
        ReferenceBuffer.Instance.UI.SetActive(true);
        //Debug.Log("RPG ON");
    }

    public void SwitchToDrone()
    {
        // Only switch to drone from rpg mode!
        if (this.mainCamera.GetComponent<Camera>().enabled == true)
        {
            // Switch To Drone
            if (this.droneCamGO.activeSelf == false)
            {
                this.droneCamGO.SetActive(true);
                this.droneCamGO.GetComponent<Camera>().enabled = true;
                this.mainSpellCamera.GetComponent<Camera>().enabled = false;
                this.player.SetActive(false);
            }
            // Swith off drone
            else
            {
                this.droneCamGO.GetComponent<Camera>().enabled = false;
                this.droneCamGO.SetActive(false);
                this.mainCamera.GetComponent<Camera>().enabled = true;
                this.player.SetActive(true);
            }
        }
    }
    
    #endregion
}