using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct VisualGroup : ISystem
    { }
}