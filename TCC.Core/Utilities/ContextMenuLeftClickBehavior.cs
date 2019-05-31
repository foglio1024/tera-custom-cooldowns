using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace TCC
{
    public static class ContextMenuLeftClickBehavior
    {
        public static bool GetIsLeftClickEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLeftClickEnabledProperty);
        }

        public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLeftClickEnabledProperty, value);
        }

        public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty.RegisterAttached(
            "IsLeftClickEnabled",
            typeof(bool),
            typeof(ContextMenuLeftClickBehavior),
            new UIPropertyMetadata(false, OnIsLeftClickEnabledChanged));

        private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement != null)
            {
                var isEnabled = e.NewValue is bool && (bool)e.NewValue;

                if (isEnabled)
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click += OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
                }
                else
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click -= OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;
            // if we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click
            // (it seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control, although I'm not sure)
            // so we have to set up ContextMenu.DataContext manually here

            if (fe.ContextMenu == null) return;
            if (fe.ContextMenu.DataContext == null)
            {
                fe.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
            }
            fe.ContextMenu.IsOpen = true;
        }
    }
}
