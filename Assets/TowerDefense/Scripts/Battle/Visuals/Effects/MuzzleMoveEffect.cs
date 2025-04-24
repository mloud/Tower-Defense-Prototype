using TowerDefense.Battle.Visuals.Effects;
using DG.Tweening;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Visuals.Effects
{
    public class MuzzleMoveEffect : BaseEffect
    {
        [SerializeField] private Transform targetTransform;
        protected override void OnPlay(object data = null)
        {
            Debug.Assert(destroyAfterFinished == false, "This effect could not be destroyed");
            var sequence = DOTween.Sequence();
            sequence.Append(targetTransform.DOLocalMoveZ(-0.1f, 0.1f));
            sequence.Append(targetTransform.DOLocalMoveZ(-0.2f, 0.1f));
            sequence.Append(targetTransform.DOLocalMoveZ(-0.1f, 0.1f));
            sequence.AppendCallback(() => { OnFinishedAction?.Invoke(); });
        }
    }
}