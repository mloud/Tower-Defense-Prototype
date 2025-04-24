using TowerDefense.Data.Definitions;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public class RewardElement : CustomElement
    {
        private StageReward StageReward { get; }
        public RewardElement(StageReward stageReward) => StageReward = stageReward;

        private IntegerField CardsField { get; set; }
        private IntegerField XpField { get; set; }
        private IntegerField CoinsField { get; set; }


        protected override VisualElement CreateVisualElement()
        {
            var visualElement = new VisualElement();
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            CardsField = new IntegerField("Cards") { value = StageReward.Cards };
            XpField = new IntegerField("Xp") { value = StageReward.Xp };
            CoinsField = new IntegerField("Coins") { value = StageReward.Coins };
            VisualElement.Add(CardsField);
            VisualElement.Add(XpField);
            VisualElement.Add(CoinsField);
        }

        protected override void OnSave()
        {
            StageReward.Cards = CardsField.value;
            StageReward.Xp = XpField.value;
            StageReward.Coins = CoinsField.value;
        }
    }
}