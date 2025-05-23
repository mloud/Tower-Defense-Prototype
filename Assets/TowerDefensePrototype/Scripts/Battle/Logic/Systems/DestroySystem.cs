using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Visuals;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct DestroyEntitySystem : ISystem
    {
        private ComponentLookup<VisualComponent> visualLookup;
        private ComponentLookup<TeamComponent> teamLoopkup;
   
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BattleProgressionComponent>();
            visualLookup = state.GetComponentLookup<VisualComponent>(true);
            teamLoopkup = state.GetComponentLookup<TeamComponent>(true);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            visualLookup.Update(ref state);
            teamLoopkup.Update(ref state);
            
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            int battlePoints = 0;
            
            foreach (var (destroyC, entity) in
                     SystemAPI.Query<
                         RefRW<DestroyComponent>>().WithEntityAccess())
            {
                destroyC.ValueRW.DestroyIn -= deltaTime;
                if (!(destroyC.ValueRW.DestroyIn <= 0)) continue;
                
                if (visualLookup.HasComponent(entity))
                {
                    VisualManager.Default.DestroyVisualObject(visualLookup.GetRefRO(entity).ValueRO.VisualIndex);
                }

                if (teamLoopkup.HasComponent(entity) && teamLoopkup[entity].Team == Team.Enemy)
                {
                    battlePoints += 1;
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