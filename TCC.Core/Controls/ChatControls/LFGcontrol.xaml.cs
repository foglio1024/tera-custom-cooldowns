using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for LFGcontrol.xaml
    /// </summary>
    public partial class LFGcontrol : UserControl
    {
        LFG _dc;
        public LFGcontrol()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = (LFG)DataContext;
            _dc.PropertyChanged += _dc_PropertyChanged;
        }

        private void _dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Refresh")
            {
                root.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Color.FromArgb(0xff, 0x00, 0xaa, 0xff), Color.FromArgb(0x55,0,0xaa,0xff), TimeSpan.FromMilliseconds(500)));
            }
        }

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProxyInterop.SendRequestPartyInfo(_dc.Id);
            ChatWindowViewModel.Instance.LastClickedLfg = _dc;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            label.Foreground = Brushes.White;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            label.Foreground = new SolidColorBrush(Color.FromArgb(0xff,0x00,0xaa,0xff));
        }
    }
}
