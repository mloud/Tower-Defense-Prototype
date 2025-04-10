using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public enum UnitType
    {
        BasicUnit,
        Barricade
    }
    public struct UnitComponent : IComponentData
    {
        public FixedString64Bytes DefinitionId;
        public UnitType UnitType;
    }
}