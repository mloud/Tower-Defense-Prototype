using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Visuals;
using CastlePrototype.Scripts.Ui.Popups;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Ui;
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
                    WorldManagers.Get<UiHelperManager>(state.World).OpenDefeatPopup().Forget();
                    VisualManager.Default.SetBattleMusicPlaying(false);
                    break;
                case 0 when enemySpawnerC.currentWave >= enemySpawnerC.waves.Length - 1:
                    PauseUtils.SetLogicPaused(true, true);
                    WorldManagers.Get<UiHelperManager>(state.World).OpenVictoryPopup().Forget();
                    VisualManager.Default.SetBattleMusicPlaying(false);
                    break;
            }
        }
    }
}