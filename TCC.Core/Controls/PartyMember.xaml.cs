using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per PartyMember.xaml
    /// </summary>
    public partial class PartyMember : UserControl
    {
        public PartyMember()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetMP(GroupWindowViewModel.Instance.MPenabled);

            AnimateIn();
            GroupWindowViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
        }
        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GroupWindowViewModel.Instance.MPenabled))
            {
                SetMP(GroupWindowViewModel.Instance.MPenabled);
            }
        }

        private void SetMP(bool mPenabled)
        {
            Dispatcher.Invoke(() =>
            {
                if (mPenabled)
                {
                    mpRect.Visibility = Visibility.Visible;
                    mpBase.Visibility = Visibility.Visible;
                }
                else
                {
                    mpRect.Visibility = Visibility.Collapsed;
                    mpBase.Visibility = Visibility.Collapsed;
                }
            });
        }

        public void AnimateIn()
        {
            var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            BeginAnimation(OpacityProperty, an);
        }

        internal void AnimateOut()
        {
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            BeginAnimation(OpacityProperty, an);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            ProxyInterop.SendAskInteractiveMessage(dc.ServerId, dc.Name);
        }
    }
}
