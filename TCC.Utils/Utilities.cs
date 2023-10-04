using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace TCC.Utils;

public static class Utilities
{
    public static event Action? CloseRequested;

    public static void RequestClose()
    {
        CloseRequested?.Invoke();
    }

#pragma warning disable CA2255
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Initializer()
    {
        _mainDispatcher = Dispatcher.CurrentDispatcher;
    }

    static Dispatcher? _mainDispatcher;

    public static Dispatcher GetMainDispatcher()
    {
        if (_mainDispatcher == null) throw new NullReferenceException("Main Dispatcher was not set");
        return _mainDispatcher;
    }
        
    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}