using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TCC.Utilities.Extensions
{
    public static class ItemsControlExtensions
    {
        public static void RefreshTemplate(this ItemsControl el, string resName)
        {
            if (el == null) return;
            el.Dispatcher.BeginInvoke(new Action(() =>
            {
                el.ItemTemplateSelector = null;
                el.ItemTemplateSelector = Application.Current.FindResource(resName) as DataTemplateSelector;
            }), DispatcherPriority.Background);
        }
        public static void RefreshTemplate(this ItemsControl el, DataTemplateSelector selector)
        {
            if (el == null) return;
            el.Dispatcher.BeginInvoke(new Action(() =>
            {
                el.ItemTemplateSelector = null;
                el.ItemTemplateSelector = selector;
            }), DispatcherPriority.Background);
        }
    }
}