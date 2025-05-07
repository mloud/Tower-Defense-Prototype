using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TMPro;
using TowerDefense.Managers;
using TowerDefense.Ui.Components;
using UnityEngine;

namespace TowerDefense.Ui.Popups
{
    public class NewLevelPopup : UiPopup
    {
        [SerializeField] private BattleCardRewardPanel cardRewardPanel;
        [SerializeField] private TextMeshProUGUI levelLabel;
        

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            var levelUpEvent = parameter.GetFirst<NewLevelBufferedEvent>();
            cardRewardPanel.Prepare(1);
            levelLabel.text = levelUpEvent.Level.ToString();

            var playerManager = ServiceLocator.Get<IPlayerManager>();
            var heroDefinition = await playerManager.DeckGetter.GetHeroDefinition(levelUpEvent.HeroId);
            cardRewardPanel.Get(0).Set(heroDefinition, 0);
        }
    }
}