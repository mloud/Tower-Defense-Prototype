using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui;
using TowerDefense.Battle.Logic.Managers.Skills;
using TowerDefense.Ui.Components;
using UnityEngine;

namespace TowerDefense.Ui.Popups
{
    public class SkillPopup : UiPopup
    {
        [SerializeField] private NewBattleSkillCard newBattleSkillCardPrefab;
        [SerializeField] private UpgradeBattleSkillCard upgradeBattleSkillCardPrefab;

        [SerializeField] private Transform skillCardContainer;

        private IUiParameter parameter;
       
        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            this.parameter = parameter;
            
            var skills = parameter.GetFirst<List<ASkill>>();

            for (int i = 0; i < skills.Count; i++)
            {
                int copyInt = i;
                switch (skills[i].SkillType)
                {
                    case SkillType.UnlockHero:
                        await Instantiate(newBattleSkillCardPrefab, skillCardContainer)
                            .SetButtonHandler(()=> OnSelectClicked(copyInt))
                            .Set(skills[i]);
                        break;
                    default: 
                        await Instantiate(upgradeBattleSkillCardPrefab, skillCardContainer)
                            .SetButtonHandler(()=> OnSelectClicked(copyInt))
                            .Set(skills[i]);
                        break;
                }
            }
        }

        protected override UniTask OnCloseFinished()
        {
            for (int i = 0; i < skillCardContainer.childCount; i++)
            {
                Destroy(skillCardContainer.GetChild(i).gameObject);
            }
            return UniTask.CompletedTask;
        }
        
        private void OnSelectClicked(int index)
        {
            parameter.Get<Action<int>>("OnClick").Invoke(index);
            Close();
        }
    }
}