using System;
using TowerDefense.Data.Definitions;
using TowerDefensePrototype.Data.Definitions.Stages.Generator.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public class StageElement : CustomElement
    {
        public Action<StageGeneratorConfig, StageElement, StageDefinition, bool> OnGenerateButtonAction;
        
        private StageDefinition StageDefinition { get; set; }

        private ObjectField StageGeneratorInputField { get; set;}
        private Toggle WaveTimingOnlyField { get; set; }
        private TextField StageNameField { get; set; }
        private TextField StageVisualKeyField { get; set; }
        private Label WavesCountField { get; set; }
        private Button GenerateButton { get; set; }
        private Toggle TestField { get; set; }

        private int Index;

        private RewardElement _rewardElement;
        private WaveElement _waveElement;
        
        public StageElement(StageDefinition stageDefinition, int index)
        {
            StageDefinition = stageDefinition;
            Index = index;
        }

        public void Refresh(StageDefinition stageDefinition)
        {
            StageDefinition = stageDefinition;
            OnCreate();
        }
        
        protected override VisualElement CreateVisualElement()
        {
            var visualElement = new VisualElement();
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            VisualElement.Clear();
            VisualElement.style.maxWidth = 300;
            VisualElement.Add(
                new Label( "Stage:" + (Index+1)));
            VisualElement.Add(
                new StageDifficultyElement(StageDifficultyCalculator.Calculate(StageDefinition)).Create().VisualElement);
         
            _rewardElement = new RewardElement(StageDefinition.Reward);
            
            _waveElement = new WaveElement(StageDefinition.Waves);
            
            StageNameField = new TextField("Stage name") { value = StageDefinition.StageName };
            StageVisualKeyField = new TextField("Stage visual key") { value = StageDefinition.StageVisualKey };
            TestField = new Toggle("Test") { value = StageDefinition.IsUnlocked };
            WavesCountField = new Label($"Waves {StageDefinition.Waves.Count}");
            StageGeneratorInputField = new ObjectField("Generator Config")
                { objectType = typeof(StageGeneratorConfig) };
            WaveTimingOnlyField = new Toggle("Apply wave timing only");
            GenerateButton = new Button(()=>OnGenerateButtonAction(
                (StageGeneratorConfig)StageGeneratorInputField.value, 
                this, 
                StageDefinition,
                WaveTimingOnlyField.value
                ))
            {
                text = "Generate"
            };
            VisualElement.Add(StageNameField);
            VisualElement.Add(StageVisualKeyField);
            VisualElement.Add(TestField);
            VisualElement.Add(WavesCountField);
            VisualElement.Add(StageGeneratorInputField);
            VisualElement.Add(WaveTimingOnlyField);
            VisualElement.Add(GenerateButton);
            VisualElement.Add(_rewardElement.Create().VisualElement);
            VisualElement.Add(_waveElement.Create().VisualElement);

        }

        protected override void OnSave()
        {
            _rewardElement.Save();
            _waveElement.Save();

            StageDefinition.StageName = StageNameField.value;
            StageDefinition.StageVisualKey = StageVisualKeyField.value;
            StageDefinition.IsUnlocked = TestField.value;
        }
    }
}