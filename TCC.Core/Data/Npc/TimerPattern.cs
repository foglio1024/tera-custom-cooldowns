using System;
using System.Timers;
using Nostrum;

namespace TCC.Data.Npc
{
    public class TimerPattern : TSPropertyChanged, IDisposable
    {
        private readonly Timer _timer;
        protected bool Running => _timer.Enabled;
        protected NPC? Target { get; set; }
        public int Duration { get; }

        public event Action? Started;
        public event Action? Ended;
        //public event Action Reset;

        public void Start()
        {
            _timer.Start();
            Started?.Invoke();
        }

        public virtual void SetTarget(NPC target)
        {
            Target = target;
        }

        protected TimerPattern(int duration)
        {
            Duration = duration;
            _timer = new Timer(Duration * 1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            Ended?.Invoke();
        }

        public void Dispose()
        {
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Stop();
            _timer.Dispose();

        }
    }
}