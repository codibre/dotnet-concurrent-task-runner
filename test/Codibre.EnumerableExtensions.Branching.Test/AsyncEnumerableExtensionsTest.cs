using System.Linq;

namespace Codibre.ConcurrentTaskRunner.Test;

[Collection("Tests")]
public class AsyncBranchingBuilderTest()
{
    private static IAsyncEnumerable<int> Op(IAsyncEnumerable<int> list) => list
            .Select(x => x * 2)
            .Select(x => x + 2)
            .Select(x => x / 2);

    [Fact]
    public async Task Should_Work_With_Perfectly_Balanced_Operations()
    {
        // Arrange
        int[] list = [1, 2, 3];
        var enumerable = Op(list.ToAsyncEnumerable());

        // Act
        await enumerable.Branch()
            .Add(x => x.ToArrayAsync(), out var a)
            .Add(x => x.ToArrayAsync(), out var b)
            .Add(x => x.ToArrayAsync(), out var c)
            .Run();

        // Assert
        a.Result.Should().BeEquivalentTo(await enumerable.ToArrayAsync());
        b.Result.Should().BeEquivalentTo(await enumerable.ToArrayAsync());
        c.Result.Should().BeEquivalentTo(await enumerable.ToArrayAsync());
    }

    [Fact]
    public async Task Should_Work_With_Empty_Collections()
    {
        // Arrange
        int[] list = [];
        var enumerable = Op(list.ToAsyncEnumerable());

        // Act
        await enumerable.Branch()
            .Add(x => x.ToArrayAsync(), out var a)
            .Add(x => x.FirstOrDefaultAsync(), out var b)
            .Add(x => x.LastOrDefaultAsync(), out var c)
            .Run();

        // Assert
        a.Result.Should().BeEquivalentTo(await enumerable.ToArrayAsync());
        b.Result.Should().Be(await enumerable.FirstOrDefaultAsync());
        c.Result.Should().Be(await enumerable.LastOrDefaultAsync());
    }

    [Fact]
    public async Task Should_Work_With_NonEmpty_Collections()
    {
        // Arrange
        int[] list = [1, 2, 2, 3, 3, 3];
        var enumerable = Op(list.ToAsyncEnumerable());

        // Act
        await enumerable.Branch()
            .Add(x => x.Distinct().ToArrayAsync(), out var a)
            .Add(x => x.MinAsync(), out var b)
            .Add(x => x.MaxAsync(), out var c)
            .Run();

        // Assert
        a.Result.Should().BeEquivalentTo(await enumerable.Distinct().ToArrayAsync());
        b.Result.Should().Be(await enumerable.MinAsync());
        c.Result.Should().Be(await enumerable.MaxAsync());
    }

    [Fact]
    public async Task Should_Intercalate_The_Steps_Between_Every_Branch()
    {
        // Arrange
        var total = 100000;
        var list = Enumerable.Range(0, total).ToAsyncEnumerable();
        List<int> steps = [];
        var enumerable = Op(list);

        // Act
        await enumerable.Branch()
            .Add(x => x.Select((x) =>
                {
                    steps.Add(1);
                    return x;
                }).ToArrayAsync(), out var a)
            .Add(x => x.Select((x) =>
                {
                    steps.Add(2);
                    return x;
                }).ToArrayAsync(), out var b)
            .Add(x => x.Select((x) =>
                {
                    steps.Add(3);
                    return x;
                }).ToArrayAsync(), out var c)
            .Run();

        // Assert
        var refValue = steps[0];
        steps.TakeWhile((x) => x == refValue).Count().Should().BeLessThan(total);
    }

    [Fact]
    public async Task Should_Intercalate_The_Steps_Between_Every_Branch_When_EnumerableIsTrulyAsync()
    {
        // Arrange
        var total = 1000;
        var list = Enumerable.Range(0, total)
            .ToAsyncEnumerable()
            .SelectAwait(async (x) =>
            {
                await Task.Yield();
                return await Task.Run(() => x);
            });
        List<int> steps = [];
        var enumerable = Op(list);

        // Act
        await enumerable.Branch()
            .Add(x => x.Select((x) =>
                {
                    steps.Add(1);
                    return x;
                }).ToArrayAsync(), out var a)
            .Add(x => x.Select((x) =>
                {
                    steps.Add(2);
                    return x;
                }).ToArrayAsync(), out var b)
            .Add(x => x.Select((x) =>
                {
                    steps.Add(3);
                    return x;
                }).ToArrayAsync(), out var c)
            .Run();

        // Assert
        var refValue = steps[0];
        steps.TakeWhile((x) => x == refValue).Count().Should().BeLessThan(total);
    }

    [Fact]
    public async Task Should_Intercalate_The_Steps_Between_Every_Branch_When_Using_AsyncContextBranch()
    {
        // Arrange
        var total = 100;
        var list = Enumerable.Range(0, total).ToAsyncEnumerable();
        List<int> steps = [];
        var enumerable = Op(list);

        // Act
        await enumerable.Branch()
            .Add(x => x.Select((x) =>
                {
                    steps.Add(1);
                    return x;
                }).ToArrayAsync(), out var a)
            .Add(x => x.Select((x) =>
                {
                    steps.Add(2);
                    return x;
                }).ToArrayAsync(), out var b)
            .Add(x => x.Select((x) =>
                {
                    steps.Add(3);
                    return x;
                }).ToArrayAsync(), out var c)
            .Run(new(1));

        // Assert
        var refValue = steps[0];
        steps.TakeWhile((x) => x == refValue).Count().Should().BeLessThan(total);
    }
}