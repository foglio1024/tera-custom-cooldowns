using System;
using System.ComponentModel;
using System.Threading;
using Nostrum.WinAPI;

namespace TCC.UI
{
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