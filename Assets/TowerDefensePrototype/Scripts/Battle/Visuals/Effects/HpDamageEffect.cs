using CastlePrototype.Battle.Visuals;
using CastlePrototype.Battle.Visuals.Effects;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Visuals
{
    public class HpDamageEffect : BaseEffect
    {
        [SerializeField] private TextMeshProUGUI label;

        private  void OnEnable()
        {
            transform.position = transform.position;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

      
        public override void Play(object data)
        {
            label.text = ((float)data).ToString();
            transform.DOScale(Vector3.one, 0.2f);
            transform.DOMove(transform.position + new Vector3(0, 90, 0), 1.0f).onComplete += () => Destroy(gameObject);
        }
    }
}