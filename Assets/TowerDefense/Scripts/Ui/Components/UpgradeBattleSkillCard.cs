using Cysharp.Threading.Tasks;
using TowerDefense.Battle.Logic.Managers.Skills;

namespace TowerDefense.Ui.Components
{
    public class UpgradeBattleSkillCard : BattleSkillCard
    {
        protected override async UniTask OnSet(ASkill skill)
        {
            nameTitle.text = skill.DefinitionId;
            descriptionLabel.text = skill.Description.Replace("{VALUE}", ((int)skill.Value).ToString());
        }
    }
}