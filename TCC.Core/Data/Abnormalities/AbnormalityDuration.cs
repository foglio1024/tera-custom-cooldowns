using Nostrum;
using System;
using System.Timers;
using System.Windows.Threading;
using TCC.Debug;

namespace TCC.Data.Abnormalities
{
    public class AbnormalityDuration : TSPropertyChanged, IDisposable
    {
        public event Action Refreshed = null!;

        public ulong Target { get; }
        public Abnormality Abnormality { get; set; }

        private readonly Timer _timer;
        private uint _duration;
        private int _stacks;
        private double _durationLeft;
        private bool _isTimerDisposed;
        private DateTime _startTime;
        private DateTime _endTime;

        public uint Duration
        {
            get => _duration;
            set
            {
                if (value == _duration) return;
                _duration = value;
                N();
            }
        }
        public int Stacks
        {
            get => _stacks;
            set
            {
                if (value == _stacks) return;
                _stacks = value;
                N();
            }
        }
        public double DurationLeft
        {
            get => _durationLeft;
            set
            {
                if (value == _durationLeft) return;
                _durationLeft = value;
                N();
            }
        }
        public bool Animated { get; private set; }

        private AbnormalityDuration(Abnormality b)
        {
            Abnormality = b;
            _timer = new Timer { Interval = 900 };
            _isTimerDisposed = false;
        }
        public AbnormalityDuration(Abnormality b, uint d, int s, ulong t, Dispatcher disp, bool animated) : this(b)
        {
            ObjectTracker.Register(this.GetType());
            Dispatcher = disp;
            Animated = animated;
            Duration = d;
            Stacks = s;
            Target = t;
            DurationLeft = d;
            _startTime = DateTime.Now;
            _endTime = DateTime.Now.AddMilliseconds(d);

            if (Abnormality.Infinity) return;
            _timer.Elapsed += DecreaseDuration;
            _timer.Start();
        }

        ~AbnormalityDuration()
        {
            ObjectTracker.Unregister(this.GetType());
        }

        private void DecreaseDuration(object sender, EventArgs e)
        {
            //DurationLeft -= 1000;
            DurationLeft = (_endTime - DateTime.Now).TotalMilliseconds;
            if (DurationLeft < 0) DurationLeft = 0;
            //if (!(DurationLeft < DurationLeft - 1000)) return;
            if (DurationLeft > 0) return;
            _timer.Stop();
        }
        public void Refresh()
        {
            if (/*_timer == null || */_isTimerDisposed) return;
            _timer?.Stop();
            _startTime = DateTime.Now;
            _endTime = _startTime.AddMilliseconds(Duration);
            if (Duration != 0) _timer?.Start();
            Refreshed?.Invoke();
        }
        public void Dispose()
        {
            _timer.Stop();
            _timer.Elapsed -= DecreaseDuration;
            _timer.Dispose();
            _isTimerDisposed = true;
        }
    }
}
