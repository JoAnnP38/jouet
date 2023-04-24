using jouet.Chess;
using jouet.Utilities;
using Index = jouet.Chess.Index;

namespace jouet.UnitTests
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void DefaultCtorTest()
        {
            Position position = new();
            Assert.IsNotNull(position);
        }

        [TestMethod]
        public void FenCtorTest()
        {
            Position position = new(Notation.FEN_START_POS);
            Assert.IsNotNull(position);
        }

        [TestMethod]
        public void CloneTest()
        {
            Position position = new(Notation.FEN_START_POS);
            Position clone = position.Clone();
            Assert.AreEqual(position.ToString(), clone.ToString());
        }

        [TestMethod]
        public void GenerateMovesTest()
        {
            Position position = new(Notation.FEN_START_POS);
            MoveList list = new();
            position.GenerateMoves(list);
            for (int n = 0; n < list.Count; n++)
            {
                Util.WriteLine(Move.ToLongString(list[n]));
            }
            Assert.AreEqual(20, list.Count);
        }

        [TestMethod]
        public void CaptureScoreTest()
        {
            int score = Position.CaptureScore(Piece.Pawn, Piece.Bishop);
            Assert.IsTrue(score >= Constants.CAPTURE_SCORE);
        }

        [TestMethod]
        public void EpOffsetTest()
        {
            int offset = Position.EpOffset(Color.White);
            Assert.AreEqual(-8, offset);

            offset = Position.EpOffset(Color.Black);
            Assert.AreEqual(8, offset);
        }

        [TestMethod]
        public void IsCheckedTest()
        {
            Position position = new("rnbqkbnr/pppp1ppp/4p3/8/8/BP6/P1PPPPPP/RN1QKBNR b KQkq - 0 1");
            ulong move = Move.Pack(Piece.King, Index.E8, Index.E7, MoveType.Normal);
            Assert.IsFalse(position.MakeMove(move));
        }
    }
}