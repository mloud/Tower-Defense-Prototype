using CastlePrototype.Battle.Logic.Managers.Skills;
using Cysharp.Threading.Tasks;

namespace CastlePrototype.Ui.Components
{
    public class UpgradeBattleSkillCard : BattleSkillCard
    {
        public override async UniTask Set(ASkill skill)
        {
            nameTitle.text = skill.DefinitionId;
            descriptionLabel.text = skill.Description.Replace("{VALUE}", ((int)skill.Value).ToString());
        }
    }
}