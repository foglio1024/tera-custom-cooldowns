using Nostrum.WinAPI;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TCC.Data;

namespace TCC.UI;

public static class InputInjector
{
    public static void InjectHotkey(IntPtr hWnd, HotKey hk)
    {
        Task.Run(async () =>
        {
            if (hk.Modifier != ModifierKeys.None)
            {
                if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYDOWN, ConvertModifier(hk.Modifier), 0))
                    throw new Win32Exception();
            }

            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYDOWN, (int)hk.Key, 0))
                throw new Win32Exception();

            await Task.Delay(5);

            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYUP, (int)hk.Key, 0))
                throw new Win32Exception();

            if (hk.Modifier != ModifierKeys.None)
            {
                if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYUP, ConvertModifier(hk.Modifier), 0))
                    throw new Win32Exception();
            }
        });

        return;

        static int ConvertModifier(ModifierKeys mod)
        {
            return mod switch
            {
                ModifierKeys.Alt => 0xA4,
                ModifierKeys.Control => 0xA2,
                ModifierKeys.Shift => 0xA0,
                ModifierKeys.Win => 0x5B,
                _ => 0
            };
        }
    }

    public static void PasteString(IntPtr hWnd, string s)
    {
        Thread.Sleep(100);
        foreach (var character in s)
        {
            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_CHAR, character, 0))
                throw new Win32Exception();
            Thread.Sleep(1);
        }
    }

    public static void NewLine(IntPtr hWnd)
    {
        if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYDOWN, (int)User32.VirtualKeys.VK_RETURN, 0))
            throw new Win32Exception();

        Thread.Sleep(1);

        if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYUP, (int)User32.VirtualKeys.VK_RETURN, 0))
            throw new Win32Exception();
    }

    public static void PgUp(IntPtr hWnd, int delayMs)
    {
        Task.Run(async () =>
        {
            await Task.Delay(delayMs);

            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYDOWN, 0x21, 0))
                throw new Win32Exception();

            await Task.Delay(5);

            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYUP, 0x21, 0))
                throw new Win32Exception();
        });
    }

    public static void PgDown(IntPtr hWnd, int delayMs)
    {
        Task.Run(async () =>
        {
            await Task.Delay(delayMs);

            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYDOWN, 0x22, 0))
                throw new Win32Exception();

            await Task.Delay(5);

            if (!User32.PostMessage(hWnd, (uint)User32.WindowsMessages.WM_KEYUP, 0x22, 0))
                throw new Win32Exception();
        });
    }
}