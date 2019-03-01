using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Annotations;
using TCC.Controls;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Settings
{
    public class WindowSettings : TSPropertyChanged
    {
        private double _w;
        private double _h;
        private bool _visible;
        private ClickThruMode _clickThruMode;
        private double _scale;
        private bool _autoDim;
        private double _dimOpacity;
        private bool _showAlways;
        private bool _enabled;
        private bool _allowOffScreen;

        public event Action ResetToCenter;
        public event Action EnabledChanged;
        public event Action ClickThruModeChanged;
        public event Action VisibilityChanged;

        public RelayCommand ResetPositionCommand { get; }

        public string Name { [UsedImplicitly] get; }
        public bool PerClassPosition { get; set; }

        private Class CurrentClass()
        {
            var cc = SessionManager.CurrentPlayer == null || SessionManager.CurrentPlayer?.Class == Class.None ? Class.Common : SessionManager.CurrentPlayer.Class;
            cc = PerClassPosition ? cc : Class.Common;
            return cc;
        }

        public double X
        {
            get
            {
                var cc = CurrentClass();
                return Positions.Position(cc).X;
            }
            set
            {
                var cc = CurrentClass();
                if (cc == Class.None) return;
                var old = Positions.Position(cc);
                if (old.X == value) return;
                Positions.SetPosition(cc, new Point(value, old.Y));
                N(nameof(X));
            }
        }

        public double Y
        {
            get
            {
                var cc = CurrentClass();
                return Positions.Position(cc).Y;
            }
            set
            {
                var cc = CurrentClass();
                if (cc == Class.None) return;
                var old = Positions.Position(cc);
                if (old.Y == value) return;
                Positions.SetPosition(cc, new Point(old.X, value));
                N(nameof(Y));
            }
        }

        public ButtonsPosition ButtonsPosition
        {
            get
            {
                var cc = CurrentClass();
                return Positions.Buttons(cc);
            }
            set
            {
                var cc = CurrentClass();
                if (cc == Class.None) return;
                Positions.SetButtons(cc, value);
                N(nameof(ButtonsPosition));
            }
        }

        public double W
        {
            get => _w;
            set
            {
                _w = value;
                N(nameof(W));
            }
        }
        public double H
        {
            get => _h;
            set
            {
                _h = value;
                N(nameof(H));
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                N(nameof(Visible));
                VisibilityChanged?.Invoke();
            }
        }
        public ClickThruMode ClickThruMode
        {
            get => _clickThruMode;
            set
            {
                _clickThruMode = value;
                N(nameof(ClickThruMode));
                ClickThruModeChanged?.Invoke();
            }
        }
        public double Scale
        {
            get => _scale;
            set
            {
                if (_scale == value) return;
                _scale = value;
                N(nameof(Scale));
            }
        }
        public bool AutoDim
        {
            get => _autoDim;
            set
            {
                _autoDim = value;
                N(nameof(AutoDim));
                WindowManager.ForegroundManager.RefreshDim();
            }
        }
        public double DimOpacity
        {
            get => _dimOpacity;
            set
            {
                if (_dimOpacity == value) return;
                _dimOpacity = value;
                N(nameof(DimOpacity));
                WindowManager.ForegroundManager.RefreshDim();
            }
        }
        public bool ShowAlways
        {
            get => _showAlways;
            set
            {
                _showAlways = value;
                N(nameof(ShowAlways));
                WindowManager.ForegroundManager.RefreshVisible();
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
                //    SafeClosed?.Invoke();
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
                    Visible = true;
                }
                MessageFactory.Update();
                EnabledChanged?.Invoke();
                N(nameof(Enabled));
            }
        }
        public bool AllowOffScreen
        {
            get => _allowOffScreen;
            set
            {
                if (_allowOffScreen == value) return;
                _allowOffScreen = value;
                N();
            }
        }

        public ClassPositions Positions { get; set; }

        public WindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen, ClassPositions positions = null, string name = "", bool perClassPosition = true, ButtonsPosition buttonsPosition = ButtonsPosition.Above)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ResetPositionCommand = new RelayCommand(o => { ResetToCenter?.Invoke(); });
            Name = name;
            _w = w;
            _h = h;
            _visible = visible;
            _clickThruMode = ctm;
            _scale = scale;
            _autoDim = autoDim;
            _dimOpacity = dimOpacity;
            _showAlways = showAlways;
            _enabled = enabled;
            _allowOffScreen = allowOffscreen;
            PerClassPosition = perClassPosition;
            Positions = positions == null ?
                new ClassPositions(x, y, buttonsPosition) :
                new ClassPositions(positions);
        }

        public virtual XElement ToXElement(string name)
        {
            var xe = new XElement("WindowSetting");
            xe.Add(new XAttribute("Name", name));
            xe.Add(new XAttribute(nameof(W), W));
            xe.Add(new XAttribute(nameof(H), H));
            xe.Add(new XAttribute(nameof(Visible), Visible));
            xe.Add(new XAttribute(nameof(ClickThruMode), ClickThruMode));
            xe.Add(new XAttribute(nameof(Scale), Scale));
            xe.Add(new XAttribute(nameof(AutoDim), AutoDim));
            xe.Add(new XAttribute(nameof(DimOpacity), DimOpacity));
            xe.Add(new XAttribute(nameof(ShowAlways), ShowAlways));
            xe.Add(new XAttribute(nameof(Enabled), Enabled));
            xe.Add(new XAttribute(nameof(AllowOffScreen), AllowOffScreen));
            xe.Add(BuildWindowPositionsXElement());
            return xe;
        }
        private XElement BuildWindowPositionsXElement()
        {
            var ret = new XElement(nameof(Positions));

            foreach (Class cl in Enum.GetValues(typeof(Class)))
            {
                ret.Add(
                    new XElement("Position", new XAttribute("class", cl),
                        new XAttribute("X", Positions.Position(cl).X),
                        new XAttribute("Y", Positions.Position(cl).Y),
                        new XAttribute("ButtonsPosition", Positions.Buttons(cl)))
                );
            }

            return ret;
        }

        public void MakePositionsGlobal()
        {
            var currentPos = new Point(X, Y);
            Positions.SetAllPositions(currentPos);
            SettingsWriter.Save();
        }
    }

    public class ChatWindowSettings : WindowSettings
    {
        private bool _fadeOut = true;
        private double _backgroundOpacity = .3;
        private bool _lfgOn = true;

        public event Action FadeoutChanged;
        public event Action OpacityChanged;
        public event Action TimeoutChanged;

        private int _hideTimeout;

        public int HideTimeout
        {
            get { return _hideTimeout; }
            set
            {
                if(_hideTimeout == value) return;
                _hideTimeout = value;
                N();
                TimeoutChanged?.Invoke();
            }
        }

        public double BackgroundOpacity
        {
            get => _backgroundOpacity;
            set
            {
                if(_backgroundOpacity == value) return;
                _backgroundOpacity = value;
                N();
                OpacityChanged?.Invoke();
            }
        }
        public bool FadeOut
        {
            get => _fadeOut;
            set
            {
                if(_fadeOut == value) return;
                _fadeOut = value;
                N();
                FadeoutChanged?.Invoke();
            }
        }

        public List<Tab> Tabs { get; set; }

        public bool LfgOn
        {
            get => _lfgOn;
            set
            {
                if(_lfgOn == value) return;
                _lfgOn = value;
                N();
            }
        }


        public ChatWindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen) : base(x, y, h, w, visible, ctm, scale, autoDim, dimOpacity, showAlways, enabled, allowOffscreen)
        {
            Tabs = new List<Tab>();
        }
        public override XElement ToXElement(string name)
        {
            var b = base.ToXElement(name);
            b.Add(SettingsWriter.BuildChatTabsXElement(Tabs));
            b.Add(new XAttribute(nameof(LfgOn), LfgOn));
            b.Add(new XAttribute(nameof(BackgroundOpacity), BackgroundOpacity));
            b.Add(new XAttribute(nameof(FadeOut), FadeOut));
            b.Add(new XAttribute(nameof(HideTimeout), HideTimeout));
            return b;
        }
    }
}