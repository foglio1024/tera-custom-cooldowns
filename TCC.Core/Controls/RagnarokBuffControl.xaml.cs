using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per RagnarokBuffControl.xaml
    /// </summary>
    public partial class RagnarokBuffControl : UserControl, INotifyPropertyChanged
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
            _context.Buff.PropertyChanged += RagnarokBuff_PropertyChanged;
            ClassManager.CurrentClassManager.ST.PropertyChanged += ST_PropertyChanged;
        }
        public string SecondsText
        {
            get => ClassManager.CurrentClassManager.ST.Val.ToString();
        }

        private void ST_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SecondsText"));
                if(ClassManager.CurrentClassManager.ST.Factor == 1)
                {
                    IconGlow.Opacity = 1;
                }
                else
                {
                    IconGlow.Opacity = 0;
                }
                if (Running) return;
                var an = new DoubleAnimation((1-ClassManager.CurrentClassManager.ST.Factor) * 359.9, TimeSpan.FromMilliseconds(50));
                InternalArc.BeginAnimation(Arc.EndAngleProperty, an);

            }
        }

        private bool _running = false;
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

        private void RagnarokBuff_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Start")
            {
                Running = true;
                var an = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(_context.Buff.Cooldown));
                an.Completed += (s, ev) =>
                {
                    Running = false;
                };
                ExternalArc.BeginAnimation(Arc.EndAngleProperty, an);
            }
        }


    }
}
