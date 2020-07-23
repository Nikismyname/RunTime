using UnityEngine;
using UnityEngine.UI;

public class LevelMainRPG : LevelBase
{
    private GenerateLevel gl;
    private TargetManagerBehaviour ms;
    private GameObject player;
    private GameObject mainCamera;
    private WorldSpaceUI worldSpaceUI;

    private void Start()
    {
        /// References!
        var main = GameObject.Find("Main");
        this.ms = main.GetComponent<TargetManagerBehaviour>();
        var rb = main.GetComponent<ReferenceBuffer>();
        this.gl = new GenerateLevel(ms, rb);
        ///... 

        /// Problem Text and Code!
        //var infoText = rb.InfoTextObject;
        //infoText.GetComponent<Text>().text = ProblemDesctiptions.Level1MoveUp;
        //rb.ShowCode.SetText(InitialCodes.Level1);
        ///...

        /// Floor!
        GameObject baseCylindcer = this.gl.CylinderBasePrefab(new Vector3(40, 1, 40), true);
        ///...

        /// Player and cam!
        this.player = this.gl.Player(new Vector3(0, 0, 10), true, true, true);
        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        ///...

        var procUI = gameObject.AddComponent<SpellcraftProcUI>();
        this.worldSpaceUI = gameObject.AddComponent<WorldSpaceUI>();
        this.worldSpaceUI.LoadLevel = true;
    }

    public override void ResetLevel()
    {
        Debug.LogError("LEVEL RESET!");
    }
}

