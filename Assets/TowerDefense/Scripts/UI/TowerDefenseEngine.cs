using System;
using System.Collections.Generic;
using UnityEngine;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.Signal;

namespace MiniGame.TowerDefense {
public class TowerDefenseEngine : MonoBehaviour
{
    public Transform TowerPoint; // center tower transform
    public enum TowerUpgradeType { Damage, Range, FireRate, ProjectileSpeed }

    private TowerManager towerManager;
    private SpawnManager spawnManager;
    private UpgradeManager upgradeManager;
    public TowerConfig TowerConfig;
    public GameObject RestartButton;

    public TowerHealthUI HealthUI;

    private bool isGameOver = false;
    private IDisposable _restartSub;

    void Awake()
    {
        towerManager = new TowerManager();
        spawnManager = new SpawnManager();
        upgradeManager = new UpgradeManager();
        spawnManager.Engine = this;

        towerManager.SpawnManager = spawnManager;
        upgradeManager.TowerManager = towerManager;
        towerManager.OnTowerDestroyed += OnTowerDestroyed;

        _restartSub = SignalBus.Instance.SubscribeTracked<TowerDefenseRestartRequestedSignal>(_ => RestartGame());
    }

    void OnDestroy()
    {
        _restartSub?.Dispose();
    }

    public void Initialize(GameObject enemyPrefab, GameObject projectilePrefab, float spawnInterval, float spawnAccelerationPerMinute, EnemyCollection enemyCollection = null, SpawnManager.SpawnMode spawnMode = SpawnManager.SpawnMode.Weighted, System.Collections.Generic.List<int> waveModelIds = null, System.Collections.Generic.List<WaveDefinition> waves = null)
    {
        if (TowerPoint == null)
        {
            var tp = GameObject.Find("TowerPoint");
            if (tp != null) TowerPoint = tp.transform;
        }

        towerManager.TowerPoint = TowerPoint;
        towerManager.ParentTransform = this.transform;
        if (projectilePrefab != null) towerManager.ProjectilePrefab = projectilePrefab;
        if (TowerConfig != null)
        {
            towerManager.ApplyConfig(TowerConfig);
            upgradeManager.Configure(TowerConfig);
        }

        spawnManager.Initialize(enemyPrefab, null, spawnInterval, spawnAccelerationPerMinute, TowerPoint, this.transform, enemyCollection);
        spawnManager.Mode = spawnMode;
        if (waveModelIds != null) spawnManager.WaveModelIds = waveModelIds;
        if (waves != null) spawnManager.Waves = waves;
    }

    public void StartGame()
    {
        isGameOver = false;
        towerManager.StartManager();
        spawnManager.StartManager();
        this.enabled = true;
        if (RestartButton != null) RestartButton.SetActive(false);
    }

    void Update()
    {
        if (!this.enabled) return;
        float dt = Time.deltaTime;
        spawnManager.Update(dt);
        towerManager.Update(dt);
    }

    public void StopGame()
    {
        towerManager.StopManager();
        spawnManager.StopManager();
        this.enabled = false;
    }

    public void ApplyDamageToTower(float dmg)
    {
        if (isGameOver) return;
        towerManager.ApplyDamage(dmg);
        if (HealthUI != null) HealthUI.PlayDamageFeedback(dmg);
    }

    private void OnTowerDestroyed()
    {
        isGameOver = true;
        StopGame();
        if (upgradeManager != null) upgradeManager.ResetAllUpgrades();
        if (RestartButton != null) RestartButton.SetActive(true);
        if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TowerDefenseFailPanel, "Your tower was destroyed");
    }

    public void RestartGame()
    {
        if (upgradeManager != null) upgradeManager.ResetAllUpgrades();
        if (spawnManager != null)
        {
            foreach (var e in new List<Enemy>(spawnManager.ActiveEnemies))
            {
                if (e != null) GameObject.Destroy(e.gameObject);
            }
            spawnManager.ActiveEnemies.Clear();
        }
        if (towerManager != null)
        {
            foreach (var p in new List<Projectile>(towerManager.ActiveProjectiles))
            {
                if (p != null) GameObject.Destroy(p.gameObject);
            }
            towerManager.ActiveProjectiles.Clear();
        }

        if (upgradeManager != null) upgradeManager.ResetAllUpgrades();
        if (TowerConfig != null) towerManager.ApplyConfig(TowerConfig);

        StartGame();
    }

    public float GetTowerHealth()
    {
        if (towerManager == null) return 0f;
        return Mathf.Max(0f, towerManager.Health);
    }

    public float GetTowerMaxHealth()
    {
        if (TowerConfig != null) return TowerConfig.BaseHealth;
        if (towerManager == null) return 0f;
        return towerManager.Health;
    }

    public void UpgradeDamage(float amount) { }
    public void UpgradeRange(float amount) { }
    public void UpgradeFireRate(float multiplier) { }

    public bool TryUpgrade(TowerUpgradeType type)
    {
        switch(type)
        {
            case TowerUpgradeType.Damage: return upgradeManager.TryUpgradeDamage();
            case TowerUpgradeType.Range: return upgradeManager.TryUpgradeRange();
            case TowerUpgradeType.FireRate: return upgradeManager.TryUpgradeFireRate();
            case TowerUpgradeType.ProjectileSpeed: return upgradeManager.TryUpgradeProjectileSpeed();
            default: return false;
        }
    }

    public int GetUpgradeCost(TowerUpgradeType type)
    {
        switch(type)
        {
            case TowerUpgradeType.Damage: return upgradeManager.GetUpgradeCostDamage();
            case TowerUpgradeType.Range: return upgradeManager.GetUpgradeCostRange();
            case TowerUpgradeType.FireRate: return upgradeManager.GetUpgradeCostFireRate();
            case TowerUpgradeType.ProjectileSpeed: return upgradeManager.GetUpgradeCostProjectileSpeed();
            default: return 0;
        }
    }

    public int GetUpgradeLevel(TowerUpgradeType type)
    {
        switch(type)
        {
            case TowerUpgradeType.Damage: return upgradeManager.GetDamageLevel();
            case TowerUpgradeType.Range: return upgradeManager.GetRangeLevel();
            case TowerUpgradeType.FireRate: return upgradeManager.GetFireRateLevel();
            case TowerUpgradeType.ProjectileSpeed: return upgradeManager.GetProjectileSpeedLevel();
            default: return 0;
        }
    }

    public float GetTowerRange()
    {
        if (towerManager == null) return 0f;
        return towerManager.Range;
    }
}
}
