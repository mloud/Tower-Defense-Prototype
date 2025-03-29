using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Scripts.Ui.Popups;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using Unity.Entities;


namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct BattleResolveSystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnerComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            // Player lost
            float playerTotalHp = 0;
            int aliveEnemies = 0;
            foreach (var (hpC, teamC) in SystemAPI.Query<RefRO<HpComponent>,RefRO<TeamComponent> >())
            {
                if (teamC.ValueRO.Team == Team.Player)
                {
                    playerTotalHp += hpC.ValueRO.Hp;
                }
                else if (teamC.ValueRO.Team == Team.Enemy && hpC.ValueRO.Hp > 0)
                {
                    aliveEnemies++;
                }
            }

            var enemySpawnerC = SystemAPI.GetSingleton<EnemySpawnerComponent>();
            switch (aliveEnemies)
            {
                case > 0 when playerTotalHp <= 0:
                    PauseUtils.SetLogicPaused(true);
                    ServiceLocator.Get<IUiManager>().OpenPopup<DefeatPopup>(null);
                    break;
                case 0 when enemySpawnerC.currentWave >= enemySpawnerC.totalWaves:
                    PauseUtils.SetLogicPaused(true);
                    ServiceLocator.Get<IUiManager>().OpenPopup<VictoryPopup>(null);
                    break;
            }
        }
    }
}