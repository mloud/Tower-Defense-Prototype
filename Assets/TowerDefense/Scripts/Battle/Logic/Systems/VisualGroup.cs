using Unity.Entities;

namespace TowerDefense.Battle.Logic.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct VisualGroup : ISystem
    { }
}