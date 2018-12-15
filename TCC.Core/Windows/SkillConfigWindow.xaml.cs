using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GongSolutions.Wpf.DragDrop;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per SkillConfigWindow.xaml
    /// </summary>
    public partial class SkillConfigWindow
    {

        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        public SkillConfigWindow()
        {
            InitializeComponent();
            DataContext = CooldownWindowViewModel.Instance;
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ClosewWindow(null,null);
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

        private void ClosewWindow(object sender, RoutedEventArgs e)
        {
            var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            FocusManager.ForceFocused = false;
            WindowManager.ForegroundManager.ForceUndim = false;

            an.Completed += (s, ev) =>
            {
                Hide();
                if (Settings.SettingsHolder.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            };
            BeginAnimation(OpacityProperty, an);
            CooldownWindowViewModel.Instance.Save();
        }

        internal void ShowWindow()
        {
            if (Settings.SettingsHolder.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
            FocusManager.ForceFocused = true;
            WindowManager.ForegroundManager.ForceUndim = true;
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
            view.Filter = o =>  ((Skill)o).ShortName.IndexOf(((TextBox) sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
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
            CooldownWindowViewModel.Instance.RemoveHiddenSkill(((Button) sender).DataContext as Cooldown);
        }
    }
}
