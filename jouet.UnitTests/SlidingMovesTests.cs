using jouet.Chess;
using jouet.Utilities;
using Index = jouet.Chess.Index;

namespace jouet.UnitTests
{
    [TestClass]
    public class SlidingMovesTests
    {
        [TestMethod]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.F1, 3)]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.B6, 9)]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.E7, 7)]
        public void ClassicMoves_GetBishopAttacksTests(string fen, int sq, int attackCount)
        {
            Position position = new(fen);
            SlidingMoves sm = new ClassicMoves();
            ulong attacks = sm.GetBishopAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.H5, 7)]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.H7, 5)]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.F8, 10)]
        public void ClassicMoves_GetRookAttacksTests(string fen, int sq, int attackCount)
        {
            Position position = new(fen);
            SlidingMoves sm = new ClassicMoves();
            ulong attacks = sm.GetRookAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("3R4/3p3n/2qp1P1R/6P1/p2Qp3/2P3r1/K5k1/3N4 w - - 0 1", Index.D4, 18)]
        [DataRow("3R4/3p3n/2qp1P1R/6P1/p2Qp3/2P3r1/K5k1/3N4 w - - 0 1", Index.C6, 15)]
        public void ClassicMoves_GetQueenAttacks(string fen, int sq, int attackCount)
        {
            Position position = new(fen); 
            SlidingMoves sm = new ClassicMoves();
            ulong attacks = sm.GetQueenAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.F1, 3)]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.B6, 9)]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.E7, 7)]
        public void FancyMagicMoves_GetBishopAttacksTests(string fen, int sq, int attackCount)
        {
            Position position = new(fen);
            SlidingMoves sm = new FancyMagicMoves();
            ulong attacks = sm.GetBishopAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.H5, 7)]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.H7, 5)]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.F8, 10)]
        public void FancyMagicMoves_GetRookAttacksTests(string fen, int sq, int attackCount)
        {
            Position position = new(fen);
            SlidingMoves sm = new FancyMagicMoves();
            ulong attacks = sm.GetRookAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.F1, 3)]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.B6, 9)]
        [DataRow("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1", Index.E7, 7)]
        public void PextMoves_GetBishopAttacksTests(string fen, int sq, int attackCount)
        {
            Position position = new(fen);
            SlidingMoves sm = new PextMoves();
            ulong attacks = sm.GetBishopAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.H5, 7)]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.H7, 5)]
        [DataRow("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1", Index.F8, 10)]
        public void PextMoves_GetRookAttacksTests(string fen, int sq, int attackCount)
        {
            Position position = new(fen);
            SlidingMoves sm = new PextMoves();
            ulong attacks = sm.GetRookAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }

        [TestMethod]
        [DataRow("3R4/3p3n/2qp1P1R/6P1/p2Qp3/2P3r1/K5k1/3N4 w - - 0 1", Index.D4, 18)]
        [DataRow("3R4/3p3n/2qp1P1R/6P1/p2Qp3/2P3r1/K5k1/3N4 w - - 0 1", Index.C6, 15)]
        public void PextMoves_GetQueenAttacks(string fen, int sq, int attackCount)
        {
            Position position = new(fen); 
            SlidingMoves sm = new PextMoves();
            ulong attacks = sm.GetQueenAttacks(sq, position.All);
            Assert.AreEqual(attackCount, BitOps.PopCount(attacks));
        }
    }
}
