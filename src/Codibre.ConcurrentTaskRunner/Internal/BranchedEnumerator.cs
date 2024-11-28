using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codibre.ConcurrentTaskRunner.Internal;

internal class BranchedEnumerator<T> : IAsyncEnumerator<T>
{
    private LinkedNode<T>? _node;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BranchedEnumerator(LinkedNode<T> root) => _node = root;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public T Current { get; set; }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_node is null) return false;
        _node = await _node.Next.Value;
        if (_node is null) return false;
        Current = _node.Value;
        return true;
    }
}