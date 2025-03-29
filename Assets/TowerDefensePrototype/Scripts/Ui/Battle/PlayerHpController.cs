using CastlePrototype.Battle.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Ui.Battle
{
    public class BattleHpController : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI label;
        private void OnEnable()
        {
            PlayerHpChanged.Event.Subscribe(OnPlayerHPChanged);
        }

        private void OnDisable()
        {
            PlayerHpChanged.Event.UnSubscribe(OnPlayerHPChanged);
        }

        private void OnPlayerHPChanged(float current, float total)
        {
            label.text = $"{current}/{total}";
            fillImage.fillAmount = current / (float)total;
        }
    }
}