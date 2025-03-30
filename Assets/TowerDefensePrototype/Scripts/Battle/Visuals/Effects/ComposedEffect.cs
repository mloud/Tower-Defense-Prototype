using System.Collections.Generic;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class ComposedEffect : BaseEffect
    {
        [SerializeField] private List<BaseEffect> effects;
        public override void Play(object data = null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Play();
            }
        }
    }
}