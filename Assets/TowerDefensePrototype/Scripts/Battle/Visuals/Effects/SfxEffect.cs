using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public class SfxEffect : BaseEffect
    {
        [SerializeField] private AudioClip clip;
        
        public override void Play(object data = null)
        {
            AudioSource.PlayClipAtPoint(clip,Vector3.zero);
        }
    }
}