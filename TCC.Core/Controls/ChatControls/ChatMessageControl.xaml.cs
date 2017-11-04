using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per ChatMessageControl.xaml
    /// </summary>
    public partial class ChatMessageControl : UserControl
    {
        public ChatMessageControl()
        {
            InitializeComponent();
            da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250)){EasingFunction = new QuadraticEase()};
            da.Completed += Da_Completed;

        }

        private void Da_Completed(object sender, EventArgs e)
        {
            SetAnimated();
        }

        private void SetAnimated()
        {
            if (!(DataContext is ChatMessage))
            {
                //Debug.WriteLine("DataContext is not a ChatMessage");
                return;
            }
            var dc = ((ChatMessage)DataContext);
            dc.Animate = false;
            //Debug.WriteLine($"{dc.RawMessage} -- set animated");

        }
        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((ChatMessage)DataContext).IsContracted)
            {
                popup.IsOpen = true;
            }
        }

        private void popup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void UserControl_Loaded(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if(!(DataContext is ChatMessage)) return;
            var dc = ((ChatMessage) DataContext);
            //Dispatcher.Invoke(new Action(() =>
            //{
            //    try
            //    {
            //    ((ChatMessage)DataContext).Rows = WindowManager.ChatWindow.GetMessageRows(this.ActualHeight);
            //    }
            //    catch (Exception) { }
            //}), DispatcherPriority.Loaded);
            //Debug.WriteLine($"{dc.RawMessage} -- loaded");

            var tg = (TransformGroup) LayoutTransform;
            var sc = tg.Children[0];
            if (!dc.Animate)
            {
                var sc2 = new ScaleTransform(1,1);
                tg.Children[0] = sc2;
                return;
            }
            sc.BeginAnimation(ScaleTransform.ScaleYProperty, da);
        }

        private DoubleAnimation da;
        private void ChatMessageControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ChatMessage)) return;
            var dc = ((ChatMessage)DataContext);
            var tg = (TransformGroup)LayoutTransform;
            var sc = tg.Children[0];
            sc.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            //Debug.WriteLine($"{dc.RawMessage} -- unloaded");

            SetAnimated();

        }
    }
}
