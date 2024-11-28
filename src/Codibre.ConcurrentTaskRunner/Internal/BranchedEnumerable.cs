using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codibre.ConcurrentTaskRunner.Internal;

internal class BranchedEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly BranchedEnumerator<T> _enumerator;
    public BranchedEnumerable(LinkedNode<T> root) => _enumerator = new(root);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => _enumerator;
}