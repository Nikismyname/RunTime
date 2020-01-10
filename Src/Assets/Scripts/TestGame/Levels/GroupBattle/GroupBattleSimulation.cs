using System;
using System.Collections.Generic;
using System.Linq;

public class GroupBattleSimulation
{
    public bool Running { get; set; } = false;

    private List<UnitData> units = new List<UnitData>();
    private List<ProjectileData> projectiles = new List<ProjectileData>();
    private TeamBundle[] teams;

    public void Start(TeamBundle[] teams, Fix64Vector2[][] initPositions, string[][] projectilesArr)
    {
        this.teams = teams;

        if (teams.Length != initPositions.Length || teams.Length != projectilesArr.Length)
        {
            throw new Exception("Team data should be the count of the teams![teams, initPositions, projectiles]");
        }

        for (int i = 0; i < teams.Length; i++)
        {
            string team = teams[i].Name;
            string[] projectiles = projectilesArr[i];

            foreach (Fix64Vector2 initPos in initPositions[i])
            {
                this.units.Add(new UnitData
                {
                    Team = team,
                    Position = initPos,
                    SpellCooldowns = projectiles.Select(x => new SpellCooldown(x, 0, 100)).ToList(),
                });
            }
        }
    }

    public void Update(out UnitData[] units, out ProjectileData[] projectiles)
    {
        units = new UnitData[0];
        projectiles = new ProjectileData[0];

        if (this.Running == false) { return; }

        foreach (TeamBundle team in this.teams)
        {
            this.MakeTeamMove(team.Name, this.units.ToArray(), this.projectiles.ToArray(), team.TB);
        }

        this.MoveProjectilesAndRemoveExpired(this.projectiles);

        this.CheckCollisions(this.units, this.projectiles);

        units = this.units.ToArray();
        projectiles = this.projectiles.ToArray();
    }

    private void CheckCollisions(List<UnitData> units, List<ProjectileData> projectiles)
    {
        for (int i = units.Count - 1; i >= 0; i--)
        {
            for (int j = projectiles.Count - 1; j >= 0; j--)
            {
                UnitData unit = units[i];
                ProjectileData proj = projectiles[j]; 

                if((unit.Position - proj.Position).Magnitude < (Fix64)2f)
                {
                    unit.Health -= proj.Demage;
                    if(unit.Health <= 0)
                    {
                        units.RemoveAt(i);
                        projectiles.RemoveAt(j);
                    }
                }
            }
        }
    }

    private void MoveProjectilesAndRemoveExpired(List<ProjectileData> projectiles)
    {
        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            ProjectileData proj = projectiles[i];
            proj.CurrentIteration++;
            if (proj.CurrentIteration >= proj.MaxIteration)
            {
                projectiles.RemoveAt(i);
                continue;
            }
            proj.Position += proj.Direction.Normalized * (Fix64)0.1f;
        }
    }

    private void MakeTeamMove(
        string teamName,
        UnitData[] allUnits,
        ProjectileData[] allProjs,
        TargetBehaviour tb)
    {
        UnitData[] friendlyUnits = allUnits.Where(x => x.Team == teamName).ToArray();

        BattleMoveOutputSingle[] result = tb.MakeMove(new BattleMoveInputWholeTeam
        {
            FriendlyUnits = friendlyUnits,
            EnemyUnits = allUnits.Where(x => x.Team != teamName).ToArray(),
            FriendlyProjs = allProjs.Where(x => x.Team == teamName).ToArray(),
            EnemyProjs = allProjs.Where(x => x.Team != teamName).ToArray(),
        });

        if (friendlyUnits.Length != result.Length)
        {
            throw new Exception("The results are not the same length as the friendly units passed!");
        }

        for (int i = 0; i < result.Length; i++)
        {
            BattleMoveOutputSingle data = result[i];
            UnitData unit = friendlyUnits[i];

            if (data.NewLocation != null)
            {
                unit.Position += data.NewLocation.Value;
            }

            if (data.ProjVelosity != null)
            {
                ProjectileData proj = new ProjectileData()
                {
                    Demage = 20,
                    Direction = data.ProjVelosity.Value,
                    Position = unit.Position,
                    Radious = (Fix64)1f,
                    Team = unit.Team,
                    Type = data.ProjType,
                    CurrentIteration = 0,
                    MaxIteration = 100,
                };

                this.projectiles.Add(proj);
            }
        }
    }

    public void Stop()
    {
        this.Running = false;
    }

    public void Start()
    {
        this.Running = true;
    }
}

public class TeamBundle
{
    public TeamBundle(string name, TargetBehaviour tB)
    {
        Name = name;
        TB = tB;
    }

    public string Name { get; set; }
    public TargetBehaviour TB { get; set; }
}
