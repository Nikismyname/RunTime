﻿using System;
using System.Collections.Generic;
using System.Linq;

public class GroupBattleSimulation
{
    public bool Running { get; set; } = false;

    private List<UnitData> units = new List<UnitData>();
    private List<ProjectileData> projectiles = new List<ProjectileData>();
    private TeamBundle[] teams;
    private Fix64 projSpeed = (Fix64)0.1f;
    private Fix64 unitSpeed = (Fix64)0.1f;
    private int cooldownDuration = 100;
    private int projDuration = 100;

    public void Start(TeamBundle[] teams, Fix64Vector2[][] initPositions, string[][] projectilesArr)
    {
        units = new List<UnitData>();
        projectiles = new List<ProjectileData>(); 

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
                    SpellCooldowns = projectiles.Select(x => new SpellCooldown(x, 0, this.cooldownDuration)).ToList(),
                });
            }
        }
    }

    public void Update(
        out UnitData[] units,
        out ProjectileData[] projectiles,
        float unitSpeed,
        float projSpeed,
        int projDuration,
        int cooldownDuration 
        )
    {
        ///Editor Variable Changes
        this.unitSpeed = (Fix64)unitSpeed;
        this.projSpeed = (Fix64)projSpeed;
        this.projDuration = projDuration; 
        foreach (var item in this.units)
        {
            foreach (var cdd in item.SpellCooldowns)
            {
                cdd.MaxIteration = cooldownDuration; 
            }
        }
        ///------------------------

        units = new UnitData[0];
        projectiles = new ProjectileData[0];

        if (this.Running == false) { return; }

        this.ProcessCooldowns(this.units);

        foreach (TeamBundle team in this.teams)
        {
            this.MakeTeamMove(team.Name, this.units.ToArray(), this.projectiles.ToArray(), team.TB);
        }

        this.MoveProjectilesAndRemoveExpired(this.projectiles);

        this.CheckCollisions(this.units, this.projectiles);

        units = this.units.ToArray();
        projectiles = this.projectiles.ToArray();
    }

    //public void Reset()
    //{

    //}

    private void ProcessCooldowns(List<UnitData> units)
    {
        foreach (var unit in units)
        {
            List<SpellCooldown> cooldowns = unit.SpellCooldowns;

            for (int i = cooldowns.Count - 1; i >= 0; i--)
            {
                SpellCooldown cooldown = cooldowns[i];

                if (cooldown.CurrentIteration > 0)
                {
                    cooldown.CurrentIteration--;
                }
            }
        }
    }

    private void CheckCollisions(List<UnitData> units, List<ProjectileData> projectiles)
    {
        List<int> removedUnitsIndecies = new List<int>();
        List<int> removedProjsIndecies = new List<int>();

        for (int i = units.Count - 1; i >= 0; i--)
        {
            for (int j = projectiles.Count - 1; j >= 0; j--)
            {
                UnitData unit = units[i];
                ProjectileData proj = projectiles[j];

                if ((unit.Position - proj.Position).Magnitude < (Fix64)1f && unit.Team != proj.Team)
                {
                    unit.Health -= proj.Demage;
                    if (unit.Health <= 0)
                    {
                        removedUnitsIndecies.Add(i);
                    }

                    removedProjsIndecies.Add(j);
                }
            }
        }

        foreach (var ind  in removedUnitsIndecies.OrderByDescending(x=>x))
        {
            units.RemoveAt(ind);
        }

        foreach (var ind in removedProjsIndecies.OrderByDescending(x => x))
        {
            projectiles.RemoveAt(ind);
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

            Fix64Vector2 direction = proj.Direction;
            Fix64Vector2 normalDirection = direction.Normalized;
            Fix64Vector2 adjustedDirection = normalDirection * this.projSpeed; /// ps -> 0.1f

            proj.Position += adjustedDirection;
        }
    }

    private void MakeTeamMove(
        string teamName,
        UnitData[] allUnits,
        ProjectileData[] allProjs,
        TargetBehaviour tb)
    {
        UnitData[] friendlyUnits = allUnits.Where(x => x.Team == teamName).ToArray();
        UnitData[] enemyUnits = allUnits.Where(x => x.Team != teamName).ToArray();
        ProjectileData[] friendlyProj = allProjs.Where(x => x.Team == teamName).ToArray();
        ProjectileData[] enemyProj = allProjs.Where(x => x.Team != teamName).ToArray();

        BattleMoveOutputSingle[] result = tb.MakeMove(new BattleMoveInputWholeTeam
        {
            FriendlyUnits = friendlyUnits,
            EnemyUnits = enemyUnits,
            FriendlyProjs = friendlyProj,
            EnemyProjs = enemyProj
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
                unit.Position += data.NewLocation.Value.Normalized * this.unitSpeed;
            }

            if (data.ProjVelosity != null)
            {
                SpellCooldown spellCD= unit.SpellCooldowns.FirstOrDefault(x => x.Spell == "fireball");

                if (spellCD.CurrentIteration <= 0)
                {
                    spellCD.CurrentIteration = spellCD.MaxIteration; 

                    ProjectileData proj = new ProjectileData()
                    {
                        Demage = 20,
                        Direction = data.ProjVelosity.Value,
                        Position = unit.Position,
                        Radious = (Fix64)1f,
                        Team = unit.Team,
                        Type = data.ProjType,
                        CurrentIteration = 0,
                        MaxIteration = this.projDuration,
                    };

                    this.projectiles.Add(proj);
                }
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

public class RemovedEntities
{
    public List<int> RemovedProjs { get; set; }

    public int[] RemovedUnits { get; set; }
}
