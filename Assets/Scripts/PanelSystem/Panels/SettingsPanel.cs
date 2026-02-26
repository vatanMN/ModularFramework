using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : BasePanel
{
    public override PanelType PanelType => PanelType.SettingsPanel;

    public Button AudioButton;
    public Button HapticButton;
    public TMPro.TextMeshProUGUI AudioButtonText;
    public TMPro.TextMeshProUGUI HapticButtonText;

    void Start()
    {
        RefreshUI();
        if (AudioButton != null) AudioButton.onClick.AddListener(ToggleAudio);
        if (HapticButton != null) HapticButton.onClick.AddListener(ToggleHaptic);
    }

    private void RefreshUI()
    {
        if (AudioButtonText != null)
            AudioButtonText.text = AudioService.Instance != null && AudioService.Instance.AudioEnabled ? "Audio: On" : "Audio: Off";
        if (HapticButtonText != null)
            HapticButtonText.text = HapticService.Instance != null && HapticService.Instance.HapticEnabled ? "Haptic: On" : "Haptic: Off";
    }

    private void ToggleAudio()
    {
        // Play tick sound and haptic feedback
        if (AudioService.Instance != null)
            AudioService.Instance.Play(AudioEnum.Tick);
        if (HapticService.Instance != null)
            HapticService.Instance.PlayHaptic(HapticType.Success);

        if (AudioService.Instance != null)
        {
            AudioService.Instance.SetAudioEnabled(!AudioService.Instance.AudioEnabled);
            RefreshUI();
        }
    }

    private void ToggleHaptic()
    {
        // Play tick sound and haptic feedback
        if (AudioService.Instance != null)
            AudioService.Instance.Play(AudioEnum.Tick);
        if (HapticService.Instance != null)
            HapticService.Instance.PlayHaptic(HapticType.Success);

        if (HapticService.Instance != null)
        {
            HapticService.Instance.SetHapticEnabled(!HapticService.Instance.HapticEnabled);
            RefreshUI();
        }
    }

    void OnDestroy()
    {
        if (AudioButton != null) AudioButton.onClick.RemoveListener(ToggleAudio);
        if (HapticButton != null) HapticButton.onClick.RemoveListener(ToggleHaptic);
    }
}
