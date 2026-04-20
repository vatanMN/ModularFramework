using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ModularFW.Core.Signal;
using ModularFW.Core.AudioSystem;
using ModularFW.Core.PanelSystem;

namespace MiniGame.TicTacToe {
public class TicTacToeResultPanel : BasePanel
{
    public override PanelType PanelType => PanelType.TicTacToeResultPanel;

    public TextMeshProUGUI MessageText;
    public TextMeshProUGUI CoinText;
    public Button RestartButton;

    public override void Show(params object[] parameters)
    {
        base.Show(parameters);
        string msg = "";
        int coins = 0;
        if (parameters != null && parameters.Length > 0)
        {
            if (parameters[0] is string) msg = (string)parameters[0];
            if (parameters.Length > 1 && parameters[1] is int) coins = (int)parameters[1];
        }
        if (MessageText != null) MessageText.text = msg;
        if (CoinText != null) CoinText.text = coins > 0 ? $"{coins}" : "0";

        if (RestartButton != null)
        {
            RestartButton.onClick.RemoveAllListeners();
            RestartButton.onClick.AddListener(OnRestartClicked);
            RestartButton.gameObject.SetActive(true);
        }
    }

    private void OnRestartClicked()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        SignalBus.Instance.Publish(new TicTacToeRestartRequestedSignal());
        base.Hide();
    }
}
}
