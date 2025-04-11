using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using Cysharp.Threading.Tasks;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Entities;
using UnityEngine;


namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct BattleResolveSystem : ISystem
    {
        private bool battleFinishResolveInProgress;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BattleStatisticComponent>();
            state.RequireForUpdate<EnemySpawnerComponent>();
            state.RequireForUpdate<BattleProgressionComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            if (battleFinishResolveInProgress)
                return;
            
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
            var battleStatisticC = SystemAPI.GetSingleton<BattleStatisticComponent>();
            var battleProgressionC = SystemAPI.GetSingleton<BattleProgressionComponent>();
            
            bool isLastWave = enemySpawnerC.currentWave >= enemySpawnerC.waves.Length - 1;
            int killedEnemies = battleStatisticC.EnemiesKilled;
            int totalEnemies = battleStatisticC.TotalEnemies;
            
#if UNITY_EDITOR
            // cheats
            if (Input.GetKeyDown(KeyCode.W))
            {
                aliveEnemies = 0;
                killedEnemies = totalEnemies;
                isLastWave = true;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                playerTotalHp = 0;
            }
#endif

            switch (aliveEnemies)
            {
                case > 0 when playerTotalHp <= 0:

                    WorldManagers.Get<StageManager>(state.World)
                        .RunStageFinishedFlow(killedEnemies, totalEnemies,battleProgressionC.Stage)
                        .Forget();
                    battleFinishResolveInProgress = true;
                    break;
                case 0 when isLastWave:
                    WorldManagers.Get<StageManager>(state.World).RunStageFinishedFlow
                            (killedEnemies, totalEnemies, battleProgressionC.Stage)
                        .Forget();
                    battleFinishResolveInProgress = true;
                    break;
            }
        }
    }
}