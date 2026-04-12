using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.TowerDefense {
[CreateAssetMenu(fileName = "EnemyCollection", menuName = "Custom/EnemyCollection")]
public class EnemyCollection : ScriptableObject
{
    [SerializeField] private List<EnemyModel> enemyModels = new List<EnemyModel>();
    private static Dictionary<int, EnemyModel> enemyModelsDic = new Dictionary<int, EnemyModel>();

    [NonSerialized]
    private bool isLoaded = false;

    private void Load()
    {
        foreach (var e in enemyModels)
        {
            enemyModelsDic[e.Id] = e;
        }
        isLoaded = true;
    }

    public EnemyModel GetEnemyModel(int id)
    {
        if (!isLoaded) Load();
        return enemyModelsDic[id];
    }

    public List<EnemyModel> GetAll()
    {
        if (!isLoaded) Load();
        return enemyModels;
    }

    public EnemyModel GetRandomWeighted()
    {
        if (enemyModels == null || enemyModels.Count == 0) return null;
        int total = 0;
        foreach (var e in enemyModels) total += Mathf.Max(0, e.Weight);
        if (total <= 0) return enemyModels[UnityEngine.Random.Range(0, enemyModels.Count)];
        int r = UnityEngine.Random.Range(0, total);
        int acc = 0;
        foreach (var e in enemyModels)
        {
            acc += Mathf.Max(0, e.Weight);
            if (r < acc) return e;
        }
        return enemyModels[enemyModels.Count - 1];
    }
}

[Serializable]
public class EnemyModel
{
    public int Id;
    public EnemyType Type = EnemyType.Basic;
    public Sprite Sprite;
    public float Speed = 1f;
    public float Health = 20f;
    public float Damage = 10f;
    public int Reward = 1;
    public int Weight = 1;
}
}

