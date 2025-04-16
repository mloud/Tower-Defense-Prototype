using CastlePrototype.Managers;
using CastlePrototype.Ui.Components;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TMPro;
using UnityEngine;

namespace CastlePrototype.Scripts.Ui.Popups
{
    public class NewLevelPopup : UiPopup
    {
        [SerializeField] private BattleCardRewardPanel cardRewardPanel;
        [SerializeField] private TextMeshProUGUI levelLabel;
        

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            var levelUpEvent = parameter.GetFirst<PlayerManager.NewLevelBufferedEvent>();
            cardRewardPanel.Prepare(1);
            levelLabel.text = levelUpEvent.Level.ToString();

            var playerManager = ServiceLocator.Get<IPlayerManager>();
            var heroDefinition = await playerManager.GetHeroDefinition(levelUpEvent.HeroId);
            cardRewardPanel.Get(0).Set(heroDefinition, 0);
        }
    }
}