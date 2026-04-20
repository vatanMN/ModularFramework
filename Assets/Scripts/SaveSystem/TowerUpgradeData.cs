using System;
using UnityEngine;

namespace ModularFW.Core.SaveSystem
{
    [Serializable]
    public class TowerUpgradeData : ISaveData
    {
        private const int CurrentVersion = 1;
        public int SaveVersion = CurrentVersion;
        public int DamageLevel = 0;
        public int RangeLevel = 0;
        public int FireRateLevel = 0;
        public int ProjectileSpeedLevel = 0;

        public string GetSaveable()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadData(string saved)
        {
            if (string.IsNullOrEmpty(saved)) return;
            try
            {
                var parsed = JsonUtility.FromJson<TowerUpgradeData>(saved);
                if (parsed.SaveVersion != CurrentVersion)
                    Debug.LogWarning($"[SaveLoad] TowerUpgradeData version mismatch: expected {CurrentVersion}, got {parsed.SaveVersion}.");
                DamageLevel = parsed.DamageLevel;
                RangeLevel = parsed.RangeLevel;
                FireRateLevel = parsed.FireRateLevel;
                ProjectileSpeedLevel = parsed.ProjectileSpeedLevel;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoad] Failed to parse TowerUpgradeData: {e.Message}");
            }
        }

        public void Update<T>(T input) where T : ISaveData
        {
            if (input is TowerUpgradeData upgradeData)
            {
                DamageLevel = upgradeData.DamageLevel;
                RangeLevel = upgradeData.RangeLevel;
                FireRateLevel = upgradeData.FireRateLevel;
                ProjectileSpeedLevel = upgradeData.ProjectileSpeedLevel;
            }
        }
    }
}
