using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "TicTacToeLoader", menuName = "Scriptable Objects/TicTacToeLoader")]
public class TicTacToeLoader : BaseMiniGameLoader
{
    public override MiniGameType MiniGameType => MiniGameType.TicTacToe;
    public int RewardCoins = 0;

    public override async Task Load()
    {
        PanelService.Instance.Show(PanelType.TicTacToeGamePanel);
        
        var board =  GameObject.FindObjectOfType<TicTacToeEngine>();
        board.Initialize(1,1);
        board.CoinReward = RewardCoins;
        board.StartGame();
        await Task.Delay(1);
    }

    public override async Task Unload()
    {
        PanelService.Instance.Hide(PanelType.TicTacToeGamePanel);
        await Task.Delay(1);
    }
}
