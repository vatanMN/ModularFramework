using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] Button Button;
    // Start is called before the first frame update
    void OnEnable()
    {
        Button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void OnDisable()
    {
        Button.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        MiniGameService.Instance.UnloadMiniGame();
    }
}
