using System.Diagnostics;
using jouet.Chess;

namespace jouet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new();
            Perft perft = new(Notation.FEN_START_POS);

            int depth = 7;
            perft.Expand(3);

            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"Iteration #{i+1}:");
                for (int d = 1; d <= depth; d++)
                {
                    sw.Restart();
                    ulong nodes = perft.Expand(d);
                    sw.Stop();

                    double Mnps = (double)nodes / (sw.Elapsed.TotalSeconds * 1000000.0D);
                    Console.WriteLine($@"{d}: Elapsed = {sw.Elapsed}, Mnps: {Mnps,6:N2}, nodes = {nodes}");
                }

                Console.WriteLine();
            }
        }
    }
}