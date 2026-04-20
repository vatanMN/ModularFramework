using System;
using UnityEngine;
using System.Threading.Tasks;
using ModularFW.Core.HapticService;
using ModularFW.Core.InventorySystem;
using ModularFW.Core.Signal;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.CurrencySystem;

namespace MiniGame.TicTacToe {
public class TicTacToeEngine : MonoBehaviour
{
    [SerializeField] private Transform boardTransform;

    private int[] cells = new int[9]; // 0 empty, 1 player X, -1 player O
    private int currentPlayer = 1;

    private const int NoRewardItemId = 0;
    private int rewardItemId = NoRewardItemId;
    private int rewardCount = 1;
    public int CoinReward = 0;

    private ITicTacToeAI _ai = new BlockingAI();
    private IDisposable _restartSub;

    public void SetAI(ITicTacToeAI ai) => _ai = ai ?? new BlockingAI();

    public void Initialize(int rewardItem, int rewardAmount)
    {
        rewardItemId = rewardItem;
        rewardCount = rewardAmount;
    }

    void Awake()
    {
        _restartSub = SignalBus.Instance.SubscribeTracked<TicTacToeRestartRequestedSignal>(_ => StartGame());
    }

    void OnDestroy()
    {
        _restartSub?.Dispose();
    }

    public void StartGame()
    {
        for (int i = 0; i < cells.Length; i++) cells[i] = 0;
        currentPlayer = 1;
        SignalBus.Instance.Publish(new TicTacToeBoardUpdatedSignal() { Board = (int[])cells.Clone() });
    }

    bool isWaiting = false;
    public bool PlayMove(int index)
    {
        if (isWaiting) return false;
        if (index < 0 || index >= cells.Length) return false;
        if (cells[index] != 0) return false;
        cells[index] = currentPlayer;
        if (currentPlayer == 1 && HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
        SignalBus.Instance.Publish(new TicTacToeBoardUpdatedSignal() { Board = (int[])cells.Clone() });
        int res = CheckWin();
        if (res != 0)
        {
            EndGame(res);
            return true;
        }
        currentPlayer *= -1;
        if (currentPlayer == -1)
        {
            AIMove();
        }
        return true;
    }

    private async void AIMove()
    {
        isWaiting = true;
        int move = _ai.GetNextMove(cells, -1);
        await Task.Delay(500);
        isWaiting = false;
        if (move >= 0) PlayMove(move);
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Warning);
    }

    private void EndGame(int result)
    {
        if (result == 1)
        {
            if (rewardItemId != NoRewardItemId)
            {
                InventoryService.Instance.GainItem(rewardItemId, rewardCount);
            }
            if (CoinReward > 0 && CurrencyService.Instance != null)
            {
                CurrencyService.Instance.AddCoins(CoinReward);
                var coinUI = CurrencyService.Instance.GetActiveCoinUI();
                if (coinUI != null)
                {
                    var boardCenter = boardTransform != null ? boardTransform.position : Vector3.zero;
                    if (boardTransform == null)
                        Debug.LogWarning("[TicTacToeEngine] boardTransform not assigned; coin spawn defaults to Vector3.zero.");
                    coinUI.SpawnFlyingCoin(boardCenter, CoinReward);
                }
            }
            SignalBus.Instance.Publish(new TicTacToeGameEndedSignal() { Message = "You Win!" });
            if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TicTacToeResultPanel, "You Win!", CoinReward);
        }
        else if (result == -1)
        {
            SignalBus.Instance.Publish(new TicTacToeGameEndedSignal() { Message = "You Lose" });
            if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TicTacToeResultPanel, "You Lose", 0);
        }
        else
        {
            SignalBus.Instance.Publish(new TicTacToeGameEndedSignal() { Message = "Draw" });
            if (PanelService.Instance != null) PanelService.Instance.Show(PanelType.TicTacToeResultPanel, "Draw", 0);
        }
    }

    public int GetCellValue(int index)
    {
        if (index < 0 || index >= cells.Length) return 0;
        return cells[index];
    }

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
}
}
