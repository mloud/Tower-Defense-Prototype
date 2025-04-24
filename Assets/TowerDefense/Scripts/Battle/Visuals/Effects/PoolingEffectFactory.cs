using TowerDefense.Battle.Visuals.Effects;
using OneDay.Core.Modules.Pooling;
using UnityEngine;

namespace TowerDefense.Battle.Visuals.Effects
{
    public class PoolingEffectFactory : IEffectFactory
    {
        private IPoolManager poolManager;

        public PoolingEffectFactory(IPoolManager poolManager) => this.poolManager = poolManager;
        public T Create<T>(string id) where T : BaseEffect
        {
            var effect = poolManager.GetSync(id);
            Debug.Assert(effect != null, $"Cannot create effect with id {id}");
            return effect.GetComponent<T>();
        }
        
        public void Release(BaseEffect effect)
        {
            poolManager.Return(effect.gameObject);
        }
    }
}