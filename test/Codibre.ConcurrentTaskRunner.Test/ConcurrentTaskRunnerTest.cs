using System.Linq;

namespace Codibre.ConcurrentTaskRunner.Test;

public class ConcurrentTaskRunnerTest()
{

    [Fact]
    public async Task Should_Respect_Concurrency_Limit()
    {
        // Arrange
        HashSet<int> list = [];
        ConcurrentTaskRunner runner = new(new ConcurrentTaskRunnerOptions()
        {
            Limit = 3
        });
        TaskCompletionSource tsc = new();

        // Act
        var waiter = Task.WhenAll([
            runner.Run(() =>
            {
                list.Add(1);
                return tsc.Task;
            }),
            runner.Run(() =>
            {
                list.Add(2);
                return tsc.Task;
            }),
            runner.Run(() =>
            {
                list.Add(3);
                return tsc.Task;
            }),
            runner.Run(() =>
            {
                list.Add(4);
                return Task.CompletedTask;
            })
        ]);
        await Task.Delay(1);

        // Assert
        list.Should().Contain(1);
        list.Should().Contain(2);
        list.Should().Contain(3);
        list.Should().NotContain(4);
        tsc.SetResult();
        await Task.Delay(1);
        list.Should().Contain(4);
        waiter.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Allow_Concurrency_Limit_Change()
    {
        // Arrange
        HashSet<int> list = [];
        var options = new ConcurrentTaskRunnerOptions()
        {
            Limit = 3
        };
        ConcurrentTaskRunner runner = new(options);
        TaskCompletionSource tsc = new();
        TaskCompletionSource tsc2 = new();

        // Act
        var waiter = Task.WhenAll([
            runner.Run(() =>
            {
                list.Add(1);
                return tsc.Task;
            }),
            runner.Run(() =>
            {
                list.Add(2);
                return tsc.Task;
            }),
            runner.Run(() =>
            {
                list.Add(3);
                return tsc.Task;
            }),
            runner.Run(() =>
            {
                list.Add(4);
                return tsc2.Task;
            })
        ]);
        await Task.Delay(1);

        // Assert
        list.Should().Contain(1);
        list.Should().Contain(2);
        list.Should().Contain(3);
        list.Should().NotContain(4);
        tsc.SetResult();
        await Task.Delay(10);
        list.Should().Contain(4);
        options.Limit = 1;
        _ = runner.Run(() => Task.FromResult(list.Add(5)));
        await Task.Delay(10);
        list.Should().Contain(5);
        tsc2.SetResult();
        waiter.IsCompleted.Should().BeTrue();
    }
}