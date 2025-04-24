using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Battle.Logic.Managers.Slots;
using TowerDefense.Battle.Visuals;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct BattleInitializeSystem : ISystem
    {
        private bool finished;
        public static int stage = 0;
        public static string stageName;
        
        public void OnUpdate(ref SystemState state)
        {
            if (finished) return;
         
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            CreateBattleFieldComponent(ref state);
            CreateBattleProgression(ref state, stage);

            WorldManagers.Get<StageManager>(state.World).CreateWaveSpawner(ref ecb, stage);
            WorldManagers.Get<StageManager>(state.World).CreateBattleStatisticEntity(ref ecb, stage);


            var barricadePosition = VisualManager.Default.GetObjectPosition("barricade");
            var wallEntity = WorldManagers.Get<UnitManager>(state.World).CreateBarricade(ref state, barricadePosition);
            WorldManagers.Get<BattleEventsManager>(state.World)
                .UpdatePlayerHp(state.EntityManager.GetComponentData<HpComponent>(wallEntity));
            
            WorldManagers.Get<BattleEventsManager>(state.World)
                .UpdateStage(stageName, stage);

            
            var slot = WorldManagers.Get<SlotManager>(state.World).GetInitialSlot();
            WorldManagers.Get<UnitManager>(state.World).CreateHeroUnit(ref ecb, slot.Position, "weapon");
            slot.IsOccupied = true;
            
            VisualManager.Default.SetBattleMusicPlaying(true);
            
            finished = true;
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        

        private void CreateBattleProgression(ref SystemState state, int stage)
        {
            var progressEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(progressEntity, new BattleProgressionComponent
            {
                BattlePoints = 0,
                BattlePointsNeeded = 3,
                BattlePointsUpdated = true,
                Stage = stage
            });
        }
       
        private void CreateBattleFieldComponent(ref SystemState state)
        {
            var fieldEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(fieldEntity, new BattleFieldComponent()
            {
                MinCorner = new float2(-8.7f/2, -16.54f/2),
                MaxCorner = new float2(8.7f/2, 15.54f/2),
            });
        }
    }
}