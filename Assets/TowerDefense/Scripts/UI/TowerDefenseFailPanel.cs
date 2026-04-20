using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.Signal;

namespace MiniGame.TowerDefense {
public class TowerDefenseFailPanel : BasePanel
{
    public override PanelType PanelType => PanelType.TowerDefenseFailPanel;

    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DetailText;
    public Button RestartButton;

    public override void Show(params object[] parameters)
    {
        base.Show(parameters);
        if (TitleText != null) TitleText.text = "You Lost";
        if (DetailText != null) DetailText.text = "The tower was destroyed.";
        if (parameters != null && parameters.Length > 0 && parameters[0] is string)
            DetailText.text = (string)parameters[0];

        if (RestartButton != null)
        {
            RestartButton.onClick.RemoveAllListeners();
            RestartButton.onClick.AddListener(OnRestartClicked);
            RestartButton.gameObject.SetActive(true);
        }
    }

    private void OnRestartClicked()
    {
        SignalBus.Instance.Publish(new TowerDefenseRestartRequestedSignal());
        base.Hide();
    }
}
}
