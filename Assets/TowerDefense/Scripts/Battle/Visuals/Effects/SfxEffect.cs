using UnityEngine;

namespace TowerDefense.Battle.Visuals.Effects
{
    public class SfxEffect : BaseEffect
    {
        [SerializeField] private AudioClip clip;
        
        protected override void OnPlay(object data = null)
        {
            AudioSource.PlayClipAtPoint(clip,Vector3.zero);
        }
    }
}