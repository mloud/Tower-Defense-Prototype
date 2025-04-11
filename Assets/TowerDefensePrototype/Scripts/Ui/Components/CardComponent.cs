using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Ui.Components
{
    public class CardComponent : MonoBehaviour
    {
        [SerializeField] private CImage icon;
        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private Image progressFill;

        public CardComponent Set(HeroProgress heroProgress, HeroDefinition heroDefinition)
        {
            icon.SetImage(heroDefinition.VisualId);
            cardName.text = heroDefinition.UnitId;
            level.text = heroProgress.Level.ToString();
            return this;
        }
    }
}