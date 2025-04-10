using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Battle.Logic.Managers.Slots;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Scripts.Ui.Popups;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Ui;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Managers.Skills
{
    public class SkillManager : WorldManager
    { 
        private List<ASkill> AvailableSkills { get; }
      
        public SkillManager(World world) : base(world)
        {
            AvailableSkills = new List<ASkill>
            {
                new IncreaseDamageSkill("Increase damage", 25, "Increases damage by {VALUE}%"),
                new DecreaseAttackInterval("Decrease attack interval", 25, "Decrease attack interval by {VALUE}%"),
                new IncreaseAttackDistance("Increase attack distance", 25, "Increase attack distance by {VALUE}"),
                new IncreaseBounceCountSkill("Increase bounce count", 1, "Increase bounce by {VALUE}"),
                new IncreaseFireAgainCountSkill("Increase fire again count", 1, "Increase fire again count by {VALUE}"),
                new UnlockHeroSkill("Unlock new hero", "soldier", "Add new hero to battle"),
                new UnlockHeroSkill("Unlock new hero", "turret", "Add new hero to battle"),
                new UnlockHeroSkill("Unlock new hero", "dron", "Add new hero to battle"),
                new UnlockHeroSkill("Unlock new hero", "scorpion", "Add new hero to battle"),
                new UnlockHeroSkill("Unlock new hero", "tank", "Add new hero to battle")
            };
        }
        
        public async UniTask RunSkillSelectionFlow(int skillsToShow)
        {
            PauseUtils.SetLogicPaused(true);
            var skills = GetRandomSkills(skillsToShow);
            await ConnectSkillsToEntities(skills);
            var selectedSkill = await OpenSkillPopup(skills);
            selectedSkill.Apply(AttachedToWorld.EntityManager);
            if (selectedSkill.SkillType == SkillType.UnlockHero)
            {
                AvailableSkills.Remove(selectedSkill);
            }

            PauseUtils.SetLogicPaused(false);
        }

        private List<ASkill> GetRandomSkills(int count)
        {
            var skills = new List<ASkill>(AvailableSkills);
            RemoveNotApplicableSkills(skills);
            int realCount = Math.Min(count, skills.Count);

            
            int skillsToRemove = skills.Count - realCount;
            for (int i = 0; i < skillsToRemove; i++)
            { 
                skills.RemoveAt(UnityEngine.Random.Range(0,skills.Count));
            }
            return skills;
        }

        private void RemoveNotApplicableSkills(List<ASkill> skills)
        {
            if (WorldManagers.Get<SlotManager>(AttachedToWorld).GetFirstAvailableSlot() == null)
            {
                skills.RemoveAll(x => x.SkillType == SkillType.UnlockHero);
            }
        }
        
        private async UniTask ConnectSkillsToEntities(IReadOnlyList<ASkill> skills)
        {
            var heroDefs = await ServiceLocator.Get<IDataManager>().GetAll<HeroDefinition>();
      
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].RelatedEntity = skills[i].NeedsUnit
                    ? QueryUtils.GetRandomPlayerUnit(AttachedToWorld.EntityManager)
                    : Entity.Null;

                if (skills[i].RelatedEntity != Entity.Null)
                {
                    skills[i].DefinitionId = AttachedToWorld.EntityManager
                        .GetComponentData<UnitComponent>(skills[i].RelatedEntity).DefinitionId.ToString();
                }

                if (!string.IsNullOrEmpty(skills[i].DefinitionId))
                {
                    skills[i].Definition = heroDefs.First(x => x.UnitId == skills[i].DefinitionId);
                }
            }
        }
 
        private async UniTask<ASkill> OpenSkillPopup(List<ASkill> proposedSkills)
        {
            int selectedIndex = -1;
            var popupRequest = ServiceLocator.Get<IUiManager>()
                .OpenPopup<SkillPopup>(
                    UiParameter
                        .Create(proposedSkills)
                        .Add("OnClick", (Action<int>)(s => selectedIndex = s)));
            await popupRequest.OpenTask;
            await popupRequest.WaitForCloseFinished();

            return proposedSkills[selectedIndex];
        }
        
        protected override void OnRelease() => AvailableSkills.Clear();
    }
}