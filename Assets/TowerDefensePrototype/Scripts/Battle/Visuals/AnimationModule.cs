using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public class AnimationModule : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [Serializable]
        public class AnimationSettings
        {
            public string Name;
            public float Value;
        }

        [SerializeField] private List<AnimationSettings> settings;
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Die = Animator.StringToHash("Die");
        public static readonly int Speed = Animator.StringToHash("Speed");
   

        private HashSet<int> cachedParameters;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                cachedParameters = animator.parameters.Select(x => x.nameHash).ToHashSet();
                settings.ForEach(x => animator.SetFloat(x.Name, x.Value));
            }
        }

        public bool PlayAttack() => SetTrigger(Attack);
        public bool PlayDeath() => SetTrigger(Die);
        public void SetMoveSpeed(float speed) => SetFloat(Speed, speed);
        public bool HasTrigger(int id) => cachedParameters.Contains(id);

        private bool SetTrigger(int triggerId)
        {
            if (animator == null)
                return false;
            if (cachedParameters == null || !cachedParameters.Contains(triggerId))
                return false;

            animator.SetTrigger(triggerId);
            return true;
        }

  
    public void SetFloat(int id, float value) => animator.SetFloat(id, value);
    }
}