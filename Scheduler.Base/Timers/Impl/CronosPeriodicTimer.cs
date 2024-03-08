// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics;
using Cronos;

namespace Econolite.Ode.Common.Scheduler.Base.Timers.Impl;

public sealed class CronosPeriodicTimer
{
    private readonly CronExpression _cronExpression; // Also used as the locker
    private static readonly TimeSpan _minDelay = TimeSpan.FromMilliseconds(500);

    public CronosPeriodicTimer(string expression, CronFormat format)
    {
        _cronExpression = CronExpression.Parse(expression, format);
    }

    public async Task WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TimeSpan delay;
        lock (_cronExpression)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime? utcNext = _cronExpression.GetNextOccurrence(utcNow + _minDelay);
            if (utcNext is null)
                throw new InvalidOperationException("Unreachable date.");

            delay = utcNext.Value - utcNow;

            Debug.Assert(delay > _minDelay);
        }
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
    }
}
