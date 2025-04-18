using System;
using System.Collections.Generic;
using OneDay.Core.Modules.Data;

namespace CastlePrototype.Data.Definitions
{
    [Serializable]
    public class StageDefinition:  BaseDataObject
    {
        public string StageName;
        public string StageVisualKey;
        public bool IsUnlocked;
        public List<WaveDefinition> Waves;
        public StageReward Reward;
    }
}