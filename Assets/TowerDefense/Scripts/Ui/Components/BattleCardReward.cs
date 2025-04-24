using OneDay.Core.Modules.Ui.Components;
using TMPro;
using TowerDefense.Data.Definitions;
using UnityEngine;

namespace TowerDefense.Ui.Components
{
    public class BattleCardReward : MonoBehaviour
    {
        [SerializeField] private CImage icon;
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI counter;

        public void Set(HeroDefinition heroDefinition, int count)
        {
            icon.SetImage(heroDefinition.VisualId);
            name.text = heroDefinition.UnitId;
            counter.text = count > 0 ? $"{count}x" : "New";
        }
    }
}