using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class ParticleEffect : BaseEffect
    {
        [SerializeField] private ParticleSystem particleSystem;
        protected override void OnPlay(object data = null)
        {
            particleSystem.Play();
        }
    }
}