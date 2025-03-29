using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Data.Definitions
{
    namespace CastlePrototype.Data.Definitions
    {
        [CreateAssetMenu(fileName = "HeroDefinitionsTable", menuName = "ScriptableObjects/HeroDefinitionsTable", order = 1)]

        public class HeroDefinitionsTable : ScriptableObjectTable<HeroDefinition>
        { }
    }
}