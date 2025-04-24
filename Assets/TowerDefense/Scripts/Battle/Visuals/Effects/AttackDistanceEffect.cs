using DG.Tweening;
using UnityEngine;

namespace TowerDefense.Battle.Visuals.Effects
{
    public class AttackDistanceEffect : BaseEffect
    {
        [SerializeField] private Transform attackCircle;
        protected override void OnPlay(object data = null)
        {
            float radius = (float)data;

            var sequence = DOTween.Sequence();
            sequence.Append(attackCircle.DOScale(new Vector3(radius, radius, 1), 0.2f).From(Vector3.zero));
            sequence.AppendInterval(3.0f);
            sequence.Append(attackCircle.DOScale(Vector3.zero, 0.2f));
            sequence.AppendCallback(() => { OnFinishedAction?.Invoke(); });
        }
    }
}