using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Visuals.Effects;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public interface IVisualFactory
    {
        T Create<T>(string id) where T : VisualObject;
    }
 
    public class PrefabVisualFactory : MonoBehaviour, IVisualFactory
    {
        [SerializeField] private List<VisualObject> visualObjects;
        
        public T Create<T>(string id) where T: VisualObject
        {
            var prefab = visualObjects.FirstOrDefault(x => x.Id == id);
            Debug.Assert(prefab != null, $"No such Visual Object prefab with id: {id} exists!!");
            return (T)Instantiate(prefab);
        }
    }
}