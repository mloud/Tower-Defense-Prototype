using System.Collections.Generic;
using System.Linq;
using TowerDefense.Battle.Visuals.Effects;
using UnityEngine;

namespace TowerDefense.Battle.Visuals.Effects
{
    public interface IEffectFactory
    {
        T Create<T>(string id) where T : BaseEffect;
        void Release(BaseEffect effect);
    }
    public class PrefabEffectFactory : MonoBehaviour, IEffectFactory
    {
        [SerializeField] private List<BaseEffect> effectPrefabs;

        public T Create<T>(string id) where T : BaseEffect
        {
            var prefab = effectPrefabs.FirstOrDefault(x => x.Id == id);
            Debug.Assert(prefab != null, $"No such Effect prefab with id: {id} exists!!");
            return (T)Instantiate(prefab);
        }
        
        public void Release(BaseEffect effect)
        {
            Destroy(effect.gameObject);
        }
    }
}