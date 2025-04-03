using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class ComposedEffect : BaseEffect
    {
        [SerializeField] private List<BaseEffect> effects;
        protected override void OnPlay(object data = null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Play().Forget();
            }
        }
    }
}