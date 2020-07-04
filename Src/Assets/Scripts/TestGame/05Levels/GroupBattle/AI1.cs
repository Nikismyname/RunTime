using System.Linq;
public class AI1 : ITeamBattleMoveMaker
{
    public BattleMoveOutputSingle[] MakeMove(BattleMoveInputWholeTeam input)
    {
        var returnVal = new BattleMoveOutputSingle[input.FriendlyUnits.Length];
        MoveHelpersDeterministic help = new MoveHelpersDeterministic();
        BattleMoveOutputSingle[] result = input.FriendlyUnits.Select(x => new BattleMoveOutputSingle()).ToArray();

        for (int i = 0; i < input.FriendlyUnits.Length; i++)
        {
            UnitData myUnit = input.FriendlyUnits[i];
            Fix64Vector2 myLocation = input.FriendlyUnits[i].Position;
            SpellCooldown fireball = myUnit.SpellCooldowns.FirstOrDefault(x => x.Spell == "fireball");
            result[i].ProjVelosity = null;
            if (fireball != null && input.EnemyUnits.Length > 0 && fireball.CurrentIteration <= 0)
            {
                result[i].ProjVelosity = input.EnemyUnits[0].Position - myLocation;
            }

            var allHits = input.EnemyProjs
                .Select(x => new { one = help.DistanceToHit(x.Position, x.Direction, myLocation), two = x })
                .Where(x => x.one != null)
                .Where(x => x.one.HitVector.Magnitude <= (Fix64)1.5f)
                .ToArray();

            result[i].NewLocation = null;
            if (allHits.Length != 0)
            {
                var thing = allHits.First(x => x.one.DistanceToProj == allHits.Min(y => y.one.DistanceToProj));
                Fix64Vector2 hitVector = thing.one.HitVector;
                Fix64Vector2 projVel = thing.two.Direction;

                if (hitVector.Magnitude == (Fix64)0)
                {
                    result[i].NewLocation = (projVel.Negative()).Rotate((Fix64)90);
                }
                else
                {
                    result[i].NewLocation = (hitVector.Negative()).Normalized;
                }
            }
        }

        return result;
    }
}