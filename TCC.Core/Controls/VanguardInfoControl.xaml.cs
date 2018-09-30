using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for VanguardInfoControl.xaml
    /// </summary>
    public partial class VanguardInfoControl : UserControl
    {
        private TimeSpan growDuration;
        private DoubleAnimation scaleUp;
        private DoubleAnimation moveUp;
        private DoubleAnimation scaleDown;
        private DoubleAnimation moveDown;
        private DoubleAnimation scaleRipple;
        private DoubleAnimation fadeRipple;


        public VanguardInfoControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //rootBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, scaleUp);
            //rootBorder.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, moveUp);
            Glow.BeginAnimation(OpacityProperty,
                new DoubleAnimation(1, TimeSpan.FromMilliseconds(50))
                { EasingFunction = new QuadraticEase() });
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (_animDown)
            //{

            //    rootBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, scaleDown);
            //    rootBorder.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, moveDown);
            //}
            //else
            //{
            //    rootBorder.RenderTransform = new TranslateTransform(0, 0);
            //    (rootBorder.Effect as DropShadowEffect).BlurRadius = 3;
            //}
            Glow.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, TimeSpan.FromMilliseconds(250))
                { EasingFunction = new QuadraticEase() });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            growDuration = TimeSpan.FromMilliseconds(150);
            scaleUp = new DoubleAnimation(-3, growDuration) { EasingFunction = new QuadraticEase() };
            moveUp = new DoubleAnimation(8, growDuration) { EasingFunction = new QuadraticEase() };
            scaleDown = new DoubleAnimation(0, growDuration) { EasingFunction = new QuadraticEase() };
            moveDown = new DoubleAnimation(3, growDuration) { EasingFunction = new QuadraticEase() };
            scaleRipple = new DoubleAnimation(0, 80, TimeSpan.FromMilliseconds(650)) { EasingFunction = new QuadraticEase() };
            fadeRipple = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(650));
            ContextMenu = new ContextMenu();
            ((Character)DataContext).PropertyChanged += (s, ev) =>
            {
                if (ev.PropertyName != nameof(Character.IsSelected)) return;
                AnimateSel();
            };
            AnimateSel();
            var i = new MenuItem { Header = "Remove" };
            i.Click += RemoveCharacter;
            ContextMenu.Items.Add(i);
        }

        private void AnimateSel()
        {
            if (DataContext == null) return;
            Sel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                ((Character) DataContext).IsSelected
                    ? new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) {EasingFunction = new QuadraticEase()}
                    : new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) {EasingFunction = new QuadraticEase()});
        }
        private void RemoveCharacter(object sender, RoutedEventArgs e)
        {
            InfoWindowViewModel.Instance.Characters.Remove((Character)DataContext);
        }

        //private bool _animDown = true;
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //_animDown = false;
            var scaleTrans = (Ripple.RenderTransform as TransformGroup).Children[0];
            (Ripple.RenderTransform as TransformGroup).Children[1] = new TranslateTransform(e.MouseDevice.GetPosition(this).X - Ripple.Width / 2, e.MouseDevice.GetPosition(this).Y - Ripple.Height / 2);

            scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, scaleRipple);
            scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, scaleRipple);
            Ripple.BeginAnimation(OpacityProperty, fadeRipple);
            //Point relativePoint = rootBorder.TransformToAncestor(WindowManager.InfoWindow)
            //                  .Transform(new Point(0, 0));
            //WindowManager.InfoWindow.ExpandCharacter(relativePoint, rootBorder.ActualWidth, this.ActualHeight);
            InfoWindowViewModel.Instance.SelectCharacter((Character)DataContext);
            //Task.Delay(300).ContinueWith(t => _animDown = true);
        }

        private void RootBorder_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
