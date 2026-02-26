using UnityEngine;

using System.Threading.Tasks;

public class BaseMiniGameLoader : ScriptableObject
{
    public virtual async Task Load()
    {
        await Task.Delay(1);
    }

    public virtual async Task Unload()
    {
        await Task.Delay(1);
    }

    public virtual MiniGameType MiniGameType => MiniGameType.None;

    public string SceneName;
}
