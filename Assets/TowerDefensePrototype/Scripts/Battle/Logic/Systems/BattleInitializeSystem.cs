using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Logic.Managers.Skills;
using CastlePrototype.Battle.Logic.Managers.Slots;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct BattleInitializeSystem : ISystem
    {
        private bool finished;
  
        public void OnUpdate(ref SystemState state)
        {
            if (finished) return;
         
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            CreateBattleProgression(ref state);
            CreateEnemySpawner(ref state);
            var wallEntity = CreateWall(ref state);
            WorldManagers.Get<BattleEventsManager>(state.World)
                .UpdatePlayerHp(state.EntityManager.GetComponentData<HpComponent>(wallEntity));
            
            var slot = WorldManagers.Get<SlotManager>(state.World).GetInitialSlot();
            HeroFactoryUtils.CreateHeroFromArchetype(ref ecb, "weapon_default", slot.Position);
            slot.IsOccupied = true;
            
            finished = true;
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private void CreateEnemySpawner(ref SystemState state)
        {
            var spawnerEntity = state.EntityManager.CreateEntity();
            
            var spawnerComponent = new EnemySpawnerComponent 
            {
                spawnPosition = new float3(0, 0, 7),
                spawnBox = new float3(6,0,3),
                spawnInterval = 0.1f,
                waveInterval = 20f,
                enemiesPerWave = 10,
                totalWaves = 3,
                currentWave = 0,
                currentWaveChanged = true,
                enemyId = new FixedString64Bytes("boss_default")
            };
            
            state.EntityManager.AddComponentData(spawnerEntity, spawnerComponent);
        }
        private Entity CreateWall(ref SystemState state)
        {
            var wallEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(wallEntity, new HpComponent { Hp = 50, MaxHp = 50 });
            state.EntityManager.AddComponentData(wallEntity, new LocalTransform { Position = new float3(0, 0, -5) });
            state.EntityManager.AddComponentData(wallEntity, new TeamComponent { Team = Team.Player });
            state.EntityManager.AddComponentData(wallEntity, new SettingComponent { DistanceAxes = new float3(0, 0, 1) });
            state.EntityManager.AddComponentData(wallEntity, new VisualComponent { VisualId = "wall" });
            return wallEntity;
        }

        private void CreateBattleProgression(ref SystemState state)
        {
            var progressEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(progressEntity, new BattleProgressionComponent
            {
                BattlePoints = 0,
                BattlePointsNeeded = 5,
                BattlePointsUpdated = true
            });
        }
    }
}