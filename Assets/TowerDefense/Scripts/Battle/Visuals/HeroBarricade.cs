using System.Collections.Generic;
using System.Linq;
using TowerDefense.Battle.Events;
using UnityEngine;

namespace TowerDefense.Battle.Visuals
{
    public class HeroBarricade : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> fireParticles;

        private List<ParticleSystem> playingParticles;
        private List<ParticleSystem> stoppedParticles;

        private void Awake()
        {
            stoppedParticles = fireParticles.ToList();
            playingParticles = new List<ParticleSystem>();
            fireParticles.ForEach(x=>x.Stop());
        }

        private void OnEnable()
        {
            PlayerHpChanged.Event.Subscribe(OnPlayerHpChanged);
        }

        private void OnDisable()
        {
            PlayerHpChanged.Event.UnSubscribe(OnPlayerHpChanged);
        }

        private void OnPlayerHpChanged(float hp, float total)
        {
            int particlesToPlay = (int)(Mathf.Clamp01(1-hp / total) * fireParticles.Count);

            if (particlesToPlay > playingParticles.Count)
            {
                for (int i = 0; i < particlesToPlay - playingParticles.Count; i++)
                {
                    stoppedParticles[^1].Play();
                    playingParticles.Add(stoppedParticles[^1]);
                    stoppedParticles.RemoveAt(stoppedParticles.Count - 1);
                }
            }
            else
            {
                for (int i = 0; i < playingParticles.Count - particlesToPlay; i++)
                {
                    playingParticles[^1].Stop();
                    stoppedParticles.Add(playingParticles[^1]);
                    playingParticles.RemoveAt(playingParticles.Count - 1);
                }
            }
        }
    }
}