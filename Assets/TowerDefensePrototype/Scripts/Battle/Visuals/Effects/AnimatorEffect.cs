using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class AnimatorEffect : BaseEffect
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string trigger;
        
        protected override void OnPlay(object data = null)
        {
            animator.SetTrigger(trigger);
        }
    }
}