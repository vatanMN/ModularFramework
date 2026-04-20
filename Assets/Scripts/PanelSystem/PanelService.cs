using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using ModularFW.Core.Locator;

namespace ModularFW.Core.PanelSystem {
public class PanelService : IService, ModularFW.Core.IPanelService
{
    public static PanelService Instance => SystemLocator.Instance.PanelService;
    public bool IsReady { get; private set; }
    private Transform PanelParent;
    Dictionary<PanelType, BasePanel> PanelDic = new Dictionary<PanelType, BasePanel>();
    Dictionary<PanelType, BasePanel> CreatedPanels = new Dictionary<PanelType, BasePanel>();

    // Panel hide events are published through SignalBus as PanelHiddenSignal

    public async Task Initialize(Transform panelParent, List<BasePanel> panels, List<BasePanel> preloadedPanels)
    {
        PanelParent = panelParent;
        foreach (var item in panels)
        {
            PanelDic[item.PanelType] = item;
        }

        foreach (var item in preloadedPanels)
        {
            CreatedPanels[item.PanelType] = item;
        }
        await Task.Delay(1);
        IsReady = true;
    }

    public void Show(PanelType panelType, params object[] parameters)
    {
        if (!CreatedPanels.ContainsKey(panelType))
        {
            var panel = GameObject.Instantiate(PanelDic[panelType], PanelParent);
            CreatedPanels[panelType] = panel;
        }
        CreatedPanels[panelType].Show(parameters);
    }

    public T Show<T>(PanelType panelType, params object[] parameters) where T : BasePanel
    {
        if (!CreatedPanels.ContainsKey(panelType))
        {
            var panel = GameObject.Instantiate(PanelDic[panelType], PanelParent);
            CreatedPanels[panelType] = panel;
        }
        CreatedPanels[panelType].Show(parameters);
        return CreatedPanels[panelType] as T;
    }

    public void Hide(PanelType panelType)
    {
        if (CreatedPanels.TryGetValue(panelType, out var panel))
            panel.Hide();
        else
            Debug.LogWarning($"[PanelService] Hide called on {panelType} which has not been shown.");
    }
}
}

public class PanelEvent : UnityEvent<PanelType> { };

public enum PanelType
{
    GainItemPanel,
    LevelEndPanel,
    InventoryPanel,
    RouletteGamePanel,
    NavigationPanel,
    LoadingPanel,
    TicTacToeGamePanel,
    TicTacToeResultPanel,
    TowerDefenseGamePanel,
    TowerDefenseFailPanel,
    SettingsPanel,
}
