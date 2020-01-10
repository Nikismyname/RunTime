#region INIT

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupBattleMainDeterministic : MonoBehaviour, ILevelMain
{
    public GameObject playerPrefab;
    private GameObject player;
    private GameObject mainCamera;
    private Main ms;
    private GenerateLevel gl;

    private bool redAttached = false;
    private bool blueAttached = false;

    TargetBehaviour red;
    TargetBehaviour blue;

    private bool simulationRunning = false;

    private List<KeyValuePair<GameObject, ProjInfo>> redProjs = new List<KeyValuePair<GameObject, ProjInfo>>();
    private List<KeyValuePair<GameObject, ProjInfo>> blueProjs = new List<KeyValuePair<GameObject, ProjInfo>>();

    private List<KeyValuePair<GameObject, int[]>> projTimeouts = new List<KeyValuePair<GameObject, int[]>>();

    /// <summary>
    /// Removign from one list must also remove from the other list!
    /// </summary>
    List<BattleUnit> teamRed = new List<BattleUnit>();
    List<Dictionary<string, float>> teamRedCooldowns = new List<Dictionary<string, float>>();
    List<BattleUnit> teamBlue = new List<BattleUnit>();
    List<Dictionary<string, float>> teamBlueCooldowns = new List<Dictionary<string, float>>();

    public const string RedTeamName = "red";
    public const string BlueTeamName = "blue";

    private const float MyDeltaTime = 0.05f;
    private float timer = 0.05f;

    #endregion

    #region SETUP

    private void Start()
    {
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

        for (int i = 0; i < 10; i++)
        {
            GameObject fighter = GameObject.Instantiate(playerPrefab);
            var tracking = fighter.AddComponent<BattleUnit>();
            tracking.SetUp("red");
            fighter.GetComponent<Renderer>().material.color = Color.red;
            fighter.transform.position = new Vector3(i * 2, 1, -10);
            teamRed.Add(tracking);
            this.teamRedCooldowns.Add(new Dictionary<string, float>());
            this.teamRedCooldowns.Last().Add("fireball", 0);
            fighter.layer = LayerMask.NameToLayer(RedTeamName);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject fighter = GameObject.Instantiate(playerPrefab);
            var tracking = fighter.AddComponent<BattleUnit>();
            tracking.SetUp("blue");
            fighter.GetComponent<Renderer>().material.color = Color.blue;
            fighter.transform.position = new Vector3(i * 2, 1, 10);
            teamBlue.Add(tracking);
            this.teamBlueCooldowns.Add(new Dictionary<string, float>());
            this.teamBlueCooldowns.Last().Add("fireball", 0);
            fighter.layer = LayerMask.NameToLayer(BlueTeamName);
        }

        GameObject controlSphereRed = gl.GenerateTestEntity("team red", "Red Sphere", TargetType.BattleMovement, new Vector3(10, 4, -10));
        GameObject controlSphereBlue = gl.GenerateTestEntity("team blue", "Blue Sphere", TargetType.BattleMovement, new Vector3(10, 4, 10));

        this.red = controlSphereRed.GetComponent<TargetBehaviour>();
        this.blue = controlSphereBlue.GetComponent<TargetBehaviour>();

        this.red.AIAttachedEvent += () => Attached("red");
        this.blue.AIAttachedEvent += () => Attached("blue");
    }

    #endregion

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
    }

    #endregion

    private void Update()
    {
        if (this.timer <= 0)
        {
            this.SimTick();
            this.timer = MyDeltaTime;
        }

        this.timer -= Time.deltaTime;
    }

    private void SimTick()
    {
        if (this.simulationRunning)
        {
            this.MoveProjs();
            this.MakeMoves();
            this.UpdateCooldowns();
        }
    }

    private void MoveProjs()
    {
        const float speed = 5f;


        for (int i = 0; i < this.projTimeouts.Count; i++)
        {
            var kvp = this.projTimeouts[i];
            var values = kvp.Value;
            values[1]++;
            if (values[1] >= values[0])
            {
                Destroy(kvp.Key);
            }
        }

        this.redProjs = this.redProjs.Where(x => x.Key != null).ToList();
        this.blueProjs = this.blueProjs.Where(x => x.Key != null).ToList();

        for (int i = 0; i < this.redProjs.Count; i++)
        {
            var kvp = this.redProjs[i];
            GameObject proj = kvp.Key;
            proj.transform.position += kvp.Value.Direction.normalized * speed * MyDeltaTime;
            kvp.Value.Position = proj.transform.position;
        }

        for (int i = 0; i < this.blueProjs.Count; i++)
        {
            var kvp = this.blueProjs[i];
            GameObject proj = kvp.Key;
            proj.transform.position += kvp.Value.Direction.normalized * speed * MyDeltaTime;
            kvp.Value.Position = proj.transform.position;
        }
    }

    private void MakeMoves()
    {
        this.teamRed = this.teamRed.Where(x => x != null).ToList();
        this.teamBlue = this.teamBlue.Where(x => x != null).ToList();

        Vector3[] redLocations = this.teamRed.Select(x => x.transform.position).ToArray();
        Vector3[] blueLocations = this.teamBlue.Select(x => x.transform.position).ToArray();

        ProjInfo[] redProjectiles = this.redProjs.Select(x => x.Value).ToArray();
        ProjInfo[] blueProjectiles = this.blueProjs.Select(x => x.Value).ToArray();

        this.MoveTeam(this.teamRed, redLocations, blueLocations, redProjectiles, blueProjectiles, this.teamRedCooldowns, this.red, this.redProjs);
        this.MoveTeam(this.teamBlue, blueLocations, redLocations, blueProjectiles, redProjectiles, this.teamBlueCooldowns, this.blue, this.blueProjs);
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < this.teamRedCooldowns.Count; i++)
        {
            var redCopy = this.teamRedCooldowns[i].ToArray();

            for (int j = 0; j < redCopy.Length; j++)
            {
                var kvp = redCopy[j];

                if (kvp.Value > 0)
                {
                    redCopy[j] = new KeyValuePair<string, float>(redCopy[j].Key, redCopy[j].Value - MyDeltaTime);
                }

                if (kvp.Value < 0)
                {
                    redCopy[j] = new KeyValuePair<string, float>(redCopy[j].Key, 0);
                }
            }

            this.teamRedCooldowns[i] = redCopy.ToDictionary(x => x.Key, x => x.Value);
        }

        for (int i = 0; i < this.teamBlueCooldowns.Count; i++)
        {
            var blueCopy = this.teamBlueCooldowns[i].ToArray();

            for (int j = 0; j < blueCopy.Length; j++)
            {
                var kvp = blueCopy[j];

                if (kvp.Value > 0)
                {
                    blueCopy[j] = new KeyValuePair<string, float>(blueCopy[j].Key, blueCopy[j].Value - MyDeltaTime);
                }

                if (kvp.Value < 0)
                {
                    blueCopy[j] = new KeyValuePair<string, float>(blueCopy[j].Key, 0);
                }
            }

            this.teamBlueCooldowns[i] = blueCopy.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    //private void MoveTeam(
    //    List<BattleUnit> team,
    //    Vector3[] friendlyLocations,
    //    Vector3[] enemyLocations,
    //    ProjInfo[] friendlyProjs,
    //    ProjInfo[] enemyProj,
    //    List<Dictionary<string, float>> thisTeamCDs,
    //    TargetBehaviour tb,
    //    List<KeyValuePair<GameObject, ProjInfo>> projs)
    //{
    //    for (int i = 0; i < team.Count; i++)
    //    {
    //        BattleUnit unit = team[i];

    //        BattleMoveResult result = tb.MakeMove(new BattleMoveInput
    //        {
    //            FriendlyLocations = friendlyLocations,
    //            EnemyLocations = enemyLocations,
    //            MyIndex = i,
    //            Cooldowns = thisTeamCDs[i],
    //            EnemyProjs = enemyProj,
    //            FriendlyProjs = friendlyProjs,
    //        });

    //        if (result.NewLocation != null)
    //        {
    //            unit.transform.position += result.NewLocation.Value * MyDeltaTime * 20;
    //        }

    //        if (result.ProjVelosity != null && thisTeamCDs[i]["fireball"] <= 0)
    //        {
    //            var proj = unit.Shoot(unit.transform.position, result.ProjVelosity.Value, "fireball");
    //            thisTeamCDs[i]["fireball"] = 4;
    //            projs.Add(new KeyValuePair<GameObject, ProjInfo>(proj, new ProjInfo
    //            {
    //                Direction = result.ProjVelosity.Value,
    //                Position = unit.transform.position,
    //                Type = result.ProjType,
    //            }));

    //            projTimeouts.Add(new KeyValuePair<GameObject, int[]>(proj, new int[] { 100, 0 }));
    //        }
    //    }
    //}

    public void ResetLevel()
    {
    }
}