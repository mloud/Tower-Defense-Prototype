using TowerDefense.Battle.Logic.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Managers.Skills
{
    public class RestoreHpSkill : ASkill
    {
        public RestoreHpSkill(string name, float value, string description) : base(name, description, value)
        {
            SkillType = SkillType.IncreaseHp;
            NeedsUnit = true;
        }

        public override void Apply(EntityManager entityManager)
        {
            Debug.Assert(RelatedEntity != Entity.Null);
            var hpC = entityManager.GetComponentData<HpComponent>(RelatedEntity);
            hpC.Hp = math.min(hpC.MaxHp, hpC.Hp + Value);
            entityManager.SetComponentData(RelatedEntity, hpC);
        }
    }
}