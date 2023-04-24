using System.Text;
using jouet.Utilities;
using Index = jouet.Chess.Index;

namespace jouet.Chess
{
    public static class BitBoard
    {
        public static string ToString(ulong bb)
        {
            StringBuilder sb = new();
            for (int rank = Coord.MAX_VALUE; rank >= 0; rank--)
            {
                for (int file = 0; file <= Coord.MAX_VALUE; file++)
                {
                    int sq = Index.ToIndex(file, rank);
                    byte bit = (byte)BitOps.GetBit(bb, sq);
                    sb.Append(bit);
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
