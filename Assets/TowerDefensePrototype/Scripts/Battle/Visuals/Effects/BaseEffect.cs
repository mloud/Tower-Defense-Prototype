using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public abstract class BaseEffect : MonoBehaviour
    {
        public string Id => id;
        [SerializeField] private string id;
        public abstract void Play();
    }
}