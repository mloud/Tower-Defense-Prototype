using CastlePrototype.Battle.Logic.Components;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Managers.Skills
{
    public class IncreaseBounceCountSkill : ASkill
    {
        private float Value { get; }

        public IncreaseBounceCountSkill(string name, float value, string description) 
            : base(name, description, value)
        {
            SkillType = SkillType.IncreaseBounceCount;
            Value = value;
            NeedsUnit = true;
        }

        public override void Apply(EntityManager entityManager)
        {
            Debug.Assert(RelatedEntity != Entity.Null);
            var attackC = entityManager.GetComponentData<AttackComponent>(RelatedEntity);
            attackC.Bounce += (int)Value;
            entityManager.SetComponentData(RelatedEntity, attackC);
        }
    }
}