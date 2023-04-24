using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using jouet.Collections;
using jouet.Utilities;

using Index = jouet.Chess.Index;

namespace jouet.Chess
{
    public unsafe class PextMoves : SlidingMoves 
    {
        #region struct PextEntry

        [StructLayout(LayoutKind.Sequential)]
        public struct PextEntry
        {
            public ulong* pAtkRook;
            public ulong* pAtkBish;
            public ulong MaskRook;
            public ulong MaskBish;

            public PextEntry(int offsetRook, int offsetBish, ulong maskRook, ulong maskBish)
            {
                fixed (ulong* ptr = &attacks[offsetRook])
                {
                    pAtkRook = ptr;
                }

                fixed (ulong* ptr = &attacks[offsetBish])
                {
                    pAtkBish = ptr;
                }
                MaskRook = maskRook;
                MaskBish = maskBish;
            }
        }

        #endregion

        static PextMoves()
        {
            for (int sq = 0; sq < Constants.MAX_SQUARES; sq++)
            {
                Index.ToCoords(sq, out int file, out int rank);
                entries[sq] = CreateEntry(sq, RelevantRookSee(file, rank), RelevantBishopSee(file, rank));
            }
        }

        public override ulong GetBishopAttacks(int square, ulong blockers)
        {
            ref PextEntry entry = ref entries[square];
            return entry.pAtkBish[BitOps.ParallelBitExtract(blockers, entry.MaskBish)];
        }

        public override ulong GetRookAttacks(int square, ulong blockers)
        {
            ref PextEntry entry = ref entries[square];
            return entry.pAtkRook[BitOps.ParallelBitExtract(blockers, entry.MaskRook)];
        }

        public override ulong GetQueenAttacks(int square, ulong blockers)
        {
            ref PextEntry entry = ref entries[square];
            return entry.pAtkBish[BitOps.ParallelBitExtract(blockers, entry.MaskBish)] |
                   entry.pAtkRook[BitOps.ParallelBitExtract(blockers, entry.MaskRook)];
        }

        public new static bool IsSupported => Bmi2.X64.IsSupported;

        private static PextEntry CreateEntry(int sq, ulong maskRook, ulong maskBish)
        {
            ClassicMoves cm = new();
            int offsetRook = attacks.Count;
            int cnt = BitOps.PopCount(maskRook);
            for (ulong i = 0; i < (1ul << cnt); i++)
            {
                ulong blockers = BitOps.ParallelBitDeposit(i, maskRook);
                attacks.Add(cm.GetRookAttacks(sq, blockers));
            }

            int offsetBish = attacks.Count;
            cnt = BitOps.PopCount(maskBish);
            for (ulong i = 0; i < (1ul << cnt); i++)
            {
                ulong blockers = BitOps.ParallelBitDeposit(i, maskBish);
                attacks.Add(cm.GetBishopAttacks(sq, blockers));
            }

            return new PextEntry(offsetRook, offsetBish, maskRook, maskBish);
        }

        private static readonly UnsafeArray<ulong> attacks = new(107648);
        private static readonly UnsafeArray<PextEntry> entries = new(Constants.MAX_SQUARES);
    }
}
