using TMPro;
using TowerDefense.Battle.Events;
using UnityEngine;

namespace TowerDefense.Ui.Battle
{
    public class BattleStageController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stageIndexLabel;
        [SerializeField] private TextMeshProUGUI stageNameLabel;
        private void OnEnable() => StageChanged.Event.Subscribe(OnBattlePointsChanged);
        private void OnDisable() => StageChanged.Event.UnSubscribe(OnBattlePointsChanged);

        private void OnBattlePointsChanged(string stageName,int stageIndex)
        {
            stageIndexLabel.text = $"Stage {stageIndex + 1}";
            stageNameLabel.text = stageName;
        }
    }
}