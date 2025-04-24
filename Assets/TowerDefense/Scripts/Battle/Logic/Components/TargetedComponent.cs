using Unity.Collections;
using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct TargetedComponent : IComponentData
    {
        public FixedList512Bytes<Entity> Targeters;
    }
}