using BenchmarkDotNet.Attributes;

namespace Codibre.ConcurrentTaskRunner.Benchmark;

public static class AddOpsExtension
{
    public static IEnumerable<int> AddOps(this IEnumerable<int> source, int value)
        => source
            .Select(x => x * value)
            .Select(x => x + value)
            .Select(x => x / value);
}

public class Benchmarks
{
    [Params(100, 1000, 10000)]
    public int _size = 100;
    private IEnumerable<int> GetBaseEnumerable()
        => Enumerable.Range(0, _size)
                .AddOps(2)
                .AddOps(3)
                .AddOps(4);

    [Benchmark]
    public void Separate()
    {
        GetBaseEnumerable().Min();
        GetBaseEnumerable().Max();
        GetBaseEnumerable().Average();
    }

    [Benchmark]
    public void ManualBranch()
    {
        var baseEnum = GetBaseEnumerable();
        baseEnum.Min();
        baseEnum.Max();
        baseEnum.Average();
    }

    [Benchmark]
    public Task Branching() => GetBaseEnumerable()
        .Branch()
        .Add(x => x.MinAsync())
        .Add(x => x.MaxAsync())
        .Add(x => x.AverageAsync())
        .Run();
}