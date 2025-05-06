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

        public override bool IsApplicable(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(typeof(BarricadeComponent), typeof(HpComponent));

            if (query.IsEmptyIgnoreFilter)
                return false;

            var barricadeEntity = query.GetSingletonEntity();
            var barricadeHpC = entityManager.GetComponentData<HpComponent>(barricadeEntity);

            return barricadeHpC.Hp < barricadeHpC.MaxHp;
        }
    }
}