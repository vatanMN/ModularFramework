using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "TowerDefenseLoader", menuName = "Scriptable Objects/TowerDefenseLoader")]
public class TowerDefenseLoader : BaseMiniGameLoader
{
    public override MiniGameType MiniGameType => MiniGameType.TowerDefense;

    // Optional prefabs/settings exposed in inspector so you can provide
    // Enemy/Projectile prefabs and spawn tuning from the editor.
    public GameObject EnemyPrefab;
    public GameObject ProjectilePrefab;
    public float SpawnInterval = 2f;
    public float SpawnAccelerationPerMinute = 0.2f;
    public EnemyCollection EnemyCollection;
    public SpawnManager.SpawnMode SpawnMode = SpawnManager.SpawnMode.Weighted;
    public System.Collections.Generic.List<int> WaveModelIds = new System.Collections.Generic.List<int>();
    public System.Collections.Generic.List<WaveDefinition> Waves = new System.Collections.Generic.List<WaveDefinition>();

    public override async Task Load()
    {
        PanelService.Instance.Show(PanelType.TowerDefenseGamePanel);

        var engine = GameObject.FindObjectOfType<TowerDefenseEngine>();
            if (engine != null)
            {
                engine.Initialize(EnemyPrefab, ProjectilePrefab, SpawnInterval, SpawnAccelerationPerMinute, EnemyCollection, SpawnMode, WaveModelIds, Waves);
                engine.StartGame();
            }

        await Task.Delay(1);
    }

    public override async Task Unload()
    {
        var engine = GameObject.FindObjectOfType<TowerDefenseEngine>();
        if (engine != null)
        {
            engine.StopGame();
        }
        PanelService.Instance.Hide(PanelType.TowerDefenseGamePanel);
        await Task.Delay(1);
    }
}
