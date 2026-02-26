using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeUIController : MonoBehaviour
{
    public TicTacToeEngine Engine;
    public TextMeshProUGUI ResultText;
    public Button RestartButton;

    void Start()
    {
        if (Engine == null) Engine = FindObjectOfType<TicTacToeEngine>();
        if (Engine != null)
        {
            SignalBus.Instance.Subscribe<TicTacToeGameEndedSignal>(OnGameEndedSignal);
            SignalBus.Instance.Subscribe<TicTacToeBoardUpdatedSignal>(OnBoardUpdatedSignal);
        }

        if (RestartButton != null) RestartButton.onClick.AddListener(()=> {
            Engine?.StartGame();
            if (ResultText!=null) ResultText.text = "";
        });
    }

    void OnDestroy()
    {
        if (Engine != null)
        {
            SignalBus.Instance.Unsubscribe<TicTacToeGameEndedSignal>(OnGameEndedSignal);
            SignalBus.Instance.Unsubscribe<TicTacToeBoardUpdatedSignal>(OnBoardUpdatedSignal);
        }
    }

    private void OnBoardUpdatedSignal(TicTacToeBoardUpdatedSignal s)
    {
        // UI cells update themselves via CellUI subscriptions; keep for extension.
    }

    private void OnGameEndedSignal(TicTacToeGameEndedSignal s)
    {
        if (ResultText != null) ResultText.text = s.Message;
    }
}
