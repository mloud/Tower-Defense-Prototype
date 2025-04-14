using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Managers;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;
using UnityEngine.UI;


namespace CastlePrototype.Ui.Components
{
    public class StageContainer : ContentPanel<StageWidget>
    {
        public int SelectedStageIndex { get; private set; }
        
        [SerializeField] private Button playButton;
        [SerializeField] private SnapScrollRect snapScrollRect;

        private List<StageDefinition> stageDefinitions;
        private PlayerProgress playerProgress;
        private void Awake()
        {
            snapScrollRect.OnItemCentered += OnCenteredItemChanged;
        }

        public async UniTask Refresh()
        {
            var playerManager = ServiceLocator.Get<IPlayerManager>();
            stageDefinitions = (await playerManager.GetAllStageDefinitions()).ToList();
            playerProgress = await playerManager.GetProgression();
            Prepare(stageDefinitions.Count);

            for (int i = 0; i < stageDefinitions.Count; i++)
            {
                var stageItem = Get(i);
                stageItem.Set(stageDefinitions[i], i, i > playerProgress.UnlockedStage);
            }

            snapScrollRect.SnapToIndex(playerProgress.UnlockedStage);
        }
        
        private void OnCenteredItemChanged(GameObject gameObject, int itemIndex)
        {
            var stageWidget = gameObject.GetComponent<StageWidget>();
            SelectedStageIndex = stageWidget.StageIndex;
            // playButton.interactable = SelectedStageIndex <= playerProgress.UnlockedStage;
            playButton.gameObject.SetActive(SelectedStageIndex <= playerProgress.UnlockedStage || stageDefinitions[itemIndex].StageName.Contains("test", StringComparison.InvariantCultureIgnoreCase));

        }
    }
}