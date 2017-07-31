using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for ChannelSetting.xaml
    /// </summary>
    public partial class ChannelSetting : UserControl
    {
        public ChannelSetting()
        {
            InitializeComponent();
        }

        private void Ellipse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (ChatChannelOnOff)DataContext;
            dc.Enabled = !dc.Enabled;
        }
    }
}
