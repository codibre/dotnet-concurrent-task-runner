[![Actions Status](https://github.com/Codibre/dotnet-enumerable/workflows/build/badge.svg)](https://github.com/Codibre/dotnet-enumerable/actions)
[![Actions Status](https://github.com/Codibre/dotnet-enumerable/workflows/test/badge.svg)](https://github.com/Codibre/dotnet-enumerable/actions)
[![Actions Status](https://github.com/Codibre/dotnet-enumerable/workflows/lint/badge.svg)](https://github.com/Codibre/dotnet-enumerable/actions)
[![Maintainability](https://api.codeclimate.com/v1/badges/09278283aa83446a1b39/maintainability)](https://codeclimate.com/github/codibre/dotnet-enumerable/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/09278283aa83446a1b39/test_coverage)](https://codeclimate.com/github/codibre/dotnet-enumerable/test_coverage)

# Codibre.ConcurrentTaskRunner

Branching operation that allows you to give a single enumerable many different, concurrent resolutions

## Why?

In many situations we have a unmaterialized enumerable, and need to use it on several different operations and will need to iterate over it many times. To do it safely, one option is to materialize it into an array and run the operations over it. Another is to manually create a loop that runs all the operations.
The first option may result in an unwanted memory consumption, the second one into create a code that ends to be too complex, mixing many responsabilities into a single point just to take advantage of one iteration.
This library offers a third option: enumerable branching.

## How to use?

First, do all the operations that are commons between the different resolutions you want

```c#
var enumerable = baseEnumerable
    .Select(DoSomeOperation)
    .SelectMany(DoSomeFlatteningOperation)
    .Where(DoSomeFilter)
    ...;
```

Finally, you ban use the Branch operation to diverge the many resolutions you need

```c#
await enumerable
    .Branch()
    .Add((e) => e.Select(Resolution1).ToArrayAsync(), out var result1)
    .Add((e) => e.Select(Resolution2).MinAsync(), out var result2)
    .Add((e) => e.Select(Resolution3).MaxAsync(), out var result3)
    .Run();
```

Now, you'll have the three results you need, accesible through the property **Value** of result1, 2 and 3!

## What this operation is doing under the hood?

The first operations run only one time, not one per branch, and then, the iteration continues concurrenctly between the branches informed. This is possible only using async tasks and yielding between them, but some buffer with partial results needs to be done for performance reasons, too, but overall, the Branch operation will try to keep as less memory in use as possible, but trying to keep performance close to what it'd be without it.