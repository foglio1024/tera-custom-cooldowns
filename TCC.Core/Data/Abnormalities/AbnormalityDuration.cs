using System;
using System.Timers;
using System.Windows.Threading;

namespace TCC.Data.Abnormalities
{
    public class AbnormalityDuration : TSPropertyChanged, IDisposable
    {
        public event Action Refreshed;

        public ulong Target { get; set; }
        public Abnormality Abnormality { get; set; }

        private readonly Timer _timer;
        private uint _duration;
        private int _stacks;
        private uint _durationLeft;
        private bool _isTimerDisposed;

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
        public uint DurationLeft
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
        public AbnormalityDuration()
        {
            _timer = new Timer { Interval = 1000 };
            _isTimerDisposed = false;
        }
        public AbnormalityDuration(Abnormality b, uint d, int s, ulong t, Dispatcher disp, bool animated) : this()
        {
            Dispatcher = disp;
            Animated = animated;
            Abnormality = b;
            Duration = d;
            Stacks = s;
            Target = t;
            DurationLeft = d;

            if (!Abnormality.Infinity)
            {
                _timer.Elapsed += DecreaseDuration;
                _timer.Start();
            }
        }


        private void DecreaseDuration(object sender, EventArgs e)
        {
            DurationLeft = DurationLeft - 1000;
            if (DurationLeft < DurationLeft - 1000) _timer.Stop();
        }
        public void Refresh()
        {
            if (/*_timer == null || */_isTimerDisposed) return;
            _timer?.Stop();
            if (Duration != 0) _timer?.Start();
            Refreshed?.Invoke();
        }
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= DecreaseDuration;
                _timer.Dispose();
            }
            _isTimerDisposed = true;
        }
    }
}
