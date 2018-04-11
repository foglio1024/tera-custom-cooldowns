using System;
using System.Windows;
using TCC.ViewModels;

namespace TCC
{
    public static class ThreadSafeManager
    {
        public static void InvokeOnWindow(Window target, Delegate action)
        {
            target.Dispatcher.Invoke(action);
        }

        public static ClassManager CurrentClassBar()
        {
            return ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).CurrentManager;
        }
    }
}
