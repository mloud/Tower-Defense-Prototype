using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.EcsUtils;
using TowerDefense.Battle.Logic.Managers.Slots;
using TowerDefense.Battle.Visuals;
using TowerDefensePrototype.Battle.Visuals.Effects;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Managers.Skills
{
    public class UnlockHeroSkill : ASkill
    {
        public UnlockHeroSkill(string name, string heroDefinitionId, string description) : base(name, description, -1)
        {
            SkillType = SkillType.UnlockHero;
            DefinitionId = heroDefinitionId;
            NeedsUnit = false;
        }

        public override void Apply(EntityManager entityManager)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var slot = WorldManagers.Get<SlotManager>(entityManager.World).GetRandomAvailableSlot();
            Debug.Assert(slot != null);
            Debug.Assert(!slot.IsOccupied);
            slot.IsOccupied = true;
            WorldManagers.Get<UnitManager>(entityManager.World).CreateHeroUnit(ref ecb, slot.Position, DefinitionId);
            VisualManager.Default.PlayEffect(EffectKeys.SpawnEffectHero,slot.Position);
            ecb.Playback(entityManager);
            ecb.Dispose();
        }
    }
}