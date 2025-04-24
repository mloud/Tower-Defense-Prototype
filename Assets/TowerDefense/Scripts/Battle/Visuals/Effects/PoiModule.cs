using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Visuals.Effects
{
    public class PoiModule : MonoBehaviour
    {
        [SerializeField] private List<Transform> registeredPois;

        private Dictionary<string, Transform> registeredPoisDict;
        private void Awake()
        {
            registeredPoisDict = registeredPois.ToDictionary(x => x.gameObject.name, x => x);
        }

        public Vector3? GetPoiPosition(string poiName)
        {
            if (registeredPoisDict.TryGetValue(poiName, out var tr))
            {
                return tr.position;
            }
            else
            {
                return null;
            }
        }
    }
}