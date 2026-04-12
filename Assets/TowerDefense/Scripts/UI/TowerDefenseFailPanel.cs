using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ModularFW.Core.PanelSystem;

namespace MiniGame.TowerDefense {
public class TowerDefenseFailPanel : BasePanel
{
    public override PanelType PanelType => PanelType.TowerDefenseFailPanel;

    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DetailText;
    public Button RestartButton;

    private TowerDefenseEngine engine;

    public override void Show(params object[] parameters)
    {
        base.Show(parameters);
        if (TitleText != null) TitleText.text = "You Lost";
        if (DetailText != null) DetailText.text = "The tower was destroyed.";
        if (parameters != null && parameters.Length > 0 && parameters[0] is string)
        {
            DetailText.text = (string)parameters[0];
            if(parameters.Length > 1 && parameters[1] is TowerDefenseEngine)
            {
                engine = (TowerDefenseEngine)parameters[1];
            }
        }

        if (RestartButton != null)
        {
            RestartButton.onClick.AddListener(OnRestartClicked);
            RestartButton.gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (RestartButton != null) RestartButton.onClick.RemoveListener(OnRestartClicked);
    }

    private void OnRestartClicked()
    {
        if (engine != null) engine.RestartGame();
        base.Hide();
    }
}
}
