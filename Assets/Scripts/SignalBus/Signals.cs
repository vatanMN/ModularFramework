using System;
using UnityEngine;

[Serializable]
public class PanelHiddenSignal
{
    public PanelType PanelType;
}

[Serializable]
public class CoinsChangedSignal
{
    public int Coins;
}

[Serializable]
public class TicTacToeBoardUpdatedSignal
{
    public int[] Board;
}

[Serializable]
public class TicTacToeGameEndedSignal
{
    public string Message;
}

[Serializable]
public class UpgradesResetSignal { }
