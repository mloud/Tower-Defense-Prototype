using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TMPro;
using TowerDefense.Managers;
using UnityEngine;
using UnityEngine.UI;


namespace TowerDefense.Ui.Panels
{
    public class PlayerProfilePanel : UiPanel
    {
        [SerializeField] private TextMeshProUGUI levelLabel;
        [SerializeField] private TextMeshProUGUI progressionLabel;

        [SerializeField] private Image xpProgress;


        public override async UniTask Initialize()
        {
            var playerManager = ServiceLocator.Get<IPlayerManager>();
            var progressionInfo = await playerManager.ProgressionGetter.GetProgressionInfo();
            
            Set(progressionInfo.xp, progressionInfo.level, progressionInfo.xpNextLevel);
            
            playerManager.ProgressionGetter.XpChanged += OnXpChanged;
        }

        private void OnXpChanged(int newXp, int nextXp, int prevLevel, int currentLevel)
        {
           Set(newXp, currentLevel, nextXp);
        }

        private void Set(int xp, int level, int xpToNextLevel)
        {
            levelLabel.text = level.ToString();
            xpProgress.fillAmount = (float)xp /xpToNextLevel;
            progressionLabel.text = $"{xp}/{xpToNextLevel}";
        }
        
      
    }
}