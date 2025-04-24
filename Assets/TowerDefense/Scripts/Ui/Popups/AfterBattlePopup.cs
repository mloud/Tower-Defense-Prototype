using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TowerDefense.Data;
using TowerDefense.Managers;
using TowerDefense.Ui.Components;
using UnityEngine;

namespace TowerDefense.Scripts.Ui.Popups
{
    public class AfterBattlePopup : UiPopup
    {
        [SerializeField] private BattleCardRewardPanel cardRewardPanel;
        [SerializeField] private ResourceWidget coinsResourceWidget;
        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            var stageReward = parameter.GetFirst<RuntimeStageReward>();
            cardRewardPanel.Prepare(stageReward.Cards.Count);

            var playerManager = ServiceLocator.Get<IPlayerManager>();

            int index = 0;
            foreach (var (heroId, counter) in stageReward.Cards)
            {
                if (counter == 0)
                {
                    cardRewardPanel.Get(index++).gameObject.SetActive(false);
                    continue;
                }

                var heroDefinition = await playerManager.DeckGetter.GetHeroDefinition(heroId);
                cardRewardPanel.Get(index++).Set(heroDefinition, counter);
            }

            if (stageReward.Coins > 0)
            {
                coinsResourceWidget.gameObject.SetActive(true);
                coinsResourceWidget.Set(stageReward.Coins);
            }
            else
            {
                coinsResourceWidget.gameObject.SetActive(false);
            }
        }
    }
}