namespace TCC.Data.NPCs
{
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

        public override void SetTarget(NPC target)
        {
            base.SetTarget(target);
            target.PropertyChanged += OnTargetPropertyChanged;
        }

        private void OnTargetPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Running) return;
            if (e.PropertyName != nameof(NPC.HPFactor)) return;
            if (!(sender is NPC npc) || !(npc.HPFactor < StartAt)) return;
            Start();
        }
    }
}