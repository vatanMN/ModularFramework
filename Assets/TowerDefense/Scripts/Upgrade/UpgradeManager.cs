using ModularFW.Core.SaveSystem;
using ModularFW.Core.HapticService;
using UnityEngine;
using ModularFW.Core.Signal;
using ModularFW.Core.CurrencySystem;

namespace MiniGame.TowerDefense {
public class UpgradeManager
{
    public TowerManager TowerManager;
    private TowerConfig config;
    private int damageLevel = 0;
    private int rangeLevel = 0;
    private int fireRateLevel = 0;
    private int projectileSpeedLevel = 0;

    public void Configure(TowerConfig cfg)
    {
        config = cfg;
        // load saved levels
        var saved = SaveLoadService.Instance.GetData<TowerUpgradeData>(DataKey.TowerUpgrades);
        if (saved != null)
        {
            damageLevel = saved.DamageLevel;
            rangeLevel = saved.RangeLevel;
            fireRateLevel = saved.FireRateLevel;
            projectileSpeedLevel = saved.ProjectileSpeedLevel;
            // apply saved upgrades to tower manager
            for (int i = 0; i < damageLevel; i++) TowerManager.ApplyDamageIncrease(config.DamageUpgrade.Amount);
            for (int i = 0; i < rangeLevel; i++) TowerManager.ApplyRangeIncrease(config.RangeUpgrade.Amount);
            for (int i = 0; i < fireRateLevel; i++) TowerManager.ApplyFireRateMultiplier(config.FireRateUpgrade.Amount);
            for (int i = 0; i < projectileSpeedLevel; i++) TowerManager.ApplyProjectileSpeedIncrease(config.ProjectileSpeedUpgrade.Amount);
        }
    }

    public bool TryUpgradeDamage()
    {
        if (config == null || TowerManager == null) return false;
        int cost = GetUpgradeCostDamage();
        if (!CurrencyService.Instance.TrySpend(cost)) return false;
        TowerManager.ApplyDamageIncrease(config.DamageUpgrade.Amount);
        damageLevel++;
        SaveLevels();
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        return true;
    }

    public bool TryUpgradeRange()
    {
        if (config == null || TowerManager == null) return false;
        int cost = GetUpgradeCostRange();
        if (!CurrencyService.Instance.TrySpend(cost)) return false;
        TowerManager.ApplyRangeIncrease(config.RangeUpgrade.Amount);
        rangeLevel++;
        SaveLevels();
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        return true;
    }

    public bool TryUpgradeFireRate()
    {
        if (config == null || TowerManager == null) return false;
        int cost = GetUpgradeCostFireRate();
        if (!CurrencyService.Instance.TrySpend(cost)) return false;
        TowerManager.ApplyFireRateMultiplier(config.FireRateUpgrade.Amount);
        fireRateLevel++;
        SaveLevels();
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        return true;
    }

    public bool TryUpgradeProjectileSpeed()
    {
        if (config == null || TowerManager == null) return false;
        int cost = GetUpgradeCostProjectileSpeed();
        if (!CurrencyService.Instance.TrySpend(cost)) return false;
        TowerManager.ApplyProjectileSpeedIncrease(config.ProjectileSpeedUpgrade.Amount);
        projectileSpeedLevel++;
        SaveLevels();
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        return true;
    }

    // scale costs per level (exponential growth)
    public int GetUpgradeCostDamage() { return config != null ? Mathf.CeilToInt(config.DamageUpgrade.Cost * Mathf.Pow(1.5f, damageLevel)) : 0; }
    public int GetUpgradeCostRange() { return config != null ? Mathf.CeilToInt(config.RangeUpgrade.Cost * Mathf.Pow(1.4f, rangeLevel)) : 0; }
    public int GetUpgradeCostFireRate() { return config != null ? Mathf.CeilToInt(config.FireRateUpgrade.Cost * Mathf.Pow(1.6f, fireRateLevel)) : 0; }
    public int GetUpgradeCostProjectileSpeed() { return config != null ? Mathf.CeilToInt(config.ProjectileSpeedUpgrade.Cost * Mathf.Pow(1.3f, projectileSpeedLevel)) : 0; }

    private void SaveLevels()
    {
        SaveLoadService.Instance.Save(DataKey.TowerUpgrades, new TowerUpgradeData()
        {
            DamageLevel = damageLevel,
            RangeLevel = rangeLevel,
            FireRateLevel = fireRateLevel,
            ProjectileSpeedLevel = projectileSpeedLevel
        });
    }
    
    // expose current levels for UI
    public int GetDamageLevel() { return damageLevel; }
    public int GetRangeLevel() { return rangeLevel; }
    public int GetFireRateLevel() { return fireRateLevel; }
    public int GetProjectileSpeedLevel() { return projectileSpeedLevel; }

    // Reset all upgrades to level 0 and persist
    public void ResetAllUpgrades()
    {
        damageLevel = 0;
        rangeLevel = 0;
        fireRateLevel = 0;
        projectileSpeedLevel = 0;
        SaveLevels();
        // reapply base config to tower manager if available
        if (config != null && TowerManager != null)
        {
            TowerManager.ApplyConfig(config);
        }
        // notify UI and interested parties that upgrades were reset
        if (SignalBus.Instance != null) SignalBus.Instance.Publish(new UpgradesResetSignal());
    }
}
}
