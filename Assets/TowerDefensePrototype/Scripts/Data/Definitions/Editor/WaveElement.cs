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
            var visualElement = new VisualElement();
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            Times = new List<FloatField>();
            EnemiesCount = new List<IntegerField>();
            EnemiesIds = new List<TextField>();
            SpawnIntervals = new List<FloatField>();

            const int maxWidth = 150;
            
            for (int i = 0; i < Waves.Count; i++)
            {
                var wave = Waves[i];
                VisualElement.Add(new Label($"Wave:{i+1}"));
                Times.Add(new FloatField("Time") { value = Waves[i].Time });
                EnemiesCount.Add(new IntegerField("Count") { value = Waves[i].EnemiesCount });
                EnemiesIds.Add(new TextField("Enemy") { value = Waves[i].EnemyId });
                SpawnIntervals.Add(new FloatField("Spawn Interval") { value = Waves[i].SpawnInterval });
                VisualElement.Add(Times[^1]);
                VisualElement.Add(EnemiesCount[^1]);
                VisualElement.Add(EnemiesIds[^1]);
                VisualElement.Add(SpawnIntervals[^1]);
                VisualElement.Add(new Button(()=>RemoveWave(wave)) { text = "-" }); 
                VisualElement.Add(VisualElementFactory.CreateSeparator(2));  
                Times[^1].style.width = maxWidth;     // absolute width
                EnemiesCount[^1].style.maxWidth = maxWidth;      // maximum width (still responsive)
                EnemiesIds[^1].style.minWidth = maxWidth;
                SpawnIntervals[^1].style.flexGrow = 1;
            }
            VisualElement.Add(new Button(AddNewWave) { text = "Add" });  
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