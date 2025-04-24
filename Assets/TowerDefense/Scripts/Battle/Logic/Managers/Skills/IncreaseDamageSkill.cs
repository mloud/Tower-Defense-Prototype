using TowerDefense.Battle.Logic.Components;
using Unity.Entities;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Managers.Skills
{
    public class IncreaseDamageSkill : ASkill
    {
        public IncreaseDamageSkill(string name, float value, string description) : base(name, description, value)
        {
            SkillType = SkillType.IncreaseDamage;
            NeedsUnit = true;
        }

        public override void Apply(EntityManager entityManager)
        {
            Debug.Assert(RelatedEntity != Entity.Null);
            var attackC = entityManager.GetComponentData<AttackComponent>(RelatedEntity);
            attackC.AttackDamage *= (1 + Value / 100);
            entityManager.SetComponentData(RelatedEntity, attackC);
        }
    }
}