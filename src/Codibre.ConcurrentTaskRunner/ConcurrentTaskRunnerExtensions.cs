namespace Codibre.ConcurrentTaskRunner;

public static class ConcurrentTaskRunnerExtensions
{
    public static Task RunConcurrently<T>(
        this IAsyncEnumerable<T> list,
        ConcurrentTaskRunner runner,
        Func<T, Task> callback,
        CancellationToken? cancellationToken = null
    )
        => list
            .TakeWhile((_) => cancellationToken?.IsCancellationRequested != true)
            .ForEachAwaitAsync((x) => runner.Run(() => callback(x), cancellationToken));

    public static Task RunConcurrently<T>(
        this IEnumerable<T> list,
        ConcurrentTaskRunner runner,
        Func<T, Task> callback,
        CancellationToken? cancellationToken = null
    )
        => list
            .ToAsyncEnumerable()
            .RunConcurrently(runner, callback, cancellationToken);
}