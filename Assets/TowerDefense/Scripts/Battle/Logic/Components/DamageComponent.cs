using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct DamageComponent : IComponentData 
    {
        public float Damage;
        public bool Knockback;
    }
}