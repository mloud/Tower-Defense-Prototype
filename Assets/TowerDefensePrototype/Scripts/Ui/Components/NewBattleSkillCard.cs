using CastlePrototype.Battle.Logic.Managers.Skills;
using Cysharp.Threading.Tasks;

namespace CastlePrototype.Ui.Components
{
    public class NewBattleSkillCard : BattleSkillCard
    {
        protected override async UniTask OnSet(ASkill skill)
        {
            nameTitle.text = skill.DefinitionId;
            descriptionLabel.text = skill.Description.Replace("{VALUE}", ((int)skill.Value).ToString());
        }
    }
}