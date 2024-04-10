﻿namespace ClickView.Extensions.Hosting.Workers;

using System;
using Cronos;
using Exceptions;

public record CronWorkerOption
{
    public bool AllowJitter { get; }
    /// <summary>
    /// Minimum extra time in second delayed before the scheduled task can start
    /// </summary>
    public TimeSpan MinJitter { get; } = TimeSpan.Zero;
    /// <summary>
    /// Maximum extra time in second delayed before the scheduled task can start
    /// </summary>
    public TimeSpan MaxJitter { get; } = TimeSpan.FromSeconds(120);

    /// <summary>
    /// Cron expression that includes second
    /// Reference: https://www.nuget.org/packages/Cronos/
    /// </summary>
    public string Schedule { get; }

    public CronWorkerOption(string schedule)
    {
        Schedule = schedule;
    }

    public CronWorkerOption(string schedule, bool allowJitter, TimeSpan minJitter, TimeSpan maxJitter)
    {
        Schedule = schedule;
        AllowJitter = allowJitter;
        MinJitter = minJitter;
        MaxJitter = maxJitter;

        Validate();
    }

    private void Validate()
    {
        if (!CronExpression.TryParse(Schedule, CronFormat.IncludeSeconds, out var _))
            throw new InvalidCronWorkerOptionException(nameof(Schedule), $"{nameof(Schedule)} is not in a correct cron format");

        if (MinJitter >= MaxJitter)
            throw new InvalidCronWorkerOptionException(nameof(MinJitter), $"{nameof(MinJitter)} value must be less than {nameof(MaxJitter)} value");

        if (MaxJitter > TimeSpan.FromSeconds(120))
            throw new InvalidCronWorkerOptionException(nameof(MaxJitter), $"{nameof(MaxJitter)} value must not exceed 120 seconds");
    }
}

