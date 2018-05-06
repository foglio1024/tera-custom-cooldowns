using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using GongSolutions.Wpf.DragDrop;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per SkillConfigWindow.xaml
    /// </summary>
    public partial class SkillConfigWindow : Window
    {
        public SkillConfigWindow()
        {
            InitializeComponent();
            DataContext = CooldownWindowViewModel.Instance;
        }
        public class GenericDragHandler : IDropTarget
        {
            public void DragOver(IDropInfo dropInfo)
            {
            }

            public void Drop(IDropInfo dropInfo)
            {
            }
        }

        public GenericDragHandler DragHandler => new GenericDragHandler();

        private void Close(object sender, RoutedEventArgs e)
        {
            var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            an.Completed += (s, ev) => Hide();
            BeginAnimation(OpacityProperty, an);
            CooldownWindowViewModel.Instance.Save();
        }
        public void CloseWindow()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
                a.Completed += (s, ev) => { Hide(); };
                BeginAnimation(OpacityProperty, a);
            }, DispatcherPriority.DataBind);
        }
        internal void ShowWindow()
        {
            Dispatcher.Invoke(() =>
            {
                var animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                if (IsVisible) return;
                Opacity = 0;
                Show();
                Activate();
                BeginAnimation(OpacityProperty, animation);
            });
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SkillSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var view = ((ICollectionView)CooldownWindowViewModel.Instance.SkillsView);
            view.Filter = o =>  ((Skill)o).ShortName.IndexOf((sender as TextBox).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }

        private void ItemSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)CooldownWindowViewModel.Instance.ItemsView;
            view.Filter = o => ((Item)o).Name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }
        private void PassivitySearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)CooldownWindowViewModel.Instance.AbnormalitiesView;
            view.Filter = o => ((Abnormality)o).Name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }

        private void RemoveHiddenSkill(object sender, RoutedEventArgs e)
        {
            CooldownWindowViewModel.Instance.HiddenSkills.Remove((sender as Button).DataContext as Skill);
        }
    }
}
