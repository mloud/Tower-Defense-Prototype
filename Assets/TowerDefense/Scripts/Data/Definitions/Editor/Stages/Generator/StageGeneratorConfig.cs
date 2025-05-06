using System.Collections.Generic;
using UnityEngine;

namespace TowerDefensePrototype.Data.Definitions.Stages.Generator.Editor
{
    [CreateAssetMenu(fileName = "StageGeneratorInput", menuName = "ScriptableObjects/StageGeneratorInput", order = 0)]
    public class StageGeneratorConfig : ScriptableObject
    {
        public int WavesCount;
        public float MinDelayBetweenWaves;
        public float MaxDelayBetweenWaves;
        public float TotalHp;
        public float TotalDps;

        public List<string> UnitsUsed = new();
        public List<float> UnitsProbabilities;
        public List<RequiredWave> RequiredWaves = new();
        public AnimationCurve DelayCurve;
        
        public bool IsValid()
        {
            return WavesCount > 0 &&
                   MinDelayBetweenWaves <= MaxDelayBetweenWaves &&
                   UnitsUsed.Count > 0;
        }
    }
}