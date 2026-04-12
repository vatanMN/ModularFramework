using UnityEngine;
using ModularFW.Core.PanelSystem;

namespace MiniGame.TicTacToe {
public class TicTacToeGamePanel : BasePanel
{
    public override PanelType PanelType => PanelType.TicTacToeGamePanel;

    public TicTacToeEngine Engine;
}
}
