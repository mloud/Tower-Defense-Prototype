using OneDay.Core.Modules.Data;

namespace CastlePrototype.Data.Definitions
{
    public class EnemyDefinition : BaseDataObject
    {
        public string UnitId;
        public float MoveSpeed;
        public float AttackInterval;
        public float Damage;
        public float Hp;
    }
}