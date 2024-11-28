

namespace Codibre.ConcurrentTaskRunner;

public class ConcurrentTaskRunnerOptions : IConcurrentTaskRunnerOptions
{
    public int Limit { get; set; } = 1;

    public Action<Exception>? OnError { get; set; } = null;
}