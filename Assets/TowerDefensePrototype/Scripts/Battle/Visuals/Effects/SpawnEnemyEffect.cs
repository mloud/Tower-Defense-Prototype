using CastlePrototype.Battle.Visuals.Effects;
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
            Debug.Assert(destroyAfterFinished == false, "This effect could not be destroyed");
            var sequence = DOTween.Sequence();
            sequence.AppendCallback( ()=>modelTransform.gameObject.SetActive(false));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback( ()=>modelTransform.gameObject.SetActive(true));
            sequence.Append(modelTransform.DOScale(originalScale, 0.3f).From(Vector3.zero));
            sequence.AppendCallback(() => { OnFinishedAction?.Invoke(); });
        }
    }
}