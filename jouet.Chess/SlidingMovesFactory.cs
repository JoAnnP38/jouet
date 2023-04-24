using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jouet.Chess
{
    public static class SlidingMovesFactory
    {
        public static SlidingMoves Create()
        {
            if (PextMoves.IsSupported)
            {
                return new PextMoves();
            }

            if (FancyMagicMoves.IsSupported)
            {
                return new FancyMagicMoves();
            }

            return new ClassicMoves();
        }
    }
}
