using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class ParticleEffect : BaseEffect
    {
        [SerializeField] private ParticleSystem particleSystem;
        public override void Play()
        {
            particleSystem.Play();
        }
    }
}