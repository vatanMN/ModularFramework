using ModularFW.Core.InventorySystem;
using ModularFW.Core.HapticService;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using ModularFW.Core.Locator;
using ModularFW.Core.Signal;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.CurrencySystem;

namespace MiniGame.TicTacToe {
// Minimal TicTacToe gameplay engine. It is intentionally simple — UI hookup
// should be done in prefabs. When the player (X) wins, it grants rewards via InventoryService.
public class TicTacToeEngine : MonoBehaviour
{
    private int[] cells = new int[9]; // 0 empty, 1 player X, -1 player O
    private int currentPlayer = 1;

    private int rewardItemId = 0;
    private int rewardCount = 1;
    public int CoinReward = 0;

    public void Initialize(int rewardItem, int rewardAmount)
    {
        rewardItemId = rewardItem;
        rewardCount = rewardAmount;
    }

    public void StartGame()
    {
        for (int i = 0; i < cells.Length; i++) cells[i] = 0;
        currentPlayer = 1;
        SignalBus.Instance.Publish(new TicTacToeBoardUpdatedSignal() { Board = (int[])cells.Clone() });
    }

    // Call to play at index from external UI
    bool isWaiting = false;
    public bool PlayMove(int index)
    {
        if (isWaiting) return false;
        if (index < 0 || index >= cells.Length) return false;
        if (cells[index] != 0) return false;
        cells[index] = currentPlayer;
        // haptic for player click only
        if (currentPlayer == 1 && HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        SignalBus.Instance.Publish(new TicTacToeBoardUpdatedSignal() { Board = (int[])cells.Clone() });
        int res = CheckWin();
        if (res != 0)
        {
            EndGame(res);
            return true;
        }
        currentPlayer *= -1;
        // simple AI move
        if (currentPlayer == -1)
        {
            AIMove();
        }
        return true;
    }

    private async void AIMove()
    {
        isWaiting =true;
        int ai = GetBestMove(-1);
        await Task.Delay(500); // simulate thinking time

        isWaiting = false;
        if (ai >= 0) PlayMove(ai);
        // haptic for AI click
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Warning);
    }

    private void EndGame(int result)
    {
        if (result == 1)
        {
            if (rewardItemId != 0)
            {
                InventoryService.Instance.GainItem(rewardItemId, rewardCount);
            }
            // award coin reward if configured
            if (CoinReward > 0 && CurrencyService.Instance != null)
            {
                CurrencyService.Instance.AddCoins(CoinReward);
                // Animate flying coin to active CoinUI
                var coinUI = CurrencyService.Instance.GetActiveCoinUI();
                if (coinUI != null)
                {
                    // Use the center of the board as the world position for the coin spawn
                    var boardCenter = Vector3.zero;
                    var boardObj = GameObject.Find("TicTacToeBoard");
                    if (boardObj != null)
                        boardCenter = boardObj.transform.position;
                    coinUI.SpawnFlyingCoin(boardCenter, CoinReward);
                }
            }
            SignalBus.Instance.Publish(new TicTacToeGameEndedSignal() { Message = "You Win!" });
            // show result panel with message and coin reward
            if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TicTacToeResultPanel, "You Win!", CoinReward, this);
        }
        else if (result == -1)
        {
            SignalBus.Instance.Publish(new TicTacToeGameEndedSignal() { Message = "You Lose" });
            if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TicTacToeResultPanel, "You Lose", 0, this);
        }
        else
        {
            SignalBus.Instance.Publish(new TicTacToeGameEndedSignal() { Message = "Draw" });
            if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TicTacToeResultPanel, "Draw", 0, this);
        }
    }

    // Expose read-only accessors and events for UI
    public int GetCellValue(int index)
    {
        if (index < 0 || index >= cells.Length) return 0;
        return cells[index];
    }

    // events migrated to SignalBus: TicTacToeBoardUpdatedSignal, TicTacToeGameEndedSignal

    // Returns 1 if X wins, -1 if O wins, 0 otherwise, 2 for draw
    private int CheckWin()
    {
        int[,] lines = new int[,] {
            {0,1,2}, {3,4,5}, {6,7,8},
            {0,3,6}, {1,4,7}, {2,5,8},
            {0,4,8}, {2,4,6}
        };
        for (int i = 0; i < 8; i++)
        {
            int a = lines[i, 0], b = lines[i, 1], c = lines[i, 2];
            if (cells[a] != 0 && cells[a] == cells[b] && cells[b] == cells[c])
                return cells[a];
        }
        bool anyEmpty = false;
        foreach (var v in cells) if (v == 0) anyEmpty = true;
        if (!anyEmpty) return 2;
        return 0;
    }

    private int GetBestMove(int player)
    {
        // try winning move
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i] == 0)
            {
                cells[i] = player;
                int r = CheckWin();
                cells[i] = 0;
                if (r == player) return i;
            }
        }
        // block opponent
        int opp = -player;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i] == 0)
            {
                cells[i] = opp;
                int r = CheckWin();
                cells[i] = 0;
                if (r == opp) return i;
            }
        }
        // first empty
        for (int i = 0; i < cells.Length; i++) if (cells[i] == 0) return i;
        return -1;
    }
}
}
