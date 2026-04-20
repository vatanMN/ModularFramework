using NUnit.Framework;
using MiniGame.TicTacToe;

namespace Tests.EditMode
{
    // Board layout:
    //  0 | 1 | 2
    //  ---------
    //  3 | 4 | 5
    //  ---------
    //  6 | 7 | 8
    // 1 = player X, -1 = AI O, 0 = empty

    [TestFixture]
    public class BlockingAITests
    {
        private BlockingAI _ai;

        [SetUp]
        public void SetUp() => _ai = new BlockingAI();

        [Test]
        public void TakesWinningMove_WhenAvailable()
        {
            // AI at 0,1 — wins at 2 (top row)
            var board = new[] { -1, -1, 0, 0, 0, 0, 0, 0, 0 };
            Assert.AreEqual(2, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void BlocksOpponentWin()
        {
            // Player at 0,1 — about to win at 2; AI should block
            var board = new[] { 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            Assert.AreEqual(2, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void PrefersWinOverBlock()
        {
            // AI can win at 2 (row 0-1-2); player can win at 5 (row 3-4-5)
            // AI should take the win, not play defence
            var board = new[] { -1, -1, 0, 1, 1, 0, 0, 0, 0 };
            Assert.AreEqual(2, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void FallsBackToFirstEmpty_WhenNoWinOrBlock()
        {
            var board = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.AreEqual(0, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void SkipsOccupiedCells_ForFirstEmpty()
        {
            // First three cells occupied — should pick index 3
            var board = new[] { 1, -1, 1, 0, 0, 0, 0, 0, 0 };
            Assert.AreEqual(3, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void ReturnsMinusOne_OnFullBoard()
        {
            var board = new[] { 1, -1, 1, -1, 1, -1, -1, 1, -1 };
            Assert.AreEqual(-1, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void LeavesBoard_Unmodified()
        {
            var board = new[] { 1, 0, 0, 0, -1, 0, 0, 0, 1 };
            var before = (int[])board.Clone();
            _ai.GetNextMove(board, -1);
            Assert.AreEqual(before, board);
        }
    }
}
