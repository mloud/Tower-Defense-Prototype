using TMPro;
using TowerDefense.Data.Definitions;
using UnityEngine;

namespace TowerDefense.Ui.Components
{
    public class StatWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI currentLabel;
        [SerializeField] private TextMeshProUGUI nextLabel;

        public void Set(StatUpgradeType statUpgradeType, float currentValue, float? nextValue)
        {
            nameLabel.text = statUpgradeType.ToString();
            currentLabel.text = currentValue.ToString();
            nextLabel.text = nextValue == null ? "" : nextValue.Value.ToString();
        }
    }
}