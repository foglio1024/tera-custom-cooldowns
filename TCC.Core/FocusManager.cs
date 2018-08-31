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


        // window styles
        private const uint WS_EX_TRANSPARENT = 0x20;      //clickthru
        private const uint WS_EX_NOACTIVATE = 0x08000000; //don't focus
        private const uint WS_EX_TOOLWINDOW = 0x00000080; //don't show in alt-tab
        private const int GWL_EXSTYLE = (-20);           //set new exStyle
        private const int WM_CHAR = 0x0102;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;

        // events
        public static event Action ForegroundChanged;

        // properties
        public static Timer FocusTimer { get; private set; }
        public static bool IsForeground { get; private set; }
        public static bool IsActive
        {
            get
            {
                if (ForegroundWindow == TeraWindow && TeraWindow != IntPtr.Zero) return true;
                if (ForegroundWindow == MeterWindow && MeterWindow != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.Settings.Handle && WindowManager.Settings.Handle != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.SkillConfigWindow.Handle && WindowManager.SkillConfigWindow.Handle != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.LfgListWindow.Handle && WindowManager.LfgListWindow.Handle != IntPtr.Zero) return true;
                if (ForegroundWindow == WindowManager.InfoWindow.Handle && WindowManager.InfoWindow.Handle != IntPtr.Zero) return true;
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

        private static IntPtr TeraWindow
        {
            get
            {
                Marshal.GetLastWin32Error();
                var result = FindWindow("LaunchUnrealUWindowsClient", "TERA");
                Marshal.GetLastWin32Error();
                return result;
            }
        }
        private static IntPtr MeterWindow
        {
            get
            {
                Marshal.GetLastWin32Error();
                var result = FindWindow("Shinra Meter", null);
                Marshal.GetLastWin32Error();
                return result;
            }
        }

        public static int TeraScreenIndex => Screen.AllScreens.ToList().IndexOf(TeraScreen);

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
        private static IntPtr ForegroundWindow => GetForegroundWindow();

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

        private static void CheckForegroundWindow(object sender, ElapsedEventArgs e)
        {
            if (IsForeground == IsActive) return;
            IsForeground = IsActive;
            ForegroundChanged?.Invoke();
        }


        // winapi
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern uint SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void Init()
        {
            FocusTimer = new Timer(1000);
            FocusTimer.Elapsed += CheckForegroundWindow;
        }
    }
}
