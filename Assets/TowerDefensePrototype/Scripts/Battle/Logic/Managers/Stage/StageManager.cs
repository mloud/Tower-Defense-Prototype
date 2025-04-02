using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Data.Definitions;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
        

        public Entity CreateWaveSpawner(ref EntityCommandBuffer ecb, int stage)
        {
            Debug.Assert(stage >= 0 && stage < stageDefinitions.Count, $"Stage {stage} is out of range");
           
            var spawnerEntity = ecb.CreateEntity();
            
            var spawnerComponent = new EnemySpawnerComponent 
            {
                spawnPosition = new float3(0, 0, 7),
                spawnBox = new float3(6,0,3),
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
            return spawnerEntity;
        }
        protected override void OnRelease()
        { }
    }
}