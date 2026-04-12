
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
                if (!instance.isInit) instance.Init();
            }
            return instance;
        }
    }

    private static SystemLocator instance;
    private bool isInit = false;

    [SerializeField] private ItemCollection ItemCollection;
    [SerializeField] private PoolCollection PoolCollection;

    [SerializeField] private AudioCollection AudioCollection;
    public PoolingService PoolingService;
    public InventoryService InventoryService;
    public PanelService PanelService;

    public MiniGameService MiniGameService;
    public SceneController SceneController;

    public SignalBus SignalBus;


    public SaveLoadService SaveLoadService;
    public CurrencyService CurrencyService;
    public AudioService AudioService;
    public HapticService.HapticService HapticService;

    public Transform PanelParent;
    public List<BasePanel> Panels;
    public List<BasePanel> PreloadedPanels;

    [SerializeField]
    public List<BaseMiniGameLoader> MinigameLoaders;

    private async void Init()
    {
        GameObject.DontDestroyOnLoad(instance);
        SignalBus = new SignalBus();
        SaveLoadService = new SaveLoadService();
        await SaveLoadService.Initialize();

        HapticService = new HapticService.HapticService();
        HapticService.Initialize();

        // currency service (depends on SaveLoadService)
        CurrencyService = new CurrencyService();
        await CurrencyService.Initialize();

        AudioService = new AudioService();
        await AudioService.Initialize(AudioCollection);

        InventoryService = new InventoryService();
        await InventoryService.Initialize(ItemCollection);
        PoolingService = new PoolingService();
        await PoolingService.Initialize(PoolCollection);


        PanelService = new PanelService();
        await PanelService.Initialize(PanelParent, Panels, PreloadedPanels);
        PanelService.Show(PanelType.NavigationPanel);

        MiniGameService = new MiniGameService();
        await MiniGameService.Initialize(MinigameLoaders);

        SceneController = new SceneController();
        await SceneController.LoadScene("NavigationScene");

        isInit = true;
    }
    }
}

public interface IService
{
    public bool IsReady { get; }
}
