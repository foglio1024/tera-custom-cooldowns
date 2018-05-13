using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.Controls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Logica di interazione per BarCooldown.xaml
    /// </summary>
    public partial class BarCooldown : INotifyPropertyChanged
    {
        public BarCooldown()
        {
            InitializeComponent();
        }

        private DispatcherTimer _numberTimer;

        private FixedSkillCooldown _context;
        private double _currentCd;
        public double CurrentCd
        {
            get => _currentCd;
            set
            {
                if (!(Math.Abs(_currentCd - value) > 0.01)) return;
                _currentCd = value;
                NPC(nameof(CurrentCd));
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                NPC(nameof(IsRunning));
            }
        }

        public SolidColorBrush Color
        {
            get => (SolidColorBrush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(BarCooldown));



        public event PropertyChangedEventHandler PropertyChanged;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private void NPC(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _context = (FixedSkillCooldown)DataContext;
                _context.PropertyChanged += _context_PropertyChanged;
            }
            CdBar.RenderTransform = new ScaleTransform(0, 1, 0, .5);
            Circle.RenderTransform = new TranslateTransform(0, 0);
            _numberTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            _numberTimer.Tick += (s, o) =>
            {
                CurrentCd = CurrentCd - 1 > 0 ? CurrentCd - 1 : 0;
            };
        }


        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Start") return;
            CurrentCd = (double)_context.Cooldown / 1000;
            AnimateCooldown(_context.Cooldown);
        }

        private void AnimateCooldown(ulong cooldown)
        {
            IsRunning = true;
            _numberTimer.IsEnabled = false;
            _numberTimer.IsEnabled = true;
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(cooldown));
            an.Completed += (s, ev) =>
            {
                IsRunning = false;
                _numberTimer.IsEnabled = false;
            };
            CdBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);

            var an2 = new DoubleAnimation(ActualWidth, 0, TimeSpan.FromMilliseconds(cooldown));
            Circle.RenderTransform.BeginAnimation(TranslateTransform.XProperty, an2);
        }
    }
}
