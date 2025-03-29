using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct UnitComponent : IComponentData
    {
        public FixedString64Bytes DefinitionId;
    }
}