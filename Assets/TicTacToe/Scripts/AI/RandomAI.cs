using System.Collections.Generic;

namespace MiniGame.TicTacToe
{
    public class RandomAI : ITicTacToeAI
    {
        private readonly System.Random _rng = new System.Random();

        public int GetNextMove(int[] board, int aiPlayer)
        {
            var empty = new List<int>();
            for (int i = 0; i < board.Length; i++)
                if (board[i] == 0) empty.Add(i);
            if (empty.Count == 0) return -1;
            return empty[_rng.Next(empty.Count)];
        }
    }
}
