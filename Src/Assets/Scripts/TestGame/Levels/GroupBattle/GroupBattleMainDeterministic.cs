#region INIT

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupBattleMainDeterministic : MonoBehaviour, ILevelMain
{
    public float UnitSpeedMulty;
    public float ProjSpeedMulty;
    public int ProjDuration;
    public int CooldownDuration; 
    public float MyDeltaTime;

    public GameObject playerPrefab;
    private GameObject player;
    private GameObject mainCamera;
    private Main ms;
    private GenerateLevel gl;

    private bool redAttached = false;
    private bool blueAttached = false;

    TargetBehaviour redTB;
    TargetBehaviour blueTB;

    private bool simulationRunning = false;

    ///----------------------------------------------------

    public readonly string RedTeamName = "red";
    public readonly string BlueTeamName = "blue";

    //private const float MyDeltaTime = 0.05f;
    private float timer = 0.05f;

    //------------------------------------------------------

    private GroupBattleSimulation battleSimulations;

    List<GameObject> unitViss;
    List<GameObject> projViss;

    Fix64Vector2[][] positions;

    #endregion

    #region SETUP

    private void Start()
    {
        /// Setting up the level
        var main = GameObject.Find("Main");
        this.ms = main.GetComponent<Main>();
        var rb = main.GetComponent<ReferenceBuffer>();
        this.gl = new GenerateLevel(this.ms, rb);
        this.player = this.gl.Player(new Vector3(20, 0, 10), true, true, true);

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.localScale = new Vector3(50, 50, 50);
        floor.GetComponent<Renderer>().material.color = Color.green;
        this.mainCamera = GameObject.Find("MainCamera");
        CamHandling camHandling = this.mainCamera.GetComponent<CamHandling>();
        camHandling.target = this.player.transform;
        ///---------------------------------------------------------------------------

        this.positions = new Fix64Vector2[2][];
        this.positions[0] = new Fix64Vector2[10];
        this.positions[1] = new Fix64Vector2[10];

        unitViss = new List<GameObject>();
        projViss = new List<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(i * 2, 1, -10);
            this.positions[0][i] = pos.ToFixed2d();
            GameObject vis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vis.GetComponent<Renderer>().material.color = Color.red;
            vis.transform.position = pos;
            Destroy(vis.GetComponent<Collider>());
            unitViss.Add(vis);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(i * 2, 1, 10);
            this.positions[1][i] = pos.ToFixed2d();
            GameObject vis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vis.GetComponent<Renderer>().material.color = Color.blue;
            vis.transform.position = pos;
            Destroy(vis.GetComponent<Collider>());
            unitViss.Add(vis);
        }

        /// Setting up the Controll Sphere
        GameObject controlSphereRed = gl.GenerateTestEntity("team red", "Red Sphere", TargetType.BattleMoveSameDom, new Vector3(10, 4, -10));
        GameObject controlSphereBlue = gl.GenerateTestEntity("team blue", "Blue Sphere", TargetType.BattleMoveSameDom, new Vector3(10, 4, 10));

        this.redTB = controlSphereRed.GetComponent<TargetBehaviour>();
        this.blueTB = controlSphereBlue.GetComponent<TargetBehaviour>();

        this.redTB.AIAttachedEvent += () => Attached("red");
        this.blueTB.AIAttachedEvent += () => Attached("blue");
        ///------------------------------------------------------------------------

        /// Starting the simulation
        this.battleSimulations = new GroupBattleSimulation();
        this.battleSimulations.Start(
            new TeamBundle[] { new TeamBundle(this.RedTeamName, new AI1()), new TeamBundle(this.BlueTeamName, new AI1()) },
            this.positions,
            new string[][] { new string[] { "fireball" }, new string[] { "fireball" } });
        this.StartSimulation();
        ///-------------------------------------------------------------------------
    }

    #endregion

    private void Update()
    {
        if (this.timer <= 0 && this.simulationRunning == true)
        {
            UnitData[] unitsData;
            ProjectileData[] peojectilesData;
            this.battleSimulations.Update(out unitsData, out peojectilesData, this.UnitSpeedMulty, this.ProjSpeedMulty,this.ProjDuration, this.CooldownDuration);
            this.UpdateVisualisation(unitsData, peojectilesData, ref this.unitViss, ref this.projViss);
            this.timer = MyDeltaTime;

            if(unitsData.Length ==0 || unitsData.All(x=>x.Team == unitsData[0].Team))
            {
                this.battleSimulations.Start(
                    new TeamBundle[] { new TeamBundle(this.RedTeamName, this.redTB), new TeamBundle(this.BlueTeamName, this.blueTB) },
                    this.positions,
                    new string[][] { new string[] { "fireball" }, new string[] { "fireball" } });
            }
        }

        this.timer -= Time.deltaTime;
    }

    private void UpdateVisualisation(UnitData[] userData, ProjectileData[] projData, ref List<GameObject> userVis, ref List<GameObject> projVis)
    {
        for (int i = 0; i < userVis.Count; i++)
        {
            Destroy(userVis[i].gameObject);
        }

        for (int i = 0; i < projVis.Count; i++)
        {
            Destroy(projVis[i].gameObject);
        }

        userVis = userData.Select(x =>
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (x.Team.ToUpper() == "RED")
            {
                go.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                go.GetComponent<Renderer>().material.color = Color.blue;
            }
            go.transform.position = x.Position.ToFloat3d(0);
            return go;
        }).ToList();

        projVis = projData.Select(x =>
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (x.Team.ToUpper() == "RED")
            {
                go.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                go.GetComponent<Renderer>().material.color = Color.blue;
            }
            go.transform.position = x.Position.ToFloat3d(0);
            return go;
        }).ToList();
    }

    #region SIMULATION_START_EVENT

    private void Attached(string color)
    {
        if (color == "red")
        {
            this.redAttached = true;
            if (this.blueAttached)
            {
                this.StartSimulation();
            }
        }
        else if (color == "blue")
        {
            this.blueAttached = true;
            if (this.redAttached)
            {
                this.StartSimulation();
            }
        }
    }

    private void StartSimulation()
    {
        this.simulationRunning = true;
        this.battleSimulations.Start();
    }

    #endregion

    public void ResetLevel()
    {
    }
}