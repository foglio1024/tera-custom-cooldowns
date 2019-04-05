using System;
using System.Threading;
using System.Windows.Threading;

namespace TCC.Utilities.Extensions
{
    public static class DispatcherExtensions
    {
        public static void InvokeIfRequired(this Dispatcher disp, Action dotIt, DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
                disp.Invoke(priority, dotIt);
            else
                dotIt();
        }
        public static void BeginInvokeIfRequired(this Dispatcher disp, Action dotIt, DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
                disp.BeginInvoke(dotIt, priority);
            else
                dotIt();
        }
    }
}