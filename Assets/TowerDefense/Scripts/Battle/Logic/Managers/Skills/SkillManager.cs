using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Ui;
using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.EcsUtils;
using TowerDefense.Battle.Logic.Managers.Slots;
using TowerDefense.Data.Definitions;
using TowerDefense.Managers;
using TowerDefense.Managers.Simulation;
using TowerDefense.Scripts.Ui.Popups;
using Unity.Entities;

namespace TowerDefense.Battle.Logic.Managers.Skills
{
    public class SkillManager : WorldManager
    {
        public List<string> UsedSkills => ActivatedSkills;
        private List<(ASkill skill, float probability)> AvailableSkills { get; set; }

        private List<string> ActivatedSkills { get; set; }
        
        public SkillManager(World world) : base(world)
        { }

        protected override async UniTask OnInitialize()
        {
            var deck = await ServiceLocator.Get<IPlayerManager>().DeckGetter.GetHeroDeck();
            
            AvailableSkills = new List<(ASkill, float)>
            {
                (new IncreaseDamageSkill("Increase damage", 30, "Increases damage by {VALUE}%"),10),
                (new IncreaseDamageSkill("Increase damage", 60, "Increases damage by {VALUE}%"), 5),
                (new DecreaseAttackInterval("Decrease attack interval", 30, "Decrease attack interval by {VALUE}%"),10),
                (new DecreaseAttackInterval("Decrease attack interval", 45, "Decrease attack interval by {VALUE}%"),5),
                (new IncreaseAttackDistance("Increase attack distance", 30, "Increase attack distance by {VALUE}%"),10),
                (new IncreaseAttackDistance("Increase attack distance", 40, "Increase attack distance by {VALUE}%"),5),
                (new IncreaseBounceCountSkill("Increase bounce count", 1, "Increase bounce by {VALUE}"),10),
                (new IncreaseFireAgainCountSkill("Increase fire again count", 1, "Increase fire again count by {VALUE}"),10),
                (new RestoreHpSkill("Restore HP", 20, "Increase HP by {VALUE}"),10),
                (new RestoreHpSkill("Restore HP", 50, "Increase HP by {VALUE}"),5),
            };

            ActivatedSkills = new List<string>();

            if (deck.Heroes.ContainsKey("palisade"))
            {
                AvailableSkills.Add((new PlaceTrapSkill("Place palisade", 5, "Place {VALUE} palisades", "palisade"), 10));
            }

            AvailableSkills.ForEach(x=>x.skill.AttachedWorld = AttachedToWorld);

       
            foreach (var (unitId, _) in deck.Heroes)
            {
                if (unitId == "barricade" || unitId == "weapon")
                    continue;
                var definition = await ServiceLocator.Get<IPlayerManager>().DeckGetter.GetHeroDefinition(unitId);
                // these units are created by skills only
                if (definition.CreatedBySkill)
                    continue;
                
                AvailableSkills.Add((new UnlockHeroSkill("Unlock new hero", unitId, "Add new hero to battle"), 10));
            }
        }
        
        public async UniTask RunSkillSelectionFlow(int skillsToShow)
        {
            PauseUtils.SetLogicPaused(true);
            var skills = GetRandomSkills(skillsToShow);
            await ConnectSkillsToEntities(skills);

            var automaticPlayManager = ServiceLocator.Get<IAutomaticPlayManager>();
            var selectedSkill =  ServiceLocator.Get<ISimulationMode>().IsActive()
                ? await automaticPlayManager.SelectSkill(skills)
                : await OpenSkillPopup(skills);

            selectedSkill.Apply(AttachedToWorld.EntityManager);
            
            // log selected skill
            ActivatedSkills.Add($"{selectedSkill.SkillType}_{selectedSkill.DefinitionId}");
            

            if (selectedSkill.SkillType == SkillType.UnlockHero || selectedSkill.SkillType == SkillType.PlaceTrap)
            {
                AvailableSkills.RemoveWhen(x=>x.skill == selectedSkill);
            }

            PauseUtils.SetLogicPaused(false);
        }

        private List<ASkill> GetRandomSkills(int count)
        {
            // selected random skills
            var selectedSkills = new List<ASkill>();
            // make copy of the skills
            var validSkillsForThisSelect = new List<(ASkill skill, float probabilities)>(AvailableSkills);
            validSkillsForThisSelect.RemoveAll(x => !x.skill.IsApplicable(AttachedToWorld.EntityManager));
            
            int skillsToSelectCount = Math.Min(count, validSkillsForThisSelect.Count);
            for (int i = 0; i < skillsToSelectCount; i++)
            {
                var rndSkillIndex = validSkillsForThisSelect
                    .GetRandomIndexWithProbabilities(
                        validSkillsForThisSelect.Select(x => x.probabilities).ToList());
                selectedSkills.Add(validSkillsForThisSelect[rndSkillIndex].skill);
                validSkillsForThisSelect.RemoveAt(rndSkillIndex);
            }
          
            return selectedSkills;
        }
  
        private async UniTask ConnectSkillsToEntities(IReadOnlyList<ASkill> skills)
        {
            var heroDefs = await ServiceLocator.Get<IDataManager>().GetAll<HeroDefinition>();
      
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].RelatedEntity = skills[i].NeedsUnit
                    ? QueryUtils.GetEntityForSkill(AttachedToWorld.EntityManager, skills[i].SkillType)
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