using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using ModularFW.Core.Locator;
using ModularFW.Core.PanelSystem;

public class MiniGameService : IService
{
    public static MiniGameService Instance => SystemLocator.Instance.MiniGameService;
    public bool IsReady { get; private set; }

    public Dictionary<MiniGameType,BaseMiniGameLoader> Loaders = new Dictionary<MiniGameType, BaseMiniGameLoader>();

    private MiniGameType miniGameType;
    public async Task Initialize(List<BaseMiniGameLoader> loader)
    {
        foreach(var mg in loader)
        {
            Loaders[mg.MiniGameType] = mg;
        }
        await Task.Delay(1);
        IsReady = true;
    }

    public async Task LoadMiniGame (MiniGameType miniGameType)
    {
        this.miniGameType = miniGameType;
        PanelService.Instance.Hide(PanelType.NavigationPanel);
        await SystemLocator.Instance.SceneController.LoadScene(
            Loaders[miniGameType].SceneName, 
            Loaders[miniGameType].Load());
    }

    public async Task UnloadMiniGame ()
    {
        await SystemLocator.Instance.SceneController.LoadScene(
            "NavigationScene", 
            Loaders[miniGameType].Unload());
        PanelService.Instance.Show(PanelType.NavigationPanel);
    }
}

public enum MiniGameType
{
    None = 0,
    Roulette = 1,
    TicTacToe = 2,
    TowerDefense = 3,
}
