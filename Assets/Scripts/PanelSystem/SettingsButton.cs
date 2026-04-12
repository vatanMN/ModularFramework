using ModularFW.Core.HapticService;
using UnityEngine;
using UnityEngine.UI;
using ModularFW.Core.AudioSystem;

namespace ModularFW.Core.PanelSystem {
public class SettingsButton : MonoBehaviour
{
    public Button Button;

    void Awake()
    {
        if (Button == null) Button = GetComponent<Button>();
        if (Button != null) Button.onClick.AddListener(OnClick);
    }

    void OnDestroy()
    {
        if (Button != null) Button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        // Play tick sound and haptic feedback
        if (AudioService.Instance != null)
            AudioService.Instance.Play(AudioEnum.Tick);
        if (HapticService.HapticService.Instance != null)
            HapticService.HapticService.Instance.PlayHaptic(HapticType.Success);

        if (PanelService.Instance != null)
        {
            PanelService.Instance.Show(PanelType.SettingsPanel);
        }
    }
}
}
