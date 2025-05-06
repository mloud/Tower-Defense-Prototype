using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Visuals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Systems
{ 
    [DisableAutoCreation]
    public partial struct ManualTargetingSystem : ISystem
    {
        private ComponentLookup<LocalTransform> transformLookup;
        private ComponentLookup<VisualComponent> visualLookup;
        private ComponentLookup<TargetComponent> targetLookup;
        private ComponentLookup<TargetedComponent> targetedLookup;

        private bool isButtonDown;
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

            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;
                var viewportPoint = VisualManager.Default.MainCamera.ScreenToViewportPoint(mousePos);

                if (viewportPoint.y < 0.2f)
                    return;

                isButtonDown = true;

            }
            else if (Input.GetMouseButton(0))
            {
                if (!isButtonDown)
                    return;
                isButtonDown = true;
                
                var mousePos = Input.mousePosition;
                var weaponEntity = SystemAPI.GetSingletonEntity<WeaponComponent>();
                var weaponScreenPos = VisualManager.Default.MainCamera.WorldToScreenPoint(transformLookup[weaponEntity].Position);
              
                var screenDir = (mousePos - weaponScreenPos).normalized;
                var direction = new float3(screenDir.x, 0f, screenDir.y);
                var rotation = quaternion.LookRotationSafe(direction, math.up());


                if (!state.EntityManager.HasComponent<ManualTargetingComponent>(weaponEntity))
                {
                    ecb.AddComponent<ManualTargetingComponent>(weaponEntity);
                    Debug.Log("XXXX adding ManualTargetingComponent");
                    
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
                if (!isButtonDown)
                    return;
                isButtonDown = false;
                var weaponEntity = SystemAPI.GetSingletonEntity<WeaponComponent>();
                VisualManager.Default.GetVisualObject(visualLookup[weaponEntity].VisualIndex).SetGameObjectActive("TargetingLine", false);
                if (state.EntityManager.HasComponent<ManualTargetingComponent>(weaponEntity))
                {
                    ecb.RemoveComponent<ManualTargetingComponent>(weaponEntity);
                    Debug.Log("XXXX removing ManualTargetingComponent");
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}