using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using FoglioUtils.Controls;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.Classes.Elements
{
    /// <summary>
    /// Logica di interazione per RagnarokBuffControl.xaml
    /// </summary>
    public partial class RagnarokBuffControl : INotifyPropertyChanged
    {
        public RagnarokBuffControl()
        {
            InitializeComponent();
        }

        private DurationCooldownIndicator _context;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (DurationCooldownIndicator)DataContext;
            _context.Buff.Started += OnRagnarokStarted;
            WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.PropertyChanged += ST_PropertyChanged;
        }

        private void OnRagnarokStarted(CooldownMode mode)
        {
            Running = true;
            var an = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(_context.Buff.Duration));
            an.Completed += (s, ev) =>
            {
                Running = false;
            };
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, an);
        }

        public string SecondsText => WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.Val.ToString();

        private void ST_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondsText)));
                IconGlow.Opacity = WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.Factor == 1 ? 1 : 0;
                if (Running) return;
                var an = new DoubleAnimation((1-WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.Factor) * 359.9, TimeSpan.FromMilliseconds(50));
                InternalArc.BeginAnimation(Arc.EndAngleProperty, an);

            }
        }

        private bool _running;
        public bool Running
        {
            get => _running;
            set
            {
                if (_running == value) return;
                _running = value;
                if (_running)
                {
                    SecondaryGrid.Opacity = 1;
                    InternalArc.Opacity = 0;
                }
                else
                {
                    SecondaryGrid.Opacity = 0;
                    InternalArc.Opacity = 1;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
