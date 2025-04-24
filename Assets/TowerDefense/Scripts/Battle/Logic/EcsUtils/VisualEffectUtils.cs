using TowerDefense.Battle.Logic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.EcsUtils
{
    public static class VisualEffectUtils
    {
        public static void PlayEffect(
            ref SystemState state, 
            ref EntityCommandBuffer ecb, 
            float3 position, 
            FixedString64Bytes effectId)
        {
            var entity = ecb.CreateEntity();
            
            ecb.AddComponent(entity, new TriggerVisualEffectComponent
            {
                Position = position,
                EffectId = effectId
            });
        }
    }
}