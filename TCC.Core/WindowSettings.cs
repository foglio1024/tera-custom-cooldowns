using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;

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
        private bool _enabled;
        private bool _allowOffScreen;

        public event Action EnabledChanged;
        public event Action ClickThruModeChanged;
        public event Action VisibilityChanged;

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                var cc = SessionManager.CurrentPlayer != null ? SessionManager.CurrentPlayer.Class == Class.None ? Class.Common : SessionManager.CurrentPlayer.Class : Class.Common;
                var old = Positions[cc];
                Positions[cc] = new Point(value, old.Y);
                NPC(nameof(X));
            }
        }
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                var cc = SessionManager.CurrentPlayer != null ? SessionManager.CurrentPlayer.Class == Class.None ? Class.Common : SessionManager.CurrentPlayer.Class : Class.Common;
                var old = Positions[cc];
                Positions[cc] = new Point(old.X, value);

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
                NPC(nameof(Visible));
                VisibilityChanged?.Invoke();
            }
        }
        public ClickThruMode ClickThruMode
        {
            get => _clickThruMode;
            set
            {
                _clickThruMode = value;
                NPC(nameof(ClickThruMode));
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
                NPC(nameof(DimOpacity));
                WindowManager.ForegroundManager.RefreshDim();
            }
        }
        public bool ShowAlways
        {
            get => _showAlways;
            set
            {
                _showAlways = value;
                NPC(nameof(ShowAlways));
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
                NPC(nameof(Enabled));
            }
        }
        public bool AllowOffScreen
        {
            get => _allowOffScreen;
            set
            {
                if (_allowOffScreen == value) return;
                _allowOffScreen = value;
                NPC();
            }
        }

        public Dictionary<Class, Point> Positions { get; set; }

        public WindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen, Dictionary<Class, Point> positions = null)
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
            _enabled = enabled;
            _allowOffScreen = allowOffscreen;
            Positions = new Dictionary<Class, Point>
            {
                {(Class) 0,   positions == null ? new Point(x,y) : new Point(positions[(Class) 0].X, positions[(Class) 0].Y)},
                {(Class) 1,   positions == null ? new Point(x,y) : new Point(positions[(Class) 1].X, positions[(Class) 1].Y)},
                {(Class) 2,   positions == null ? new Point(x,y) : new Point(positions[(Class) 2].X, positions[(Class) 2].Y)},
                {(Class) 3,   positions == null ? new Point(x,y) : new Point(positions[(Class) 3].X, positions[(Class) 3].Y)},
                {(Class) 4,   positions == null ? new Point(x,y) : new Point(positions[(Class) 4].X, positions[(Class) 4].Y)},
                {(Class) 5,   positions == null ? new Point(x,y) : new Point(positions[(Class) 5].X, positions[(Class) 5].Y)},
                {(Class) 6,   positions == null ? new Point(x,y) : new Point(positions[(Class) 6].X, positions[(Class) 6].Y)},
                {(Class) 7,   positions == null ? new Point(x,y) : new Point(positions[(Class) 7].X, positions[(Class) 7].Y)},
                {(Class) 8,   positions == null ? new Point(x,y) : new Point(positions[(Class) 8].X, positions[(Class) 8].Y)},
                {(Class) 9,   positions == null ? new Point(x,y) : new Point(positions[(Class) 9].X, positions[(Class) 9].Y)},
                {(Class) 10,  positions == null ? new Point(x,y) : new Point(positions[(Class) 10].X, positions[(Class) 10].Y)},
                {(Class) 11,  positions == null ? new Point(x,y) : new Point(positions[(Class) 11].X, positions[(Class) 11].Y)},
                {(Class) 12,  positions == null ? new Point(x,y) : new Point(positions[(Class) 12].X, positions[(Class) 12].Y)},
                {(Class) 255, positions == null ? new Point(x,y) : new Point(positions[(Class) 255].X, positions[(Class) 255].Y)}
            };
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
            xe.Add(new XAttribute(nameof(Enabled), Enabled));
            xe.Add(new XAttribute(nameof(AllowOffScreen), AllowOffScreen));
            xe.Add(BuildWindowPositionsXElement());
            return xe;
        }
        private XElement BuildWindowPositionsXElement()
        {
            var ret = new XElement(nameof(Positions));

            foreach (var keyVal in Positions)
            {
                ret.Add(
                    new XElement("Position", new XAttribute("class", keyVal.Key),
                        new XAttribute("X", keyVal.Value.X),
                        new XAttribute("Y", keyVal.Value.Y))
                );
            }
            return ret;
        }

        public void MakePositionsGlobal()
        {
            var currentPos = Positions[SessionManager.CurrentPlayer.Class];
            for (int i = 0; i < 13; i++)
            {
                Positions[(Class) i] = currentPos;
            }

            X = currentPos.X;
            Y = currentPos.Y;
        }
    }

    public class ChatWindowSettings : WindowSettings
    {
        public double BackgroundOpacity { get; set; } = .3;
        public List<Tab> Tabs { get; set; }
        public bool LfgOn { get; set; } = true;

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
            return b;
        }
    }
}