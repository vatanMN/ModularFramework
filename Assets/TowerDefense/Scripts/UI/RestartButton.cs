using UnityEngine;
using ModularFW.Core.AudioSystem;

namespace MiniGame.TowerDefense {
public class RestartButton : MonoBehaviour
{
    public TowerDefenseEngine Engine;

    public void OnRestartClicked()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        if (Engine != null) Engine.RestartGame();
    }
}
}
