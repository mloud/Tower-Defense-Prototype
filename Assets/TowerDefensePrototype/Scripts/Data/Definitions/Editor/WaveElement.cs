using System.Collections.Generic;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Data.Definitions;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public class WaveElement : CustomElement
    {
        private List<WaveDefinition>  Waves  { get; }
        public WaveElement(List<WaveDefinition> waves) => Waves = waves;

        private List<FloatField> Times;
        private List<IntegerField> EnemiesCount;
        private List<TextField> EnemiesIds;
        private List<FloatField> SpawnIntervals;

        protected override VisualElement CreateVisualElement()
        {
            var visualElement = VisualElementFactory.CreateScrollView(false);
            visualElement.style.width = 300;
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            Times = new List<FloatField>();
            EnemiesCount = new List<IntegerField>();
            EnemiesIds = new List<TextField>();
            SpawnIntervals = new List<FloatField>();

           // const int maxWidth = 150;
            
            for (int i = 0; i < Waves.Count; i++)
            {
                var wave = Waves[i];
                VisualElement.Add(new Label($"Wave:{i+1}"));
                Times.Add(VisualElementFactory.CreateFloatField("Time", Waves[i].Time ));
                EnemiesCount.Add(VisualElementFactory.CreateIntegerField("Count", Waves[i].EnemiesCount ));
                EnemiesIds.Add(VisualElementFactory.CreateTextField("Id", Waves[i].EnemyId ));
                SpawnIntervals.Add(VisualElementFactory.CreateFloatField("Interval", Waves[i].SpawnInterval ));
                

                var panel = new VisualElement();
                panel.style.flexDirection = FlexDirection.Column;
                panel.style.alignSelf = Align.FlexStart;    // Prevents stretching in parent container
                panel.style.flexBasis = StyleKeyword.Auto;
                panel.style.maxWidth = 300;
            

                panel.Add(Times[^1]);
                panel.Add(EnemiesCount[^1]);
                panel.Add(EnemiesIds[^1]);
                panel.Add(SpawnIntervals[^1]);
   
                
                VisualElement.Add(panel);
                
                VisualElement.Add(new Button(()=>RemoveWave(wave)) { text = "-" }); 
                VisualElement.Add(VisualElementFactory.CreateSeparator(2));  
       
            }
            VisualElement.Add(new Button(AddNewWave) { text = "Add wave" });  
        }

        protected override void OnSave()
        {
            for (int i = 0; i < Waves.Count; i++)
            {
                Waves[i].Time = Times[i].value;
                Waves[i].EnemiesCount = EnemiesCount[i].value;
                Waves[i].EnemyId = EnemiesIds[i].value;
                Waves[i].SpawnInterval = SpawnIntervals[i].value;
            }
        }

        private void RemoveWave(WaveDefinition wave)
        {
            Waves.Remove(wave);
            Refresh();
        }
        
        private void AddNewWave()
        {
            Waves.Add(new WaveDefinition());
            Refresh();
        }

        private void Refresh()
        {
            VisualElement.Clear();
            OnCreate();
        }
    }
}