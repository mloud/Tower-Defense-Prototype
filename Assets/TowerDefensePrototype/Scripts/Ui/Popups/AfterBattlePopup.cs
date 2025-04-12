using CastlePrototype.Data;
using CastlePrototype.Managers;
using CastlePrototype.Ui.Components;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace CastlePrototype.Scripts.Ui.Popups
{
    public class AfterBattlePopup : UiPopup
    {
        [SerializeField] private BattleCardRewardPanel cardRewardPanel;

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            var stageReward = parameter.GetFirst<RuntimeStageReward>();
            cardRewardPanel.Prepare(stageReward.Cards.Count);

            var playerManager = ServiceLocator.Get<IPlayerManager>();

            int index = 0;
            foreach (var (heroId, counter) in stageReward.Cards)
            {
                if (counter == 0)
                    continue;
                var heroDefinition = await playerManager.GetHeroDefinition(heroId);
                cardRewardPanel.Get(index++).Set(heroDefinition, counter);
            }
        }
    }
}