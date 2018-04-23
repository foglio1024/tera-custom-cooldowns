using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Parsing;
using TCC.ViewModels;

namespace TCC
{
    public class WindowSettings : TSPropertyChanged
    {
        private double x;
        private double y;
        private double w;
        private double h;
        private bool visible;
        private ClickThruMode clickThruMode;
        private double scale;
        private bool autoDim;
        private double dimOpacity;
        private bool showAlways;
        private bool allowTransparency;
        private bool enabled;

        public event Action NotifyWindowSafeClose;

        public double X
        {
            get => x;
            set
            {
                x = value;
                NPC(nameof(X));
            }
        }
        public double Y
        {
            get => y;
            set
            {
                y = value;
                NPC(nameof(Y));
            }
        }
        public double W
        {
            get => w;
            set
            {
                w = value;
                NPC(nameof(W));
            }
        }
        public double H
        {
            get => h;
            set
            {
                h = value;
                NPC(nameof(H));
            }
        }
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                NPC(nameof(Visible));
            }
        }
        public ClickThruMode ClickThruMode
        {
            get => clickThruMode;
            set
            {
                clickThruMode = value;
                NPC(nameof(ClickThruMode));
            }
        }
        public double Scale
        {
            get => scale;
            set
            {
                if (scale == value) return;
                scale = value;
                NPC(nameof(Scale));
            }
        }
        public bool AutoDim
        {
            get => autoDim;
            set
            {
                autoDim = value;
                NPC(nameof(AutoDim));
            }
        }
        public double DimOpacity
        {
            get => dimOpacity;
            set
            {
                if (dimOpacity == value) return;
                dimOpacity = value;
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
            get => showAlways;
            set
            {
                showAlways = value;
                NPC(nameof(ShowAlways));
            }
        }
        public bool AllowTransparency
        {
            get => allowTransparency;
            set
            {
                allowTransparency = value;
                NPC(nameof(AllowTransparency));
            }
        }
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                if (value == false)
                {
                    if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
                        MessageBoxResult.Cancel) return;
                    //SettingsManager.CooldownWindowSettings.Enabled = value;
                    //Visibility = Visibility.Hidden;
                    Visible = value;
                    enabled = value;
                    //WindowManager.CooldownWindow.CloseWindowSafe();
                    NotifyWindowSafeClose?.Invoke();
                }
                else
                {
                    MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
                    //SettingsManager.CooldownWindowSettings.Enabled = value;
                    //Visibility = Visibility.Visible;
                    Visible = value;
                    enabled = value;

                    App.Restart();
                }
                MessageFactory.Update();
                NPC(nameof(Enabled));
            }
        }

        public WindowSettings(double _x, double _y, double _h, double _w, bool _visible, ClickThruMode _ctm, double _scale, bool _autoDim, double _dimOpacity, bool _showAlways, bool _allowTransparency, bool _enabled)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            visible = _visible;
            clickThruMode = _ctm;
            scale = _scale;
            autoDim = _autoDim;
            dimOpacity = _dimOpacity;
            showAlways = _showAlways;
            allowTransparency = _allowTransparency;
            enabled = _enabled;
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
        private bool lfgOn;

        public List<Tab> Tabs { get; set; }
        public bool LfgOn
        {
            get { return lfgOn; }
            set
            {
                if (lfgOn == value) return;
                lfgOn = value;
                NPC();
            }
        }

        public ChatWindowSettings(double _x, double _y, double _h, double _w, bool _visible, ClickThruMode _ctm, double _scale, bool _autoDim, double _dimOpacity, bool _showAlways, bool _allowTransparency, bool _enabled) : base(_x, _y, _h, _w, _visible, _ctm, _scale, _autoDim, _dimOpacity, _showAlways, _allowTransparency, _enabled)
        {
            Tabs = new List<Tab>();
        }
        public override XElement ToXElement(string name)
        {
            var b = base.ToXElement(name);
            b.Add(SettingsManager.BuildChatTabsXElement(Tabs));
            b.Add(new XElement(nameof(LfgOn), LfgOn));
            return b;
        }
    }
}