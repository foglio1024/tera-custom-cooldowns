using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
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

        // window styles
        // ReSharper disable InconsistentNaming
        private const uint WS_EX_TRANSPARENT = 0x20;      //clickthru
        private const uint WS_EX_NOACTIVATE = 0x08000000; //don't focus
        private const uint WS_EX_TOOLWINDOW = 0x00000080; //don't show in alt-tab
        private const int GWL_EXSTYLE = -20;           //set new exStyle
        private const int WM_CHAR = 0x0102;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;
        // ReSharper restore InconsistentNaming

        // events
        public static event Action ForegroundChanged;
        public static event Action FocusTick;

        // properties
        private static Timer FocusTimer { get; set; }

        public static bool ForceFocused
        {
            get => _forceFocused;
            set
            {
                if(_forceFocused == value) return;
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
                //if (ForegroundWindow == WindowManager.SkillConfigWindow?.Handle && WindowManager.SkillConfigWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.LfgListWindow?.Handle && WindowManager.LfgListWindow?.Handle != IntPtr.Zero) return true;
                //if (ForegroundWindow == WindowManager.Dashboard?.Handle && WindowManager.Dashboard?.Handle != IntPtr.Zero) return true;
                return false;
            }
        }

        public static void SendString(string s)
        {
            if (TeraWindow == IntPtr.Zero) { return; }
            PasteString(TeraWindow, s);
        }
        public static void SendNewLine()
        {
            if (TeraWindow == IntPtr.Zero) { return; }
            NewLine(TeraWindow);
        }
        private static void PasteString(IntPtr hWnd, string s)
        {
            Thread.Sleep(100);
            foreach (var character in s)
            {
                if (!PostMessage(hWnd, WM_CHAR, character, 0)) { throw new Win32Exception(); }
                Thread.Sleep(1);
            }
        }
        private static void NewLine(IntPtr hWnd)
        {
            if (!PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0)) { throw new Win32Exception(); }
            Thread.Sleep(1);
            if (!PostMessage(hWnd, WM_KEYUP, VK_RETURN, 0)) { throw new Win32Exception(); }
        }

        private static IntPtr TeraWindow => FindWindow("LaunchUnrealUWindowsClient", "TERA");
        private static IntPtr MeterWindow => FindWindow("Shinra Meter", null);
        private static IntPtr ForegroundWindow => GetForegroundWindow();

        public static Screen TeraScreen
        {
            get
            {
                var rect = new RECT();
                GetWindowRect(TeraWindow, ref rect);
                return Screen.AllScreens.FirstOrDefault(x => x.Bounds.IntersectsWith(new Rectangle(
                    new Point(rect.Left, rect.Top),
                    new Size(rect.Right - rect.Left, rect.Bottom - rect.Top))));
            }
        }
        public static bool PauseTopmost { get; set; }

        public static void MakeUnfocusable(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_NOACTIVATE);
        }
        public static void UndoUnfocusable(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_NOACTIVATE);
        }

        public static void HideFromToolBar(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
        }
        public static void MakeClickThru(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public static void UndoClickThru(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }

        public static void FocusTera()
        {
            SetForegroundWindow(TeraWindow);
        }

        private static void CheckForegroundWindow(object sender, ElapsedEventArgs e)
        {
            if (_disposed) return;
            FocusTick?.Invoke();
            if (IsForeground == IsActive) return;
            IsForeground = IsActive;
            ForegroundChanged?.Invoke();
        }

        public static void Init()
        {
            FocusTimer = new Timer(1000);
            FocusTimer.Elapsed += CheckForegroundWindow;
            FocusTimer.Start();
        }
        public static void Dispose()
        {
            _disposed = true;
            FocusTimer?.Stop();
        }

        // winapi
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern uint SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

    }
}
