using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Visuals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(VisualGroup))]
    public partial struct VisualObjectSystem : ISystem 
    {
      
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
                visual.SetPosition(transform.ValueRO.Position);
                visual.SetRotation(transform.ValueRO.Rotation);
            }   
            
            // Sync  visuals' speed
            foreach (var (moveC, visualC) in SystemAPI.Query<RefRO<MovementComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);
                
                visual.SetMoveSpeed(moveC.ValueRO.Speed / moveC.ValueRO.MaxSpeed);
            }   
            
            // Sync  visuals' hp
            foreach (var (hpC, visualC) in SystemAPI.Query<RefRO<HpComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);
                visual.SetHp(hpC.ValueRO.Hp / hpC.ValueRO.MaxHp);
            } 
            
            
            // Sync attack
            foreach (var (attackC, visualC) in SystemAPI.Query<RefRW<AttackComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);

                visual.SetAttackCooldown((float)(SystemAPI.Time.ElapsedTime - attackC.ValueRO.LastAttackTime) / attackC.ValueRO.Cooldown);
              
                if (attackC.ValueRW.PlayAttack)
                {
                    visual.Attack();
                    attackC.ValueRW.PlayAttack = false;
                }
            }  
        }
    }
}