
namespace Codibre.ConcurrentTaskRunner;

public interface IConcurrentTaskRunnerOptions
{
    int Limit { get; set; }
    Action<Exception>? OnError { get; }
}