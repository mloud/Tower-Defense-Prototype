using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct DamageComponent : IComponentData 
    {
        public float Damage;
        public bool Knockback;
    }
}