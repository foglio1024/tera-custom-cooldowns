using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public static DispatcherTimer FocusTimer;

        public static event ForegroundWindowChangedEventHandler ForegroundWindowChanged;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern uint SetWindowLong(IntPtr hwnd, int index, uint newStyle);

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
        public static void CheckForegroundWindow(object sender, EventArgs e)
        {
            IntPtr hwnd = FocusManager.GetForegroundWindow();
            FocusManager.GetWindowThreadProcessId(hwnd, out uint procId);
            Process proc = Process.GetProcessById((int)procId);

            if (proc.ProcessName == "TERA" || proc.ProcessName == "TCC" || proc.ProcessName == "devenv")
            {
                ForegroundWindowChanged?.Invoke(true);
            }
            else
            {
                ForegroundWindowChanged?.Invoke(false);
            }
        }
    }
}
