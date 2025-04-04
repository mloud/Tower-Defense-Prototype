using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class ParticleEffect : BaseEffect
    {
        [SerializeField] private ParticleSystem particleSystem;
        protected override void OnPlay(object data = null)
        {
            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
            particleSystem.Play();
        }

        private void OnParticleSystemStopped()
        {
            OnFinishedAction?.Invoke();
            if (destroyAfterFinished)
            {
                Destroy(gameObject);
            }
        }
    }
}