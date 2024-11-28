[![Actions Status](https://github.com/Codibre/dotnet-concurrent-task-runner/workflows/build/badge.svg)](https://github.com/Codibre/dotnet-concurrent-task-runner/actions)
[![Actions Status](https://github.com/Codibre/dotnet-concurrent-task-runner/workflows/test/badge.svg)](https://github.com/Codibre/dotnet-concurrent-task-runner/actions)
[![Actions Status](https://github.com/Codibre/dotnet-concurrent-task-runner/workflows/lint/badge.svg)](https://github.com/Codibre/dotnet-concurrent-task-runner/actions)
[![Maintainability](https://api.codeclimate.com/v1/badges/1bb9ce35589dc5714669/maintainability)](https://codeclimate.com/github/codibre/dotnet-concurrent-task-runner/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/1bb9ce35589dc5714669/test_coverage)](https://codeclimate.com/github/codibre/dotnet-concurrent-task-runner/test_coverage)

# Codibre.ConcurrentTaskRunner

A lib for concurrent, semaphore limited task runner under the hood

## How to use?

Create an instance of options specifying the desired concurrency limit

```c#
ConcurrentTaskRunnerOptions options = new()
{
    Limit = 3
};
```

Then, crate your runner instance

```c#
ConcurrentTaskRunner runner = new(options);
```

Finally, delegate the calls you want to limit concurrency

```c#
await runner.Run(MyCall);
```

Notice that, if the method Run never waits for MyCall to finish, it actually waits for a run slot to be freed, that is, if the concurrency limit hasn't been reached yet, it'll not wait and just pass through to MyCall execution in background.

## Error handling

By default, any error the callback passed to Run throws is ignored, but you can treat it somehow informing a callback in the options to deal with it, like this:

```c#
ConcurrentTaskRunnerOptions options = new()
{
    Limit = 3,
    OnError = (Exception err) => Console.WriteLine(err.Message)
}
```

Errors thrown by OnError callback are not ignored, though, so write it carefully making sure it'll not throw anything so you don't get an untreated exception.

## Procressing an Enumerable or AsyncEnumerable

It's also possible to use ConcurrentTaskRunner to process in a concurrently limited pace an enumerable or async enumerable instance. You just need to use our extension method **RunConcurrently**, like this:

```c#
await enumerable
    .RunConcurrently(runner, (x) => MyCall(x));
```

Notice that a AsyncEnumerable may be infinite, so this is a blocking line in your code if that's the case. A CancellationToken instance can be used to create a stoppage condition:

```c#
await enumerable
    .RunConcurrently(runner, (x) => MyCall(x), cancellationToken);
```

In this implementation, cancellationToken will not throw an error, but before executing the callback, it'll be checked to validate whether the cancellation had been requested or not, and it's not propagated to the callback. If you need it, though, you can propagate it yourself, like this:

```c#
await enumerable
    .RunConcurrently(runner, (x) => MyCall(x, cancellationToken), cancellationToken);
```