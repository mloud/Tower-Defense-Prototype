using TowerDefense.Battle.Logic.Components;
using Unity.Entities;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Managers.Skills
{
    public class IncreaseAttackDistance : ASkill
    {
        private float Value { get; }

        public IncreaseAttackDistance(string name, float value, string description) 
            : base(name, description, value)
        {
            SkillType = SkillType.IncreaseAttackDistance;
            Value = value;
            NeedsUnit = true;
        }

        public override void Apply(EntityManager entityManager)
        {
            Debug.Assert(RelatedEntity != Entity.Null);
            var attackC = entityManager.GetComponentData<AttackComponent>(RelatedEntity);
            attackC.AttackDistance *= (1 + Value / 100);
            entityManager.SetComponentData(RelatedEntity, attackC);
        }
    }
}