using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using TowerDefense.Managers;
using TowerDefense.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Ui.Popups
{
    public class UnitDetailPopup : UiPopup
    {
        [SerializeField] private CImage icon;
        [SerializeField] private TextMeshProUGUI unitName;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private Image progressFill;
        [SerializeField] private StatsPanel statsPanel;
        [SerializeField] private CurrencyRequirement coindRequirement;
        private string heroId;

        protected void Awake()
        {
            upgradeButton.onClick.AddListener(()=>OnUpgrade().Forget());
        }
        
        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            heroId = parameter.GetFirst<string>();
            await Set();
        }

        private async UniTask Set()
        {
            var playerManager = ServiceLocator.Get<IPlayerManager>();
            var unlockedHero = await playerManager.DeckGetter.GetUnlockedHero(heroId);

            int coinsToLevelUp = unlockedHero.definition.GetCoinsNeededToLevelUp(unlockedHero.progress.Level);
            if (coinsToLevelUp > 0)
            {
                coindRequirement.SetRequirement(coinsToLevelUp);
                coindRequirement.gameObject.SetActive(true);
            }
            else
            {
                coindRequirement.gameObject.SetActive(false);
            }
            icon.SetImage(unlockedHero.definition.VisualId);
            icon.name = unlockedHero.definition.UnitId;
            level.text = unlockedHero.progress.Level.ToString();
            unitName.text = unlockedHero.definition.UnitId;
            
            upgradeButton.interactable = await playerManager.DeckGetter.CanLevelUpHero(heroId);

            bool isMaxed = unlockedHero.definition.IsMaxLevel(unlockedHero.progress.Level);
            
            counter.text = isMaxed
                ? "Maxed" 
                : $"{unlockedHero.progress.CardsCount}/{unlockedHero.definition.GetCardsNeededToLevelUp(unlockedHero.progress.Level)}";

            if (isMaxed)
            {
                progressFill.fillAmount = 1.0f;
            }
            else
            {
                progressFill.fillAmount = (float)unlockedHero.progress.CardsCount /
                                          unlockedHero.definition.GetCardsNeededToLevelUp(unlockedHero.progress.Level);
            }

            await statsPanel.Initialize(unlockedHero.progress, unlockedHero.definition);
        }

        protected override UniTask OnCloseFinished()
        {
            return UniTask.CompletedTask;
        }

        private async UniTask OnUpgrade()
        {
            await ServiceLocator.Get<IPlayerManager>().DeckGetter.LevelUpHero(heroId);
            await Set();
        }
    }
}