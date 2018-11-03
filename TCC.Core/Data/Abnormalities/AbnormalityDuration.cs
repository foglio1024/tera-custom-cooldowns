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
                NPC();
            }
        }
        public int Stacks
        {
            get => _stacks;
            set
            {
                if (value == _stacks) return;
                _stacks = value;
                NPC();
            }
        }
        public uint DurationLeft
        {
            get => _durationLeft;
            set
            {
                if (value == _durationLeft) return;
                _durationLeft = value;
                NPC();
            }
        }
        public bool Animated { get; private set; }
        DateTime AbnormalAdditonTime;       // by HQ
        uint interval = 1000;               // by HQ


        public AbnormalityDuration(Abnormality b, uint d, int s, ulong t, Dispatcher disp, bool animated)
        {
            // by HQ
            /*
            Dispatcher = disp;
            Animated = animated;
            Abnormality = b;
            Duration = d;
            Stacks = s;
            Target = t;
            _isTimerDisposed = false;

            DurationLeft = d;
            if (!Abnormality.Infinity)
            {
                _timer = new Timer(1000);
                _timer.Elapsed += DecreaseDuration;
                _timer.Start();
            }
            */
            // by HQ
            Dispatcher = disp;
            Animated = animated;
            Abnormality = b;
            Duration = d;
            Stacks = s;
            Target = t;
            _isTimerDisposed = false;

            DurationLeft = d;
            if (!Abnormality.Infinity)
            {
                AbnormalAdditonTime = DateTime.Now;
                uint DurationLeftRemainder = 0;

                if ((DurationLeft <= Settings.Settings.BuffDecimalPlaceSeconds * 1000) && (Settings.Settings.ShowBuffDecimalPlace == true) && (Target == SessionManager.CurrentPlayer.EntityId))
                {
                    DurationLeftRemainder = DurationLeft % 100;
                    interval = DurationLeftRemainder + 1;
                }
                else
                {
                    DurationLeftRemainder = DurationLeft % 1000;
                    interval = DurationLeftRemainder + 1;
                }
                _timer = new Timer(interval);
                _timer.Elapsed += DecreaseDuration;
                _timer.Start();
            }
        }

        private void DecreaseDuration(object sender, ElapsedEventArgs e)
        {
            // by HQ
            /*
            DurationLeft = DurationLeft - 1000;
            if(DurationLeft < DurationLeft - 1000) _timer.Stop();
            */
            // by HQ
            TimeSpan Timediff = DateTime.Now - AbnormalAdditonTime;
            if (Duration > (uint)Timediff.TotalMilliseconds)
            {
                DurationLeft = Duration - (uint)Timediff.TotalMilliseconds;
                uint DurationLeftRemainder = 0;
                if ((DurationLeft <= Settings.Settings.BuffDecimalPlaceSeconds * 1000) && (Settings.Settings.ShowBuffDecimalPlace == true) && (Target == SessionManager.CurrentPlayer.EntityId))
                {
                    DurationLeftRemainder = DurationLeft % 100;
                    interval = DurationLeftRemainder + 1;
                }
                else
                {
                    DurationLeftRemainder = DurationLeft % 1000;
                    interval = DurationLeftRemainder + 1;
                }
                _timer.Stop();
                if (DurationLeft > interval)
                {
                    _timer.Interval = interval;
                    _timer.Start();
                }
            }
            else
            {
                DurationLeft = 0;
                _timer.Stop();
            }
        }
        public void Refresh()
        {
            // by HQ
            /*
            if(_timer == null || _isTimerDisposed) return;
            _timer?.Stop();
            if (Duration != 0) _timer?.Start();
            Refreshed?.Invoke();
            */
            // by HQ
            if (_timer == null || _isTimerDisposed) return;
            _timer?.Stop();
            if (Duration != 0)
            {
                _timer?.Start();
                AbnormalAdditonTime = DateTime.Now;
            }
            Refreshed?.Invoke();
        }
        public void Dispose()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Elapsed -= DecreaseDuration;
            _timer.Dispose();
            _isTimerDisposed = true;

        }
    }
}
