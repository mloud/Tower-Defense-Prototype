using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Data;
using TowerDefense.Data.Progress;
using TowerDefense.Scripts.Managers;
using UnityEngine;

namespace TowerDefense.Managers.Vallet
{
    public delegate void CurrencyChangedDelegate(Currency currency, int oldValue, int newValue);
    
    public interface IValetGetter
    {
        CurrencyChangedDelegate CurrencyChanged { get; set; } 
        
        UniTask<int> GetCurrency(Currency currency);
    }
    
    public interface IValetSetter
    {
        UniTask AddCurrency(Currency currency, int value);
        UniTask SpendCurrency(Currency currency, int value);
    }
    
    public enum Currency
    {
        Coins
    }
    
    public class ValetGetter : IValetGetter, IValetSetter, IPlugin
    {
        public CurrencyChangedDelegate CurrencyChanged { get; set; }
        private IDataManager dataManager;
     
        public ValetGetter(IDataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        
        public async UniTask<int> GetCurrency(Currency currency)
        {
            var valet = (await dataManager.GetAll<Valet>()).First();
            return valet.Coins;
        }

        public async UniTask AddCurrency(Currency currency, int value)
        {
            if (value == 0)
                return;
            
            var valet = (await dataManager.GetAll<Valet>()).First();
            switch (currency)
            {
                case Currency.Coins:
                    int oldValue = valet.Coins;
                    valet.Coins += value;
                    var result = await dataManager.Actualize(valet);
                    Debug.Assert(result);
                    CurrencyChanged?.Invoke(Currency.Coins, oldValue, valet.Coins);
                    break;
                default:
                    throw new ArgumentException($"Currency {currency} is not supported");
            }
        }

        public async UniTask SpendCurrency(Currency currency, int value)
        {
            if (value == 0)
                return;
            
            var valet = (await dataManager.GetAll<Valet>()).First();
            switch (currency)
            {
                case Currency.Coins:
                    if (valet.Coins < value)
                        throw new ArgumentException($"Not enough currency {currency} needed: {value} has: {valet.Coins}");
                    int oldValue = valet.Coins;
                    valet.Coins -= value;
                    var result = await dataManager.Actualize(valet);
                    Debug.Assert(result);
                    CurrencyChanged?.Invoke(Currency.Coins, oldValue, valet.Coins);
                    break;
                default:
                    throw new ArgumentException($"Currency {currency} is not supported");
            }
        }
    }
}