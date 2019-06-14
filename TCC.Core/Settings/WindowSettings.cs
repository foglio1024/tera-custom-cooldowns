using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using Newtonsoft.Json;
using TCC.Annotations;
using TCC.Controls;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;
using TCC.Windows;
using TeraDataLite;

namespace TCC.Settings
{
    public class WindowSettings : TSPropertyChanged
    {
        protected double _w;
        protected double _h;
        protected bool _visible;
        protected ClickThruMode _clickThruMode;
        protected double _scale;
        protected bool _autoDim;
        protected double _dimOpacity;
        protected bool _showAlways;
        protected bool _enabled;
        protected bool _allowOffScreen;


        public event Action ResetToCenter;
        public event Action EnabledChanged;
        public event Action ClickThruModeChanged;
        public event Action VisibilityChanged;

        [JsonIgnore]
        public string Name { [UsedImplicitly] get; }
        [JsonIgnore]
        public bool ForcedClickable { get; protected set; }
        [JsonIgnore]
        public double X
        {
            get
            {
                var cc = CurrentClass();
                return Positions.Position(cc).X;
            }
            set
            {
                if (value >= int.MaxValue) return;
                var cc = CurrentClass();
                if (cc == Class.None) return;
                var old = Positions.Position(cc);
                if (old.X == value) return;
                Positions.SetPosition(cc, new Point(value, old.Y));
                N(nameof(X));
            }
        }
        [JsonIgnore]
        public double Y
        {
            get
            {
                var cc = CurrentClass();
                return Positions.Position(cc).Y;
            }
            set
            {
                if (value >= int.MaxValue) return;
                var cc = CurrentClass();
                if (cc == Class.None) return;
                var old = Positions.Position(cc);
                if (old.Y == value) return;
                Positions.SetPosition(cc, new Point(old.X, value));
                N(nameof(Y));
            }
        }
        [JsonIgnore]
        public bool IgnoreSize { get; set; } = true;
        [JsonIgnore]
        public bool UndimOnFlyingGuardian { get; set; } = true;
        [JsonIgnore]
        public bool PerClassPosition { get; set; } = true;
        [JsonIgnore]
        public RelayCommand ResetPositionCommand { get; }

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

                if (App.Loading) return;
                PacketAnalyzer.Processor.Update();
                EnabledChanged?.Invoke();
                N(nameof(Enabled));
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
        public ClickThruMode ClickThruMode
        {
            get => ForcedClickable ? ClickThruMode.Never : _clickThruMode;
            set
            {
                _clickThruMode = value;
                N(nameof(ClickThruMode));
                InvokeClickThruModeChanged();
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
        public ClassPositions Positions { get; set; }

        public WindowSettings()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Positions = new ClassPositions();
            ResetPositionCommand = new RelayCommand(o => { ResetToCenter?.Invoke(); });
        }
        public WindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen, ClassPositions positions = null, string name = "", bool perClassPosition = true, ButtonsPosition buttonsPosition = ButtonsPosition.Above) : this()
        {
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

        protected WindowSettings(WindowSettings other) : this()
        {
            _w = other.W;
            _h = other.H;
            _visible = other.Visible;
            _clickThruMode = other._clickThruMode;
            _scale = other.Scale;
            _autoDim = other.AutoDim;
            _dimOpacity = other.DimOpacity;
            _showAlways = other.ShowAlways;
            _enabled = other.Enabled;
            _allowOffScreen = other.AllowOffScreen;
            PerClassPosition = other.PerClassPosition;
            Positions = other.Positions;
            Name = other.Name;


        }

        public virtual XElement ToXElement(string name)
        {
            var xe = new XElement("WindowSetting");
            xe.Add(new XAttribute("Name", name));
            xe.Add(new XAttribute(nameof(W), W));
            xe.Add(new XAttribute(nameof(H), H));
            xe.Add(new XAttribute(nameof(Visible), Visible));
            xe.Add(new XAttribute(nameof(ClickThruMode), _clickThruMode));
            xe.Add(new XAttribute(nameof(Scale), Scale));
            xe.Add(new XAttribute(nameof(AutoDim), AutoDim));
            xe.Add(new XAttribute(nameof(DimOpacity), DimOpacity));
            xe.Add(new XAttribute(nameof(ShowAlways), ShowAlways));
            xe.Add(new XAttribute(nameof(Enabled), Enabled));
            xe.Add(new XAttribute(nameof(AllowOffScreen), AllowOffScreen));
            xe.Add(BuildWindowPositionsXElement());
            return xe;
        }
        protected XElement BuildWindowPositionsXElement()
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
            App.Settings.Save();
        }

        protected void InvokeClickThruModeChanged() => ClickThruModeChanged?.Invoke();

