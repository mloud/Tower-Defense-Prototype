using System.Collections.Generic;
using Core.Modules.Ui.Loading;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Pooling;
using TowerDefensePrototype.Battle.Visuals.Effects;

namespace CastlePrototype.States
{
    public class BattlePooler
    {
        public async UniTask Pool(ILoading loading)
        {  
            var poolManager = ServiceLocator.Get<IPoolManager>();
            var loadingTracker = new LoadingTracker(loading);
            
            loadingTracker.RegisterPreloadGroup(poolManager, "Effects", new Dictionary<string, int>
            {
                {EffectKeys.HitEffectSmall, 50},
                {EffectKeys.HitEffectAoeNormal, 10},
                {EffectKeys.HpDamageText, 20},
                {EffectKeys.SpawnEffectHero, 5},
                {EffectKeys.SpawnEffectEnemy, 30},
                {EffectKeys.AttackDistance, 10},

            } );
            
            loadingTracker.RegisterPreloadGroup(poolManager, "Environment", new Dictionary<string, int>
            {
                { "environment_1", 1},
            } );
            
            loadingTracker.RegisterPreloadGroup(poolManager, "Units", new Dictionary<string, int>
            {
                {"wall", 1},
                {"dron", 1},
                {"scorpion", 11},
                {"soldier", 1},
                {"turret", 1},
                {"weapon", 1},
                {"tank", 1},
                {"zombie", 50},
                {"dragon", 50},
                {"archer", 30},
                {"boss", 1},
                {"golem", 1},

            } );
            
            loadingTracker.RegisterPreloadGroup(poolManager, "Projectiles", new Dictionary<string, int>
            {
                {"projectile_dron", 20},
                {"projectile_scorpion", 50},
                {"projectile_soldier", 20},
                {"projectile_turret", 20},
                {"projectile_weapon", 20},
                {"projectile_tank", 5},
                {"projectile_archer", 10}
            } );


            await loadingTracker.Execute();
        }

        public void Clear()
        {
            var poolManager = ServiceLocator.Get<IPoolManager>();
            
            poolManager.ClearPool("environment_1");
            poolManager.ClearPool("dron");
            poolManager.ClearPool("dragon");
            poolManager.ClearPool("scorpion");
            poolManager.ClearPool("soldier");
            poolManager.ClearPool("turret");
            poolManager.ClearPool("weapon");
            poolManager.ClearPool("zombie");
            poolManager.ClearPool("boss");
            poolManager.ClearPool("archer");
            poolManager.ClearPool("wall");
            poolManager.ClearPool("golem");

            
            poolManager.ClearPool("projectile_dron");
            poolManager.ClearPool("projectile_scorpion");
            poolManager.ClearPool("projectile_soldier");
            poolManager.ClearPool("projectile_turret");
            poolManager.ClearPool("projectile_weapon");
            poolManager.ClearPool("projectile_tank");
            poolManager.ClearPool("projectile_archer");
            
            // effects
            poolManager.ClearPool(EffectKeys.HitEffectSmall);
            poolManager.ClearPool(EffectKeys.HitEffectAoeNormal);
            poolManager.ClearPool(EffectKeys.HpDamageText);
            poolManager.ClearPool(EffectKeys.SpawnEffectHero);
            poolManager.ClearPool(EffectKeys.SpawnEffectEnemy);
            poolManager.ClearPool(EffectKeys.AttackDistance);
        }
    }
}