using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Logic.Managers.Skills;
using CastlePrototype.Battle.Logic.Managers.Slots;
using CastlePrototype.Battle.Visuals;
using Cysharp.Threading.Tasks;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
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
            CreateBattleFieldComponent(ref state);
            CreateBattleProgression(ref state);

            WorldManagers.Get<StageManager>(state.World).CreateWaveSpawner(ref ecb, 0);


            var barricadePosition = VisualManager.Default.GetObjectPosition("barricade");
            var wallEntity = WorldManagers.Get<UnitManager>(state.World).CreateBarricade(ref state, barricadePosition, "barricade");
            WorldManagers.Get<BattleEventsManager>(state.World)
                .UpdatePlayerHp(state.EntityManager.GetComponentData<HpComponent>(wallEntity));
            
            var slot = WorldManagers.Get<SlotManager>(state.World).GetInitialSlot();
            WorldManagers.Get<UnitManager>(state.World).CreateHeroUnit(ref ecb, slot.Position, "weapon");
            slot.IsOccupied = true;
            
            VisualManager.Default.SetBattleMusicPlaying(true);
            
            finished = true;
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        

        private void CreateBattleProgression(ref SystemState state)
        {
            var progressEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(progressEntity, new BattleProgressionComponent
            {
                BattlePoints = 0,
                BattlePointsNeeded = 3,
                BattlePointsUpdated = true
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