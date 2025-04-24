using System;
using CastlePrototype.Data.Progress;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Ui.Popups;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Ui.Components
{
    public class CardWidget : MonoBehaviour
    {
        public Action<CardWidget> OnLevelUp;
            
        [SerializeField] private CImage icon;
        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private GameObject progressBar;
        [SerializeField] private Image progressFill;
        [SerializeField] private Button levelUpButton;
        [SerializeField] private Button detailButton;
        
        public string Id { get; private set; }
        
        private void Awake()
        {
            levelUpButton.onClick.AddListener(()=>OnLevelUp(this));
            detailButton.onClick.AddListener(OnDetailClicked);
        }

        public async UniTask<CardWidget> Set(HeroProgress heroProgress, HeroDefinition heroDefinition)
        {
            Id = heroDefinition.UnitId;
            icon.SetImage(heroDefinition.VisualId);
            cardName.text = heroDefinition.UnitId;
            level.text = heroProgress.Level.ToString();

            int cardsCounter = heroProgress.CardsCount;
            int cardsNeeded = heroDefinition.GetCardsNeededToLevelUp(heroProgress.Level);


            if (heroDefinition.IsMaxLevel(heroProgress.Level))
            {
                counter.text = "Maxed";
                progressBar.SetActive(true);
                levelUpButton.gameObject.SetActive(false);
                progressFill.fillAmount = 1.0f;
            }
            else
            {
                counter.text = $"{cardsCounter}/{cardsNeeded}";
                progressFill.fillAmount = (float)cardsCounter / cardsNeeded;
                progressBar.SetActive(cardsCounter < cardsNeeded);
                levelUpButton.gameObject.SetActive(cardsCounter >= cardsNeeded);
            }

            return this;
        }
        
        private void OnDetailClicked()
        {
            ServiceLocator.Get<IUiManager>().OpenPopup<UnitDetailPopup>(UiParameter.Create(Id));
        }
    }
}