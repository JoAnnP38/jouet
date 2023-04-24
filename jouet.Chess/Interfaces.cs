using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jouet.Chess
{
    public interface IMoveContext
    {
        void Update(Position position);
        Color SideToMove { get; }
        Color Opponent { get; }
        public ulong FriendlyPawns { get; }
        public ulong FriendlyKnights { get; }
        public ulong FriendlyBishops { get; }
        public ulong FriendlyRooks { get; }
        public ulong FriendlyQueens { get; }
        public ulong FriendlyKings { get; }
        public ulong EnemyPawns { get; }
        public ulong EnemyKings { get; }
        public int PawnCaptureShiftLeft { get; }
        public int PawnCaptureShiftRight { get; }
        ulong Friends { get; }
        ulong Enemies { get; }
        ulong All { get; }
        int PawnStartingRank { get; }
        int PawnPromoteRank { get; }
        int StartingKingSquare { get; }
        CastlingRights KingSideMask { get; }
        ulong KingSideClearMask { get; }
        int KingSideTo { get; }
        CastlingRights QueenSideMask { get; }
        ulong QueenSideClearMask { get; }
        int QueenSideTo { get; }
        ulong PawnShift(ulong value, int shift);
    }
}
