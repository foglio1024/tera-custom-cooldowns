using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per ChatMessageControl.xaml
    /// </summary>
    public partial class ChatMessageControl
    {
        private readonly DoubleAnimation _anim;

        public ChatMessageControl()
        {
            InitializeComponent();
            _anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250)){EasingFunction = new QuadraticEase()};
            _anim.Completed += AnimCompleted;
            Timeline.SetDesiredFrameRate(_anim, 30);
        }
        private void AnimCompleted(object sender, EventArgs e)
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
                Popup.IsOpen = true;
            }
        }

        private void Popup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void UserControl_Loaded(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if(!(DataContext is ChatMessage)) return;
            var dc = ((ChatMessage) DataContext);
            var tg = (TransformGroup) LayoutTransform;
            var sc = tg.Children[0];
            if (!dc.Animate)
            {
                var sc2 = new ScaleTransform(1,1);
                tg.Children[0] = sc2;
                return;
            }
            sc.BeginAnimation(ScaleTransform.ScaleYProperty, _anim);
        }

    }
}
