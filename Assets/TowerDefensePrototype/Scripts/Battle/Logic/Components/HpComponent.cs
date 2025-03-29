using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct HpComponent : IComponentData
    {
        public float Hp;
        public float MaxHp;
    }
}