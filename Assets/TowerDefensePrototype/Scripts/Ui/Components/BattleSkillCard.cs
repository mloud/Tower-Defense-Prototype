using System;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers.Skills;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CastlePrototype.Ui.Components
{
    public abstract class BattleSkillCard : UiElement
    {
        [SerializeField] private Button button;
        [SerializeField] protected TextMeshProUGUI nameTitle;
        [SerializeField] protected TextMeshProUGUI descriptionLabel;

        public abstract UniTask Set(ASkill skill);

        public BattleSkillCard SetButtonHandler(Action onClicked)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClicked());
            return this;
        }
    }
}