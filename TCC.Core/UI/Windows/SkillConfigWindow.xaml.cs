using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TCC.ViewModels.Widgets;
using TeraDataLite;

namespace TCC.UI.Windows
{
    /// <summary>
    /// Logica di interazione per SkillConfigWindow.xaml
    /// </summary>
    public partial class SkillConfigWindow
    {

        public IntPtr Handle { get; private set; }
        private CooldownWindowViewModel VM { get; }
        public SkillConfigWindow()
        {
            InitializeComponent();
            DataContext = WindowManager.ViewModels.CooldownsVM;
            VM = DataContext as CooldownWindowViewModel;

            Closing += OnClosing;
            Loaded += (_, __) => Handle = new WindowInteropHelper(this).Handle;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (Opacity != 0) e.Cancel = true;
            ClosewWindow(null, null);
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
        public HiddenSKillsDragHandler HiddenSkillsDropHandler => new HiddenSKillsDragHandler();

        public static bool IsOpen { get; private set; }

        private void ClosewWindow(object sender, RoutedEventArgs e)
        {

            var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            FocusManager.ForceFocused = false;
            //WindowManager.VisibilityManager.ForceUndim = false;
            VM.Settings.ForcedClickable = false;
            VM.Settings.ForcedVisible = false;

            WindowManager.SkillConfigWindow = null;

            an.Completed += (s, ev) =>
            {
                Close();
                IsOpen = false;
                if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            };
            BeginAnimation(OpacityProperty, an);
            VM.SaveConfig();
        }

        internal void ShowWindow()
        {
            if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
            FocusManager.ForceFocused = true;
            //WindowManager.VisibilityManager.ForceUndim = true;
            VM.Settings.ForcedClickable = true;
            VM.Settings.ForcedVisible = true;
            WindowManager.SkillConfigWindow = this;
            Dispatcher?.Invoke(() =>
            {
                IsOpen = true;
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
            if (VM.SkillsView == null) return;
            var view = (ICollectionView)VM.SkillsView;
            view.Filter = o => ((Skill)o).ShortName.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }

        private void ItemSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)VM.ItemsView;
            view.Filter = o => ((Item)o).Name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }
        private void PassivitySearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)VM.AbnormalitiesView;
            view.Filter = o => ((Abnormality)o).Name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }

        private void RemoveHiddenSkill(object sender, RoutedEventArgs e)
        {
            VM.RemoveHiddenSkill(((Button)sender).DataContext as Cooldown);
        }
    }
    public class HiddenSKillsDragHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var l = dropInfo.TargetCollection.TryGetList();
            if (l.Cast<Cooldown>().Any(cd =>
            {
                var ret = dropInfo.Data switch
                {
                    Skill s => (cd.Skill.IconName == s.IconName),
                    Item i => (cd.Skill.IconName == i.IconName),
                    Abnormality a => (cd.Skill.IconName == a.IconName),
                    _ => false
                };

                return ret;
            })) return;

            switch (dropInfo.Data)
            {
                case Skill s:
                    l.Add(new Cooldown(s, false));
                    break;
                case Item i:
                    Game.DB.ItemsDatabase.TryGetItemSkill(i.Id, out var itemSkill);
                    l.Add(new Cooldown(itemSkill, false, CooldownType.Item));
                    break;
                case Abnormality a:
                    l.Add(new Cooldown(new Skill(a.Id,Class.None, a.Name, a.ToolTip){IconName = a.IconName}, false, CooldownType.Passive));
                    break;
            }
            //dropInfo.DragInfo.SourceCollection.TryGetList().Remove(dropInfo.Data);
        }
    }
}
