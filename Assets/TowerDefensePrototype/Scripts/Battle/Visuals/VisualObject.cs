using System.Collections.Generic;
using CastlePrototype.Battle.Visuals.Effects;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public class VisualObject : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private List<BaseEffect> effects;
        [SerializeField] private float defaultHeight;

        [SerializeField] private ProgressBar hpProgressBar;
        [SerializeField] private ProgressBar cooldownProgressBar;
        
        protected static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Die = Animator.StringToHash("Die");

        public string Id => id;
        public float DefaultHeight => defaultHeight;
        public int Index { get; private set; }

        public Animator Animator { get; private set; }

        public void Initialize()
        {
            if (hpProgressBar != null)
            {
                hpProgressBar.PlaceToCanvas();
            }
            if (cooldownProgressBar != null)
            {
                cooldownProgressBar.PlaceToCanvas();
            }
        }
        public void Destroy(float time)
        {
            if (hpProgressBar != null)
            {
                hpProgressBar.ReturnFromCanvas();
                hpProgressBar.enabled = false;
            }
            if (cooldownProgressBar != null)
            {
                cooldownProgressBar.ReturnFromCanvas();
                cooldownProgressBar.enabled = false;
            }
            
            Destroy(gameObject, time);
            
        }

        public void SetAttackCooldown(float progress01)
        {
            if (cooldownProgressBar == null)
                return;
            
            cooldownProgressBar.SetProgress(progress01);
        }
        
        public void SetHp(float progress01)
        {
            if (hpProgressBar == null)
                return;
            
            hpProgressBar.SetProgress(progress01);
        }
        
        public bool PlayEffect(string effectId)
        {
            bool found = false;
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].Id == effectId)
                {
                    effects[i].Play();
                    found = true;
                }
            }

            return found;
        }
        
        private void Awake()
        {
            Index = VisualManager.Default.TrackVisualObject(this);
            Animator = GetComponentInChildren<Animator>();
        }

        private void OnDestroy()
        {
            VisualManager.Default?.UnTrackVisualObject(this);
        }
    }
}