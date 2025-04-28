using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Battle.Visuals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct DamageSystem : ISystem
    {
        private ComponentLookup<VisualComponent> visualLookup;
        private ComponentLookup<DestroyComponent> destroyLookup;
        public void OnCreate(ref SystemState state)
        {
            visualLookup = state.GetComponentLookup<VisualComponent>();
            destroyLookup = state.GetComponentLookup<DestroyComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            visualLookup.Update(ref state);
            destroyLookup.Update(ref state);
            
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            
            foreach (var (damageC, hpC, teamC, localTrC, entity) in 
                     SystemAPI.Query<
                         RefRW<DamageComponent>, 
                         RefRW<HpComponent>,
                         RefRO<TeamComponent>,
                         RefRW<LocalTransform>>()
                         .WithEntityAccess())
            {
                hpC.ValueRW.Hp = Mathf.Max(0, hpC.ValueRO.Hp - damageC.ValueRO.Damage);

                if (teamC.ValueRO.Team == Team.Player && state.EntityManager.HasComponent<BarricadeComponent>(entity))
                {
                    WorldManagers.Get<BattleEventsManager>(state.World).UpdatePlayerHp(hpC.ValueRO);
                }
                else
                {
                    if (visualLookup.HasComponent(entity))
                    {
                        VisualManager.Default.ShowDamage(visualLookup[entity].VisualIndex, damageC.ValueRO.Damage);
                    }
                }
                
                // death
                if (hpC.ValueRO.Hp <= 0)
                {
                    if (!destroyLookup.HasComponent(entity))
                    {
                        ecb.AddComponent<DestroyComponent>(entity);
                    }
                    else
                    {
                        Debug.Assert(false, "already has destroy component");
                    }
                }
                else
                {
                    if (damageC.ValueRO.Knockback && !state.EntityManager.HasComponent<KnockBackResistent>(entity))
                    {
                        localTrC.ValueRW.Position += new float3(0, 0, 0.2f);
                    }
                }
                ecb.RemoveComponent<DamageComponent>(entity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}