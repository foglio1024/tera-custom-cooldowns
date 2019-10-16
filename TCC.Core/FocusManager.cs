using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using FoglioUtils.WinAPI;
using TCC.Utils;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Timer = System.Timers.Timer;

namespace TCC
{
    public static class FocusManager
    {
        private static bool _isForeground;
        private static bool _forceFocused;
        private static bool _disposed;
        private static bool _pauseTopmost;
        private static Screen _teraScreen;
        private static Timer _focusTimer;

        // events
        public static event Action TeraScreenChanged;
        public static event Action ForegroundChanged;
        public static event Action FocusTick;

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
            private set => _isForeground = value;
        }
        private static bool IsActive
        {
            get
            {
                if (ForegroundWindow == TeraWindow && TeraWindow != IntPtr.Zero) return true;
                if (ForegroundWindow == MeterWindow && MeterWindow != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.SettingsWindow?.Handle && WindowManager.SettingsWindow?.Handle != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.SkillConfigWindow?.Handle && WindowManager.SkillConfigWindow?.Handle != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.LfgListWindow?.Handle && WindowManager.LfgListWindow?.Handle != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.DashboardWindow?.Handle && WindowManager.DashboardWindow?.Handle != IntPtr.Zero) return true;
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
                var rect = new User32.RECT();
                User32.GetWindowRect(TeraWindow, ref rect);
                var ret = Screen.AllScreens.FirstOrDefault(x => x.Bounds.IntersectsWith(new Rectangle(
                              new Point(rect.Left, rect.Top),
                              new Size(rect.Right - rect.Left, rect.Bottom - rect.Top)))) ?? Screen.PrimaryScreen;
                if (Equals(_teraScreen, ret)) return _teraScreen;
                _teraScreen = ret;
                TeraScreenChanged?.Invoke();
                return _teraScreen;
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
            var extendedStyle = User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, extendedStyle | User32.WS_EX_NOACTIVATE);
        }
        public static void UndoUnfocusable(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, extendedStyle & ~User32.WS_EX_NOACTIVATE);
        }

        public static void HideFromToolBar(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, extendedStyle | User32.WS_EX_TOOLWINDOW);
        }

        public static void MakeClickThru(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, extendedStyle | User32.WS_EX_TRANSPARENT);
        }
        public static void UndoClickThru(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, extendedStyle & ~User32.WS_EX_TRANSPARENT);
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


        #region WinAPI


        #endregion
    }

    public static class InputInjector
    {
        public static void PasteString(IntPtr hWnd, string s)
        {
            Thread.Sleep(100);
            foreach (var character in s)
            {
                if (!User32.PostMessage(hWnd, User32.WM_CHAR, character, 0)) { throw new Win32Exception(); }
                Thread.Sleep(1);
            }
        }
        public static void NewLine(IntPtr hWnd)
        {
            if (!User32.PostMessage(hWnd, User32.WM_KEYDOWN, User32.VK_RETURN, 0)) { throw new Win32Exception(); }
            Thread.Sleep(1);
            if (!User32.PostMessage(hWnd, User32.WM_KEYUP, User32.VK_RETURN, 0)) { throw new Win32Exception(); }
        }

    }
}
