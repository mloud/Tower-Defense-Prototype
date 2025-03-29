using CastlePrototype.Battle.Logic.Components;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct MovementSystem : ISystem 
    {
        public void OnUpdate(ref SystemState state)
        {
            var moveDirection = new float3(0,0,-1);
            foreach (var (moveComponent, transform, attackComponent) in SystemAPI.Query<RefRW<MovementComponent>, RefRW<LocalTransform>, RefRO<AttackComponent>>())
            {
                if (!attackComponent.ValueRO.IsInAttackDistance)
                {
                    transform.ValueRW.Position +=
                        moveDirection * moveComponent.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                    transform.ValueRW.Rotation = Quaternion.LookRotation(moveDirection);
                    moveComponent.ValueRW.Speed = moveComponent.ValueRO.MaxSpeed;
                }
                else
                {
                    moveComponent.ValueRW.Speed = 0;
                }
            }
        }
    }
}