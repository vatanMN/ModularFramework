using System;
using UnityEngine;

[Serializable]
public class TowerUpgradeData : ISaveData
{
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
            var d = JsonUtility.FromJson<TowerUpgradeData>(saved);
            DamageLevel = d.DamageLevel;
            RangeLevel = d.RangeLevel;
            FireRateLevel = d.FireRateLevel;
            ProjectileSpeedLevel = d.ProjectileSpeedLevel;
        }
        catch { }
    }

    public void Update<T>(T input) where T : ISaveData
    {
        if (input is TowerUpgradeData td)
        {
            DamageLevel = td.DamageLevel;
            RangeLevel = td.RangeLevel;
            FireRateLevel = td.FireRateLevel;
            ProjectileSpeedLevel = td.ProjectileSpeedLevel;
        }
    }
}
