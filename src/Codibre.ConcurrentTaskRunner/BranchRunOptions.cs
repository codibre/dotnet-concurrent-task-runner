
namespace Codibre.ConcurrentTaskRunner;

public readonly struct BranchRunOptions(int limit)
{
    public static readonly BranchRunOptions Default = new(ushort.MaxValue / 4);
    public static readonly BranchRunOptions Yielder = new(1);
    public int Limit => limit;
}