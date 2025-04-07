using OneDay.Core.Modules.Pooling;
using UnityEngine;
using UnityEngine.Rendering;

namespace CastlePrototype.Battle.Visuals
{
    public class PoolingVisualFactory : IVisualFactory
    {
        private IPoolManager poolManager;

        public PoolingVisualFactory(IPoolManager poolManager) => this.poolManager = poolManager;

        public T Create<T>(string id) where T : VisualObject
        {
            var effect = poolManager.GetSync(id);
            Debug.Assert(effect != null, $"Cannot create visual with id {id}");
            return effect.GetComponent<T>();
        }

        public void Release(VisualObject visualObject)
        {
            if (visualObject.Key == null)
            {
                Object.Destroy(visualObject.gameObject);
            }
            else
            {
                poolManager.Return(visualObject.gameObject);
            }
        }
    }
}