using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct LookAtTargetComponent : IComponentData
    {
        public bool LookAtTarget;
    }
}