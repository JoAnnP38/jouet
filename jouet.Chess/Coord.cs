using System.Runtime.CompilerServices;
using jouet.Utilities;

namespace jouet.Chess
{
    public static class Coord
    {
        public const int MAX_VALUE = 7;
        public const int MIN_VALUE = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(int value)
        {
            return value is >= MIN_VALUE and <= MAX_VALUE;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFile(int value)
        {
            Util.Assert(IsValid(value));
            return new string((char)('a' + value), 1);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToRank(int value)
        {
            Util.Assert(IsValid(value));
            return new string((char)('1' + value), 1);
        }

        #region Named Coords
        public const int FileA = 0;
        public const int FileB = 1;
        public const int FileC = 2;
        public const int FileD = 3;
        public const int FileE = 4;
        public const int FileF = 5;
        public const int FileG = 6;
        public const int FileH = 7;

        public const int Rank1 = 0;
        public const int Rank2 = 1;
        public const int Rank3 = 2;
        public const int Rank4 = 3;
        public const int Rank5 = 4;
        public const int Rank6 = 5;
        public const int Rank7 = 6;
        public const int Rank8 = 7;
        #endregion
    }
}
