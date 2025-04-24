using System;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using TMPro;
using TowerDefense.Managers;
using TowerDefense.Managers.Vallet;
using UnityEngine;

namespace TowerDefense.Ui.Components
{
    public class ValetCounter : MonoBehaviour
    {
        [SerializeField] private Currency currency;
        [SerializeField] private TextMeshProUGUI label;

        private async void Start()
        {
            await UniTask.WaitUntil(() => ServiceLocator.Get<IPlayerManager>() != null);
            var valet = ServiceLocator.Get<IPlayerManager>().ValetGetter;
            valet.CurrencyChanged += OnCurrencyChanged;
            var current = await valet.GetCurrency(currency);
            UpdateCounter(current);
        }

        private void OnCurrencyChanged(Currency updatedCurrency, int oldvalue, int newValue)
        {
            if (updatedCurrency != currency)
                return;
            UpdateCounter(newValue);
        }

        private void UpdateCounter(int value)
        {
            label.text = value.ToString();
        }
    }
}