
using BenchmarkDotNet.Attributes;
using jouet.Collections;

namespace jouet.Benchmarks
{
    public class Array2DBenchmarks
    {
        [Params(10, 100, 1000)]
        public int Size { get; set; }

        public int[] I = Array.Empty<int>();
        public int[] J = Array.Empty<int>();
        public int[,]? MultiDim;
        public int[][]? Jagged;
        public Array2D<int>? Mapped;
        public UnsafeArray2D<int>? Unsafe;

        [GlobalSetup]
        public void GlobalSetup()
        {
            I = new int[Size];
            J = new int[Size];
            MultiDim = new int[Size, Size];
            Jagged = new int[Size][];
            Mapped = new Array2D<int>(Size, Size, true);
            Unsafe = new UnsafeArray2D<int>(Size, Size, true);

            for (int i = 0; i < Size; i++)
            {
                I[i] = Random.Shared.Next(0, Size);
                J[i] = Random.Shared.Next(0, Size);
                Jagged[i] = new int[Size];
                for (int j = 0; j < Size; j++)
                {
                    MultiDim[i, j] = i * Size + j;
                    Jagged[i][j] = i * Size + j;
                    Mapped[i, j] = i * Size + j;
                    Unsafe[i, j] = i * Size + j;
                }
            }
        }

        [Benchmark(Baseline = true)]
        public long MultiDimBenchmark()
        {
            long sum = 0;
            for (int n = 0; n < Size; n++)
            {
                for (int m = 0; m < Size; m++)
                {
                    sum += MultiDim![I[n], J[m]];
                }
            }

            return sum;
        }

        [Benchmark]
        public long JaggedBenchmark()
        {
            long sum = 0;
            for (int n = 0; n < Size; n++)
            {
                for (int m = 0; m < Size; m++)
                {
                    sum += Jagged![I[n]][J[m]];
                }
            }

            return sum;
        }

        [Benchmark]
        public long MappedBenchmark()
        {
            long sum = 0;
            for (int n = 0; n < Size; n++)
            {
                for (int m = 0; m < Size; m++)
                {
                    sum += Mapped![I[n], J[m]];
                }
            }

            return sum;
        }

        [Benchmark]
        public long UnsafeBenchmark()
        {
            long sum = 0;
            for (int n = 0; n < Size; n++)
            {
                for (int m = 0; m < Size; m++)
                {
                    sum += Unsafe![I[n], J[m]];
                }
            }

            return sum;
        }
    }
}
