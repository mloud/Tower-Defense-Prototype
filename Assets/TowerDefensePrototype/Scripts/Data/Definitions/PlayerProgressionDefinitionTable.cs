using System;
using System.Collections.Generic;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Data.Definitions
{
    [CreateAssetMenu(fileName = "PlayerProgressionDefinitionTable", menuName = "ScriptableObjects/PlayerProgressionDefinitionTable",
        order = 1)]

    public class PlayerProgressionDefinitionTable : ScriptableObjectTable<PlayerProgressionDefinition>
    {
    }

    [Serializable]
    public class PlayerProgressionDefinition : BaseDataObject
    {
        public List<int> XpNeededToNextLevel;
    }
}