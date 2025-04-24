using TowerDefense.Battle.Visuals.Effects;
using DG.Tweening;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Visuals.Effects
{
    public class BossIncomingEffect : BaseEffect
    {
        [SerializeField] private GameObject bossMessage;
        protected override void OnPlay(object data = null)
        {
            Debug.Assert(destroyAfterFinished == false, "This effect could not be destroyed");
            var sequence = DOTween.Sequence();
            sequence.Append(bossMessage.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero));
            sequence.AppendInterval(2.0f);
            sequence.Append(bossMessage.transform.DOScale(Vector3.zero, 0.2f));
            sequence.AppendCallback(() => { OnFinishedAction?.Invoke(); });
        }
    }
}