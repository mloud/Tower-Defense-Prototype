using CastlePrototype.Data.Definitions;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public class StageDifficultyElement : CustomElement
    {
        private StageDifficultyCalculator.StageDifficulty StageDifficulty { get; }

        private FloatField TotalHp { get; set; }
        private FloatField TotalDps { get; set; }
        private FloatField Threat { get; set; }
     
        public StageDifficultyElement(StageDifficultyCalculator.StageDifficulty stageDifficulty)
        {
            StageDifficulty = stageDifficulty;
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualElement = new VisualElement();
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            TotalHp = new FloatField("Total HP") { value = StageDifficulty.TotalHp};
            TotalDps = new FloatField("Total DPS") { value = StageDifficulty.TotalDps};
            Threat = new FloatField("Threat") { value = StageDifficulty.Threat};

            VisualElement.Add(TotalHp);
            VisualElement.Add(TotalDps);
            VisualElement.Add(Threat);
        }

        protected override void OnSave()
        {
        }
    }
}