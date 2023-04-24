using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jouet.Utilities;

namespace jouet.Chess
{
    public sealed class Perft
    {
        public Perft(string fen)
        {
            position = new Position(fen);
        }

        public Perft() : this(Notation.FEN_START_POS)
        {}

        public IEnumerable<(string move, string fen, ulong nodes)> Divide(int depth)
        {
            List<(string move, string fen, ulong nodes)> results = new();

            MoveList list = moveListPool.Get();
            position.GenerateMoves(list);
            for (int n = 0; n < list.Count; n++)
            {
                ulong move = list[n];
                if (!position.MakeMove(move))
                {
                    continue;
                }

                string strMove = Move.ToString(move);
                string fen = position.ToString("F");
                ulong nodes = depth == 1 ? 1 : Expand(depth - 1);
                results.Add((strMove, fen, nodes));
                position.UnmakeMove();
            }

            moveListPool.Return(list);
            return results;
        }

        public ulong Expand(int depth)
        {
            ulong nodes = 0;
            MoveList list = moveListPool.Get();
            position.GenerateMoves(list);
            for (int n = 0; n < list.Count; n++)
            {
                ulong move = list[n];
                if (!position.MakeMove(move))
                {
                    continue;
                }
                nodes += depth == 1 ? 1 : Expand(depth - 1);
                position.UnmakeMove();
            }

            moveListPool.Return(list);
            return nodes;
        }

        private readonly Position position;
        private readonly ObjectPool<MoveList> moveListPool = new(10, 10);
    }
}
