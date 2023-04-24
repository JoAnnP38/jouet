using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using Index = jouet.Chess.Index;

namespace jouet.Chess
{
    public static partial class Notation
    {
        public const string REGEX_FEN = @"^\s*([rnbqkpRNBQKP1-8]+/){7}[rnbqkpRNBQKP1-8]+\s[bw]\s(-|K?Q?k?q?)\s(-|[a-h][36])\s\d+\s\d+\s*$";
        public const string REGEX_MOVE = @"^[a-h][1-8][a-h][1-8](n|b|r|q)?$";
        public const string REGEX_INDEX = @"^-|[a-h][1-8]$";
        public const string FEN_EMPTY = @"8/8/8/8/8/8/8/8 w - - 0 0";
        public const string FEN_START_POS = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidFen(string fen)
        {
            return fenRegex.IsMatch(fen);
        }

        public static (Color color, Piece piece) ParseFenPiece(char ch)
        {
            Color color = char.IsLower(ch) ? Color.Black : Color.White;
            Piece piece = char.ToUpper(ch) switch
            {
                'P' => Piece.Pawn,
                'N' => Piece.Knight,
                'B' => Piece.Bishop,
                'R' => Piece.Rook,
                'Q' => Piece.Queen,
                'K' => Piece.King,
                _ => throw new ArgumentException($"Character '{ch}' is not a valid FEN piece.")
            };
            return (color, piece);
        }

        public static Color ParseFenColorToMove(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s switch
            {
                "w" => Color.White,
                "b" => Color.Black,
                _ => throw new ArgumentException($"'{s}' is not a valid FEN color.")
            };
        }

        public static CastlingRights ParseFenCastlingRights(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            CastlingRights castling = CastlingRights.None;
            for (int n = 0; n < s.Length; n++)
            {
                CastlingRights cr = s[n] switch
                {
                    'K' => CastlingRights.WhiteKingSide,
                    'Q' => CastlingRights.WhiteQueenSide,
                    'k' => CastlingRights.BlackKingSide,
                    'q' => CastlingRights.BlackQueenSide,
                    '-' => CastlingRights.None,
                    _ => throw new ArgumentException($"Illegal character found in FEN castling availability '{s[n]}'.")
                };
                castling |= cr;
            }

            return castling;
        }

        public static int ParseFenEnPassant(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (!indexRegex.IsMatch(s))
            {
                throw new ArgumentException($"FEN en passant specification is unrecognized '{s}'.");
            }

            if (s == "-")
            {
                return Index.NONE;
            }

            return Index.Parse(s);
        }

        #pragma warning disable SYSLIB1045
        private static readonly Regex fenRegex = new(REGEX_FEN, RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex indexRegex = new(REGEX_INDEX, RegexOptions.Compiled | RegexOptions.Singleline);
        #pragma warning restore SYSLIB1045
    }
}
