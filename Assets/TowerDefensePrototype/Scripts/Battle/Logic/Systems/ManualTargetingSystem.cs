using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Visuals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{ 
    [DisableAutoCreation]
    public partial struct ManualTargetingSystem : ISystem
    {
        private ComponentLookup<LocalTransform> transformLookup;
        private ComponentLookup<VisualComponent> visualLookup;
        private ComponentLookup<TargetComponent> targetLookup;
        private ComponentLookup<TargetedComponent> targetedLookup;
      
        public void OnCreate(ref SystemState state)
        {
            transformLookup = state.GetComponentLookup<LocalTransform>();
            visualLookup = state.GetComponentLookup<VisualComponent>();
            targetLookup = state.GetComponentLookup<TargetComponent>();
            targetedLookup = state.GetComponentLookup<TargetedComponent>();
            state.RequireForUpdate<WeaponComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            visualLookup.Update(ref state);
            targetLookup.Update(ref state);

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            
            if (Input.GetMouseButton(0))
            {
                var weaponEntity = SystemAPI.GetSingletonEntity<WeaponComponent>();
                var weaponScreenPos = VisualManager.Default.MainCamera.WorldToScreenPoint(transformLookup[weaponEntity].Position);
                var mousePos = Input.mousePosition;
                var screenDir = (mousePos - weaponScreenPos).normalized;
                var direction = new float3(screenDir.x, 0f, screenDir.y);
                var rotation = quaternion.LookRotationSafe(direction, math.up());


                if (!state.EntityManager.HasComponent<ManualTargetingComponent>(weaponEntity))
                {
                    ecb.AddComponent<ManualTargetingComponent>(weaponEntity);
                    
                    if (!targetLookup.HasComponent(weaponEntity))
                        ecb.AddComponent<TargetComponent>(weaponEntity);

                    if (targetLookup.HasComponent(weaponEntity))
                    {
                        targetLookup.GetRefRW(weaponEntity).ValueRW.Target = Entity.Null;
                    }
                }

                transformLookup.GetRefRW(weaponEntity).ValueRW.Rotation = rotation;
                
                VisualManager.Default.GetVisualObject(visualLookup[weaponEntity].VisualIndex).SetGameObjectActive("TargetingLine", true);
                
            }
            else if (Input.GetMouseButtonUp(0))
            {
                
                var weaponEntity = SystemAPI.GetSingletonEntity<WeaponComponent>();
                VisualManager.Default.GetVisualObject(visualLookup[weaponEntity].VisualIndex).SetGameObjectActive("TargetingLine", false);
                if (state.EntityManager.HasComponent<ManualTargetingComponent>(weaponEntity))
                    ecb.RemoveComponent<ManualTargetingComponent>(weaponEntity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}