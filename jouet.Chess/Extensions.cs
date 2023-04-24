using System.Runtime.CompilerServices;
using System.Text;

namespace jouet.Chess
{
    public static class Extensions
    {
        public static string ToFenString(this CastlingRights castling)
        {
            StringBuilder sb = new();
            if ((castling & CastlingRights.WhiteKingSide) != CastlingRights.None)
            {
                sb.Append('K');
            }

            if ((castling & CastlingRights.WhiteQueenSide) != CastlingRights.None)
            {
                sb.Append('Q');
            }

            if ((castling & CastlingRights.BlackKingSide) != CastlingRights.None)
            {
                sb.Append('k');
            }

            if ((castling & CastlingRights.BlackQueenSide) != CastlingRights.None)
            {
                sb.Append('q');
            }

            return sb.ToString();
        }

        public static string ToFenString(this Color color)
        {
            return color switch
            {
                Color.White => "w",
                Color.Black => "b",
                _ => string.Empty
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Flip(this Color color)
        {
            return (Color)((int)color ^ 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Value(this Piece piece)
        {
            return pieceValues[(int)piece + 1];
        }

        public static string ToSanPiece(this Piece piece)
        {
            return piece switch
            {
                Piece.Pawn => "P",
                Piece.Knight => "N",
                Piece.Bishop => "B",
                Piece.Rook => "R",
                Piece.Queen => "Q",
                Piece.King => "K",
                Piece.None => "",
                _ => throw new InvalidOperationException("Invalid piece encountered.")
            };
        }

        private static readonly int[] pieceValues = { 0, 100, 300, 300, 500, 900, 9900 };
    }
}
