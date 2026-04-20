
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

using ModularFW.Core.AudioSystem;
using ModularFW.Core.Signal;
using ModularFW.Core.SaveSystem;
using ModularFW.Core.PoolSystem;
using ModularFW.Core.HapticService;
using ModularFW.Core.InventorySystem;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.CurrencySystem;
using ModularFW.Core.Constants;
using ModularFW.Core.Analytics;
using ModularFW.Core;

namespace ModularFW.Core.Locator
{

    public class SystemLocator : MonoBehaviour
{
    public static SystemLocator Instance
    {
        get {
            if (instance == null)
            {
                instance = GameObject.FindFirstObjectByType<SystemLocator>();
                if (instance == null)
                {
                    Debug.LogError("[SystemLocator] No SystemLocator found in scene. Ensure the SystemLocator prefab is present.");
                    return null;
                }
                if (!instance.isInit) instance.Init();
            }
            return instance;
        }
    }

    private static SystemLocator instance;
    private bool isInit = false;
    private readonly Dictionary<Type, object> _registry = new Dictionary<Type, object>();

    [SerializeField] private ItemCollection ItemCollection;
    [SerializeField] private PoolCollection PoolCollection;
    [SerializeField] private AudioCollection AudioCollection;
    [SerializeField] private AudioSourceBlock AudioSourceBlock;

    public PoolingService PoolingService;
    public InventoryService InventoryService;
    public PanelService PanelService;

    public MiniGameService MiniGameService;
    public SceneController SceneController;

    public SignalBus SignalBus;
    public AnalyticsService AnalyticsService;

    public SaveLoadService SaveLoadService;
    public CurrencyService CurrencyService;
    public AudioService AudioService;
    public HapticService.HapticService HapticService;

    public Transform PanelParent;
    public List<BasePanel> Panels;
    public List<BasePanel> PreloadedPanels;

    [SerializeField]
    public List<BaseMiniGameLoader> MinigameLoaders;

    public T Get<T>() where T : class
    {
        _registry.TryGetValue(typeof(T), out var obj);
        return obj as T;
    }

    private void Register<TInterface>(object service) where TInterface : class
    {
        _registry[typeof(TInterface)] = service;
    }

    private async void Init()
    {
        GameObject.DontDestroyOnLoad(instance);

        SignalBus = new SignalBus();
        Register<SignalBus>(SignalBus);
        Register<ISignalBus>(SignalBus);

        AnalyticsService = new AnalyticsService();
        await AnalyticsService.Initialize();
        Register<AnalyticsService>(AnalyticsService);
        Register<IAnalyticsService>(AnalyticsService);

        SaveLoadService = new SaveLoadService();
        await SaveLoadService.Initialize();
        Register<SaveLoadService>(SaveLoadService);
        Register<ISaveLoadService>(SaveLoadService);

        HapticService = new HapticService.HapticService();
        HapticService.Initialize();
        Register<HapticService.HapticService>(HapticService);
        Register<IHapticService>(HapticService);

        CurrencyService = new CurrencyService();
        await CurrencyService.Initialize();
        Register<CurrencyService>(CurrencyService);
        Register<ICurrencyService>(CurrencyService);

        AudioService = new AudioService();
        await AudioService.Initialize(AudioCollection, AudioSourceBlock);
        Register<AudioService>(AudioService);
        Register<IAudioService>(AudioService);

        InventoryService = new InventoryService();
        await InventoryService.Initialize(ItemCollection);
        Register<InventoryService>(InventoryService);
        Register<IInventoryService>(InventoryService);

        PoolingService = new PoolingService();
        await PoolingService.Initialize(PoolCollection);
        Register<PoolingService>(PoolingService);
        Register<IPoolingService>(PoolingService);

        PanelService = new PanelService();
        await PanelService.Initialize(PanelParent, Panels, PreloadedPanels);
        Register<PanelService>(PanelService);
        Register<IPanelService>(PanelService);
        PanelService.Show(PanelType.NavigationPanel);

        MiniGameService = new MiniGameService();
        await MiniGameService.Initialize(MinigameLoaders);

        SceneController = new SceneController();
        await SceneController.LoadScene(SceneNames.Navigation);

        isInit = true;
    }

    private void OnDestroy()
    {
        if (isInit) SaveLoadService?.Shutdown();
    }
    }

public interface IService
{
    public bool IsReady { get; }
}
}
