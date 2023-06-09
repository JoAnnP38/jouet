﻿using jouet.Collections;
using jouet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace jouet.Chess
{
    public sealed class MoveList : ValueList<ulong>, IPooledObject<MoveList>
    {
        public MoveList() : base(Constants.AVG_MOVES_PER_PLY * 2)
        { }

        public void Sort(int n)
        {
            int largest = -1;
            int score = -1;
            for (int i = n; i < insertIndex; ++i)
            {
                int mvScore = Move.GetScore(array[i]);
                if (mvScore > score)
                {
                    largest = i;
                    score = mvScore;
                }
            }

            if (largest > n)
            {
                (array[n], array[largest]) = (array[largest], array[n]);
            }
        }

        public void Add(Piece piece, int from, int to, MoveType type = MoveType.Normal, Piece capture = Piece.None,
            Piece promote = Piece.None, int score = 0)
        {
            Add(Move.Pack(piece, from, to, type, capture, promote, score));
        }

        public ReadOnlySpan<ulong> ToSpan() => new(array, 0, Count);

        public ref ulong LastAdded
        {
            get
            {
                if (insertIndex == 0)
                {
                    throw new InvalidOperationException("Move list is empty.");
                }

                return ref array[insertIndex - 1];
            }
        }
    }
}
