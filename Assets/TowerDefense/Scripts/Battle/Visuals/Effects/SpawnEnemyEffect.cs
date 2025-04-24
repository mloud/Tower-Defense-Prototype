using TowerDefense.Battle.Visuals.Effects;
using DG.Tweening;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Visuals.Effects
{
    public class SpawnEnemyEffect : BaseEffect
    {
        [SerializeField] private Transform modelTransform;
        
        protected override void OnPlay(object data = null)
        {
            var originalScale = modelTransform.localScale;
            modelTransform.localScale = Vector3.zero;
            Debug.Assert(destroyAfterFinished == false, "This effect could not be destroyed");
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.1f);
            sequence.Append(modelTransform.DOScale(originalScale, 0.3f).From(Vector3.zero));
            sequence.AppendCallback(() => { OnFinishedAction?.Invoke(); });
        }
    }
}