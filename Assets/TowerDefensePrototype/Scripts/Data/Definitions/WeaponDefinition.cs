using System;
using OneDay.Core.Modules.Data;

namespace CastlePrototype.Data.Definitions
{
    [Serializable]
    public class WeaponDefinition : BaseDataObject
    {
        public string UnitId;
        public float AttackInterval;
        public float Damage;
        public float Hp;
    }
}