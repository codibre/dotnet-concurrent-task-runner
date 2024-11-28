using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codibre.ConcurrentTaskRunner.Internal;

namespace Codibre.ConcurrentTaskRunner;

public abstract class BaseBranchingBuilder<T>
{
    private readonly List<Func<IAsyncEnumerable<T>, Task>> _branches = [];

    internal BaseBranchingBuilder<T> Add(Func<IAsyncEnumerable<T>, Task> branch)
    {
        _branches.Add(branch);
        return this;
    }

    public async Task Run(BranchRunOptions? options = null)
    {
        var node = Iterate(options ?? BranchRunOptions.Default);
        await Task.WhenAll(_branches.Select(x => x(node.GetBranchedIterable())));
    }

    internal abstract LinkedNode<T> Iterate(BranchRunOptions limit);
}