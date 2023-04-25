#if DEBUG
#undef DEBUG
#endif
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using jouet.Collections;
using jouet.Utilities;
using Index = jouet.Chess.Index;

namespace jouet.Chess
{
    public sealed class Position : ICloneable
    {
        #region struct CastlingRookMove

        public readonly struct CastlingRookMove
        {
            public readonly int KingFrom;
            public readonly int KingTo;
            public readonly int KingMoveThrough;
            public readonly int RookFrom;
            public readonly int RookTo;
            public readonly CastlingRights CastlingMask;

            public CastlingRookMove(int kingFrom, int kingTo, int kingMoveThrough, int rookFrom, int rookTo, CastlingRights mask)
            {
                KingFrom = kingFrom;
                KingTo = kingTo;
                KingMoveThrough = kingMoveThrough;
                RookFrom = rookFrom;
                RookTo = rookTo;
                CastlingMask = mask;
            }
        }

        #endregion

        #region struct Ray

        public readonly struct Ray
        {
            private readonly ulong north;
            private readonly ulong northEast;
            private readonly ulong east;
            private readonly ulong southEast;
            private readonly ulong south;
            private readonly ulong southWest;
            private readonly ulong west;
            private readonly ulong northWest;

            public Ray(ulong north, ulong northEast, ulong east, ulong southEast, ulong south, ulong southWest, ulong west, ulong northWest)
            {
                this.north = north;
                this.northEast = northEast;
                this.east = east;
                this.southEast = southEast;
                this.south = south;
                this.southWest = southWest;
                this.west = west;
                this.northWest = northWest;
            }

            public ulong North => north;
            public ulong NorthEast => northEast;
            public ulong East => east;
            public ulong SouthEast => southEast;
            public ulong South => south;
            public ulong SouthWest => southWest;
            public ulong West => west;
            public ulong NorthWest => northWest;
        }

        #endregion

        #region struct MoveContext

        public class WhiteMoveContext : IMoveContext
        {
            public void Update(Position position)
            {
                Friends = position.UnitBitBoard(SideToMove);
                Enemies = position.UnitBitBoard(Opponent);
                All = position.All;
                FriendlyPawns = position.Pawns & Friends;
                FriendlyKnights = position.Knights & Friends;
                FriendlyBishops = position.Bishops & Friends;
                FriendlyRooks = position.Rooks & Friends;
                FriendlyQueens = position.Queens & Friends;
                FriendlyKings = position.Kings & Friends;
                EnemyPawns = position.Pawns & Enemies;
                EnemyKings = position.Kings & Enemies;
            }

            public Color SideToMove => Color.White;
            public Color Opponent => Color.Black;

            public ulong FriendlyPawns { get; private set; }

            public ulong FriendlyKnights { get; private set; }

            public ulong FriendlyBishops { get; private set; }

            public ulong FriendlyRooks { get; private set; }

            public ulong FriendlyQueens { get; private set; }

            public ulong FriendlyKings { get; private set; }
            public ulong EnemyPawns { get; private set; }
            public ulong EnemyKings { get; private set; }

            public ulong Friends { get; private set; }

            public ulong Enemies { get; private set; }
            public ulong All { get; private set; }
            public int PawnStartingRank => Coord.Rank2;
            public int PawnPromoteRank => Coord.Rank7;
            public int StartingKingSquare => Index.E1;
            public CastlingRights KingSideMask => CastlingRights.WhiteKingSide;
            public ulong KingSideClearMask => (1ul << Index.F1) | (1ul << Index.G1);
            public int KingSideTo => Index.G1;
            public CastlingRights QueenSideMask => CastlingRights.WhiteQueenSide;
            public ulong QueenSideClearMask => (1ul << Index.B1) | (1ul << Index.C1) | (1ul << Index.D1);
            public int QueenSideTo => Index.C1;
            public int PawnCaptureShiftLeft => 7;
            public int PawnCaptureShiftRight => 9;

            public ulong PawnShift(ulong value, int shift)
            {
                return value >> shift;
            }
        }

        public class BlackMoveContext : IMoveContext
        {
            public void Update(Position position)
            {
                Friends = position.UnitBitBoard(SideToMove);
                Enemies = position.UnitBitBoard(Opponent);
                All = position.All;
                FriendlyPawns = position.Pawns & Friends;
                FriendlyKnights = position.Knights & Friends;
                FriendlyBishops = position.Bishops & Friends;
                FriendlyRooks = position.Rooks & Friends;
                FriendlyQueens = position.Queens & Friends;
                FriendlyKings = position.Kings & Friends;
                EnemyPawns = position.Pawns & Enemies;
                EnemyKings = position.Kings & Enemies;
            }

            public Color SideToMove => Color.Black;
            public Color Opponent => Color.White;
            public ulong FriendlyPawns { get; private set; }
            public ulong FriendlyKnights { get; private set; }
            public ulong FriendlyBishops { get; private set; }
            public ulong FriendlyRooks { get; private set; }
            public ulong FriendlyQueens { get; private set; }
            public ulong FriendlyKings { get; private set; }
            public ulong EnemyPawns { get; private set; }
            public ulong EnemyKings { get; private set; }
            public ulong Friends { get; private set; }
            public ulong Enemies { get; private set; }
            public ulong All { get; private set; }
            public int PawnStartingRank => Coord.Rank7;
            public int PawnPromoteRank => Coord.Rank2;
            public int StartingKingSquare => Index.E8;
            public CastlingRights KingSideMask => CastlingRights.BlackKingSide;
            public ulong KingSideClearMask => (1ul << Index.F8) | (1ul << Index.G8);
            public int KingSideTo => Index.G8;
            public CastlingRights QueenSideMask => CastlingRights.BlackQueenSide;
            public ulong QueenSideClearMask => (1ul << Index.B8) | (1ul << Index.C8) | (1ul << Index.D8);
            public int QueenSideTo => Index.C8;
            public int PawnCaptureShiftLeft => 9;
            public int PawnCaptureShiftRight => 7;
            public ulong PawnShift(ulong value, int shift)
            {
                return value << shift;
            }
        }

        #endregion

        #region struct State

        public struct State
        {
            public State(Position pos, ulong move)
            {
                SideToMove = pos.sideToMove;
                Castling = pos.castling;
                EnPassant = pos.enPassant;
                EnPassantValidated = pos.enPassantValidated;
                HalfMoveClock = pos.halfMoveClock;
                FullMoveCounter = pos.fullMoveCounter;
                Hash = pos.hash;
                LastMove = move;
            }

