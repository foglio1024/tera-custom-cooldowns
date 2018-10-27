namespace TCC.Data.Npc
{
    public class HpTriggeredTimerPattern : TimerPattern
    {
        public float StartAt { get; }
        public HpTriggeredTimerPattern(int duration, float startAt) : base(duration)
        {
            StartAt = startAt;
        }

        public override void SetTarget(Npc target)
        {
            base.SetTarget(target);
            Target.PropertyChanged += OnTargetPropertyChanged;
        }

        private void OnTargetPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Running) return;
            if (e.PropertyName != nameof(Npc.CurrentFactor)) return;
            if (Target.CurrentFactor < StartAt) Start();
        }
    }
}