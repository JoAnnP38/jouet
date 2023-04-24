using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jouet.Chess
{
    public static class Constants
    {
        public const int MAX_SQUARES = 64;
        public const int MAX_PIECES = 6;
        public const int MAX_COLORS = 2;
        public const int MAX_COORDS = 8;
        public const int AVG_MOVES_PER_PLY = 36;
        public const short PV_SCORE = 20000;
        public const short BAD_CAPTURE = 11400;
        public const short CAPTURE_SCORE = 10000;
        public const short PROMOTE_SCORE = 9000;
        public const short KILLER_SCORE = 7000;
        public const short HISTORY_SCORE = 5000;
        public const short MAX_GAME_LENGTH = 640;
    }
}
