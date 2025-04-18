using CastlePrototype.Data.Definitions;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public class StageElement : CustomElement
    {
        private StageDefinition StageDefinition { get; }

        private TextField StageNameField { get; set; }
        private TextField StageVisualKeyField { get; set; }

        public StageElement(StageDefinition stageDefinition)
        {
            StageDefinition = stageDefinition;
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualElement = new VisualElement();
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            StageNameField = new TextField("Stage name") { value = StageDefinition.StageName };
            StageVisualKeyField = new TextField("Stage visual key") { value = StageDefinition.StageVisualKey };
          
            VisualElement.Add(StageNameField);
            VisualElement.Add(StageVisualKeyField);
        }

        protected override void OnSave()
        {
            StageDefinition.StageName = StageNameField.value;
            StageDefinition.StageVisualKey = StageVisualKeyField.value;
        }
    }
}