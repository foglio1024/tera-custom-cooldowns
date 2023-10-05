using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;
using Nostrum.WPF;

namespace TCC.UI.Controls;

public class TccTrayIcon
{
    bool _connected;
    readonly NotifyIcon _trayIcon;
    readonly ContextMenu _contextMenu;
    readonly Icon? _defaultIcon;
    readonly Icon? _connectedIcon;

    public bool Connected
    {
        get => _connected;
        set
        {
            if(_connected == value) return;
            _connected = value;

            _trayIcon.Icon = _connected ? _connectedIcon : _defaultIcon;
        }
    }
    public string Text
    {
        get => _trayIcon.Text;
        set => _trayIcon.Text = value;
    }

    public TccTrayIcon()
    {
        _defaultIcon = MiscUtils.GetEmbeddedIcon("resources/tcc_off.ico");
        _connectedIcon = MiscUtils.GetEmbeddedIcon("resources/tcc_on.ico");

        _trayIcon = new NotifyIcon
        {
            Icon = _defaultIcon,
            Visible = true
        };
        _trayIcon.MouseDown += OnMouseDown;
        _trayIcon.MouseDoubleClick += (_, _) => WindowManager.SettingsWindow.ShowWindow();
        _trayIcon.Text = $"{App.AppVersion} - not connected";

        _contextMenu = new ContextMenu();
        _contextMenu.Items.Add(new MenuItem { Header = "Dashboard", Command = new RelayCommand(_ => WindowManager.DashboardWindow.ShowWindow()) });
        _contextMenu.Items.Add(new MenuItem { Header = "Settings", Command = new RelayCommand(_ => WindowManager.SettingsWindow.ShowWindow()) });
        _contextMenu.Items.Add(new MenuItem
        {
            Header = "Close",
            Command = new RelayCommand(_ =>
            {
                _contextMenu.Closed += (_, _) => App.Close();
                _contextMenu.IsOpen = false;
            })
        });

    }

    void OnMouseDown(object? sender, MouseEventArgs e)
    {
        _contextMenu.IsOpen = e.Button switch
        {
            MouseButtons.Right => true,
            MouseButtons.Left => false,
            _ => _contextMenu.IsOpen
        };
    }

    public void Dispose()
    {
        _trayIcon.Dispose();
    }
}