        public void ApplyScreenCorrection(Size sc)
        {
            Positions.ApplyCorrection(sc);
        }
        protected Class CurrentClass()
        {
            var cc = Session.Me == null || Session.Me?.Class == Class.None ? Class.Common : Session.Me.Class;
            cc = PerClassPosition ? cc : Class.Common;
            return cc;
        }

    }

    public class ChatWindowSettings : WindowSettings
    {
        private bool _fadeOut = true;
        private double _backgroundOpacity = .3;
        private double _frameOpacity = 1;
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
                if (_hideTimeout == value) return;
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
                if (_backgroundOpacity == value) return;
                _backgroundOpacity = value;
                N();
                OpacityChanged?.Invoke();
            }
        }
        public double FrameOpacity
        {
            get => _frameOpacity;
            set
            {
                if (_frameOpacity == value) return;
                _frameOpacity = value;
                N();
                OpacityChanged?.Invoke();
            }
        }
        public bool FadeOut
        {
            get => _fadeOut;
            set
            {
                if (_fadeOut == value) return;
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
                if (_lfgOn == value) return;
                _lfgOn = value;
                N();
            }
        }

        public ChatWindowSettings()
        {
            Tabs = new List<Tab>();
            PerClassPosition = false;
        }
        public ChatWindowSettings(WindowSettings other) : base(other)
        {
            Tabs = new List<Tab>();
            PerClassPosition = false;

        }
        public ChatWindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen) : base(x, y, h, w, visible, ctm, scale, autoDim, dimOpacity, showAlways, enabled, allowOffscreen)
        {
            Tabs = new List<Tab>();
            PerClassPosition = false;

        }
        public override XElement ToXElement(string name)
        {
            var b = base.ToXElement(name);
            b.Add(XmlSettingsWriter.BuildChatTabsXElement(Tabs));
            b.Add(new XAttribute(nameof(LfgOn), LfgOn));
            b.Add(new XAttribute(nameof(BackgroundOpacity), BackgroundOpacity));
            b.Add(new XAttribute(nameof(FrameOpacity), FrameOpacity));
            b.Add(new XAttribute(nameof(FadeOut), FadeOut));
            b.Add(new XAttribute(nameof(HideTimeout), HideTimeout));
            return b;
        }

        public void ForceToggleClickThru()
        {
            ForcedClickable = !ForcedClickable;
            InvokeClickThruModeChanged();
        }
    }

    public class CooldownWindowSettings : WindowSettings
    {
        public bool ShowItems { get; set; }
        public CooldownBarMode Mode { get; set; }

        public CooldownWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.4, .7, ButtonsPosition.Above);

            Mode = CooldownBarMode.Fixed;
            ShowItems = true;
            UndimOnFlyingGuardian = false;
        }
    }

    public class CharacterWindowSettings : WindowSettings
    {
        public bool CompactMode { get; set; }

        public CharacterWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.4, 1, ButtonsPosition.Above);

            CompactMode = true;
            UndimOnFlyingGuardian = false;
        }
    }
    public class NpcWindowSettings : WindowSettings
    {
        public bool ShowOnlyBosses { get; set; }
        public bool AccurateHp { get; set; }
        public EnrageLabelMode EnrageLabelMode { get; set; }

        public NpcWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.4, 0, ButtonsPosition.Above);

            EnrageLabelMode = EnrageLabelMode.Remaining;
            AccurateHp = true;
            ShowOnlyBosses = false;
        }
    }

    public class BuffWindowSettings : WindowSettings
    {
        public bool ShowAll { get; set; } // by HQ
        public FlowDirection Direction { get; set; }
        public Dictionary<Class, List<uint>> MyAbnormals { get; } // by HQ

        public BuffWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(1, .7, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;

            Direction = FlowDirection.RightToLeft;
            ShowAll = true;
            MyAbnormals = new Dictionary<Class, List<uint>>()
            {
                {       0, new List<uint>{ 100800, 100801 }},
                {(Class)1, new List<uint>{ 200230, 200231, 200232, 201701 }},
                {(Class)2, new List<uint>{ 300800, 300801, 300805 }},
                {(Class)3, new List<uint>{ 401705, 401706, 401710, 400500, 400501, 400508, 400710, 400711 }},
                {(Class)4, new List<uint>{ 21170, 22120, 23180, 26250, 29011, 25170, 25171, 25201, 25202, 500100, 500150, 501600, 501650, 502001, 502051, 502070, 502071, 502072 }},
                {(Class)5, new List<uint>{ 601400, 601450, 601460, 88608101, 88608102, 88608103, 88608104, 88608105, 88608106, 88608107, 88608108, 88608109, 88608110,602101,602102,602103,601611 }},
                {(Class)6, new List<uint>()},
                {(Class)7, new List<uint>()},
                {(Class)8, new List<uint>{ 10151010, 10151131, 10151192 }},
                {(Class)9, new List<uint>{ 89105101, 89105102, 89105103, 89105104, 89105105, 89105106, 89105107, 89105108, 89105109, 89105110, 89105111, 89105112, 89105113, 89105114, 89105115, 89105116, 89105117, 89105118, 89105119, 89105120, 10152340, 10152351 }},
                {(Class)10, new List<uint>{ 31020, 10153210 }},
                {(Class)11, new List<uint>{ 89314201, 89314202, 89314203, 89314204, 89314205, 89314206, 89314207, 89314208, 89314209, 89314210, 89314211, 89314212, 89314213, 89314214, 89314215, 89314216, 89314217, 89314218, 89314219, 89314220, 10154480, 10154450 }},
                {(Class)12, new List<uint>{ 10155130, 10155551, 10155510, 10155512, 10155540, 10155541, 10155542 }},
                {(Class)255, new List<uint>{ 6001, 6002, 6003, 6004, 6012, 6013, 702004, 805800, 805803, 200700, 200701, 200731, 800300, 800301, 800302, 800303, 800304, 702001 }},
            };

        }
    }
    public class ClassWindowSettings : WindowSettings
    {
        public bool WarriorShowTraverseCut { get; set; }
        public bool WarriorShowEdge { get; set; }
        public WarriorEdgeMode WarriorEdgeMode { get; set; }
        public bool SorcererReplacesElementsInCharWindow { get; set; }

        public ClassWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.25, .6, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;

            WarriorShowTraverseCut = true;
            WarriorShowEdge = true;
            WarriorEdgeMode = WarriorEdgeMode.Rhomb;
            SorcererReplacesElementsInCharWindow = true;

        }
    }
    public class GroupWindowSettings : WindowSettings
    {
        public GroupWindowLayout Layout { get; set; }
        public bool IgnoreMe { get; set; }
        public bool ShowOnlyAggroStacks { get; set; }
        public bool ShowHpLabels { get; set; }
        public bool ShowLaurels { get; set; }
        public bool ShowAllAbnormalities { get; set; }
        public bool ShowDetails { get; set; }
        public bool ShowAwakenIcon { get; set; }
        public uint GroupSizeThreshold { get; set; }
        public uint HideBuffsThreshold { get; set; }
        public uint HideDebuffsThreshold { get; set; }
        public uint DisableAbnormalitiesThreshold { get; set; }
        public uint HideHpThreshold { get; set; }
        public uint HideMpThreshold { get; set; }
        public Dictionary<Class, List<uint>> GroupAbnormals { get; }

        public GroupWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(0, 0, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;
            IgnoreSize = false;

            GroupSizeThreshold = 7;
            HideBuffsThreshold = 7;
            HideDebuffsThreshold = 7;
            DisableAbnormalitiesThreshold = 7;
            HideHpThreshold = 7;
            HideMpThreshold = 7;
            ShowOnlyAggroStacks = true;
            ShowDetails = true;
            ShowAwakenIcon = true;
            ShowHpLabels = true;
            Layout = GroupWindowLayout.RoleSeparated;
            GroupAbnormals = new Dictionary<Class, List<uint>>()
            {
                {       0, new List<uint>()},
                {(Class)1, new List<uint>()},
                {(Class)2, new List<uint>()},
                {(Class)3, new List<uint>()},
                {(Class)4, new List<uint>()},
                {(Class)5, new List<uint>()},
                {(Class)6, new List<uint>()},
                {(Class)7, new List<uint>()},
                {(Class)8, new List<uint>()},
                {(Class)9, new List<uint>()},
                {(Class)10, new List<uint>()},
                {(Class)11, new List<uint>()},
                {(Class)12, new List<uint>()},
                {(Class)255, new List<uint>()},
            };

        }
    }
    public class FlightWindowSettings : WindowSettings
    {
        public bool Flip { get; set; }
        public double Rotation { get; set; }

        public FlightWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = false;
            _dimOpacity = 1;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.5, .5, ButtonsPosition.Above);

            PerClassPosition = false;

            Rotation = 0;
            Flip = false;
        }
    }
    public class FloatingButtonWindowSettings : WindowSettings
    {
        public bool ShowNotificationBubble { get; set; }

        public FloatingButtonWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = false;
            _dimOpacity = 1;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = true;
            Positions = new ClassPositions(0, 0, ButtonsPosition.Above);

            PerClassPosition = false;

            ShowNotificationBubble = true;

        }
    }
    public class CivilUnrestWindowSettings : WindowSettings
    {
        public CivilUnrestWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(1, .45, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;

        }
    }
}