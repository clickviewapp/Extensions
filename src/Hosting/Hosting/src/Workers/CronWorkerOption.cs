﻿namespace ClickView.Extensions.Hosting.Workers;

using Exceptions;

public record CronWorkerOption
{
    public bool AllowExtraDelay { get; }
    public uint MinDelayInSecond { get; }
    public uint MaxDelayInSecond { get; } = 120;

    public CronWorkerOption(bool allowExtraDelay)
    {
        AllowExtraDelay = allowExtraDelay;
    }

    public CronWorkerOption(bool allowExtraDelay, uint minDelayInSecond, uint maxDelayInSecond)
    {
        AllowExtraDelay = allowExtraDelay;
        MinDelayInSecond = minDelayInSecond;
        MaxDelayInSecond = maxDelayInSecond;

        Validate();
    }

    private void Validate()
    {
        if (MinDelayInSecond >= MaxDelayInSecond)
            throw new InvalidSchedulerOptionException(nameof(MinDelayInSecond), $"{nameof(MinDelayInSecond)} value must be less than {nameof(MaxDelayInSecond)} value");

        if (MaxDelayInSecond > 120)
            throw new InvalidSchedulerOptionException(nameof(MaxDelayInSecond), $"{nameof(MaxDelayInSecond)} value must not exceed 120");
    }
}
