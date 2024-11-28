using System.Runtime.CompilerServices;
using System.Xml.XPath;

namespace Codibre.ConcurrentTaskRunner.Internal;

internal interface IBranchContext<T>
{
    ValueTask<LinkedNode<T>?> FillNext();
}