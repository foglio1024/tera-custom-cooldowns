using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace TCC
{
    public delegate void ForegroundWindowChangedEventHandler(bool visible);

    public static class FocusManager
    {
        const uint WS_EX_TRANSPARENT = 0x20;      //clickthru
        const uint WS_EX_NOACTIVATE = 0x08000000; //don't focus
        const uint WS_EX_TOOLWINDOW = 0x00000080; //don't show in alt-tab
        const int GWL_EXSTYLE = (-20);           //set new exStyle
        public const int WM_CHAR = 0x0102;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;

        public static System.Timers.Timer FocusTimer;
        public static bool Running { get; set; } = true;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern uint SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        public static IntPtr FindTeraWindow()
        {
            Marshal.GetLastWin32Error();
            var result = FindWindow("LaunchUnrealUWindowsClient", "TERA");
            Marshal.GetLastWin32Error();
            return result;
        }

        public static IntPtr settingsWindowHandle = IntPtr.Zero;

        public static bool IsActive()
        {
            var teraWindow = FindTeraWindow();
            var activeWindow = GetForegroundWindow();
            return teraWindow != IntPtr.Zero && (teraWindow == activeWindow || settingsWindowHandle == activeWindow);
        }

        public static void MakeUnfocusable(IntPtr hwnd)
        {
            uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_NOACTIVATE);
        }
        public static void HideFromToolBar(IntPtr hwnd)
        {
            uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
        }
        public static void MakeTransparent(IntPtr hwnd)
        {
            uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public static void UndoTransparent(IntPtr hwnd)
        {
            uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }
        public static void CheckForegroundWindow(object sender, ElapsedEventArgs e)
        {

            WindowManager.IsFocused = IsActive();
        }

        public static void NewLine(IntPtr hWnd)
        {
            if (!PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0)) { throw new Win32Exception(); }
            Thread.Sleep(1);
            if (!PostMessage(hWnd, WM_KEYUP, VK_RETURN, 0)) { throw new Win32Exception(); }
            return;
            Thread.Sleep(50);
            if (!PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0)) { throw new Win32Exception(); }
            Thread.Sleep(1);
            if (!PostMessage(hWnd, WM_KEYUP, VK_RETURN, 0)) { throw new Win32Exception(); }
        }

    }
}
