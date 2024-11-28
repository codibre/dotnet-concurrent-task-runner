using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codibre.ConcurrentTaskRunner.Internal;

namespace Codibre.ConcurrentTaskRunner;

public sealed class BranchingBuilder<T>(IEnumerable<T> source) : BaseBranchingBuilder<T>
{
    internal override LinkedNode<T> Iterate(BranchRunOptions options) => LinkedNode<T>.Root(source.GetEnumerator(), options);
}