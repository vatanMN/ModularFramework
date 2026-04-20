using NUnit.Framework;
using MiniGame.TicTacToe;

namespace Tests.EditMode
{
    [TestFixture]
    public class RandomAITests
    {
        private RandomAI _ai;

        [SetUp]
        public void SetUp() => _ai = new RandomAI();

        [Test]
        public void ReturnsValidIndex_OnNonFullBoard()
        {
            var board = new[] { 1, 0, 0, 0, -1, 0, 0, 0, 1 };
            int move = _ai.GetNextMove(board, -1);
            Assert.IsTrue(move >= 0 && move < 9, $"Expected 0–8, got {move}");
            Assert.AreEqual(0, board[move], "Returned index must point to an empty cell");
        }

        [Test]
        public void ReturnsMinusOne_OnFullBoard()
        {
            var board = new[] { 1, -1, 1, -1, 1, -1, -1, 1, -1 };
            Assert.AreEqual(-1, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void NeverPicksOccupiedCell()
        {
            // Only index 4 is empty — must always return 4
            var board = new[] { 1, 1, 1, 1, 0, 1, 1, 1, 1 };
            for (int i = 0; i < 30; i++)
                Assert.AreEqual(4, _ai.GetNextMove(board, -1));
        }

        [Test]
        public void LeavesBoard_Unmodified()
        {
            var board = new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            var before = (int[])board.Clone();
            _ai.GetNextMove(board, -1);
            Assert.AreEqual(before, board);
        }
    }
}
