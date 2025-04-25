using UnityEngine;

namespace TowerDefense.Data.Definitions
{
    namespace CastlePrototype.Data.Definitions
    {
        [CreateAssetMenu(fileName = "StageDefinitionsTable", menuName = "ScriptableObjects/StageDefinitionsTable",
            order = 1)]
        
        public class StageDefinitionsTable : DefinitionTable<StageDefinition>
        {
        }
    }
}