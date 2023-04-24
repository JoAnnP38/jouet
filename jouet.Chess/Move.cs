using jouet.Utilities;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace jouet.Chess
{
    public static class Move
    {
        public static readonly ulong NullMove = Pack(Piece.None, 0, 0, MoveType.Null);
        public static ulong Pack(Piece piece, int from, int to, MoveType type = MoveType.Normal, Piece capture = Piece.None, 
            Piece promote = Piece.None, int score = 0)
        {
            Util.Assert(Index.IsValid(from));
            Util.Assert(Index.IsValid(to));
            Util.Assert(score is >= 0 and <= short.MaxValue);

            ulong move = ((ulong)piece & 0x0f) |
                         (((ulong)from & 0x3f) << 4) |
                         (((ulong)to & 0x03f) << 10) |
                         (((ulong)type & 0x0f) << 16) |
                         (((ulong)capture & 0x0f) << 20) |
                         (((ulong)promote & 0x0f) << 24) |
                         (((ulong)score & 0x0ffff) << 28);

            return move;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExcludeScore(ulong move)
        {
            return move & 0x0ffffffful;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SetScore(ulong move, short score)
        {
            return BitOps.BitFieldSet(move, score, 28, 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece GetPiece(ulong move)
        {
            int piece = (int)move & 0x0f;
            return piece == 0x0f ? Piece.None : (Piece)piece;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFrom(ulong move)
        {
            return BitOps.BitFieldExtract(move, 4, 6);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetTo(ulong move)
        {
            return BitOps.BitFieldExtract(move, 10, 6);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MoveType GetMoveType(ulong move)
        {
            return (MoveType)BitOps.BitFieldExtract(move, 16, 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece GetCapture(ulong move)
        {
            int piece = BitOps.BitFieldExtract(move, 20, 4);
            return piece == 0x0f ? Piece.None : (Piece)piece;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece GetPromote(ulong move)
        {
            int piece = BitOps.BitFieldExtract(move, 24, 4);
            return piece == 0x0f ? Piece.None : (Piece)piece;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetScore(ulong move)
        {
            return BitOps.BitFieldExtract(move, 28, 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCapture(ulong move)
        {
            return GetCapture(move) != Piece.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPromote(ulong move)
        {
            return GetPromote(move) != Piece.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPawnMove(ulong move)
        {
            return GetPiece(move) == Piece.Pawn;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQuiet(ulong move)
        {
            return !IsCapture(move) && !IsPromote(move);
        }

        public static void Unpack(ulong move, out Piece piece, out int from, out int to, out MoveType type, 
            out Piece capture, out Piece promote, out int score)
        {
            piece = GetPiece(move);
            from = GetFrom(move);
            to = GetTo(move);
            type = GetMoveType(move);
            capture = GetCapture(move);
            promote = GetPromote(move);
            score = GetScore(move);
        }

        /*
        public static bool TryParseMove(Position board, string s, out ulong move)
        {
            move = 0;
            if (s.Length < 4)
            {
                throw new ArgumentException(@"Parameter to short to represent a valid move.", nameof(s));
            }

            if (!Index.TryParse(s[..2], out int from))
            {
                throw new ArgumentException(@"Invalid from square in move.", nameof(s));
            }

            if (!Index.TryParse(s[2..4], out int to))
            {
                throw new ArgumentException(@"Invalid to square in move.", nameof(s));
            }

            Piece promote = s.Length > 4 ? Conversion.ParsePiece(s[4]) : Piece.None;

            MoveList moveList = new();
            board.GenerateMoves(moveList);

            for (int n = 0; n < moveList.Count; ++n)
            {
                ulong mv = moveList[n];
                string mvString = Move.ToString(mv);
                if (from == GetFrom(mv) && to == GetTo(mv) && promote == GetPromote(mv))
                {
                    bool legal = board.MakeMove(mv);
                    if (legal)
                    {
                        board.UnmakeMove();
                        move = mv;
                        return true;
                    }
                }
            }

            return false;
        }
        
        public static string ToString(ulong move)
        {
            int from = GetFrom(move);
            int to = GetTo(move);
            Piece promote = GetPromote(move);
            return $"{Index.ToString(from)}{Index.ToString(to)}{Conversion.PieceToString(promote)}";
        }
        */

        public static string ToString(ulong move)
        {
            StringBuilder sb = new();
            int from = Move.GetFrom(move);
            int to = Move.GetTo(move);
            MoveType type = GetMoveType(move);
            if (type == MoveType.Castle)
            {
                int file = Chess.Index.GetFile(to);
                return file == Coord.FileC ? "O-O-O" : "O-O";
            }
            Piece promote = GetPromote(move);
            sb.Append(Index.ToString(from));
            if (IsCapture(move))
            {
                sb.Append('x');
            }

            sb.Append(Index.ToString(to));
            sb.Append(promote.ToSanPiece());
            return sb.ToString();
        }

        public static string ToLongString(ulong move)
        {
            Unpack(move, out Piece piece, out int from, out int to, out MoveType type, out Piece capture, out Piece promote,
                out int score);

            return
                $"(Piece = {piece}, From = {Index.ToString(from)}, To = {Index.ToString(to)}, Type = {type}, Capture = {capture}, Promote = {promote}, Score = {score})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(ulong move1, ulong move2)
        {
            return (int)ExcludeScore(move1) - (int)ExcludeScore(move2);
        }

        /*
        public static string ToSanString(ulong move, Position board)
        {
            if (board.IsLegalMove(move))
            {
                throw new InvalidOperationException("Invalid move.");
            }

            StringBuilder sb = new();
            Move.UnpackMove(move, out int from, out int to, out MoveType type, out Piece capture, out Piece promote, out int _);
            if (type == MoveType.Castle)
            {
                if (Index.GetFile(to) == 2)
                {
                    sb.Append("O-O-O");
                }
                else
                {
                    sb.Append("O-O");
                }
            }
            else
            {
                Square sq = board.PieceBoard[from];
                if (sq.Piece != Piece.Pawn)
                {
                    sb.Append(Conversion.PieceToString(board.PieceBoard[from].Piece).ToUpper());
                }

                MoveList moveList = new();
                board.GenerateLegalMoves(moveList);

                var ambiguous = moveList.Where(m =>
                    Move.GetTo(m) == to && board.PieceBoard[Move.GetFrom(m)].Piece == sq.Piece &&
                    Move.Compare(move, m) == 0).ToArray();

                if (ambiguous.Length > 0)
                {
                    if (ambiguous.Any(m => Index.GetFile(Move.GetFrom(m)) == Index.GetFile(from)))
                    {
                        sb.Append(ambiguous.Any(m => Index.GetRank(Move.GetFrom(m)) == Index.GetRank(from))
                            ? Index.ToString(from)
                            : Coord.ToRank(Index.GetRank(from)));
                    }
                    else
                    {
                        sb.Append(Coord.ToFile(Index.GetFile(from)));
                    }
                }

                if (IsCapture(move))
                {
                    sb.Append('x');
                }

                sb.Append(Index.ToString(to));

                if (IsPromote(move))
                {
                    sb.Append('=');
                    sb.Append(Conversion.PieceToString(promote).ToUpper());
                }

                board.MakeMove(move);
                if (board.IsChecked())
                {
                    if (board.NoLegalMoves())
                    {
                        sb.Append('#');
                    }
                    else
                    {
                        sb.Append('+');
                    }
                }

                board.UnmakeMove();
            }

            return sb.ToString();
        }
        */
    }
}
