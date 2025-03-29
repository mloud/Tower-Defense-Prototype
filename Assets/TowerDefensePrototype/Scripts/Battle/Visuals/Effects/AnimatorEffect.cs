using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class AnimatorEffect : BaseEffect
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string trigger;
        
        public override void Play()
        {
            animator.SetTrigger(trigger);
        }
    }
}