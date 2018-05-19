using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Interop;

namespace TCC
{

    public static class FocusManager
    {
        private const uint WS_EX_TRANSPARENT = 0x20;      //clickthru
        private const uint WS_EX_NOACTIVATE = 0x08000000; //don't focus
        private const uint WS_EX_TOOLWINDOW = 0x00000080; //don't show in alt-tab
        private const int GWL_EXSTYLE = (-20);           //set new exStyle
        public const int WM_CHAR = 0x0102;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;

        public static event Action ForegroundChanged;

        public static System.Timers.Timer FocusTimer;
        public static bool Running { get; set; } = true;
        public static bool IsForeground { get; set; }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern uint SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        public static IntPtr FindTeraWindow()
        {
            Marshal.GetLastWin32Error();
            var result = FindWindow("LaunchUnrealUWindowsClient", "TERA");
            Marshal.GetLastWin32Error();
            return result;
        }
        public static IntPtr FindMeterWindow()
        {
            Marshal.GetLastWin32Error();
            var result = FindWindow("Shinra Meter", null);
            Marshal.GetLastWin32Error();
            return result;
        }

        private static bool _isActive;
        public static bool IsActive()
        {
            //TODO: add TCC windows here
            if (GetForegroundWindow() == FindTeraWindow() && FindTeraWindow() != IntPtr.Zero) return true;
            if (GetForegroundWindow() == FindMeterWindow() && FindMeterWindow() != IntPtr.Zero) return true;
            if (GetForegroundWindow() == WindowManager.Settings.Handle && WindowManager.Settings.Handle != IntPtr.Zero) return true;
            if (GetForegroundWindow() == WindowManager.SkillConfigWindow.Handle && WindowManager.SkillConfigWindow.Handle != IntPtr.Zero) return true;
            return false;
        }

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
        public static void CheckForegroundWindow(object sender, ElapsedEventArgs e)
        {

            //Console.WriteLine($"[Focus manager] IsActive() = {IsActive()}");
            //WindowManager.IsFocused = IsActive();
            if (IsForeground != IsActive())
            {
                IsForeground = IsActive();
                ForegroundChanged?.Invoke();
            }

        }

        public static void NewLine(IntPtr hWnd)
        {
            if (!PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0)) { throw new Win32Exception(); }
            Thread.Sleep(1);
            if (!PostMessage(hWnd, WM_KEYUP, VK_RETURN, 0)) { throw new Win32Exception(); }
            //Thread.Sleep(50);
            //if (!PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0)) { throw new Win32Exception(); }
            //Thread.Sleep(1);
            //if (!PostMessage(hWnd, WM_KEYUP, VK_RETURN, 0)) { throw new Win32Exception(); }
        }

    }
}
