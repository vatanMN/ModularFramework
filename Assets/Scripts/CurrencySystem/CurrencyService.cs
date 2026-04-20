using ModularFW.Core.SaveSystem;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using ModularFW.Core.Locator;
using ModularFW.Core.Signal;

namespace ModularFW.Core.CurrencySystem {
public class CurrencyService : IService, ModularFW.Core.ICurrencyService
{
    public static CurrencyService Instance => SystemLocator.Instance.CurrencyService;
    public bool IsReady { get; private set; }

    private int coins = 0;
    private readonly Stack<CoinUI> coinUIStack = new Stack<CoinUI>();
    public CoinUI GetActiveCoinUI() => coinUIStack.Count > 0 ? coinUIStack.Peek() : null;

    // events migrated to SignalBus: publish CoinsChangedSignal

    public async Task Initialize()
    {
        // load from save
        var data = SaveLoadService.Instance.GetData<CurrencyData>(DataKey.Currency);
        if (data != null) coins = data.Coins;
        IsReady = true;
        await Task.Delay(1);
    }

    public int GetCoins() => coins;

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        coins += amount;
        Save();
           SignalBus.Instance.Publish(new CoinsChangedSignal() { Coins = coins });
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (coins >= amount)
        {
            coins -= amount;
            Save();
               SignalBus.Instance.Publish(new CoinsChangedSignal() { Coins = coins });
            return true;
        }
        return false;
    }

    private void Save()
    {
        SaveLoadService.Instance.Save(DataKey.Currency, new CurrencyData() { Coins = coins });
    }

    public void RegisterCoinUI(CoinUI coinUI)
    {
        if (coinUI != null && !coinUIStack.Contains(coinUI))
            coinUIStack.Push(coinUI);
    }

    public void UnregisterCoinUI(CoinUI coinUI)
    {
        if (coinUI != null && coinUIStack.Contains(coinUI))
        {
            // Remove all instances above and including this one
            var temp = new Stack<CoinUI>();
            while (coinUIStack.Count > 0)
            {
                var top = coinUIStack.Pop();
                if (top == coinUI) break;
                temp.Push(top);
            }
            while (temp.Count > 0) coinUIStack.Push(temp.Pop());
        }
    }
}
}
