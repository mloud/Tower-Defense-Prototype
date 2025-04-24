using System.Collections.Generic;
using System.Linq;
using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.EcsUtils;
using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Scripts.Ui.Popups;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Ui;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Logic.Managers.Ui
{
    public class UiHelperManager : WorldManager
    {
        public UiHelperManager(World world) : base(world)
        {
        }

        protected override async UniTask OnInitialize()
        { }


        public async UniTask OpenDefeatPopup(RuntimeStageReward runtimeStageReward)
        {
            await UniTask.WaitForSeconds(2.0f);
            ServiceLocator.Get<IUiManager>().OpenPopup<DefeatPopup>(UiParameter.Create(runtimeStageReward));
        }
        
        public async UniTask OpenVictoryPopup(RuntimeStageReward runtimeStageReward)
        {
            await UniTask.WaitForSeconds(2.0f);
            ServiceLocator.Get<IUiManager>().OpenPopup<VictoryPopup>(UiParameter.Create(runtimeStageReward));
        }
     
        protected override void OnRelease()
        { }
    }
}