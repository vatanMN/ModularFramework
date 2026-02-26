using System;
using UnityEngine;

[Serializable]
public class CurrencyData : ISaveData
{
    public int Coins = 0;

    public string GetSaveable()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadData(string saved)
    {
        if (string.IsNullOrEmpty(saved)) return;
        try
        {
            var d = JsonUtility.FromJson<CurrencyData>(saved);
            Coins = d.Coins;
        }
        catch { }
    }

    public void Update<T>(T input) where T : ISaveData
    {
        if (input is CurrencyData cd)
        {
            Coins = cd.Coins;
        }
    }
}
