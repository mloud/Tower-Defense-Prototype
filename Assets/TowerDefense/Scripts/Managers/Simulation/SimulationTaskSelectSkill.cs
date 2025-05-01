using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core.Extensions;
using TowerDefense.Battle.Logic.Managers.Skills;

namespace TowerDefense.Managers.Simulation
{
    public class SimulationTaskSelectSkill : ASimulationTask
    {
        public override async UniTask<T> Perform<T>(object input) where T: class
        {
            var proposedSkills = (List<ASkill>)input;
            
            var unlockHeroes = proposedSkills.Where(x => x.SkillType == SkillType.UnlockHero).ToList();
            if (unlockHeroes.Any())
            {
                return unlockHeroes.GetRandom() as T;
            }

            proposedSkills.RemoveAll(x => x.SkillType == SkillType.IncreaseAttackDistance);
            return proposedSkills.GetRandom() as T;
        }
    }
}