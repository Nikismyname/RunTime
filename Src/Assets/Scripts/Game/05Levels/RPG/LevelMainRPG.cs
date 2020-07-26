using UnityEngine;

public class LevelMainRPG : LevelBase
{
    private GameObject player;
    private GameObject mainCamera;
    private GameObject mainSpellCamera;
    private GameObject secondSpellCamera;
    private WorldSpaceUI worldSpaceUI;
    public ActionKeyPersistance persistanse = new ActionKeyPersistance();

    private void Start()
    {
        /// Floor!
        GameObject baseCylindcer = ReferenceBuffer.Instance.gl.CylinderBasePrefab(new Vector3(40, 1, 40), true);

        /// Player and cam!
        this.player = ReferenceBuffer.Instance.gl.Player(new Vector3(0, 0, 10), true, true, true);
        this.mainCamera = GameObject.Find("MainCamera");
        this.mainSpellCamera = GameObject.Find("Camera"); /// second cam is Camera2
        this.secondSpellCamera = GameObject.Find("Camera2");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        ///...

        var procUI = gameObject.AddComponent<SpellcraftProcUI>();
        this.worldSpaceUI = gameObject.AddComponent<WorldSpaceUI>();
        this.worldSpaceUI.Setup(new Vector3(0, -40, 0));
        this.worldSpaceUI.LoadLevel = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            /// RPG Mode active
            if (this.mainCamera.GetComponent<Camera>().enabled == true) 
            {
                this.mainCamera.GetComponent<Camera>().enabled = false;
                this.mainSpellCamera.GetComponent<Camera>().enabled = true;
                ReferenceBuffer.Instance.UI.SetActive(false);
                //Debug.Log("RPG OFF");
            }
            /// Spellcraft Mode active
            else
            {
                this.mainCamera.GetComponent<Camera>().enabled = true;
                this.mainSpellCamera.GetComponent<Camera>().enabled = false;
                this.secondSpellCamera.GetComponent<Camera>().enabled = false;
                ReferenceBuffer.Instance.UI.SetActive(true);
                //Debug.Log("RPG ON");
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && this.mainCamera.GetComponent<Camera>().enabled == false)
        {
            this.worldSpaceUI.SwitchToMenu();
            //Debug.Log("SPELL SWITCH");
        }
    }

    public void SwithchToSpellCraft()
    {
        if (this.mainCamera.GetComponent<Camera>().enabled == true)
        {
            this.mainCamera.GetComponent<Camera>().enabled = false;
            this.mainSpellCamera.GetComponent<Camera>().enabled = true;
            ReferenceBuffer.Instance.UI.SetActive(false);
            //Debug.Log("RPG OFF");
        }
    }

    public void SwithchToRPG()
    {
        if (this.mainCamera.GetComponent<Camera>().enabled == false)
        {
            this.mainCamera.GetComponent<Camera>().enabled = true;
            this.mainSpellCamera.GetComponent<Camera>().enabled = false;
            this.secondSpellCamera.GetComponent<Camera>().enabled = false;
            ReferenceBuffer.Instance.UI.SetActive(true);
            //Debug.Log("RPG ON");
        }
    }

    public override void ResetLevel()
    {
        this.player.transform.position = new Vector3(0, 0, 10);
    }
}

