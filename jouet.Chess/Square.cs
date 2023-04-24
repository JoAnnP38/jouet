using System.Runtime.CompilerServices;

namespace jouet.Chess
{
    public readonly struct Square : IEquatable<Square>, IComparable<Square>
    {
        // BIT 0    : if one indicates the square contains a piece
        // BIT 1    : the piece color 
        // BIT 2-4  : the piece 
        private readonly byte _contents;

        public Square()
        {
            _contents = 0;
        }

        public Square(Color color, Piece piece)
        {
            if (color == Color.None || piece == Piece.None)
            {
                _contents = 0;
            }
            else
            {
                _contents = (byte)(1 | (sbyte)color << 1 | (sbyte)piece << 2);
            }
        }

        public bool IsEmpty => _contents == 0;
        public Color Color => IsEmpty ? Color.None : (Color)((_contents >> 1) & 0x01);
        public Piece Piece => IsEmpty ? Piece.None : (Piece)(_contents >> 2);
        public byte Contents => _contents;

        public string ToFenString()
        {
            return Piece switch
            {
                Chess.Piece.Pawn => Color == Chess.Color.White ? "P" : "p",
                Chess.Piece.Knight => Color == Chess.Color.White ? "N" : "n",
                Chess.Piece.Bishop => Color == Chess.Color.White ? "B" : "b",
                Chess.Piece.Rook => Color == Chess.Color.White ? "R" : "r",
                Chess.Piece.Queen => Color == Chess.Color.White ? "Q" : "q",
                Chess.Piece.King => Color == Chess.Color.White ? "K" : "k",
                _ => string.Empty
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Square other)
        {
            return _contents == other._contents;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Square other)
        {
            return _contents - other._contents;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Square square)
            {
                return Equals(square);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _contents.GetHashCode();
        }

        public static bool operator ==(Square sq1, Square sq2) => sq1.Equals(sq2);
        public static bool operator !=(Square sq1, Square sq2) => !sq1.Equals(sq2);

        public static explicit operator Color(Square sq) => sq.Color;
        public static explicit operator Piece(Square sq) => sq.Piece;

        public static Square Create(Color color, Piece piece)
        {
            int c = (int)color + 1;
            int p = (int)piece + 1;
            return lookup[p * 3 + c];

        }

        public static Square Empty = new();
        public static Square WhitePawn = new(Color.White, Piece.Pawn);
        public static Square WhiteKnight = new(Color.White, Piece.Knight);
        public static Square WhiteBishop = new(Color.White, Piece.Bishop);
        public static Square WhiteRook = new(Color.White, Piece.Rook);
        public static Square WhiteQueen = new(Color.White, Piece.Queen);
        public static Square WhiteKing = new(Color.White, Piece.King);
        public static Square BlackPawn = new(Color.Black, Piece.Pawn);
        public static Square BlackKnight = new(Color.Black, Piece.Knight);
        public static Square BlackBishop = new(Color.Black, Piece.Bishop);
        public static Square BlackRook = new(Color.Black, Piece.Rook);
        public static Square BlackQueen = new(Color.Black, Piece.Queen);
        public static Square BlackKing = new(Color.Black, Piece.King);

        public static Square[] lookup =
        {
            Empty,
            Empty,
            Empty,
            Empty,
            WhitePawn,
            BlackPawn,
            Empty,
            WhiteKnight,
            BlackKnight,
            Empty,
            WhiteBishop,
            BlackBishop,
            Empty,
            WhiteRook,
            BlackRook,
            Empty,
            WhiteQueen,
            BlackQueen,
            Empty,
            WhiteKing,
            BlackKing
        };
    }

    public class SquareEqualityComparer : IEqualityComparer<Square>
    {
        public bool Equals(Square x, Square y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Square obj)
        {
            return obj.Contents.GetHashCode();
        }
    }
}
