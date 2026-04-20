using System;
using UnityEngine;

namespace ModularFW.Core.SaveSystem
{
    [Serializable]
    public class CurrencyData : ISaveData
    {
        private const int CurrentVersion = 1;
        public int SaveVersion = CurrentVersion;
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
                var parsed = JsonUtility.FromJson<CurrencyData>(saved);
                if (parsed.SaveVersion != CurrentVersion)
                    Debug.LogWarning($"[SaveLoad] CurrencyData version mismatch: expected {CurrentVersion}, got {parsed.SaveVersion}.");
                Coins = parsed.Coins;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoad] Failed to parse CurrencyData: {e.Message}");
            }
        }

        public void Update<T>(T input) where T : ISaveData
        {
            if (input is CurrencyData currencyData)
                Coins = currencyData.Coins;
        }
    }
}