            public void Restore(Position pos)
            {
                pos.sideToMove = SideToMove;
                pos.castling = Castling;
                pos.enPassant = EnPassant;
                pos.enPassantValidated = EnPassantValidated;
                pos.halfMoveClock = HalfMoveClock;
                pos.fullMoveCounter = FullMoveCounter;
                pos.hash = Hash;
            }

            public Color SideToMove;
            public CastlingRights Castling;
            public sbyte EnPassant;
            public sbyte EnPassantValidated;
            public short HalfMoveClock;
            public short FullMoveCounter;
            public ulong Hash;
            public ulong LastMove;
        }

        #endregion

        #region Constructors

        public Position()
        {
            sideToMove = Color.None;
            castling = CastlingRights.None;
            enPassant = Index.NONE;
            enPassantValidated = Index.NONE;
        }

        public Position(string fen)
        {
            LoadFen(fen);
        }

        private Position(Position other)
        {
            Array.Copy(other.pieces, pieces, Constants.MAX_PIECES);
            Array.Copy(other.units, units, Constants.MAX_COLORS);
            Array.Copy(other.board, board, Constants.MAX_SQUARES);
            sideToMove = other.sideToMove;
            castling = other.castling;
            enPassant = other.enPassant;
            enPassantValidated = other.enPassantValidated;
            halfMoveClock = other.halfMoveClock;
            fullMoveCounter = other.fullMoveCounter;
            hash = other.hash;
        }

        #endregion

        #region Accessors

        public ref ulong PieceBitBoard(Piece piece)
        {
            return ref pieces[(int)piece];
        }

        public ref ulong UnitBitBoard(Color color)
        {
            return ref units[(int)color];
        }

        public ref ulong Pawns => ref pieces[(int)Piece.Pawn];
        public ref ulong Knights => ref pieces[(int)Piece.Knight];
        public ref ulong Bishops => ref pieces[(int)Piece.Bishop];
        public ref ulong Rooks => ref pieces[(int)Piece.Rook];
        public ref ulong Queens => ref pieces[(int)Piece.Queen];
        public ref ulong Kings => ref pieces[(int)Piece.King];
        public ref ulong WhitePieces => ref units[(int)Color.White];
        public ref ulong BlackPieces => ref units[(int)Color.Black];
        public ulong All => all;

        public Color SideToMove => sideToMove;
        public Color Opponent => sideToMove.Flip();
        public CastlingRights Castling => castling;
        public sbyte EnPassant => enPassant;
        public sbyte EnPassantValidated => enPassantValidated;
        public short HalfMoveClock => halfMoveClock;
        public ulong Hash => hash;
        public short FullMoveCounter => fullMoveCounter;

        public ulong LastMove
        {
            get
            {
                if (gameStack.TryPeek(out State item))
                {
                    return item.LastMove;
                }

                return Move.NullMove;
            }
        }
        #endregion

        #region Incremental Board Updates

        public void AddPawn(Color color, int square)
        {
            AddPiece(color, Piece.Pawn, square);
            // TODO: update pawn hash when added
        }

        public void AddPiece(Color color, Piece piece, int square)
        {
            Util.Assert(color != Color.None);
            Util.Assert(piece != Piece.None);
            Util.Assert(Index.IsValid(square));
            Util.Assert(board[square].IsEmpty);

            ref ulong pieces = ref PieceBitBoard(piece);
            ref ulong units = ref UnitBitBoard(color);
            board[square] = Square.Create(color, piece);
            pieces = BitOps.SetBit(pieces, square);
            units = BitOps.SetBit(units, square);
            all = BitOps.SetBit(all, square);
            hash = ZobristHash.HashPiece(hash, color, piece, square);
        }

        public void AddPieceNoHash(Color color, Piece piece, int square)
        {
            Util.Assert(color != Color.None);
            Util.Assert(piece != Piece.None);
            Util.Assert(Index.IsValid(square));
            Util.Assert(board[square].IsEmpty);

            ref ulong pieces = ref PieceBitBoard(piece);
            ref ulong units = ref UnitBitBoard(color);
            board[square] = Square.Create(color, piece);
            pieces = BitOps.SetBit(pieces, square);
            units = BitOps.SetBit(units, square);
            all = BitOps.SetBit(all, square);
        }

        public void RemovePawn(Color color, int square)
        {
            RemovePiece(color, Piece.Pawn, square);
            // TODO: update pawn hash when added
        }

        public void RemovePiece(Color color, Piece piece, int square)
        {
            Util.Assert(color != Color.None);
            Util.Assert(piece != Piece.None);
            Util.Assert(Index.IsValid(square));
            Util.Assert(!board[square].IsEmpty);

            ref ulong pieces = ref PieceBitBoard(piece);
            ref ulong units = ref UnitBitBoard(color);
            board[square] = Square.Empty;
            pieces = BitOps.ResetBit(pieces, square);
            units = BitOps.ResetBit(units, square);
            all = BitOps.ResetBit(all, square);
            hash = ZobristHash.HashPiece(hash, color, piece, square);
        }

        public void RemovePieceNoHash(Color color, Piece piece, int square)
        {
            Util.Assert(color != Color.None);
            Util.Assert(piece != Piece.None);
            Util.Assert(Index.IsValid(square));
            Util.Assert(!board[square].IsEmpty);

            ref ulong pieces = ref PieceBitBoard(piece);
            ref ulong units = ref UnitBitBoard(color);
            board[square] = Square.Empty;
            pieces = BitOps.ResetBit(pieces, square);
            units = BitOps.ResetBit(units, square);
            all = BitOps.ResetBit(all, square);
        }

        #endregion

        #region FEN String / ToString

        public bool LoadFen(string fen)
        {
            if (!Notation.IsValidFen(fen))
            {
                return false;
            }

            Clear();
            string[] fenSections = fen.Split(' ');
            FenParsePieces(fenSections[0]);
            sideToMove = Notation.ParseFenColorToMove(fenSections[1]);
            hash = ZobristHash.HashActiveColor(hash, sideToMove);

            castling = Notation.ParseFenCastlingRights(fenSections[2]);
            hash = ZobristHash.HashCastling(hash, castling);

            enPassantValidated = Index.NONE;
            enPassant = (sbyte)Notation.ParseFenEnPassant(fenSections[3]);
            if (IsEnPassantValid(sideToMove))
            {
                enPassantValidated = enPassant;
                hash = ZobristHash.HashEnPassant(hash, enPassantValidated);
            }
            halfMoveClock = short.Parse(fenSections[4]);
            fullMoveCounter = short.Parse(fenSections[5]);
            return true;
        }

