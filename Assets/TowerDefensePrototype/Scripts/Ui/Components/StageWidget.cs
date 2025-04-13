using System;
using CastlePrototype.Data.Definitions;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CastlePrototype.Ui.Components
{
    public class StageWidget : UiElement
    {
        public int StageIndex { get; private set; }
        public Action<int> OnClicked;
        
        [SerializeField] private TextMeshProUGUI stageNameLabel;
        [SerializeField] private TextMeshProUGUI stageOrderLabel;
        [SerializeField] private GameObject locked;
        [SerializeField] private Button button;

        public UniTask Set(StageDefinition stageDefinition, int stageIndex, bool isLocked)
        {
            StageIndex= stageIndex;

            stageOrderLabel.text = $"Stage {stageIndex+1}";
            stageNameLabel.text = stageDefinition.StageName;

            button.interactable = !isLocked;
            locked.SetActive(isLocked);
            
            return UniTask.CompletedTask;
        }
    }
}