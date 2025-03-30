using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public abstract class BaseEffect : MonoBehaviour
    {
        public bool IsScreenSpace => isScreenSpace;
        [SerializeField] private bool isScreenSpace;
        public string Id => id;
        [SerializeField] private string id;
        public abstract void Play(object data = null);
    }
}