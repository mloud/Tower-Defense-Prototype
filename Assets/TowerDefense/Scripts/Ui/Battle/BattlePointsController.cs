using TMPro;
using TowerDefense.Battle.Events;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Ui.Battle
{
    public class BattlePointsController : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI label;
        private void OnEnable() => BattlePointsChanged.Event.Subscribe(OnBattlePointsChanged);

        private void OnDisable() => BattlePointsChanged.Event.UnSubscribe(OnBattlePointsChanged);

        private void OnBattlePointsChanged(int current, int total)
        {
            label.text = $"{current}/{total}";
            fillImage.fillAmount = current / (float)total;
        }
    }
}