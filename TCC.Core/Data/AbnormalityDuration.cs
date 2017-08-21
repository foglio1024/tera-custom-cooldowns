using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;

namespace TCC
{
    public class AbnormalityDuration : TSPropertyChanged, IDisposable
    {
        public ulong Target { get; set; }
        private Abnormality _abnormality;
        public Abnormality Abnormality
        {
            get { return _abnormality; }
            set
            {
                if (_abnormality == value) return;
                _abnormality = value;
            }
        }
        private uint _duration;
        public uint Duration
        {
            get { return _duration; }
            set
            {
                if (value == _duration) return;
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }
        private int _stacks;
        public int Stacks
        {
            get { return _stacks; }
            set
            {
                if (value == _stacks) return;
                _stacks = value;
                NotifyPropertyChanged("Stacks");
            }
        }
        private readonly System.Timers.Timer timer;
        private uint _durationLeft;
        public uint DurationLeft
        {
            get { return _durationLeft; }
            set
            {
                if (value == _durationLeft) return;
                _durationLeft = value;
                NotifyPropertyChanged("DurationLeft");
            }
        }

        private double _iconSize;
        public double IconSize
        {
            get { return _iconSize; }
            set
            {
                if (_iconSize == value) return;
                _iconSize = value;
            }
        }
        public double BackgroundEllipseSize { get; set; }
        public Thickness IndicatorMargin { get; set; }
        static int _count = 0;
        public bool Animated { get; private set; }
        public AbnormalityDuration(Abnormality b, uint d, int s, ulong t, Dispatcher disp, bool animated, double iconSize, double bgEllSize, Thickness margin)
        {
            _count++;
            _dispatcher = disp;
            Animated = animated;
            Abnormality = b;
            Duration = d;
            Stacks = s;
            Target = t;


            IconSize = iconSize;
            BackgroundEllipseSize = bgEllSize;
            IndicatorMargin = margin;

            DurationLeft = d;
            if (!Abnormality.Infinity)
            {
                timer = new System.Timers.Timer(1000);
                timer.Elapsed += DecreaseDuration;
                timer.Start();
            }
        }

        private void DecreaseDuration(object sender, ElapsedEventArgs e)
        {
            DurationLeft = DurationLeft - 1000;
            if (DurationLeft < 0) timer.Stop();
        }

        public void Refresh()
        {
            timer?.Stop();
            if (Duration != 0) timer?.Start();
            NotifyPropertyChanged("Refresh");
        }

        public void Dispose()
        {
            _count--;

            if (timer == null) return;
            timer.Stop();
            timer.Elapsed -= DecreaseDuration;
            timer.Dispose();

        }
    }


}
