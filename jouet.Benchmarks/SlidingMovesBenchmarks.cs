using BenchmarkDotNet.Attributes;
using jouet.Chess;
using Index = jouet.Chess.Index;

namespace jouet.Benchmarks
{
    public class SlidingMovesBenchmarks
    {
        public SlidingMoves classic = new ClassicMoves();
        public SlidingMoves fancy = new FancyMagicMoves();
        public SlidingMoves pext = new PextMoves();
        public Position bishopPosition = new("8/1p2B2n/1b6/P4Pp1/1p3p1p/2P3k1/4K3/5BNR w - - 0 1");
        public Position rookPosition = new("5R2/4K2R/6pN/Ppn2P1r/3Q4/k7/P3pP2/3q4 w - - 0 1");

        [GlobalSetup]
#pragma warning disable CA1822
        public void GlobalSetup()
#pragma warning restore CA1822
        {
            bool _ = ClassicMoves.IsSupported &&
                     FancyMagicMoves.IsSupported &&
                     PextMoves.IsSupported;
        }

        [Benchmark(Baseline = true)]
        public ulong ClassicMovesBenchmark()
        {
            ulong attacks = classic.GetBishopAttacks(Index.F1, bishopPosition.All);
            attacks |= classic.GetRookAttacks(Index.H5, rookPosition.All);
            return attacks;
        }

        [Benchmark]
        public ulong FancyMagicMovesBenchmark()
        {
            ulong attacks = fancy.GetBishopAttacks(Index.F1, bishopPosition.All);
            attacks |= fancy.GetRookAttacks(Index.H5, rookPosition.All);
            return attacks;
        }

        [Benchmark]
        public ulong PextMovesBenchmark()
        {
            ulong attacks = pext.GetBishopAttacks(Index.F1, bishopPosition.All);
            attacks |= pext.GetRookAttacks(Index.H5, rookPosition.All);
            return attacks;
        }
    }
}
