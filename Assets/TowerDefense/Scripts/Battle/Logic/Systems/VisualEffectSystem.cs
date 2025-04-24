using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Visuals;
using Unity.Entities;

namespace TowerDefense.Battle.Logic.Systems
{ 
    [DisableAutoCreation]
    [UpdateInGroup(typeof(VisualGroup))]
    public partial struct VisualEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (effectC, entity) in SystemAPI.Query<RefRO<TriggerVisualEffectComponent>>()
                         .WithEntityAccess())
            {
                VisualManager.Default.PlayEffect(effectC.ValueRO.EffectId.ToString(), effectC.ValueRO.Position);
                ecb.DestroyEntity(entity);
            }   
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}