namespace MiniGame.TicTacToe
{
    public class BlockingAI : ITicTacToeAI
    {
        public int GetNextMove(int[] board, int aiPlayer)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    board[i] = aiPlayer;
                    if (CheckWin(board) == aiPlayer) { board[i] = 0; return i; }
                    board[i] = 0;
                }
            }
            int opp = -aiPlayer;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    board[i] = opp;
                    if (CheckWin(board) == opp) { board[i] = 0; return i; }
                    board[i] = 0;
                }
            }
            for (int i = 0; i < board.Length; i++) if (board[i] == 0) return i;
            return -1;
        }

        private static int CheckWin(int[] cells)
        {
            int[,] lines = new int[,] {
                {0,1,2},{3,4,5},{6,7,8},
                {0,3,6},{1,4,7},{2,5,8},
                {0,4,8},{2,4,6}
            };
            for (int i = 0; i < 8; i++)
            {
                int a = lines[i, 0], b = lines[i, 1], c = lines[i, 2];
                if (cells[a] != 0 && cells[a] == cells[b] && cells[b] == cells[c])
                    return cells[a];
            }
            return 0;
        }
    }
}
