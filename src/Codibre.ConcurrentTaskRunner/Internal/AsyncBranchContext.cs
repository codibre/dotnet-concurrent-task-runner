using System.Runtime.CompilerServices;
using System.Xml.XPath;

namespace Codibre.ConcurrentTaskRunner.Internal;

internal sealed record AsyncBranchContext<T>(Func<IBranchContext<T>, ValueTask<LinkedNode<T>?>> GetNext)
    : IBranchContext<T>
{

    public ValueTask<LinkedNode<T>?> FillNext()
    {
        var result = GetNext(this);
        return result.IsCompleted ? new(Task.Run(() => result.Result)) : result;
    }
}