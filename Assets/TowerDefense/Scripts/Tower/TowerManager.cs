using UnityEngine;
using DG.Tweening;

public class TowerManager
{
    public Transform TowerPoint;
    public GameObject ProjectilePrefab;

    // Stats
    public float Damage = 10f;
    public float Range = 500f;
    public float FireRate = 1f; // shots per second
    public float ProjectileSpeed = 300f;
    public float Health = 100f;

    private TowerConfig config;
    private int damageLevel = 0;
    private int rangeLevel = 0;
    private int fireRateLevel = 0;
    private int projectileSpeedLevel = 0;

    private float timeSinceLastShot = 0f;
    public SpawnManager SpawnManager;
    public Transform ParentTransform;
    private bool isRunning = false;
    private Tween boingTween;
    private Vector3 baseScale;

    // called when health reaches zero
    public System.Action OnTowerDestroyed;

    public void Update(float deltaTime)
    {
        if (!isRunning) return;
        timeSinceLastShot += deltaTime;
        HandleShooting();
    }

    private void HandleShooting()
    {
        if (TowerPoint == null) return;
        if (SpawnManager == null) return;
        var target = SpawnManager.GetClosestEnemy(TowerPoint.position);
        if (target == null) return;
        float dist = Vector3.Distance(target.position, TowerPoint.position);
        if (dist > Range) return;

        float interval = 1f / Mathf.Max(0.0001f, FireRate);
        if (timeSinceLastShot >= interval)
        {
            Shoot(target);
            timeSinceLastShot = 0f;
        }
    }

    private void Shoot(Transform target)
    {
        if (ProjectilePrefab == null || target == null) return;
        var parent = ParentTransform != null ? ParentTransform : TowerPoint;
        Projectile proj = null;
        if (PoolingService.Instance != null)
        {
            proj = PoolingService.Instance.Create<Projectile>(PoolEnum.Projectile, parent);
            proj.ResetForReuse();
            proj.transform.position = TowerPoint.position;
            proj.transform.rotation = Quaternion.identity;
        }
        else
        {
            var projGO = GameObject.Instantiate(ProjectilePrefab, TowerPoint.position, Quaternion.identity, parent);
            proj = projGO.GetComponent<Projectile>();
        }
        if (proj != null) proj.Initialize(target, Damage, ProjectileSpeed);
        // play hit sound when projectile launched
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Hit);
        // tower boing feedback when shooting
        DoBoing(0.12f, 0.12f);
    }

    public void ApplyConfig(TowerConfig cfg)
    {
        config = cfg;
        if (config != null)
        {
            Damage = config.BaseDamage;
            Range = config.BaseRange;
            FireRate = config.BaseFireRate;
            ProjectileSpeed = config.ProjectileSpeed;
            Health = config.BaseHealth;
        }
    }

    public void ApplyDamage(float dmg)
    {
        Health -= dmg;
        if (Health <= 0f)
        {
            Health = 0f;
            StopManager();
            OnTowerDestroyed?.Invoke();
        }
    }

    public void DoBoing(float intensity = 0.12f, float period = 0.15f)
    {
        if (TowerPoint == null) return;
        if (boingTween != null) {
            boingTween.Kill();
            TowerPoint.localScale = baseScale;
        }
        baseScale = TowerPoint.localScale;
        float targetScale = 1f + intensity;
        boingTween = TowerPoint.DOScale(baseScale * targetScale, period).SetLoops(1, LoopType.Yoyo).SetEase(Ease.OutSine);
    }

    // called by UpgradeManager when an upgrade is applied
    public void ApplyDamageIncrease(float amount) { Damage += amount; damageLevel++; }
    public void ApplyRangeIncrease(float amount) { Range += amount; rangeLevel++; }
    public void ApplyFireRateMultiplier(float multiplier) { FireRate *= multiplier; fireRateLevel++; }
    public void ApplyProjectileSpeedIncrease(float amount) { ProjectileSpeed += amount; projectileSpeedLevel++; }

    public void StartManager()
    {
        timeSinceLastShot = 0f;
        isRunning = true;
    }

    public void StopManager()
    {
        isRunning = false;
    }
}
