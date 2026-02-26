using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseEngine : MonoBehaviour
{
    public Transform TowerPoint; // center tower transform
    public enum TowerUpgradeType { Damage, Range, FireRate, ProjectileSpeed }

    // internal managers
    private TowerManager towerManager;
    private SpawnManager spawnManager;
    private UpgradeManager upgradeManager;
    public TowerConfig TowerConfig;
    public GameObject RestartButton;

    public TowerHealthUI HealthUI;

    private bool isGameOver = false;

    void Awake()
    {
        towerManager = new TowerManager();
        spawnManager = new SpawnManager();
        upgradeManager = new UpgradeManager();

        // wire references
        towerManager.SpawnManager = spawnManager;
        upgradeManager.TowerManager = towerManager;
        // subscribe to tower death
        towerManager.OnTowerDestroyed += OnTowerDestroyed;
    }

    // Initialize engine from loader (keeps previous API)
    public void Initialize(GameObject enemyPrefab, GameObject projectilePrefab, float spawnInterval, float spawnAccelerationPerMinute, EnemyCollection enemyCollection = null, SpawnManager.SpawnMode spawnMode = SpawnManager.SpawnMode.Weighted, System.Collections.Generic.List<int> waveModelIds = null, System.Collections.Generic.List<WaveDefinition> waves = null)
    {
        // ensure TowerPoint
        if (TowerPoint == null)
        {
            var tp = GameObject.Find("TowerPoint");
            if (tp != null) TowerPoint = tp.transform;
        }

        // apply to managers
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
        // store TowerConfig from loader if provided (loader should set public TowerConfig before Initialize)
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
        // reset upgrades when tower destroyed
        if (upgradeManager != null) upgradeManager.ResetAllUpgrades();
        // show restart button if available
        if (RestartButton != null) RestartButton.SetActive(true);
        // show fail panel
        if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TowerDefenseFailPanel, "Your tower was destroyed");
    }

    // Restart the game without touching currency
    public void RestartGame()
    {
        if (upgradeManager != null) upgradeManager.ResetAllUpgrades();
        // clear enemies and projectiles
        var enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (var e in enemies) GameObject.Destroy(e.gameObject);
        var projs = GameObject.FindObjectsOfType<Projectile>();
        foreach (var p in projs) GameObject.Destroy(p.gameObject);

        // reset managers
        if (upgradeManager != null) upgradeManager.ResetAllUpgrades();
        if (TowerConfig != null) towerManager.ApplyConfig(TowerConfig);
        // reset tower health
        // (ApplyConfig above sets Health)

        // restart
        StartGame();
    }

    // expose tower health for UI
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

    // Upgrade API exposed for UI (delegates to UpgradeManager)
    // legacy signatures kept for compatibility (no-op)
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

    // expose tower range for UI/visualizers
    public float GetTowerRange()
    {
        if (towerManager == null) return 0f;
        return towerManager.Range;
    }
}
