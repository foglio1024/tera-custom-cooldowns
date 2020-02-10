using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.Factories;
using TCC.Data.Chat;

namespace TCC.Controls.Chat
{
    public partial class ChatMessageControl
    {
        private readonly DoubleAnimation _anim;
        private ChatMessage _dc;
        public ChatMessageControl()
        {
            InitializeComponent();
            _anim = AnimationFactory.CreateDoubleAnimation(250, 1, 0, true, AnimCompleted, 30);
            Timeline.SetDesiredFrameRate(_anim, 30);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_dc == null) return;
            _dc.IsVisible = true;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if(_dc == null) return;
            _dc.IsVisible = false;
            _dc = null;
        }

        private void AnimCompleted(object sender, EventArgs e)
        {
            SetAnimated();
        }

        private void SetAnimated()
        {
            if (_dc == null) return;
            _dc.Animate = false;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if(!(DataContext is ChatMessage)) return;
            _dc = (ChatMessage) DataContext;
            var tg = (TransformGroup) LayoutTransform;
            var sc = tg.Children[0];
            if (!_dc.Animate)
            {
                var sc2 = new ScaleTransform(1,1);
                tg.Children[0] = sc2;
                return;
            }
            sc.BeginAnimation(ScaleTransform.ScaleYProperty, _anim);
        }
    }
}
