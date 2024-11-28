namespace Codibre.ConcurrentTaskRunner;

public class ConcurrentTaskRunner(IConcurrentTaskRunnerOptions options)
{
    private int _lastLimit = options.Limit;
    private SemaphoreSlim _semaphore = new(options.Limit, options.Limit);
    public async Task Run(Func<Task> callback, CancellationToken? cancellationToken = null)
    {
        UpdateSemaphore();
        await _semaphore.WaitAsync();
        _ = RunInBackGround(callback, _semaphore, cancellationToken);
    }

    private async Task RunInBackGround(Func<Task> callback, SemaphoreSlim semaphore, CancellationToken? cancellationToken)
    {
        if (cancellationToken?.IsCancellationRequested ?? false) return;
        try
        {
            await callback();
        }
        catch (Exception ex)
        {
            if (options.OnError is not null) options.OnError(ex);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private void UpdateSemaphore()
    {
        if (_lastLimit == options.Limit) return;
        _semaphore = new(options.Limit, options.Limit);
        _lastLimit = options.Limit;
    }
}