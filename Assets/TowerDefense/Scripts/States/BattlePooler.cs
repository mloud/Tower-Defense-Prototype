using System.Collections.Generic;
using System.Linq;
using Core.Modules.Ui.Loading;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Pooling;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;
using TowerDefensePrototype.Battle.Visuals.Effects;
using Unity.VisualScripting;

namespace TowerDefense.States
{
    public class BattlePooler
    {
        private Dictionary<string, Dictionary<string, int>> preloadedGroups = new();
        
        public async UniTask Pool(ILoading loading, StageDefinition definition, HeroDeck heroDeck)
        {  
            var poolManager = ServiceLocator.Get<IPoolManager>();
            var loadingTracker = new LoadingTracker(loading);

            preloadedGroups = new Dictionary<string, Dictionary<string, int>>();

            preloadedGroups.Add("Effects", new Dictionary<string, int>
            {
                { EffectKeys.HitEffectSmall, 50 },
                { EffectKeys.HitEffectAoeNormal, 10 },
                { EffectKeys.HpDamageText, 20 },
                { EffectKeys.SpawnEffectHero, 5 },
                { EffectKeys.SpawnEffectEnemy, 30 },
                { EffectKeys.AttackDistance, 10 },
                { EffectKeys.BossIncoming, 1 }
            });
             
            preloadedGroups.Add("Environment", new Dictionary<string, int>()
            {
                {definition.StageVisualKey, 1}
            });

            // hero deck
            var heroUnits = heroDeck.Heroes.ToDictionary(pair => pair.Key, hero => 1);
            if (heroUnits.ContainsKey("palisade"))
                heroUnits["palisade"] = 10;
            // part of environment
            heroUnits.Remove("barricade");
            preloadedGroups.Add("Hero Units", heroUnits);
           
            // enemy units
            var enemies = definition.Waves
                .Select(wave=>wave.EnemyId)
                .Distinct()
                .ToDictionary(
                    enemyId => enemyId, 
                    enemyId => enemyId.Contains("boss") ? 1 : enemyId.Contains("ellite") ? 8: 30);
          
            preloadedGroups.Add("Enemy units", enemies);
            
            
            // preloadedGroups.Add("Units", new Dictionary<string, int>
            // {
            //     {"wall", 1},
            //     {"zombie", 50},
            //     {"dragon", 50},
            //     {"archer", 30},
            //     {"boss", 1},
            //     {"boss_golem", 1},
            //     {"boss_dragon", 1},
            //     {"ellite_dragon", 5},
            // });

            preloadedGroups.Add("Projectiles", new Dictionary<string, int>()
            {
                { "projectile_dron", 20 },
                { "projectile_scorpion", 50 },
                { "projectile_soldier", 20 },
                { "projectile_turret", 20 },
                { "projectile_weapon", 20 },
                { "projectile_tank", 5 },
                { "projectile_archer", 10 },
                { "projectile_fireball", 4 },
                { "projectile_sniper", 4 }
            });
          
            foreach (var (group, keyCountPair) in preloadedGroups)
            {
                loadingTracker.RegisterPreloadGroup(poolManager, group, keyCountPair);
            }
          
            await loadingTracker.Execute();
        }

        public void Clear()
        {
            var poolManager = ServiceLocator.Get<IPoolManager>();
            var preloadedKeys = preloadedGroups.SelectMany(x => x.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
            foreach (var keys in preloadedKeys)
            {
                poolManager.ClearPool(keys.Key);
            }
        }
    }
}