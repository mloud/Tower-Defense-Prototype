using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct DamageSystem : ISystem 
    {
        
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            
            foreach (var (damageC, hpC, teamC, entity) in 
                     SystemAPI.Query<
                         RefRW<DamageComponent>, 
                         RefRW<HpComponent>,
                         RefRO<TeamComponent>>().WithEntityAccess())
            {
                hpC.ValueRW.Hp = Mathf.Max(0, hpC.ValueRO.Hp - damageC.ValueRO.Damage);

                if (teamC.ValueRO.Team == Team.Player)
                {
                    WorldManagers.Get<BattleEventsManager>(state.World).UpdatePlayerHp(hpC.ValueRO);
                }
                
                // death
                if (hpC.ValueRO.Hp <= 0)
                {
                    ecb.AddComponent<DestroyComponent>(entity);
                }
                ecb.RemoveComponent<DamageComponent>(entity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}