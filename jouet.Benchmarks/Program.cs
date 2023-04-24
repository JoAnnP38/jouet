using BenchmarkDotNet.Running;

namespace jouet.Benchmarks
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<SlidingMovesBenchmarks>();
        }
    }
}