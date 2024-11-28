using System.Runtime.CompilerServices;
using System.Xml.XPath;

namespace Codibre.ConcurrentTaskRunner.Internal;

internal sealed record BranchContext<T>(Func<IBranchContext<T>, ValueTask<LinkedNode<T>?>> GetNext, BranchRunOptions options)
    : IBranchContext<T>
{
    private ushort _count = 0;

    public ValueTask<LinkedNode<T>?> FillNext()
    {
        var result = GetNext(this);
        if (result.IsCompleted) return ++_count <= options.Limit ? result : GetYielded(result.Result);
        _count = 0;
        return result;
    }

    private async ValueTask<LinkedNode<T>?> GetYielded(LinkedNode<T>? result)
    {
        _count = 0;
        await Task.Yield();
        return result;
    }
}