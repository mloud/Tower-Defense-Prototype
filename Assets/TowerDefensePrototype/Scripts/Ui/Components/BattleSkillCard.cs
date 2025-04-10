using System;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers.Skills;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


namespace CastlePrototype.Ui.Components
{
    public abstract class BattleSkillCard : UiElement
    {
        [SerializeField] private Button button;
        [SerializeField] protected TextMeshProUGUI nameTitle;
        [SerializeField] protected TextMeshProUGUI descriptionLabel;
        [SerializeField] protected CImage image;
        public async UniTask Set(ASkill skill)
        {
            SetImage(skill);
            await OnSet(skill);
        }

        public BattleSkillCard SetButtonHandler(Action onClicked)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClicked());
            return this;
        }

        protected abstract UniTask OnSet(ASkill skill);
        private void SetImage(ASkill skill)
        {
            Debug.Assert(skill.Definition != null);
            image.SetImage(skill.Definition.VisualId);
        }
    }
}