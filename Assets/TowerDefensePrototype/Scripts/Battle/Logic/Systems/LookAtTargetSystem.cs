using CastlePrototype.Battle.Logic.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct LookAtTargetSystem :ISystem
    {
        const float RotationSpeed = 20;
        private ComponentLookup<LocalTransform> transformLookup;
       
        public void OnCreate(ref SystemState state)
        {
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            
            foreach (var (transformC,targetC, lookAtC) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<TargetComponent>, RefRO<LookAtTargetComponent>>())
            {
                if (targetC.ValueRO.Target == Entity.Null)
                    continue;
                if (!lookAtC.ValueRO.LookAtTarget)
                    continue;
                
                //look at target
                var direction = transformLookup[targetC.ValueRO.Target].Position - transformC.ValueRO.Position;
                var targetRotation = quaternion.LookRotation(direction, new float3(0,1,0));

                
                // ensures smooth interpolation over time, regardless of FPS.
                transformC.ValueRW.Rotation = math.slerp(
                    transformC.ValueRW.Rotation, 
                    targetRotation, 
                    1f - math.exp(-RotationSpeed * SystemAPI.Time.DeltaTime));
                //transformC.ValueRW.Rotation = math.slerp(transformC.ValueRW.Rotation, targetRotation, 0.1f);
            }
        }
    }
}