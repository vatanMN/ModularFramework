using UnityEngine;
using System;

namespace MiniGame.TowerDefense {
[CreateAssetMenu(fileName = "TowerConfig", menuName = "TowerDefense/TowerConfig")]
public class TowerConfig : ScriptableObject
{
    [Header("Base Stats")]
    public float ProjectileSpeed = 300f;
    public float BaseDamage = 10f;
    public float BaseRange = 500f;
    public float BaseFireRate = 1f; // shots per second
	public float BaseHealth = 100f;

    [Header("Upgrades")]
    public UpgradeInfo DamageUpgrade = new UpgradeInfo() { Cost = 5, Amount = 2f };
    public UpgradeInfo RangeUpgrade = new UpgradeInfo() { Cost = 5, Amount = 50f };
    public UpgradeInfo FireRateUpgrade = new UpgradeInfo() { Cost = 8, Amount = 1.1f }; // multiplier
    public UpgradeInfo ProjectileSpeedUpgrade = new UpgradeInfo() { Cost = 3, Amount = 50f };
}

[Serializable]
public class UpgradeInfo
{
    public int Cost = 1;
    public float Amount = 1f;
}
}
