using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Visuals;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


namespace TowerDefense.Battle.Logic.Systems
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

                if (visualC.ValueRO.Level > 0)
                {
                    visual.SetLevel(visualC.ValueRO.Level);
                }
            }

            // Sync visuals' position and rotation
            foreach (var (transform, visualComp) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<VisualComponent>>())
            {
                if (!visualComp.ValueRO.HasVisual) continue;
                var visual = VisualManager.Default.GetVisualObject(visualComp.ValueRO.VisualIndex);
                if (visual != null)
                {
                    visual.SetPosition(transform.ValueRO.Position);
                    visual.SetRotation(transform.ValueRO.Rotation);
                }
                else
                {
                    Debug.LogError($"Could not find Visual object with visual index {visualComp.ValueRO.VisualIndex}");
                }
            }   
            
            // Sync  visuals' speed
            foreach (var (moveC, visualC) in SystemAPI.Query<RefRO<MovementComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);
                if (visual == null)
                    continue;
                visual.SetMoveSpeed(moveC.ValueRO.Speed / moveC.ValueRO.MaxSpeed);
            }

            // Sync  visuals' hp
            foreach (var (hpC, visualC) in SystemAPI.Query<RefRO<HpComponent>, RefRO<VisualComponent>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);
                if (visual == null)
                    continue;
                visual.SetHp(hpC.ValueRO.Hp / hpC.ValueRO.MaxHp);
            } 
            
            
            // Sync attack
            foreach (var (attackC, visualC, localC) in SystemAPI.Query<RefRW<AttackComponent>, RefRO<VisualComponent>,  RefRO<LocalTransform>>())
            {
                var visual = VisualManager.Default.GetVisualObject(visualC.ValueRO.VisualIndex);

                if (visual == null)
                    continue;
                
                visual.SetAttackCooldown(1-(float)attackC.ValueRO.NextMainAttackTime / attackC.ValueRO.Cooldown);
              
                if (attackC.ValueRW.PlayAttack)
                {
                    visual.Attack();
                    attackC.ValueRW.PlayAttack = false;
                }

                if (visual.TriggerAttackDistanceShow)
                {
                    visual.ShowAttackDistance(attackC.ValueRO.AttackDistance);
                }
            }  
        }
    }
}