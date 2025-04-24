using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Battle.Visuals.Effects
{
    public class EffectModule : MonoBehaviour
    {
        [SerializeField] private List<BaseEffect> effects;

        public bool PlayEffect(string effectId)
        {
            bool found = false;
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].Id == effectId)
                {
                    StartCoroutine(effects[i].Play());
                    found = true;
                }
            }

            return found;
        }
    }
}