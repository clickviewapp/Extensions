namespace ClickView.Extensions.Hosting.Workers;

using Exceptions;

public record CronWorkerOption
{
    public bool AllowJitter { get; }
    /// <summary>
    /// Minimum extra time in second delayed before the scheduled task can start
    /// </summary>
    public uint MinJitter { get; }
    /// <summary>
    /// Maximum extra time in second delayed before the scheduled task can start
    /// </summary>
    public uint MaxJitter { get; } = 120;

    public CronWorkerOption(bool allowJitter)
    {
        AllowJitter = allowJitter;
    }

    public CronWorkerOption(bool allowJitter, uint minJitter, uint maxJitter)
    {
        AllowJitter = allowJitter;
        MinJitter = minJitter;
        MaxJitter = maxJitter;

        Validate();
    }

    private void Validate()
    {
        if (MinJitter >= MaxJitter)
            throw new InvalidSchedulerOptionException(nameof(MinJitter), $"{nameof(MinJitter)} value must be less than {nameof(MaxJitter)} value");

        if (MaxJitter > 120)
            throw new InvalidSchedulerOptionException(nameof(MaxJitter), $"{nameof(MaxJitter)} value must not exceed 120");
    }
}

