using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;
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

            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (DurationCooldownIndicator)DataContext;
            _context.Buff.PropertyChanged += Buff_PropertyChanged;
        }

        private void Buff_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Start")
            {
                externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(_context.Buff.Cooldown)));
            }
        }
    }
}
