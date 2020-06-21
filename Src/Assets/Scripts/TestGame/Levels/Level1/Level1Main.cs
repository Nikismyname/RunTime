using Assets.Scripts.TestGame.Common.InitialCodes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level1Main : MonoBehaviour, ILevelMain
{
    private LevelManager lm;
    private GameObject target, goal;
    private GenerateLevel gl;
    private Main ms;
    private GameObject player;
    private GameObject mainCamera;
    private List<GameObject> toDestroy = new List<GameObject>();

    private void Start()
    {
        /// References!
        var main = GameObject.Find("Main");
        this.lm = main.GetComponent<LevelManager>();
        this.ms = main.GetComponent<Main>();
        var rb = main.GetComponent<ReferenceBuffer>();
        this.gl = new GenerateLevel(ms, rb);
        ///... 

        /// Problem Text and Code!
        var infoText = rb.InfoTextObject;
        infoText.GetComponent<Text>().text = ProblemDesctiptions.Level1MoveUp;
        rb.ShowCode.SetText(InitialCodes.Level1);
        ///...

        /// Floor!
        GameObject baseCylindcer = this.gl.CylinderBasePrefab(new Vector3(40, 1, 40), true);
        toDestroy.Add(baseCylindcer);
        ///...

        /// Player and cam!
        this.player = this.gl.Player(new Vector3(0, 0, 10), true, true, true);
        this.toDestroy.Add(this.player);
        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        ///...

        /// Generate entities!
        this.target = gl.GenerateEntity(EntityType.Target, new Vector3(0, 0, 0), PrimitiveType.Cube, Color.gray, null, "Target");
        this.toDestroy.Add(this.target);
        this.goal = gl.GenerateEntity(EntityType.NonTarget, new Vector3(0, 8, 0), PrimitiveType.Cube, Color.blue, null, "Goal");
        this.toDestroy.Add(this.goal);
        ///...
    }

    private void Update()
    {

        if (target == null) 
        {
            return;
        }  

        if (this.target.transform.position.y > this.goal.transform.position.y)
        {
            this.lm.Success();
        }

        if((this.target.transform.position - this.goal.transform.position).magnitude > 12)
        {
            this.lm.Failure("You suck!");
        }
    }

    public void ResetLevel()
    {
        this.ms.UnregisterTarget(this.target);
        Destroy(this.target);
        this.target = this.gl.GenerateEntity(EntityType.Target, new Vector3(0, 0, 0), PrimitiveType.Cube, Color.gray, null, "Target");
    }

    public void Destroy()
    {
        for (int i = 0; i < this.toDestroy.Count; i++)
        {
            Destroy(this.toDestroy[i]);
        }

        Destroy(this);
    }
}

//this.camera = Camera.main;
//camera.transform.position = new Vector3(0, 0, -20);
//camera.transform.eulerAngles = new Vector3(0,0,0);

//this.planes = GeometryUtility.CalculateFrustumPlanes(camera);
//this.targetCollider = target.GetComponent<Collider>();
//Debug.Log(targetCollider);
