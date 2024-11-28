using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codibre.ConcurrentTaskRunner.Internal;

internal static class BranchingHelper
{
    internal static IAsyncEnumerable<T> GetBranchedIterable<T>(this LinkedNode<T> root)
        => new BranchedEnumerable<T>(root);
}