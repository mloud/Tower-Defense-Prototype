using CastlePrototype.Battle.Visuals.Effects;
using DG.Tweening;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Visuals.Effects
{
    public class GoUnderGroundEffect : BaseEffect
    {
        protected override void OnPlay(object data = null)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.1f);
            sequence.Append(transform.DOLocalMoveY(transform.position.y - 0.5f, 0.5f));
            sequence.AppendCallback(() => { OnFinishedAction?.Invoke(); });
        }
    }
}