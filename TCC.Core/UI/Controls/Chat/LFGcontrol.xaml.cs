using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data.Chat;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.UI.Controls.Chat;

//TODO: rework when?
public partial class LfgControl
{
    private Lfg? _dc;
    public LfgControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
        if (_dc == null) return;
        _dc.PropertyChanged -= DC_PropertyChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _dc = (Lfg)DataContext;
        _dc.PropertyChanged += DC_PropertyChanged;
    }

    private void DC_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Refresh")
        {
            //TODO: ewww
            Root.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Color.FromArgb(0xff, 0x00, 0xaa, 0xff), Color.FromArgb(0x55, 0, 0xaa, 0xff), TimeSpan.FromMilliseconds(500))); //TODO: resources
        }
    }

    private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (App.Settings.LfgWindowSettings.Enabled)
        {
            StubInterface.Instance.StubClient.RequestListings(App.Settings.LfgWindowSettings.MinLevel, App.Settings.LfgWindowSettings.MaxLevel); //ProxyOld.RequestLfgList();
            Task.Delay(1000).ContinueWith(_ =>
            {
                if (_dc == null) return;
                WindowManager.ViewModels.LfgVM.Listings.ToList()
                    .ForEach(x => x.IsExpanded = x.LeaderId == _dc.Id);
            });
        }

        if (_dc == null) return;
        ChatManager.Instance.LastClickedLfg = _dc;
        StubInterface.Instance.StubClient.RequestPartyInfo(_dc.Id, _dc.ServerId); //ProxyOld.RequestPartyInfo(_dc.Id);
    }

    //private void UserControl_MouseEnter(object sender, MouseEventArgs e)
    //{
    //    Label.Foreground = Brushes.White;
    //}

    //private void UserControl_MouseLeave(object sender, MouseEventArgs e)
    //{
    //    Label.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xaa, 0xff));
    //}
}