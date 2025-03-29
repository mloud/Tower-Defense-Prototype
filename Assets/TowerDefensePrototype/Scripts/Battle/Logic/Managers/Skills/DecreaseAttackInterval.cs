using CastlePrototype.Battle.Logic.Components;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Managers.Skills
{
    public class DecreaseAttackInterval : ASkill
    {
        private float Value { get; }

        public DecreaseAttackInterval(string name, float value, string description) 
            : base(name, description, value)
        {
            SkillType = SkillType.DecreaseAttackInterval;
            Value = value;
            NeedsUnit = true;
        }

        public override void Apply(EntityManager entityManager)
        {
            Debug.Assert(RelatedEntity != Entity.Null);
            var attackC = entityManager.GetComponentData<AttackComponent>(RelatedEntity);
            attackC.AttackInterval *= (1 - Value / 100);
            entityManager.SetComponentData(RelatedEntity, attackC);
        }
    }
}