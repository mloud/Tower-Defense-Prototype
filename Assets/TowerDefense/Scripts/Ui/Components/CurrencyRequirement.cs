using Cysharp.Threading.Tasks;
using OneDay.Core;
using TMPro;
using TowerDefense.Managers;
using TowerDefense.Managers.Vallet;
using UnityEngine;

namespace TowerDefense.Ui.Components
{
    public class CurrencyRequirement : MonoBehaviour
    {
        [SerializeField] private Currency currency;
        [SerializeField] private Color hasEnoughColor;
        [SerializeField] private Color notEnoughColor;
        [SerializeField] private TextMeshProUGUI valueLabel;
        
        private int requiredValue;
        private int currentValue;
        private async void Start()
        {
            await UniTask.WaitUntil(() => ServiceLocator.Get<IPlayerManager>() != null);
            var valet = ServiceLocator.Get<IPlayerManager>().ValetGetter;
            valet.CurrencyChanged += OnCurrencyChanged;
            currentValue = await valet.GetCurrency(currency);
            UpdateCounter();
        }

        public void SetRequirement(int requirement)
        {
            requiredValue = requirement;
            UpdateCounter();
        }
        
        private void OnCurrencyChanged(Currency updatedCurrency, int oldValue, int newValue)
        {
            if (updatedCurrency != currency)
                return;
            currentValue = newValue;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            valueLabel.text = requiredValue.ToString();
            valueLabel.color = currentValue >= requiredValue ? hasEnoughColor : notEnoughColor;
        }
    }
}