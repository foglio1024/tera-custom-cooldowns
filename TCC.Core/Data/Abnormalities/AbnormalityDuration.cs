using System;
using System.Globalization;
using System.Timers;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using Nostrum;

namespace TCC.Data.Abnormalities
{
    public class DurationLabelConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = 0D;
            if (value != null) val = System.Convert.ToDouble(value);
            var seconds = Math.Floor(val / 1000);
            var minutes = Math.Floor(seconds / 60);
            var hours = Math.Floor(minutes / 60);
            var days = Math.Floor(hours / 24);

            if (minutes < 3) return seconds.ToString();
            if (hours < 3) return $"{minutes}m";
            if (days < 1) return $"{hours}h";
            return $"{days}d";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    public class AbnormalityDuration : TSPropertyChanged, IDisposable
    {
        public event Action Refreshed;

        public ulong Target { get; set; }
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
        public AbnormalityDuration()
        {
            _timer = new Timer { Interval = 900 };
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
            _startTime = DateTime.Now;
            _endTime = DateTime.Now.AddMilliseconds(d);

            if (Abnormality.Infinity) return;
            _timer.Elapsed += DecreaseDuration;
            _timer.Start();
        }


        private void DecreaseDuration(object sender, EventArgs e)
        {
            //DurationLeft -= 1000;
            DurationLeft = (_endTime - DateTime.Now).TotalMilliseconds;
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
            if (_timer != null && !_isTimerDisposed)
            {
                _timer?.Stop();
                _timer.Elapsed -= DecreaseDuration;
                _timer?.Dispose();
            }
            _isTimerDisposed = true;
        }
    }
}
