using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Visuals.Effects;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public interface IEffectFactory
    {
        T Create<T>(string id) where T : BaseEffect;
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
    }
}