using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Visuals;
using Unity.Entities;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct DestroyEntitySystem : ISystem
    {
        private ComponentLookup<VisualComponent> visualLookup;
        private ComponentLookup<TeamComponent> teamLoopkup;
        private ComponentLookup<UnitComponent> unitLookup;

   
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BattleStatisticComponent>();
            state.RequireForUpdate<BattleProgressionComponent>();
            visualLookup = state.GetComponentLookup<VisualComponent>(true);
            teamLoopkup = state.GetComponentLookup<TeamComponent>(true);
            unitLookup = state.GetComponentLookup<UnitComponent>(true);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            visualLookup.Update(ref state);
            teamLoopkup.Update(ref state);
            unitLookup.Update(ref state);
            
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            int battlePoints = 0;
            
            foreach (var (destroyC, entity) in
                     SystemAPI.Query<
                         RefRW<DestroyComponent>>().WithEntityAccess())
            {
                destroyC.ValueRW.DestroyIn -= deltaTime;
                if (destroyC.ValueRW.DestroyIn > 0) continue;

                if (visualLookup.HasComponent(entity))
                {
                    if (visualLookup.GetRefRO(entity).ValueRO.HasVisual)
                    {
                        VisualManager.Default.DestroyVisualObject(visualLookup.GetRefRO(entity).ValueRO.VisualIndex);
                    }
                }

                if (teamLoopkup.HasComponent(entity) && teamLoopkup[entity].Team == Team.Enemy && unitLookup.HasComponent(entity))
                {
                    battlePoints += 1;
                    var battleStatisticComponent = SystemAPI.GetSingleton<BattleStatisticComponent>();
                    battleStatisticComponent.EnemiesKilled++;
                    Debug.Log($"XXX Destroying entity with def:: {unitLookup[entity].DefinitionId}");
                    Debug.Log($"XXX Enemies killed {battleStatisticComponent.EnemiesKilled} / {battleStatisticComponent.TotalEnemies}");
                    if (VisualManager.Default.GetVisualObject(visualLookup.GetRefRO(entity).ValueRO.VisualIndex) == null)
                    {
                        Debug.Assert(false ,$"XXX does not gave visual object");

                    }
                    Debug.Log($"XXX =====");
                    SystemAPI.SetSingleton(battleStatisticComponent);
                }

                ecb.DestroyEntity(entity);
            }

            if (battlePoints > 0)
            {
                if (SystemAPI.HasSingleton<BattleProgressionComponent>())
                {
                    var progressionC = SystemAPI.GetSingleton<BattleProgressionComponent>();
                    progressionC.BattlePoints += battlePoints;
                    progressionC.BattlePointsUpdated = true;
                    SystemAPI.SetSingleton(progressionC);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}