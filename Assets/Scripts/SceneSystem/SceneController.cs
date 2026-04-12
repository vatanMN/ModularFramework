using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ModularFW.Core.Locator;
using ModularFW.Core.PanelSystem;

public class SceneController : IService
{
    public static SceneController Instance => SystemLocator.Instance.SceneController;
    public bool IsReady { get; private set; }

    public SceneController()
    {
        IsReady = true;
    }

    public async Task LoadScene(string sceneName, Task AdditinalLoadTask = null)
    {
        // Show loading panel before starting load
        PanelService.Instance.Show(PanelType.LoadingPanel);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        int count = 0;
        while (!asyncLoad.isDone && count < 200)
        {
            await Task.Delay(1);
            count++;
        }
        
        if(AdditinalLoadTask != null) await AdditinalLoadTask;

        // Ensure loaders have a chance to initialize; small delay
        await Task.Delay(1);

        // Hide loading panel after scene is loaded
        PanelService.Instance.Hide(PanelType.LoadingPanel);
    }
}
