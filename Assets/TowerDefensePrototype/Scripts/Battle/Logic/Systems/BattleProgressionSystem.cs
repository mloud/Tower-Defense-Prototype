using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Logic.Managers.Skills;
using Cysharp.Threading.Tasks;
using Unity.Entities;


namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct BattleProgressionSystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        { 
            state.RequireForUpdate<BattleProgressionComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var battleProgressionC = SystemAPI.GetSingleton<BattleProgressionComponent>();
            if (battleProgressionC.BattlePoints >= battleProgressionC.BattlePointsNeeded)
            {
                battleProgressionC.BattlePoints -= battleProgressionC.BattlePointsNeeded;
                battleProgressionC.BattlePointsNeeded += 1;
                battleProgressionC.BattlePointsUpdated = true;
                WorldManagers.Get<SkillManager>(state.World).RunSkillSelectionFlow(3).Forget();
            }

            float prevTime = battleProgressionC.Timer;
            battleProgressionC.Timer += SystemAPI.Time.DeltaTime;
            battleProgressionC.BattleTimeUpdated = (int)prevTime != (int)battleProgressionC.Timer;
            SystemAPI.SetSingleton(battleProgressionC);
        }
    }
}