using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Battle.Logic.Managers.Slots;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Managers.Skills
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
            HeroFactoryUtils.CreateHeroFromArchetype(ref ecb, DefinitionId, slot.Position);
            ecb.Playback(entityManager);
            ecb.Dispose();
        }
    }
}