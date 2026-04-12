using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ModularFW.Core.Signal;
using ModularFW.Core.AudioSystem;

namespace MiniGame.TicTacToe {
public class CellUI : MonoBehaviour
{
    public int Index;
    public TicTacToeEngine Engine;
    public UnityEngine.UI.Image DisplayImage;
    public Sprite XSprite;
    public Sprite OSprite;

    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnClick);
    }

    void Start()
    {
        SignalBus.Instance.Subscribe<TicTacToeBoardUpdatedSignal>(OnBoardUpdatedSignal);
        Refresh();
    }

    void OnDestroy()
    {
        SignalBus.Instance.Unsubscribe<TicTacToeBoardUpdatedSignal>(OnBoardUpdatedSignal);
    }

    private void OnBoardUpdatedSignal(TicTacToeBoardUpdatedSignal s)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (DisplayImage == null) return;
        if (Engine == null)
        {
            DisplayImage.enabled = false;
        }
        else
        {
            var v = Engine.GetCellValue(Index);
            if (v == 1)
            {
                DisplayImage.sprite = XSprite;
                DisplayImage.enabled = true;
            }
            else if (v == -1)
            {
                DisplayImage.sprite = OSprite;
                DisplayImage.enabled = true;
            }
            else
            {
                DisplayImage.enabled = false;
            }
        }
        if (btn != null) btn.interactable = (Engine==null) ? false : (Engine.GetCellValue(Index)==0);
    }

    private void OnClick()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        Engine?.PlayMove(Index);
    }
}
}
