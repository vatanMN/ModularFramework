using UnityEngine;
using System.Collections.Generic;

public class SpawnManager
{
    private List<Enemy> activeEnemies = new List<Enemy>();
    public enum SpawnMode { Random, Weighted, Wave }

    public GameObject EnemyPrefab;
    public Transform EnemyParent;
    public Transform TowerPoint;
    public EnemyCollection EnemyCollection;

    public float SpawnInterval = 2f;
    public float SpawnAccelerationPerMinute = 0.2f;

    private float lastSpawn = 0f;
    private float elapsed = 0f;
    private bool isRunning = false;

    public SpawnMode Mode = SpawnMode.Weighted;
    public List<int> WaveModelIds = new List<int>();
    public List<WaveDefinition> Waves = new List<WaveDefinition>();
    private int waveIndex = 0;
    private int spawnedThisWave = 0;
    private int currentWaveIndex = 0;

    public void Update(float deltaTime)
    {
        if (!isRunning) return;
        elapsed += deltaTime;
        HandleSpawning();
    }

    private void HandleSpawning()
    {
        if (EnemyPrefab == null || EnemyParent == null) return;
        float interval = Mathf.Max(0.2f, SpawnInterval - (elapsed / 60f) * SpawnAccelerationPerMinute);
        if (Time.time - lastSpawn >= interval)
        {
            SpawnEnemy();
            lastSpawn = Time.time;
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomPointOnEdge(Camera.main);
        GameObject go = null;
        Enemy enemy = null;
        if (PoolingService.Instance != null)
        {
            enemy = PoolingService.Instance.Create<Enemy>(PoolEnum.Enemy, EnemyParent);
            go = enemy.gameObject;
            go.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0f);
            go.transform.rotation = Quaternion.identity;
            // reset transient state on reuse
            enemy.ResetForReuse();
        }
        else
        {
            go = GameObject.Instantiate(EnemyPrefab, new Vector3(spawnPos.x, spawnPos.y, 0f), Quaternion.identity, EnemyParent);
            enemy = go.GetComponent<Enemy>();
        }
        // apply model from collection if available
        if (EnemyCollection != null)
        {
            EnemyModel model = null;
            var list = EnemyCollection.GetAll();
            if (list != null && list.Count > 0)
            {
                if (Mode == SpawnMode.Random)
                {
                    model = list[UnityEngine.Random.Range(0, list.Count)];
                }
                else if (Mode == SpawnMode.Weighted)
                {
                    model = EnemyCollection.GetRandomWeighted();
                }
                else if (Mode == SpawnMode.Wave)
                {
                    if (Waves != null && Waves.Count > 0)
                    {
                        var wave = Waves[currentWaveIndex % Waves.Count];
                        if (wave.ModelIds != null && wave.ModelIds.Count > 0)
                        {
                            int pick = wave.ModelIds[UnityEngine.Random.Range(0, wave.ModelIds.Count)];
                            model = EnemyCollection.GetEnemyModel(pick);
                        }
                        spawnedThisWave++;
                        if (spawnedThisWave >= wave.Count)
                        {
                            spawnedThisWave = 0;
                            currentWaveIndex++;
                        }
                    }
                    else if (WaveModelIds != null && WaveModelIds.Count > 0)
                    {
                        int id = WaveModelIds[waveIndex % WaveModelIds.Count];
                        model = EnemyCollection.GetEnemyModel(id);
                        waveIndex++;
                    }
                }

                if (model == null) model = list[UnityEngine.Random.Range(0, list.Count)];

                if (enemy != null)
                {
                    enemy.Type = model.Type;
                    enemy.Speed = model.Speed;
                    enemy.Health = model.Health;
                    enemy.Damage = model.Damage;
                    enemy.Reward = model.Reward;
                    // ensure colliders are enabled when reused from pool
                    var c2 = enemy.GetComponent<Collider2D>(); if (c2 != null) c2.enabled = true;
                    var c3 = enemy.GetComponent<Collider>(); if (c3 != null) c3.enabled = true;
                }

                // set sprite on spawned GameObject if it has a SpriteRenderer
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr == null) sr = go.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && model.Sprite != null)
                {
                    sr.sprite = model.Sprite;
                }
                var img = go.GetComponent<UnityEngine.UI.Image>();
                if (img == null) img = go.GetComponentInChildren<UnityEngine.UI.Image>();
                if (img != null && model.Sprite != null)
                {
                    img.sprite = model.Sprite;
                }
            }
        }

        if (enemy != null && TowerPoint != null) enemy.SetTarget(TowerPoint);
        if (enemy != null)
        {
            float intensity = Mathf.Clamp(enemy.Speed / 10f, 0.02f, 0.25f);
            float period = Mathf.Clamp(0.5f / (enemy.Speed / 5f), 0.15f, 0.8f);
            enemy.StartBoing(intensity, period);
            if (!activeEnemies.Contains(enemy)) activeEnemies.Add(enemy);
            enemy.SpawnManager = this;
        }
    }

    public Transform GetClosestEnemy(Vector3 position)
    {
        if (activeEnemies.Count == 0) return null;
        Enemy best = null;
        float bestDist = float.MaxValue;
        foreach (var e in activeEnemies)
        {
            if (e == null) continue;
            float d = Vector3.Distance(e.transform.position, position);
            if (d < bestDist)
            {
                bestDist = d;
                best = e;
            }
        }
        return best != null ? best.transform : null;
    }

    private Vector2 GetRandomPointOnEdge(Camera cam)
    {
        
        /*f (cam == null) cam = Camera.main;
        if(Camera.main == null) Debug.LogError("SpawnManager: No camera found for spawn point calculation!");
        var bounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));
        */
        float left = 0;
        float right = Screen.width;
        float top = Screen.height ;
        float bottom = 0;

        int side = UnityEngine.Random.Range(0, 4);
        switch (side)
        {
            case 0: return new Vector2(UnityEngine.Random.Range(left, right), top);
            case 1: return new Vector2(UnityEngine.Random.Range(left, right), bottom);
            case 2: return new Vector2(left, UnityEngine.Random.Range(bottom, top));
            default: return new Vector2(right, UnityEngine.Random.Range(bottom, top));
        }
    }

    public void Initialize(GameObject enemyPrefab, Transform enemyParent, float spawnInterval, float spawnAccelerationPerMinute, Transform towerPoint, Transform parentForEnemies, EnemyCollection enemyCollection = null)
    {
        if (enemyPrefab != null) EnemyPrefab = enemyPrefab;
        if (enemyParent != null) EnemyParent = enemyParent;
        SpawnInterval = spawnInterval;
        SpawnAccelerationPerMinute = spawnAccelerationPerMinute;
        TowerPoint = towerPoint;
        EnemyCollection = enemyCollection;

        if (EnemyParent == null)
        {
            var go = new GameObject("Enemies");
            if (parentForEnemies != null) go.transform.SetParent(parentForEnemies, false);
            EnemyParent = go.transform;
        }
    }

    public void StartManager()
    {
        lastSpawn = -SpawnInterval;
        elapsed = 0f;
        isRunning = true;
    }

    public void StopManager()
    {
        isRunning = false;
        activeEnemies.Clear();
        if (EnemyParent != null)
        {
            for (int i = EnemyParent.childCount - 1; i >= 0; i--)
            {
                var c = EnemyParent.GetChild(i);
                if (c != null) GameObject.Destroy(c.gameObject);
            }
        }
    }

    // Called by EnemyDestroyNotifier when an enemy is destroyed
    public void NotifyEnemyDestroyed(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }
}
