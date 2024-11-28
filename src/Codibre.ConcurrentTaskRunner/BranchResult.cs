using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codibre.ConcurrentTaskRunner;

public sealed record BranchResult<R>
{
    private bool _set = false;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private R _result;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public R Result
    {
        get
        {
            if (_set) return _result;
            throw new InvalidOperationException("Result not set yet");
        }
        internal set
        {
            _result = value;
            _set = true;
        }
    }
}