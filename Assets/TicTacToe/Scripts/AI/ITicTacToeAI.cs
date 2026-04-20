namespace MiniGame.TicTacToe
{
    public interface ITicTacToeAI
    {
        int GetNextMove(int[] board, int aiPlayer);
    }
}
