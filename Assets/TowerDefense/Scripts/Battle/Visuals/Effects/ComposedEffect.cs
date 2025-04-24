using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TowerDefense.Battle.Visuals.Effects
{
    public class ComposedEffect : BaseEffect
    {
        [SerializeField] private List<BaseEffect> effects;
        protected override void OnPlay(object data = null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                StartCoroutine(effects[i].Play());
            }
        }
    }
}