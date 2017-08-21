using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per LancerBuffCooldownControl.xaml
    /// </summary>
    public partial class BuffCooldownControl : UserControl
    {
        public BuffCooldownControl()
        {
            InitializeComponent();
        }

        DurationCooldownIndicator _context;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(50000)));

            if (DesignerProperties.GetIsInDesignMode(this) || DataContext == null) return;
            _context = (DurationCooldownIndicator)DataContext;
            _context.Buff.PropertyChanged += Buff_PropertyChanged;
        }

        private void Buff_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.InvokeIfRequired(() =>
            {

                if (e.PropertyName == "Start")
                {
                    externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(_context.Buff.Cooldown)));
                    return;
                }
                if (e.PropertyName == "Refresh")
                {
                    externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(_context.Buff.Cooldown)));
                }
            }, System.Windows.Threading.DispatcherPriority.DataBind);

        }
    }
}