        public override string ToString()
        {
            return ToString("F");
        }

        public string ToString(string? format)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F";
            }

            return format.ToUpperInvariant() switch
            {
                "F" => ToFenString(),
                "V" => ToVerboseString(),
                _ => throw new ArgumentException($"'{format}' is an invalid format string for Position.")
            };
        }

        private string ToVerboseString()
        {
            StringBuilder sb = new();
            sb.AppendLine("  A B C D E F G H");
            for (int rank = Coord.MAX_VALUE; rank >= 0; rank--)
            {
                sb.Append(rank + 1);
                sb.Append(' ');
                for (int file = 0; file <= Coord.MAX_VALUE; file++)
                {
                    int sq = Index.ToIndex(file, rank);
                    sb.Append(board[sq].IsEmpty ? "." : board[sq].ToFenString());
                    sb.Append(' ');
                }

                sb.Append(rank + 1);
                sb.AppendLine();
            }
            sb.AppendLine("  A B C D E F G H");
            sb.AppendLine($"Turn: {sideToMove}");
            sb.AppendLine($"Castling: {castling}");
            sb.AppendLine($"EnPassant: {Index.ToString(enPassant)}");
            sb.AppendLine($"Half Move Clock: {halfMoveClock}");
            sb.AppendLine($"Turn: {fullMoveCounter}");
            sb.AppendLine($"Hash: 0x{hash,8:X}");
            return sb.ToString();
        }

        private string ToFenString()
        {
            StringBuilder sbFen = new();
            FenFormatPieces(sbFen);
            sbFen.Append(' ');
            sbFen.Append(sideToMove.ToFenString());
            sbFen.Append(' ');
            sbFen.Append(castling.ToFenString());
            sbFen.Append(' ');
            sbFen.Append(enPassant == Index.NONE ? "-" : Index.ToString(enPassant));
            sbFen.Append(' ');
            sbFen.Append($"{halfMoveClock} {fullMoveCounter}");
            return sbFen.ToString();
        }

        void FenFormatPieces(StringBuilder sb)
        {
            for (int rank = Coord.MAX_VALUE; rank >= 0; rank--)
            {
                int emptyCount = 0;
                for (int file = 0; file <= Coord.MAX_VALUE; file++)
                {
                    int sq = Index.ToIndex(file, rank);
                    if (board[sq].IsEmpty)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            sb.Append(emptyCount);
                            emptyCount = 0;
                        }

                        sb.Append(board[sq].ToFenString());
                    }
                }

                if (emptyCount > 0)
                {
                    sb.Append(emptyCount);
                }

                if (rank > 0)
                {
                    sb.Append('/');
                }
            }
        }

        private void FenParsePieces(string fenSection)
        {
            int rank = Coord.MAX_VALUE;
            int file = Coord.MIN_VALUE;

            for (int n = 0; n < fenSection.Length; n++)
            {
                char ch = fenSection[n];
                switch (ch)
                {
                    case >= '1' and <= '8':
                        file += ch - '0';
                        break;

                    case '/':
                        file = 0;
                        rank--;
                        break;

                    case 'p': case 'n': case 'b': case 'r': case 'q': case 'k':
                    case 'P': case 'N': case 'B': case 'R': case 'Q': case 'K':
                        var (color, piece) = Notation.ParseFenPiece(ch);
                        int sq = Index.ToIndex(file++, rank);
                        AddPiece(color, piece, sq);
                        break;

                    default:
                        throw new ArgumentException($"Illegal piece encountered in FEN record '{ch}'.");
                }
            }
        }
        #endregion

        #region Move Generation

        public void GenerateMoves(MoveList list)
        {
            IMoveContext ctx = contexts[(int)sideToMove];
            ctx.Update(this);

            if (ctx.FriendlyPawns != 0)
            {
                if (enPassantValidated != Index.NONE)
                {
                    GenerateEnPassant(list, ctx);
                }

                GeneratePawnMoves(list, ctx);
            }

            GenerateCastling(list, ctx);
            GeneratePieceMoves(list, Piece.Knight, ctx.FriendlyKnights, ctx);
            GeneratePieceMoves(list, Piece.Bishop, ctx.FriendlyBishops, ctx);
            GeneratePieceMoves(list, Piece.Rook, ctx.FriendlyRooks, ctx);
            GeneratePieceMoves(list, Piece.Queen, ctx.FriendlyQueens, ctx);
            GeneratePieceMoves(list, Piece.King, ctx.FriendlyKings, ctx);
        }

        public void GenerateEnPassant(MoveList list, IMoveContext ctx)
        {
            ulong bb = PawnDefends(ctx.SideToMove, enPassantValidated) & ctx.FriendlyPawns;
            for (; bb != 0; bb = BitOps.ResetLsb(bb))
            {
                int from = BitOps.TzCount(bb);
                int captIndex = enPassantValidated + EpOffset(ctx.SideToMove);
                list.Add(Piece.Pawn, from, enPassantValidated, MoveType.EnPassant, capture: (Piece)board[captIndex],
                    score: CaptureScore((Piece)board[captIndex], Piece.Pawn));
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void GeneratePawnMoves(MoveList list, IMoveContext ctx)
        {
            ulong bb1 = ctx.FriendlyPawns & ctx.PawnShift(ctx.Enemies & ~MaskFile(7), ctx.PawnCaptureShiftLeft);
            ulong bb2 = ctx.FriendlyPawns & ctx.PawnShift(ctx.Enemies & ~MaskFile(0), ctx.PawnCaptureShiftRight);
            ulong bb3 = ctx.FriendlyPawns & ~ctx.PawnShift(ctx.All, 8);
            ulong bb4 = bb3 & MaskRank(ctx.PawnStartingRank) & ~ctx.PawnShift(ctx.All, 16);
            ulong promoteMask = MaskRank(ctx.PawnPromoteRank);

            // extract out the pawns that will be moving to promote
            ulong bb1p = bb1 & promoteMask;
            bb1 &= ~promoteMask;
            ulong bb2p = bb2 & promoteMask;
            bb2 &= ~promoteMask;
            ulong bb3p = bb3 & promoteMask;
            bb3 &= ~promoteMask;

            Piece capture;
            int from, to;
            for (; bb1 != 0; bb1 = BitOps.ResetLsb(bb1))
            {
                from = BitOps.TzCount(bb1);
                to = pawnLeft[(int)ctx.SideToMove, from];
                capture = board[to].Piece;
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.Capture, capture, score: CaptureScore(capture, Piece.Pawn)));
            }

            for (; bb2 != 0; bb2 = BitOps.ResetLsb(bb2))
            {
                from = BitOps.TzCount(bb2);
                to = pawnRight[(int)ctx.SideToMove, from];
                capture = board[to].Piece;
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.Capture, capture, score: CaptureScore(capture, Piece.Pawn)));
            }

            for (; bb3 != 0; bb3 = BitOps.ResetLsb(bb3))
            {
                from = BitOps.TzCount(bb3);
                to = pawnPlus[(int)ctx.SideToMove, from];
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PawnMove));
            }

            for (; bb4 != 0; bb4 = BitOps.ResetLsb(bb4))
            {
                from = BitOps.TzCount(bb4);
                to = pawnDouble[(int)ctx.SideToMove, from];
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.DblPawnMove));
            }

            // process promotions
            for (; bb1p != 0; bb1p = BitOps.ResetLsb(bb1p))
            {
                from = BitOps.TzCount(bb1p);
                to = pawnLeft[(int)ctx.SideToMove, from];
                capture = board[to].Piece;
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Queen,
                    CaptureScore(capture, Piece.Pawn, Piece.Queen)));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Rook,
                    CaptureScore(capture, Piece.Pawn)));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Bishop,
                    CaptureScore(capture, Piece.Pawn)));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Knight,
                    CaptureScore(capture, Piece.Pawn, Piece.Knight)));
            }

            for (; bb2p != 0; bb2p = BitOps.ResetLsb(bb2p))
            {
                from = BitOps.TzCount(bb2p);
                to = pawnRight[(int)ctx.SideToMove, from];
                capture = board[to].Piece;
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Queen,
                    CaptureScore(capture, Piece.Pawn, Piece.Queen)));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Rook,
                    CaptureScore(capture, Piece.Pawn)));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Bishop,
                    CaptureScore(capture, Piece.Pawn)));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.PromoteCapture, capture, Piece.Knight,
                    CaptureScore(capture, Piece.Pawn, Piece.Knight)));
            }

            for (; bb3p != 0; bb3p = BitOps.ResetLsb(bb3p))
            {
                from = BitOps.TzCount(bb3p);
                to = pawnPlus[(int)ctx.SideToMove, from];
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.Promote, promote: Piece.Queen,
                    score: Constants.PROMOTE_SCORE + Piece.Queen.Value()));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.Promote, promote: Piece.Rook,
                    score: Constants.PROMOTE_SCORE));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.Promote, promote: Piece.Bishop,
                    score: Constants.PROMOTE_SCORE));
                list.Add(Move.Pack(Piece.Pawn, from, to, MoveType.Promote, promote: Piece.Knight,
                    score: Constants.PROMOTE_SCORE + Piece.Knight.Value()));
            }
        }

        public void GenerateCastling(MoveList list, IMoveContext ctx)
        {
            if ((castling & ctx.KingSideMask) != 0 && (ctx.All & ctx.KingSideClearMask) == 0)
            {
                list.Add(Move.Pack(Piece.King, ctx.StartingKingSquare, ctx.KingSideTo, MoveType.Castle));
            }

            if ((castling & ctx.QueenSideMask) != 0 && (ctx.All & ctx.QueenSideClearMask) == 0)
            {
                list.Add(Move.Pack(Piece.King, ctx.StartingKingSquare, ctx.QueenSideTo, MoveType.Castle));
            }
        }

        public void GeneratePieceMoves(MoveList list, Piece piece, ulong bbPieces, IMoveContext ctx)
        {
            for (ulong bb1 = bbPieces; bb1 != 0; bb1 = BitOps.ResetLsb(bb1))
            {
                int from = BitOps.TzCount(bb1);
                ulong moves = GetPieceMoves(piece, from, ctx.All);

                for (ulong bb2 = moves & ctx.Enemies; bb2 != 0; bb2 = BitOps.ResetLsb(bb2))
                {
                    int to = BitOps.TzCount(bb2);
                    Piece capture = board[to].Piece;
                    list.Add(Move.Pack(piece, from, to, MoveType.Capture, capture, score: CaptureScore(capture, piece)));
                }

                for (ulong bb3 = moves & ~ctx.All; bb3 != 0; bb3 = BitOps.ResetLsb(bb3))
                {
                    int to = BitOps.TzCount(bb3);
                    list.Add(Move.Pack(piece, from, to));
                }
            }
        }

        public ulong GetPieceMoves(Piece piece, int from, ulong blockers)
        {
            return piece switch
            {
                Piece.Knight => knightMoves[from],
                Piece.Bishop => slidingMoves.GetBishopAttacks(from, blockers),
                Piece.Rook => slidingMoves.GetRookAttacks(from, blockers),
                Piece.Queen => slidingMoves.GetQueenAttacks(from, blockers),
                Piece.King => kingMoves[from],
                _ => 0
            };
        }

        #endregion

        #region Make/Unmake Moves

        public bool MakeMove(ulong move)
        {
            PushGameState(move);

            if (enPassantValidated != Index.NONE)
            {
                hash = ZobristHash.HashEnPassant(hash, enPassantValidated);
            }

            enPassant = Index.NONE;
            enPassantValidated = Index.NONE;
            hash = ZobristHash.HashCastling(hash, castling);
            Color opponent = sideToMove.Flip();

            Move.Unpack(move, out Piece piece, out int from, out int to, out MoveType type, out Piece capture, out Piece promote, out int _);
            
            switch (type)
            {
                case MoveType.Normal:
                    RemovePiece(sideToMove, piece, from);
                    AddPiece(sideToMove, piece, to);
                    castling &= (CastlingRights)(castleMask[from] & castleMask[to]);
                    halfMoveClock++;
                    break;

                case MoveType.Capture:
                    RemovePiece(opponent, capture, to);
                    RemovePiece(sideToMove, piece, from);
                    AddPiece(sideToMove, piece, to);
                    castling &= (CastlingRights)(castleMask[from] & castleMask[to]);
                    halfMoveClock = 0;
                    break;

                case MoveType.Castle:
                    CastlingRookMove rookMove = LookupRookMoves(to);
                    if (IsSquareAttackedByColor(from, opponent) ||
                        IsSquareAttackedByColor(rookMove.KingMoveThrough, opponent))
                    {
                        ref State state = ref gameStack.Pop();
                        state.Restore(this);
                        return false;
                    }

                    RemovePiece(sideToMove, Piece.King, from);
                    AddPiece(sideToMove, Piece.King, to);
                    RemovePiece(sideToMove, Piece.Rook, rookMove.RookFrom);
                    AddPiece(sideToMove, Piece.Rook, rookMove.RookTo);
                    castling &= (CastlingRights)(castleMask[from] & castleMask[to]);
                    halfMoveClock++;
                    break;

                case MoveType.EnPassant:
                    RemovePawn(opponent, to + EpOffset(sideToMove));
                    RemovePawn(sideToMove, from);
                    AddPawn(sideToMove, to);
                    halfMoveClock = 0;
                    break;

                case MoveType.PawnMove:
                    RemovePawn(sideToMove, from);
                    AddPawn(sideToMove, to);
                    halfMoveClock = 0;
                    break;

                case MoveType.DblPawnMove:
                    RemovePawn(sideToMove, from);
                    AddPawn(sideToMove, to);
                    enPassant = (sbyte)(to + EpOffset(sideToMove));
                    if (IsEnPassantValid(opponent))
                    {
                        enPassantValidated = enPassant;
                        hash = ZobristHash.HashEnPassant(hash, enPassantValidated);
                    }

                    halfMoveClock = 0;
                    break;

                case MoveType.Promote:
                    RemovePawn(sideToMove, from);
                    AddPiece(sideToMove, promote, to);
                    halfMoveClock = 0;
                    break;

                case MoveType.PromoteCapture:
                    RemovePiece(opponent, capture, to);
                    RemovePawn(sideToMove, from);
                    AddPiece(sideToMove, promote, to);
                    castling &= (CastlingRights)(castleMask[from] & castleMask[to]);
                    halfMoveClock = 0;
                    break;

                case MoveType.Null:
                    // do nothing
                    break;

                default:
                    Util.Fail($"Invalid move encountered in MakeMove: {Move.ToLongString(move)}.");
                    Util.TraceError($"Invalid move encountered in MakeMove: {Move.ToLongString(move)}.");
                    throw new ArgumentException("Invalid move passed to MakeMove.", nameof(move));
            }

            if (IsChecked(opponent))
            {
                sideToMove = opponent;
                UnmakeMove();
                return false;
            }

            fullMoveCounter += (short)sideToMove;
            hash = ZobristHash.HashCastling(hash, castling);
            hash = ZobristHash.HashActiveColor(hash, sideToMove);
            sideToMove = opponent;
            hash = ZobristHash.HashActiveColor(hash, sideToMove);
            return true;
        }

        public void UnmakeMove()
        {
            Color opponent = sideToMove;
            sideToMove = sideToMove.Flip();
            ref State state = ref gameStack.Pop();

            Move.Unpack(state.LastMove, out Piece piece, out int from, out int to, out MoveType type, out Piece capture,
                out Piece promote, out int _);

            switch (type)
            {
                case MoveType.Normal:
                case MoveType.PawnMove:
                case MoveType.DblPawnMove:
                    RemovePieceNoHash(sideToMove, piece, to);
                    AddPieceNoHash(sideToMove, piece, from);
                    break;

                case MoveType.Capture:
                    RemovePieceNoHash(sideToMove, piece, to);
                    AddPieceNoHash(sideToMove, piece, from);
                    AddPieceNoHash(opponent, capture, to);
                    break;

                case MoveType.Castle:
                    CastlingRookMove rookMove = LookupRookMoves(to);
                    RemovePieceNoHash(sideToMove, Piece.Rook, rookMove.RookTo);
                    AddPieceNoHash(sideToMove, Piece.Rook, rookMove.RookFrom);
                    RemovePieceNoHash(sideToMove, Piece.King, to);
                    AddPieceNoHash(sideToMove, Piece.King, from);
                    break;

                case MoveType.EnPassant:
                    RemovePieceNoHash(sideToMove, Piece.Pawn, to);
                    AddPieceNoHash(sideToMove, Piece.Pawn, from);
                    AddPieceNoHash(opponent, Piece.Pawn, to + EpOffset(sideToMove));
                    break;

                case MoveType.Promote:
                    RemovePieceNoHash(sideToMove, promote, to);
                    AddPieceNoHash(sideToMove, Piece.Pawn, from);
                    break;

                case MoveType.PromoteCapture:
                    RemovePieceNoHash(sideToMove, promote, to);
                    AddPieceNoHash(sideToMove, Piece.Pawn, from);
                    AddPieceNoHash(opponent, capture, to);
                    break;

                case MoveType.Null:
                    // do nothing
                    break;

                default:
                    Util.Fail($"Invalid move encountered in MakeMove: {Move.ToLongString(state.LastMove)}.");
                    Util.TraceError($"Invalid move encountered in MakeMove: {Move.ToLongString(state.LastMove)}.");
                    throw new InvalidOperationException("Invalid move encountered in UnmakeMove.");

            }

            state.Restore(this);
        }

        public void PushGameState(ulong move)
        {
            State state;
            state.Castling = castling;
            state.EnPassant = enPassant;
            state.EnPassantValidated = enPassantValidated;
            state.FullMoveCounter = fullMoveCounter;
            state.HalfMoveClock = halfMoveClock;
            state.Hash = hash;
            state.SideToMove = sideToMove;
            state.LastMove = move;
            gameStack.Push(ref state);
        }
        #endregion

        public void Clear()
        {
            Array.Clear(pieces);
            Array.Clear(units);
            Array.Clear(board);
            all = 0;
            sideToMove = Color.None;
            castling = CastlingRights.None;
            enPassant = Index.NONE;
            enPassantValidated = Index.NONE;
            halfMoveClock = 0;
            fullMoveCounter = 0;
            hash = 0;
            gameStack.Clear();
        }

        #region Positional Status

        public bool IsEnPassantValid(Color color)
        {
            return (enPassant != Index.NONE) && (PawnDefends(color, enPassant) & Pawns & UnitBitBoard(color)) != 0;
        }

        public bool IsSquareAttackedByColor(int square, Color color)
        {
            ulong units = UnitBitBoard(color);
            ulong pawns = Pawns & units;
            ulong knights = Knights & units;
            ulong kings = Kings & units;
            ulong dSliders = (Bishops | Queens) & units;
            ulong oSliders = (Rooks | Queens) & units;

            return (PawnDefends(color, square) & pawns) != 0 ||
                   (knightMoves[square] & knights) != 0 ||
                   (kingMoves[square] & kings) != 0 ||
                   (GetPieceMoves(Piece.Bishop, square, all) & dSliders) != 0 ||
                   (GetPieceMoves(Piece.Rook, square, all) & oSliders) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsChecked(Color byColor)
        {
            ulong kings = Kings & UnitBitBoard(byColor.Flip());
            int kingIndex = BitOps.TzCount(kings);
            return IsSquareAttackedByColor(kingIndex, byColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsChecked()
        {
            return IsChecked(sideToMove.Flip());
        }

        #endregion

        #region IClone Implementation

        public Position Clone()
        {
            return new Position(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Position state

        // the bitboards representing the current position
        private readonly ulong[] pieces = new ulong[Constants.MAX_PIECES];
        private readonly ulong[] units = new ulong[Constants.MAX_COLORS];
        private ulong all;

        // a simple, mailbox representation of position
        //
        // WARNING: care must be taken to make sure this structure
        // does not become out of sync with bitboards above
        private readonly Square[] board = new Square[Constants.MAX_SQUARES];

        // game state
        private Color sideToMove;
        private CastlingRights castling;
        private sbyte enPassant;
        private sbyte enPassantValidated;
        private short halfMoveClock;
        private short fullMoveCounter;
        private ulong hash;

        // runtime sliding move generation
        private readonly SlidingMoves slidingMoves = SlidingMovesFactory.Create();

        // objects that facility move generation
        private readonly IMoveContext[] contexts = { new WhiteMoveContext(), new BlackMoveContext() };

        // historical state of the game
        private readonly ValueStack<State> gameStack = new(Constants.MAX_GAME_LENGTH);

        #endregion

        #region Static data used by move generation logic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CastleMask(int square)
        {
            Util.Assert(Index.IsValid(square));
            return castleMask[square];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CastlingRookMove LookupRookMoves(int kingTo)
        {
            return kingTo switch
            {
                Index.C1 => wqRookMove,
                Index.G1 => wkRookMove,
                Index.C8 => bqRookMove,
                Index.G8 => bkRookMove,
                _ => throw new ArgumentException("Invalid king target/to square invalid.", nameof(kingTo)),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CaptureScore(Piece captured, Piece attacker, Piece promote = Piece.None)
        {
            return Constants.CAPTURE_SCORE + promote.Value() + ((int)captured << 3) + (Constants.MAX_PIECES - (int)attacker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PawnDefends(Color color, int defendedSq)
        {
            Util.Assert(Index.IsValid(defendedSq));
            return pawnDefends[(int)color, defendedSq];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PawnCaptures(Color color, int captureSq)
        {
            Util.Assert(Index.IsValid(captureSq));
            return pawnCaptures[(int)color, captureSq];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MaskFile(int file)
        {
            Util.Assert(Coord.IsValid(file));
            return 0x0101010101010101ul << file;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MaskRank(int rank)
        {
            Util.Assert(Coord.IsValid(rank));
            return 0x00000000000000fful << (rank << 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EpOffset(Color color)
        {
            return 8 * (-1 + ((int)color << 1));
        }

        private static readonly UnsafeArray<int> castleMask = new(Constants.MAX_SQUARES)
        {
            13, 15, 15, 15, 12, 15, 15, 14,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
             7, 15, 15, 15,  3, 15, 15, 11
        };

        private static readonly CastlingRookMove wqRookMove = new(Index.E1, Index.C1, Index.D1, Index.A1, Index.D1,
            CastlingRights.WhiteQueenSide);

        private static readonly CastlingRookMove wkRookMove = new(Index.E1, Index.G1, Index.F1, Index.H1, Index.F1,
            CastlingRights.WhiteKingSide);

        private static readonly CastlingRookMove bqRookMove = new(Index.E8, Index.C8, Index.D8, Index.A8, Index.D8,
            CastlingRights.BlackQueenSide);

        private static readonly CastlingRookMove bkRookMove = new(Index.E8, Index.G8, Index.F8, Index.H8, Index.F8,
            CastlingRights.BlackKingSide);

        private static readonly UnsafeArray2D<ulong> pawnDefends = new(Constants.MAX_COLORS, Constants.MAX_SQUARES)
        {
            #region pawnDefends data

            // squares defended by white pawns
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000002ul, 0x0000000000000005ul, 0x000000000000000Aul, 0x0000000000000014ul,
            0x0000000000000028ul, 0x0000000000000050ul, 0x00000000000000A0ul, 0x0000000000000040ul,
            0x0000000000000200ul, 0x0000000000000500ul, 0x0000000000000A00ul, 0x0000000000001400ul,
            0x0000000000002800ul, 0x0000000000005000ul, 0x000000000000A000ul, 0x0000000000004000ul,
            0x0000000000020000ul, 0x0000000000050000ul, 0x00000000000A0000ul, 0x0000000000140000ul,
            0x0000000000280000ul, 0x0000000000500000ul, 0x0000000000A00000ul, 0x0000000000400000ul,
            0x0000000002000000ul, 0x0000000005000000ul, 0x000000000A000000ul, 0x0000000014000000ul,
            0x0000000028000000ul, 0x0000000050000000ul, 0x00000000A0000000ul, 0x0000000040000000ul,
            0x0000000200000000ul, 0x0000000500000000ul, 0x0000000A00000000ul, 0x0000001400000000ul,
            0x0000002800000000ul, 0x0000005000000000ul, 0x000000A000000000ul, 0x0000004000000000ul,
            0x0000020000000000ul, 0x0000050000000000ul, 0x00000A0000000000ul, 0x0000140000000000ul,
            0x0000280000000000ul, 0x0000500000000000ul, 0x0000A00000000000ul, 0x0000400000000000ul,
            0x0002000000000000ul, 0x0005000000000000ul, 0x000A000000000000ul, 0x0014000000000000ul,
            0x0028000000000000ul, 0x0050000000000000ul, 0x00A0000000000000ul, 0x0040000000000000ul,

            // squares defended by black pawns
            0x0000000000000200ul, 0x0000000000000500ul, 0x0000000000000A00ul, 0x0000000000001400ul,
            0x0000000000002800ul, 0x0000000000005000ul, 0x000000000000A000ul, 0x0000000000004000ul,
            0x0000000000020000ul, 0x0000000000050000ul, 0x00000000000A0000ul, 0x0000000000140000ul,
            0x0000000000280000ul, 0x0000000000500000ul, 0x0000000000A00000ul, 0x0000000000400000ul,
            0x0000000002000000ul, 0x0000000005000000ul, 0x000000000A000000ul, 0x0000000014000000ul,
            0x0000000028000000ul, 0x0000000050000000ul, 0x00000000A0000000ul, 0x0000000040000000ul,
            0x0000000200000000ul, 0x0000000500000000ul, 0x0000000A00000000ul, 0x0000001400000000ul,
            0x0000002800000000ul, 0x0000005000000000ul, 0x000000A000000000ul, 0x0000004000000000ul,
            0x0000020000000000ul, 0x0000050000000000ul, 0x00000A0000000000ul, 0x0000140000000000ul,
            0x0000280000000000ul, 0x0000500000000000ul, 0x0000A00000000000ul, 0x0000400000000000ul,
            0x0002000000000000ul, 0x0005000000000000ul, 0x000A000000000000ul, 0x0014000000000000ul,
            0x0028000000000000ul, 0x0050000000000000ul, 0x00A0000000000000ul, 0x0040000000000000ul,
            0x0200000000000000ul, 0x0500000000000000ul, 0x0A00000000000000ul, 0x1400000000000000ul,
            0x2800000000000000ul, 0x5000000000000000ul, 0xA000000000000000ul, 0x4000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul

            #endregion
        };
        private static readonly UnsafeArray2D<ulong> pawnCaptures = new(Constants.MAX_COLORS, Constants.MAX_SQUARES)
        {
            #region pawnCaptures data

            // squares attacked by white pawns
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000020000ul, 0x0000000000050000ul, 0x00000000000A0000ul, 0x0000000000140000ul,
            0x0000000000280000ul, 0x0000000000500000ul, 0x0000000000A00000ul, 0x0000000000400000ul,
            0x0000000002000000ul, 0x0000000005000000ul, 0x000000000A000000ul, 0x0000000014000000ul,
            0x0000000028000000ul, 0x0000000050000000ul, 0x00000000A0000000ul, 0x0000000040000000ul,
            0x0000000200000000ul, 0x0000000500000000ul, 0x0000000A00000000ul, 0x0000001400000000ul,
            0x0000002800000000ul, 0x0000005000000000ul, 0x000000A000000000ul, 0x0000004000000000ul,
            0x0000020000000000ul, 0x0000050000000000ul, 0x00000A0000000000ul, 0x0000140000000000ul,
            0x0000280000000000ul, 0x0000500000000000ul, 0x0000A00000000000ul, 0x0000400000000000ul,
            0x0002000000000000ul, 0x0005000000000000ul, 0x000A000000000000ul, 0x0014000000000000ul,
            0x0028000000000000ul, 0x0050000000000000ul, 0x00A0000000000000ul, 0x0040000000000000ul,
            0x0200000000000000ul, 0x0500000000000000ul, 0x0A00000000000000ul, 0x1400000000000000ul,
            0x2800000000000000ul, 0x5000000000000000ul, 0xA000000000000000ul, 0x4000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,

            // squares attacked by black pawns
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000002ul, 0x0000000000000005ul, 0x000000000000000Aul, 0x0000000000000014ul,
            0x0000000000000028ul, 0x0000000000000050ul, 0x00000000000000A0ul, 0x0000000000000040ul,
            0x0000000000000200ul, 0x0000000000000500ul, 0x0000000000000A00ul, 0x0000000000001400ul,
            0x0000000000002800ul, 0x0000000000005000ul, 0x000000000000A000ul, 0x0000000000004000ul,
            0x0000000000020000ul, 0x0000000000050000ul, 0x00000000000A0000ul, 0x0000000000140000ul,
            0x0000000000280000ul, 0x0000000000500000ul, 0x0000000000A00000ul, 0x0000000000400000ul,
            0x0000000002000000ul, 0x0000000005000000ul, 0x000000000A000000ul, 0x0000000014000000ul,
            0x0000000028000000ul, 0x0000000050000000ul, 0x00000000A0000000ul, 0x0000000040000000ul,
            0x0000000200000000ul, 0x0000000500000000ul, 0x0000000A00000000ul, 0x0000001400000000ul,
            0x0000002800000000ul, 0x0000005000000000ul, 0x000000A000000000ul, 0x0000004000000000ul,
            0x0000020000000000ul, 0x0000050000000000ul, 0x00000A0000000000ul, 0x0000140000000000ul,
            0x0000280000000000ul, 0x0000500000000000ul, 0x0000A00000000000ul, 0x0000400000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul,
            0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul, 0x0000000000000000ul

            #endregion pawnCaptures data
        };
        private static readonly UnsafeArray<ulong> knightMoves = new(Constants.MAX_SQUARES)
        {
            #region knightMoves data

            0x0000000000020400ul, 0x0000000000050800ul, 0x00000000000A1100ul, 0x0000000000142200ul,
            0x0000000000284400ul, 0x0000000000508800ul, 0x0000000000A01000ul, 0x0000000000402000ul,
            0x0000000002040004ul, 0x0000000005080008ul, 0x000000000A110011ul, 0x0000000014220022ul,
            0x0000000028440044ul, 0x0000000050880088ul, 0x00000000A0100010ul, 0x0000000040200020ul,
            0x0000000204000402ul, 0x0000000508000805ul, 0x0000000A1100110Aul, 0x0000001422002214ul,
            0x0000002844004428ul, 0x0000005088008850ul, 0x000000A0100010A0ul, 0x0000004020002040ul,
            0x0000020400040200ul, 0x0000050800080500ul, 0x00000A1100110A00ul, 0x0000142200221400ul,
            0x0000284400442800ul, 0x0000508800885000ul, 0x0000A0100010A000ul, 0x0000402000204000ul,
            0x0002040004020000ul, 0x0005080008050000ul, 0x000A1100110A0000ul, 0x0014220022140000ul,
            0x0028440044280000ul, 0x0050880088500000ul, 0x00A0100010A00000ul, 0x0040200020400000ul,
            0x0204000402000000ul, 0x0508000805000000ul, 0x0A1100110A000000ul, 0x1422002214000000ul,
            0x2844004428000000ul, 0x5088008850000000ul, 0xA0100010A0000000ul, 0x4020002040000000ul,
            0x0400040200000000ul, 0x0800080500000000ul, 0x1100110A00000000ul, 0x2200221400000000ul,
            0x4400442800000000ul, 0x8800885000000000ul, 0x100010A000000000ul, 0x2000204000000000ul,
            0x0004020000000000ul, 0x0008050000000000ul, 0x00110A0000000000ul, 0x0022140000000000ul,
            0x0044280000000000ul, 0x0088500000000000ul, 0x0010A00000000000ul, 0x0020400000000000ul

            #endregion
        };

        private static readonly UnsafeArray<ulong> kingMoves = new(Constants.MAX_SQUARES)
        {
            #region kingMoves data

            0x0000000000000302ul, 0x0000000000000705ul, 0x0000000000000E0Aul, 0x0000000000001C14ul,
            0x0000000000003828ul, 0x0000000000007050ul, 0x000000000000E0A0ul, 0x000000000000C040ul,
            0x0000000000030203ul, 0x0000000000070507ul, 0x00000000000E0A0Eul, 0x00000000001C141Cul,
            0x0000000000382838ul, 0x0000000000705070ul, 0x0000000000E0A0E0ul, 0x0000000000C040C0ul,
            0x0000000003020300ul, 0x0000000007050700ul, 0x000000000E0A0E00ul, 0x000000001C141C00ul,
            0x0000000038283800ul, 0x0000000070507000ul, 0x00000000E0A0E000ul, 0x00000000C040C000ul,
            0x0000000302030000ul, 0x0000000705070000ul, 0x0000000E0A0E0000ul, 0x0000001C141C0000ul,
            0x0000003828380000ul, 0x0000007050700000ul, 0x000000E0A0E00000ul, 0x000000C040C00000ul,
            0x0000030203000000ul, 0x0000070507000000ul, 0x00000E0A0E000000ul, 0x00001C141C000000ul,
            0x0000382838000000ul, 0x0000705070000000ul, 0x0000E0A0E0000000ul, 0x0000C040C0000000ul,
            0x0003020300000000ul, 0x0007050700000000ul, 0x000E0A0E00000000ul, 0x001C141C00000000ul,
            0x0038283800000000ul, 0x0070507000000000ul, 0x00E0A0E000000000ul, 0x00C040C000000000ul,
            0x0302030000000000ul, 0x0705070000000000ul, 0x0E0A0E0000000000ul, 0x1C141C0000000000ul,
            0x3828380000000000ul, 0x7050700000000000ul, 0xE0A0E00000000000ul, 0xC040C00000000000ul,
            0x0203000000000000ul, 0x0507000000000000ul, 0x0A0E000000000000ul, 0x141C000000000000ul,
            0x2838000000000000ul, 0x5070000000000000ul, 0xA0E0000000000000ul, 0x40C0000000000000ul

            #endregion
        };

        private static readonly UnsafeArray2D<sbyte> pawnLeft = new(Constants.MAX_COLORS, Constants.MAX_SQUARES)
        {
            #region pawnLeft data
            -1,  8,  9, 10, 11, 12, 13, 14,
            -1, 16, 17, 18, 19, 20, 21, 22,
            -1, 24, 25, 26, 27, 28, 29, 30,
            -1, 32, 33, 34, 35, 36, 37, 38,
            -1, 40, 41, 42, 43, 44, 45, 46,
            -1, 48, 49, 50, 51, 52, 53, 54,
            -1, 56, 57, 58, 59, 60, 61, 62,
            -1, -1, -1, -1, -1, -1, -1, -1,
            
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1,  0,  1,  2,  3,  4,  5,  6,
            -1,  8,  9, 10, 11, 12, 13, 14,
            -1, 16, 17, 18, 19, 20, 21, 22,
            -1, 24, 25, 26, 27, 28, 29, 30,
            -1, 32, 33, 34, 35, 36, 37, 38,
            -1, 40, 41, 42, 43, 44, 45, 46,
            -1, 48, 49, 50, 51, 52, 53, 54
            #endregion
        };

        private static readonly UnsafeArray2D<sbyte> pawnRight = new(Constants.MAX_COLORS, Constants.MAX_SQUARES)
        {
            #region pawnRight data
             9, 10, 11, 12, 13, 14, 15, -1,
            17, 18, 19, 20, 21, 22, 23, -1,
            25, 26, 27, 28, 29, 30, 31, -1,
            33, 34, 35, 36, 37, 38, 39, -1,
            41, 42, 43, 44, 45, 46, 47, -1,
            49, 50, 51, 52, 53, 54, 55, -1,
            57, 58, 59, 60, 61, 62, 63, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            
            -1, -1, -1, -1, -1, -1, -1, -1,
             1,  2,  3,  4,  5,  6,  7, -1,
             9, 10, 11, 12, 13, 14, 15, -1,
            17, 18, 19, 20, 21, 22, 23, -1,
            25, 26, 27, 28, 29, 30, 31, -1,
            33, 34, 35, 36, 37, 38, 39, -1,
            41, 42, 43, 44, 45, 46, 47, -1,
            49, 50, 51, 52, 53, 54, 55, -1
            #endregion
        };

        private static readonly UnsafeArray2D<sbyte> pawnPlus = new(Constants.MAX_COLORS, Constants.MAX_SQUARES)
        {
            #region pawnPlus data
             8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55,
            56, 57, 58, 59, 60, 61, 62, 63,
            -1, -1, -1, -1, -1, -1, -1, -1,
            
            -1, -1, -1, -1, -1, -1, -1, -1,
             0,  1,  2,  3,  4,  5,  6,  7,
             8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55
            #endregion
        };

        private static readonly UnsafeArray2D<sbyte> pawnDouble = new(Constants.MAX_COLORS, Constants.MAX_SQUARES)
        {
            #region pawnDouble data
            16, 17, 18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55,
            56, 57, 58, 59, 60, 61, 62, 63,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
             0,  1,  2,  3,  4,  5,  6,  7,
             8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45, 46, 47
            #endregion
        };

        #endregion
    }
}