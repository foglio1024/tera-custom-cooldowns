namespace TCC.Data.Npc;

public class HpTriggeredTimerPattern : TimerPattern
{
    public float StartAt { get; }

    /// <summary>
    /// Creates a new TimerPattern triggered by HP value.
    /// </summary>
    /// <param name="duration">timer duration in seconds</param>
    /// <param name="startAt">HP value trigger</param>
    public HpTriggeredTimerPattern(int duration, float startAt) : base(duration)
    {
        StartAt = startAt;
    }

    public override void SetTarget(Npc target)
    {
        base.SetTarget(target);
        target.HpFactorChanged += OnTargetHpChanged;
    }

    private void OnTargetHpChanged(double hpFactor)
    {
        if (hpFactor >= StartAt)
        {
            Reset();
            return;
        }
        if (IsRunning) return;

        Start();
    }
}