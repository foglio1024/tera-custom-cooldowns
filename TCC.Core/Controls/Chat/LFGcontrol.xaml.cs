using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data.Chat;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.Controls.Chat
{
    //TODO: rework when?
    public partial class LFGcontrol
    {
        private LFG _dc;
        public LFGcontrol()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            _dc.PropertyChanged -= _dc_PropertyChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (LFG)DataContext;
            _dc.PropertyChanged += _dc_PropertyChanged;
        }

        private void _dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Refresh")
            {
                Root.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Color.FromArgb(0xff, 0x00, 0xaa, 0xff), Color.FromArgb(0x55,0,0xaa,0xff), TimeSpan.FromMilliseconds(500))); //TODO: resources
            }
        }

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (App.Settings.LfgWindowSettings.Enabled)
            {
                ProxyInterface.Instance.Stub.RequestListings(); //ProxyOld.RequestLfgList();
                Task.Delay(1000).ContinueWith(t => 
                WindowManager.ViewModels.LfgVM.Listings.ToList().ForEach(x => x.IsExpanded = x.LeaderId == _dc.Id)
                    );
            }
            ChatWindowManager.Instance.LastClickedLfg = _dc;
            ProxyInterface.Instance.Stub.RequestPartyInfo(_dc.Id); //ProxyOld.RequestPartyInfo(_dc.Id);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Label.Foreground = Brushes.White;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Label.Foreground = new SolidColorBrush(Color.FromArgb(0xff,0x00,0xaa,0xff));
        }
    }
}
