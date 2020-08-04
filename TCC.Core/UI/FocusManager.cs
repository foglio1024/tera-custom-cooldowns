using System;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Nostrum.WinAPI;
using TCC.UI.Windows;
using TCC.UI.Windows.Widgets;
using TCC.Utils;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Timer = System.Timers.Timer;

namespace TCC.UI
{
    public static class FocusManager
    {
        private static bool _isForeground;
        private static bool _forceFocused;
        private static bool _disposed;
        private static bool _pauseTopmost;
        private static readonly object _lock = new object();
        private static Screen? _teraScreen;
        private static Timer? _focusTimer;

        // events
        public static event Action<Point, Point, Size> TeraScreenChanged = null!;
        public static event Action ForegroundChanged = null!;
        public static event Action FocusTick = null!;

        // properties
        public static bool ForceFocused
        {
            get => _forceFocused;
            set
            {
                if (_forceFocused == value) return;
                _forceFocused = value;
                ForegroundChanged?.Invoke();
            }
        }
        public static bool IsForeground
        {
            get => _isForeground || ForceFocused;
            private set
            {
                _isForeground = value;
            }
        }

        private static bool IsActive
        {
            get
            {
                if (ForegroundWindow == TeraWindow && TeraWindow != IntPtr.Zero) return true;
                if (ForegroundWindow == MeterWindow && MeterWindow != IntPtr.Zero) return true;
                if (TccWidget.Exists(ForegroundWindow) || TccWindow.Exists(ForegroundWindow)) return true;
                //if (ForegroundWindow == WindowManager.SettingsWindow?.Handle && WindowManager.SettingsWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.SkillConfigWindow?.Handle && WindowManager.SkillConfigWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.LfgListWindow?.Handle && WindowManager.LfgListWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.DashboardWindow?.Handle && WindowManager.DashboardWindow?.Handle != IntPtr.Zero) return true;

                //if (ForegroundWindow == WindowManager.CooldownWindow?.Handle && WindowManager.CooldownWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.CharacterWindow?.Handle && WindowManager.CharacterWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.BossWindow?.Handle && WindowManager.BossWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.BuffWindow?.Handle && WindowManager.BuffWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.FloatingButton?.Handle && WindowManager.FloatingButton?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.CivilUnrestWindow?.Handle && WindowManager.CivilUnrestWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.ClassWindow?.Handle && WindowManager.ClassWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.FlightDurationWindow?.Handle && WindowManager.FlightDurationWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.GroupWindow?.Handle && WindowManager.GroupWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.NotificationArea?.Handle && WindowManager.NotificationArea?.Handle != IntPtr.Zero) return true;
                return false;
            }
        }
        public static bool PauseTopmost
        {
            get => _pauseTopmost;
            set
            {
                Log.CW($"Setting PauseTopmost to {value}");
                _pauseTopmost = value;
            }
        }

        private static IntPtr TeraWindow => User32.FindWindow("LaunchUnrealUWindowsClient", "TERA");
        private static IntPtr MeterWindow => User32.FindWindow("Shinra Meter", null);
        private static IntPtr ForegroundWindow => User32.GetForegroundWindow();

        public static Screen TeraScreen
        {
            get
            {
                lock (_lock)
                {
                    var rect = new User32.RECT();
                    User32.GetWindowRect(TeraWindow, ref rect);
                    var ret = Screen.AllScreens.FirstOrDefault(x => x.Bounds.IntersectsWith(new Rectangle(
                              new Point(rect.Left, rect.Top),
                              new Size(rect.Right - rect.Left, rect.Bottom - rect.Top)))) ?? Screen.PrimaryScreen;
                    if (Equals(_teraScreen, ret)) return _teraScreen;
                    var old = _teraScreen;
                    _teraScreen = ret;

                    if (old != null)
                        TeraScreenChanged?.Invoke(old.Bounds.Location, _teraScreen.Bounds.Location, _teraScreen.Bounds.Size);
                    return _teraScreen;
                }
            }
        }

        public static void Init()
        {
            _focusTimer = new Timer(1000);
            _focusTimer.Elapsed += CheckForegroundWindow;
            _focusTimer.Start();

            WindowManager.DisposeEvent += Dispose;
        }
        public static void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _focusTimer?.Stop();
        }

        public static void SendString(string s)
        {
            if (TeraWindow == IntPtr.Zero) return;
            InputInjector.PasteString(TeraWindow, s);
        }
        public static void SendNewLine()
        {
            if (TeraWindow == IntPtr.Zero) { return; }
            InputInjector.NewLine(TeraWindow);
        }
        public static void MakeUnfocusable(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle | (int)User32.ExtendedWindowStyles.WS_EX_NOACTIVATE);
        }
        public static void UndoUnfocusable(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle & ~(uint)User32.ExtendedWindowStyles.WS_EX_NOACTIVATE);
        }

        public static void HideFromToolBar(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle | (int)User32.ExtendedWindowStyles.WS_EX_TOOLWINDOW);
        }

        public static void MakeClickThru(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle | (int)User32.ExtendedWindowStyles.WS_EX_TRANSPARENT);
        }
        public static void UndoClickThru(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle & ~(uint)User32.ExtendedWindowStyles.WS_EX_TRANSPARENT);
        }
        public static void FocusTera()
        {
            User32.SetForegroundWindow(TeraWindow);
        }

        private static void CheckForegroundWindow(object sender, ElapsedEventArgs e)
        {
            if (_disposed) return;
            if (!PauseTopmost) FocusTick?.Invoke();
            if (IsForeground == IsActive) return;
            IsForeground = IsActive;
            ForegroundChanged?.Invoke();
        }
    }
}
