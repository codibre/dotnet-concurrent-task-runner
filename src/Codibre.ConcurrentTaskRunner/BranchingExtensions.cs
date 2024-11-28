using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codibre.ConcurrentTaskRunner.Internal;

namespace Codibre.ConcurrentTaskRunner;

public static class BranchingExtensions
{
    public static BaseBranchingBuilder<T> Add<T, R>(this BaseBranchingBuilder<T> builder, Func<IAsyncEnumerable<T>, ValueTask<R>> branch, out BranchResult<R> result)
    {
        var refResult = result = new();
        return builder.Add(async (x) => refResult.Result = await branch(x));
    }

    public static BaseBranchingBuilder<T> Add<T, R>(this BaseBranchingBuilder<T> builder, Func<IAsyncEnumerable<T>, ValueTask<R>> branch)
        => builder.Add((x) => branch(x).AsTask());
}