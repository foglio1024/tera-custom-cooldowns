using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using FoglioUtils;
using Newtonsoft.Json;
using TCC.Annotations;
using TCC.Controls;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Utilities;
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
        public event Action<bool> EnabledChanged;
        public event Action ClickThruModeChanged;
        public event Action<bool> VisibilityChanged;

        [JsonIgnore]
        public string Name { [UsedImplicitly] get; }
        [JsonIgnore]
        public bool ForcedClickable { get; protected set; }
        [JsonIgnore]
        public double X
        {
            get => Positions.Position(!PerClassPosition ? Class.Common : CurrentClass()).X;
            set
            {
                if (value >= int.MaxValue) return;
                var cc = CurrentClass();
                if (cc == Class.None || !PerClassPosition) cc = Class.Common;
                var old = Positions.Position(cc);
                if (old.X == value) return;
                Positions.SetPosition(cc, new Point(value, old.Y));
                N(nameof(X));
            }
        }
        [JsonIgnore]
        public double Y
        {
            get => Positions.Position(!PerClassPosition ? Class.Common : CurrentClass()).Y;
            set
            {
                if (value >= int.MaxValue) return;
                var cc = CurrentClass();
                if (cc == Class.None || !PerClassPosition) cc = Class.Common;
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
                EnabledChanged?.Invoke(_enabled);
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
                VisibilityChanged?.Invoke(_visible);
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
            Log.CW($"[{GetType().Name}] {nameof(MakePositionsGlobal)}()");

            var currentPos = new Point(X, Y);
            Positions.SetAllPositions(currentPos);
            App.Settings.Save();
        }

        protected void InvokeClickThruModeChanged() => ClickThruModeChanged?.Invoke();

        public void ApplyScreenCorrection(Size sc)
        {
            Log.CW($"[{GetType().Name}] {nameof(ApplyScreenCorrection)}({sc})");

            Positions.ApplyCorrection(sc);
        }
        protected Class CurrentClass()
        {
            var cc = Game.Me == null || Game.Me?.Class == Class.None ? Class.Common : Game.Me.Class;
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
        private int _hideTimeout;
        private bool _canCollapse = true;
        private bool _staysCollapsed;

        public event Action FadeoutChanged;
        public event Action OpacityChanged;
        public event Action TimeoutChanged;
        public event Action CanCollapseChanged;
        public event Action StaysCollapsedChanged;


        public int HideTimeout
        {
            get => _hideTimeout;
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
        public bool CanCollapse
        {
            get => _canCollapse;
            set
            {
                if (_canCollapse == value) return;
                _canCollapse = value;
                N();
                if (!_canCollapse) StaysCollapsed = false;
                CanCollapseChanged?.Invoke();
            }
        }
        public bool StaysCollapsed
        {
            get => _staysCollapsed;
            set
            {
                if (_staysCollapsed== value) return;
                _staysCollapsed= value;
                N();
                StaysCollapsedChanged?.Invoke();
            }
        }

        public List<TabData> Tabs { get; set; }


        public ChatWindowSettings()
        {
            Tabs = new List<TabData>();
            PerClassPosition = false;
            IgnoreSize = false;
        }
        public ChatWindowSettings(WindowSettings other) : base(other)
        {
            Tabs = new List<TabData>();
            PerClassPosition = false;

        }
        public ChatWindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen) : base(x, y, h, w, visible, ctm, scale, autoDim, dimOpacity, showAlways, enabled, allowOffscreen)
        {
            Tabs = new List<TabData>();
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
        public event Action ShowItemsChanged;
        public event Action ModeChanged;

        private bool _showItems;
        private CooldownBarMode _mode;

        public bool ShowItems
        {
            get => _showItems;
            set
            {
                if (_showItems == value) return;
                _showItems = value;
                N();
                ShowItemsChanged?.Invoke();
            }
        }

        public CooldownBarMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                N();
                ModeChanged?.Invoke();
            }
        }

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
    public class NotificationAreaSettings : WindowSettings
    {
        public int MaxNotifications { get; set; }
        public NotificationAreaSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = false;
            _dimOpacity = 1;
            _showAlways = true;
            _enabled = true;
            _allowOffScreen = false;
            PerClassPosition = false;
            Positions = new ClassPositions(0, .5, ButtonsPosition.Above);

            MaxNotifications = 5;
        }
    }
    public class CharacterWindowSettings : WindowSettings
    {
        public event Action CompactModeChanged;

        private bool _compactMode;
        public bool CompactMode
        {
            get => _compactMode;
            set
            {
                if (_compactMode == value) return;
                _compactMode = value;
                N();
                CompactModeChanged?.Invoke();
            }
        }

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
        private bool _accurateHp;
        private bool _hideAdds;
        private EnrageLabelMode _enrageLabelMode;

        public event Action AccurateHpChanged;
        public event Action HideAddsChanged;

        public bool HideAdds
        {
            get => _hideAdds;
            set
            {
                if (_hideAdds == value) return;
                _hideAdds = value;
                N();
                HideAddsChanged?.Invoke();
            }
        }

        public bool AccurateHp
        {
            get => _accurateHp;
            set
            {
                if (_accurateHp == value) return;
                _accurateHp = value;
                N();
                AccurateHpChanged?.Invoke();
            }
        }

        public EnrageLabelMode EnrageLabelMode
        {
            get => _enrageLabelMode;
            set
            {
                if (_enrageLabelMode == value) return;
                _enrageLabelMode = value;
                N();
            }
        }

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
            HideAdds = false;
        }
    }
    public class BuffWindowSettings : WindowSettings
    {
        public event Action DirectionChanged;

        private FlowDirection _direction;
        public bool ShowAll { get; set; } // by HQ

        public FlowDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction == value) return;
                _direction = value;
                N();
                DirectionChanged?.Invoke();
            }
        }

        public Dictionary<Class, List<uint>> MyAbnormals { get; } // by HQ
        public bool Pass(Abnormality ab) // by HQ
        {
            if (ShowAll) return true;
            if (MyAbnormals.TryGetValue(Class.Common, out var commonList))
            {
                if (commonList.Contains(ab.Id)) return true;
                if (MyAbnormals.TryGetValue(Game.Me.Class, out var classList))
                {
                    if (!classList.Contains(ab.Id)) return false;
                }
                else return false;
            }
            else return false;

            return true;
        }

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
        public event Action WarriorShowEdgeChanged;
        public event Action WarriorEdgeModeChanged;
        public event Action WarriorShowTraverseCutChanged;
        public event Action SorcererReplacesElementsInCharWindowChanged;

        private bool _warriorShowEdge;
        private bool _sorcererReplacesElementsInCharWindow;
        private bool _warriorShowTraverseCut;
        private WarriorEdgeMode _warriorEdgeMode;

        public bool WarriorShowEdge
        {
            get => _warriorShowEdge;
            set
            {
                if (_warriorShowEdge == value) return;
                _warriorShowEdge = value;
                N();
                WarriorShowEdgeChanged?.Invoke();
            }
        }
        public bool SorcererReplacesElementsInCharWindow
        {
            get => _sorcererReplacesElementsInCharWindow;
            set
            {
                if (_sorcererReplacesElementsInCharWindow == value) return;
                _sorcererReplacesElementsInCharWindow = value;
                N();
                SorcererReplacesElementsInCharWindowChanged?.Invoke();
            }
        }
        public bool WarriorShowTraverseCut
        {
            get => _warriorShowTraverseCut;
            set
            {
                if (_warriorShowTraverseCut == value) return;
                _warriorShowTraverseCut = value;
                N();
                WarriorShowTraverseCutChanged?.Invoke();

            }
        }
        public WarriorEdgeMode WarriorEdgeMode
        {
            get => _warriorEdgeMode;
            set
            {
                if (_warriorEdgeMode == value) return;
                _warriorEdgeMode = value;
                N();
                WarriorEdgeModeChanged?.Invoke();
            }
        }

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
        public event Action SettingsUpdated;
        public event Action IgnoreMeChanged;
        public event Action ThresholdChanged;
        public event Action LayoutChanged;

        private bool _showHpLabels;
        private bool _ignoreMe;
        private uint _hideBuffsThreshold;
        private uint _hideDebuffsThreshold;
        private uint _hideHpThreshold;
        private uint _hideMpThreshold;
        private uint _disableAbnormalitiesThreshold;
        private uint _groupSizeThreshold;
        private GroupWindowLayout _layout;
        private bool _showAwakenIcon;
        private bool _showDetails;
        private bool _showLaurels;
        private bool _showOnlyAggroStacks;

        public bool ShowHpLabels
        {
            get => _showHpLabels;
            set
            {
                if (_showHpLabels == value) return;
                _showHpLabels = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public bool IgnoreMe
        {
            get => _ignoreMe;
            set
            {
                if (_ignoreMe == value) return;
                _ignoreMe = value;
                N();
                IgnoreMeChanged?.Invoke();
            }
        }
        public uint HideBuffsThreshold
        {
            get => _hideBuffsThreshold;
            set
            {
                if (_hideBuffsThreshold == value) return;
                _hideBuffsThreshold = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public uint HideDebuffsThreshold
        {
            get => _hideDebuffsThreshold;
            set
            {
                if (_hideDebuffsThreshold == value) return;
                _hideDebuffsThreshold = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public uint HideHpThreshold
        {
            get => _hideHpThreshold;
            set
            {
                if (_hideHpThreshold == value) return;
                _hideHpThreshold = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public uint HideMpThreshold
        {
            get => _hideMpThreshold;
            set
            {
                if (_hideMpThreshold == value) return;
                _hideMpThreshold = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public uint DisableAbnormalitiesThreshold
        {
            get => _disableAbnormalitiesThreshold;
            set
            {
                if (_disableAbnormalitiesThreshold == value) return;
                _disableAbnormalitiesThreshold = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public uint GroupSizeThreshold
        {
            get => _groupSizeThreshold;
            set
            {
                if (_groupSizeThreshold == value) return;
                _groupSizeThreshold = value;
                N();
                ThresholdChanged?.Invoke();
            }
        }
        public GroupWindowLayout Layout
        {
            get => _layout;
            set
            {
                if (_layout == value) return;
                _layout = value;
                LayoutChanged?.Invoke();
                N();
            }
        }
        public bool ShowAwakenIcon
        {
            get => _showAwakenIcon;
            set
            {
                if (_showAwakenIcon == value) return;
                _showAwakenIcon = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public bool ShowDetails
        {
            get => _showDetails;
            set
            {
                if (_showDetails == value) return;
                _showDetails = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public bool ShowLaurels
        {
            get => _showLaurels;
            set
            {
                if (_showLaurels == value) return;
                _showLaurels = value;
                N();
                SettingsUpdated?.Invoke();
            }
        }
        public bool ShowOnlyAggroStacks
        {
            get => _showOnlyAggroStacks;
            set
            {
                if (_showOnlyAggroStacks == value) return;
                _showOnlyAggroStacks = value;
                N();
            }
        }



        public bool ShowAllAbnormalities { get; set; }

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
        public event Action RotationChanged;
        public event Action FlipChanged;

        private bool _flip;
        private double _rotation;

        public bool Flip
        {
            get => _flip;
            set
            {
                if (_flip == value) return;
                _flip = value;
                N();
                FlipChanged?.Invoke();
            }
        }

        public double Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value) return;
                _rotation = value;
                N();
                RotationChanged?.Invoke();
            }
        }

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
        private bool _showNotificationBubble;

        public bool ShowNotificationBubble
        {
            get => _showNotificationBubble;
            set
            {
                if (_showNotificationBubble == value) return;
                _showNotificationBubble = value;
                N();
            }
        }

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
    public class LfgWindowSettings : WindowSettings
    {
        private bool _hideTradeListings;
        public event Action HideTradeListingsChangedEvent;

        public bool HideTradeListings
        {
            get => _hideTradeListings;
            set
            {
                if (_hideTradeListings == value) return;
                _hideTradeListings = value;
                N();
                HideTradeListingsChangedEvent?.Invoke();
            }
        }

        public LfgWindowSettings()
        {
            HideTradeListings = true;
        }
    }
}