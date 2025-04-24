using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct HpComponent : IComponentData
    {
        public float Hp;
        public float MaxHp;
    }
}