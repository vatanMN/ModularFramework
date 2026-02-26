using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeResultPanel : BasePanel
{
    public override PanelType PanelType => PanelType.TicTacToeResultPanel;

    public TextMeshProUGUI MessageText;
    public TextMeshProUGUI CoinText;
    public Button RestartButton;

    TicTacToeEngine engine;

    public override void Show(params object[] parameters)
    {
        base.Show(parameters);
        string msg = "";
        int coins = 0;
        if (parameters != null && parameters.Length > 0)
        {
            if (parameters[0] is string) msg = (string)parameters[0];
                if (parameters.Length > 1 && parameters[1] is int) coins = (int)parameters[1];
                if (parameters.Length > 2 && parameters[2] is TicTacToeEngine) engine = (TicTacToeEngine)parameters[2];
        }
        if (MessageText != null) MessageText.text = msg;
        if (CoinText != null) CoinText.text = (coins > 0) ? $"{coins}" : "0";

        if (RestartButton != null)
        {
            RestartButton.onClick.AddListener(OnRestartClicked);
            RestartButton.onClick.AddListener(() => {
                if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
            });
            RestartButton.gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (RestartButton != null) RestartButton.onClick.RemoveListener(OnRestartClicked);
    }

    private void OnRestartClicked()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        if (engine != null)
        {
            engine.StartGame();
        }
        base.Hide();
    }

    public override void Hide()
    {
        if (engine != null)
        {
            engine.StartGame();
        }
        base.Hide();
    }
}
