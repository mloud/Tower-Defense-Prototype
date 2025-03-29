using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Visuals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(VisualGroup))]
    public partial struct VisualObjectSystem : ISystem 
    {
        private static readonly int Speed = Animator.StringToHash("Speed");

        public void OnUpdate(ref SystemState state)
        {
            foreach (var visualC in SystemAPI.Query<RefRW<VisualComponent>>())
            {
                // visual not created
                if (visualC.ValueRW.HasVisual) continue;
                
                var visual = VisualManager.Default.OnUnitCreated(visualC.ValueRO.VisualId.ToString());
                visualC.ValueRW.VisualIndex = visual.Index;
                visualC.ValueRW.HasVisual = true;
            }

            // Sync visuals' position and rotation
            foreach (var (transform, visualComp) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualComp.ValueRO.VisualIndex);
                visual.transform.position = transform.ValueRO.Position + new float3(0, visual.DefaultHeight, 0);
                visual.transform.rotation = transform.ValueRO.Rotation;
            }   
            
            // Sync  visuals' speed
            foreach (var (moveC, visualC) in SystemAPI.Query<RefRO<MovementComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);
                visual.Animator.SetFloat(Speed, moveC.ValueRO.Speed / moveC.ValueRO.MaxSpeed);
            }   
            
            
            // Sync attack
            foreach (var (attackC, visualC) in SystemAPI.Query<RefRW<AttackComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);

                visual.SetAttackCooldown((float)(SystemAPI.Time.ElapsedTime - attackC.ValueRO.LastAttackTime) / attackC.ValueRO.AttackInterval);
              
                if (attackC.ValueRW.PlayAttack)
                {
                    visual.PlayEffect("Attack");
                    attackC.ValueRW.PlayAttack = false;
                }
            }  
        }
    }
}