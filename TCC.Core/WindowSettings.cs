using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC
{
    public class WindowSettings : TSPropertyChanged
    {
        private double _x;
        private double _y;
        private double _w;
        private double _h;
        private bool _visible;
        private ClickThruMode _clickThruMode;
        private double _scale;
        private bool _autoDim;
        private double _dimOpacity;
        private bool _showAlways;
        private bool _allowTransparency;
        private bool _enabled;

        public event Action NotifyEnableWindow;
        public event Action NotifyWindowSafeClose;

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                NPC(nameof(X));
            }
        }
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                NPC(nameof(Y));
            }
        }
        public double W
        {
            get => _w;
            set
            {
                _w = value;
                NPC(nameof(W));
            }
        }
        public double H
        {
            get => _h;
            set
            {
                _h = value;
                NPC(nameof(H));
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                //Console.WriteLine($"Visible changed to {value}");
                NPC(nameof(Visible));
            }
        }
        public ClickThruMode ClickThruMode
        {
            get => _clickThruMode;
            set
            {
                _clickThruMode = value;
                NPC(nameof(ClickThruMode));
            }
        }
        public double Scale
        {
            get => _scale;
            set
            {
                if (_scale == value) return;
                _scale = value;
                NPC(nameof(Scale));
            }
        }
        public bool AutoDim
        {
            get => _autoDim;
            set
            {
                _autoDim = value;
                NPC(nameof(AutoDim));
            }
        }
        public double DimOpacity
        {
            get => _dimOpacity;
            set
            {
                if (_dimOpacity == value) return;
                _dimOpacity = value;
                //if (WindowManager.IsTccDim)
                //{
                //    WindowManager.SkillsEnded = false;
                //    WindowManager.SkillsEnded = true;
                //}
                NPC(nameof(DimOpacity));
                WindowManager.RefreshDim();
            }
        }
        public bool ShowAlways
        {
            get => _showAlways;
            set
            {
                _showAlways = value;
                NPC(nameof(ShowAlways));
            }
        }
        public bool AllowTransparency
        {
            get => _allowTransparency;
            set
            {
                _allowTransparency = value;
                NPC(nameof(AllowTransparency));
            }
        }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;
                //if (value == false && TccMessageBox.Show("TCC",
                //        "Re-enabling this later will require TCC restart.\nDo you want to continue?",
                //        MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
                //    MessageBoxResult.Cancel)
                //{
                //    return;
                //}
                //else if (value == false)
                //{
                //    Visible = false;
                //    _enabled = false;
                //    NotifyWindowSafeClose?.Invoke();
                //}
                //else
                //{
                //    TccMessageBox.Show("TCC", "TCC will now be restarted.", MessageBoxButton.OK,
                //        MessageBoxImage.Information);
                //    Visible = true;
                //    _enabled = true;
                //    App.Restart();
                //}
                _enabled = value;
                if (_enabled)
                {
                    NotifyEnableWindow?.Invoke();
                    Visible = true;
                }
                else
                {
                    NotifyWindowSafeClose?.Invoke();
                }
                MessageFactory.Update();
                NPC(nameof(Enabled));
            }
        }

        public WindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool allowTransparency, bool enabled)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _x = x;
            _y = y;
            _w = w;
            _h = h;
            _visible = visible;
            _clickThruMode = ctm;
            _scale = scale;
            _autoDim = autoDim;
            _dimOpacity = dimOpacity;
            _showAlways = showAlways;
            _allowTransparency = allowTransparency;
            _enabled = enabled;
        }

        public virtual XElement ToXElement(string name)
        {
            var xe = new XElement("WindowSetting");
            xe.Add(new XAttribute("Name", name));
            xe.Add(new XAttribute(nameof(X), X));
            xe.Add(new XAttribute(nameof(Y), Y));
            xe.Add(new XAttribute(nameof(W), W));
            xe.Add(new XAttribute(nameof(H), H));
            xe.Add(new XAttribute(nameof(Visible), Visible));
            xe.Add(new XAttribute(nameof(ClickThruMode), ClickThruMode));
            xe.Add(new XAttribute(nameof(Scale), Scale));
            xe.Add(new XAttribute(nameof(AutoDim), AutoDim));
            xe.Add(new XAttribute(nameof(DimOpacity), DimOpacity));
            xe.Add(new XAttribute(nameof(ShowAlways), ShowAlways));
            xe.Add(new XAttribute(nameof(AllowTransparency), AllowTransparency));
            xe.Add(new XAttribute(nameof(Enabled), Enabled));
            return xe;
        }
    }

    public class ChatWindowSettings : WindowSettings
    {
        public double BackgroundOpacity { get; set; } = .3;
        public List<Tab> Tabs { get; set; }
        public bool LfgOn { get; set; } = true;
        public new bool Enabled
        {
            get => SettingsManager.ChatEnabled;
            set
            {
                if (SettingsManager.ChatEnabled == value) return;
                SettingsManager.ChatEnabled = value;
                if(!value) ChatWindowManager.Instance.CloseAllWindows();
                else ChatWindowManager.Instance.InitWindows();
                NPC();
            }
        }

        public ChatWindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool allowTransparency, bool enabled) : base(x, y, h, w, visible, ctm, scale, autoDim, dimOpacity, showAlways, allowTransparency, enabled)
        {
            Tabs = new List<Tab>();
        }
        public override XElement ToXElement(string name)
        {
            var b = base.ToXElement(name);
            b.Add(SettingsManager.BuildChatTabsXElement(Tabs));
            b.Add(new XAttribute(nameof(LfgOn), LfgOn));
            b.Add(new XAttribute(nameof(BackgroundOpacity), BackgroundOpacity));
            return b;
        }
    }
}