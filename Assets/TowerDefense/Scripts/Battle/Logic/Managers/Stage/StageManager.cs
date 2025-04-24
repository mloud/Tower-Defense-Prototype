using System.Collections.Generic;
using System.Linq;
using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.EcsUtils;
using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Battle.Visuals;
using TowerDefense.Data.Definitions;
using TowerDefense.Managers;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Ui;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units
{
    public class StageManager : WorldManager
    {
        private IDataManager dataManager;
        private List<StageDefinition> stageDefinitions;

        public StageManager(World world) : base(world)
        {
            dataManager = ServiceLocator.Get<IDataManager>();
        }

        protected override async UniTask OnInitialize()
        {
            stageDefinitions = (await dataManager.GetAll<StageDefinition>()).ToList();
        }

        public async UniTask RunStageFinishedFlow(int killedEnemies, int totalEnemies, int stage, bool playerWon)
        {
            PauseUtils.SetLogicPaused(true, true);
            VisualManager.Default.PauseVisualObjects();
            VisualManager.Default.SetBattleMusicPlaying(false);
        
            var battleProgress01 = (float)killedEnemies / totalEnemies;
            var runtimeStageReward = await ServiceLocator.Get<IPlayerManager>().StageGetter.FinishBattle(stage, battleProgress01, playerWon);

            if (playerWon)
            {
                await WorldManagers.Get<UiHelperManager>(AttachedToWorld).OpenVictoryPopup(runtimeStageReward);
            }
            else
            {
                await WorldManagers.Get<UiHelperManager>(AttachedToWorld).OpenDefeatPopup(runtimeStageReward);
            }
        }
       
        public void CreateBattleStatisticEntity(ref EntityCommandBuffer ecb, int stage)
        {
            var statisticEntity = ecb.CreateEntity();
            ecb.AddComponent(statisticEntity, new BattleStatisticComponent
            {
                EnemiesKilled = 0,
                TotalEnemies = stageDefinitions[stage].Waves.Sum(x=>x.EnemiesCount)
            });
        }
        
        
        public void CreateWaveSpawner(ref EntityCommandBuffer ecb, int stage)
        {
            Debug.Assert(stage >= 0 && stage < stageDefinitions.Count, $"Stage {stage} is out of range");
           
            var spawnerEntity = ecb.CreateEntity();
            
            var spawnerComponent = new EnemySpawnerComponent 
            {
                spawnPosition = new float3(0, 0, 6),
                spawnBox = new float3(6,0,0.5f),
                currentWave = 0,
                currentWaveChanged = true,
            };

            spawnerComponent.waves = new FixedList4096Bytes<Wave>();
            foreach (var wave in stageDefinitions[stage].Waves)
            {
                spawnerComponent.waves.Add(new Wave
                {
                    EnemyId = wave.EnemyId,
                    EnemiesCount = wave.EnemiesCount,
                    SpawnInterval = wave.SpawnInterval,
                    Time = wave.Time
                });
            }
            ecb.AddComponent(spawnerEntity, spawnerComponent);
        }

        protected override void OnRelease()
        { }
    }
}