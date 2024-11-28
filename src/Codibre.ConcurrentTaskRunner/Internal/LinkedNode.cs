namespace Codibre.ConcurrentTaskRunner.Internal;

internal sealed record LinkedNode<T>
{
    public T Value { get; }
    public Lazy<ValueTask<LinkedNode<T>?>> Next { get; private set; }

    public LinkedNode(T value, IBranchContext<T> context)
    {
        Value = value;
        Next = new(context.FillNext, LazyThreadSafetyMode.ExecutionAndPublication);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private LinkedNode(T value) => Value = value;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static LinkedNode<T>? New(IEnumerator<T> source, BranchRunOptions options, IBranchContext<T> context)
    {
        if (!source.MoveNext()) return null;
        var preload = options.Limit;
        LinkedNode<T> root = new(source.Current);
        var node = root;
        while (preload-- > 0 && source.MoveNext()) node = (node.Next = new(ValueTask.FromResult<LinkedNode<T>?>(new(source.Current)))).Value.Result!;
        node.Next = new(context.FillNext, LazyThreadSafetyMode.ExecutionAndPublication);
        return root;
    }

    public static async ValueTask<LinkedNode<T>?> New(IAsyncEnumerator<T> source, BranchRunOptions options, IBranchContext<T> context)
    {
        if (!await source.MoveNextAsync()) return null;
        var preload = options.Limit;
        LinkedNode<T> root = new(source.Current);
        var node = root;
        while (preload-- > 0 && await source.MoveNextAsync()) node = (node.Next = new(ValueTask.FromResult<LinkedNode<T>?>(new(source.Current)))).Value.Result!;
        node.Next = new(context.FillNext, LazyThreadSafetyMode.ExecutionAndPublication);
        return root;
    }

    public static LinkedNode<T> Root(IEnumerator<T> source, BranchRunOptions options)
        => new(default(T)!)
        {
            Next = new(
                    ValueTask.FromResult(
                        New(
                            source,
                            options,
                            new AsyncBranchContext<T>(
                                (c) => ValueTask.FromResult(LinkedNode<T>.New(source, options, c))
                            )
                        )
                    )
                )
        };

    public static LinkedNode<T> Root(IAsyncEnumerator<T> source, BranchRunOptions options)
        => new(default(T)!)
        {
            Next = new(
                    New(
                        source,
                        BranchRunOptions.Yielder,
                        new BranchContext<T>(
                            (c) => LinkedNode<T>.New(source, BranchRunOptions.Yielder, c),
                            options
                        )
                    )
                )
        };
}