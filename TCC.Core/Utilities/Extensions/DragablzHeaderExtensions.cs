using System.Windows;

namespace TCC.Utilities.Extensions
{
    public class DragablzHeaderExtensions
    {
        public static float GetFrameOpacity(DependencyObject obj)
        {
            return (float)obj.GetValue(FrameOpacityProperty);
        }
        public static void SetFrameOpacity(DependencyObject obj, float value)
        {
            obj.SetValue(FrameOpacityProperty, value);
        }
        public static readonly DependencyProperty FrameOpacityProperty =
            DependencyProperty.RegisterAttached("FrameOpacity", typeof(float), typeof(DragablzHeaderExtensions), new PropertyMetadata(0f));
    }
}