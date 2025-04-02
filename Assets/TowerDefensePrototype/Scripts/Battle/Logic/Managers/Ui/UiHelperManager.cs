using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Scripts.Ui.Popups;
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


        public async UniTask OpenDefeatPopup()
        {
            await UniTask.WaitForSeconds(2.0f);
            ServiceLocator.Get<IUiManager>().OpenPopup<DefeatPopup>(null);
        }
        
        public async UniTask OpenVictoryPopup()
        {
            await UniTask.WaitForSeconds(2.0f);
            ServiceLocator.Get<IUiManager>().OpenPopup<VictoryPopup>(null);
        }
     
        protected override void OnRelease()
        { }
    }
}