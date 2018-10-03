using Dragablz;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    public partial class FixedSkillContainers
    {
        private object[] _mainOrder;
        private object[] _secondaryOrder;
        private readonly DispatcherTimer _mainButtonTimer;
        private readonly DispatcherTimer _secButtonTimer;
        private readonly DoubleAnimation _opacityUp;
        private readonly DoubleAnimation _opacityDown;
        private static readonly Action EmptyDelegate = delegate { };
        private string _lastSender = "";

        public FixedSkillContainers()
        {
            InitializeComponent();
            AddHandler(DragablzItem.DragStarted, new DragablzDragStartedEventHandler(ItemDragStarted), true);
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
            _mainButtonTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _secButtonTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _mainButtonTimer.Tick += MainButtonTimer_Tick;
            _secButtonTimer.Tick += SecButtonTimer_Tick;
            _opacityUp = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() };
            _opacityDown = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase(), BeginTime = TimeSpan.FromMilliseconds(1000) };
            SelectionPopup.Closed += SelectionPopup_Closed;
            SelectionPopup.Opened += SelectionPopup_Opened;
            CooldownWindowViewModel.Instance.SecondarySkills.CollectionChanged += SecondarySkills_CollectionChanged;
            CooldownWindowViewModel.Instance.MainSkills.CollectionChanged += MainSkills_CollectionChanged;
            CooldownWindowViewModel.Instance.SkillsLoaded += OnSkillsLoaded;
        }

        //really absurd way of fixing order issue
        private void OnSkillsLoaded()
        {
            Dispatcher.Invoke(() =>
            {
                ReorderSkillContainer(MainSkills, CooldownWindowViewModel.Instance.MainSkills);
                ReorderSkillContainer(SecSkills, CooldownWindowViewModel.Instance.SecondarySkills);
            });
        }

        //really absurd way of fixing order issue
        private void ReorderSkillContainer(DragablzItemsControl container, SynchronizedObservableCollection<FixedSkillCooldown> collection)
        {
            var positions = new Dictionary<int, double>(); //index, X
            for (var i = 0; i < container.Items.Count; i++)
            {
                if (!(container.ItemContainerGenerator.ContainerFromIndex(i) is UIElement curr)) continue;
                var p = curr.TransformToAncestor(this).Transform(new Point(0, 0));
                positions.Add(i, p.X);
            }

            var needsReorder = false;
            for (var j = 0; j < positions.Count; j++)
            {
                if (j + 1 == positions.Count) continue;
                if (positions[j] > positions[j + 1])
                {
                    needsReorder = true;
                    break;
                }
            }

            if (!needsReorder) return;
            container.ItemsSource = null;
            container.ItemsSource = collection;
        }

        public SkillDropHandler DropHandler => new SkillDropHandler();

        private void MainSkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) return;
            MainSkills.InvalidateMeasure();
            MainSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            CooldownWindowViewModel.Instance.Save();
        }

        private void SecondarySkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                SecSkills.InvalidateMeasure();
                SecSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                CooldownWindowViewModel.Instance.Save();
            }

            OtherSkills.Margin = ((SynchronizedObservableCollection<FixedSkillCooldown>)sender).Count == 0
                ? new Thickness(-60, 0, 0, 0)
                : new Thickness(0);
        }

        private void SelectionPopup_Opened(object sender, EventArgs e)
        {
            FocusManager.FocusTimer.Enabled = false;
        }

        private void SelectionPopup_Closed(object sender, EventArgs e)
        {
            _mainButtonTimer.Start();
            _secButtonTimer.Start();
            ChoiceListBox.UnselectAll();
            FocusManager.FocusTimer.Enabled = true;
        }

        private void SelectionPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectionPopup.IsOpen = false;
        }

        private void MainButtonTimer_Tick(object sender, EventArgs e)
        {
            _mainButtonTimer.Stop();
            if (!MainSkillsGrid.IsMouseOver && !SelectionPopup.IsMouseOver)
            {
                if (SelectionPopup.IsOpen) SelectionPopup.IsOpen = false;
                AnimateAddButton(false/*, Spacer, AddButtonGrid*/);
            }
        }

        private void SecButtonTimer_Tick(object sender, EventArgs e)
        {
            _secButtonTimer.Stop();
            if (SecSkillsGrid.IsMouseOver || SelectionPopup.IsMouseOver) return;
            if (SelectionPopup.IsOpen) SelectionPopup.IsOpen = false;
            AnimateAddButton(false/*, Spacer2, AddButtonGrid2*/);
        }

        private void AnimateAddButton(bool open/*, Grid targetspacer, Grid addButtonGrid*/)
        {
            SettingsButton.BeginAnimation(OpacityProperty, open ? _opacityUp : _opacityDown);
/*
            if (open && ((ScaleTransform)targetspacer.LayoutTransform).ScaleX == 1) return;
            if (!open && ((ScaleTransform)targetspacer.LayoutTransform).ScaleX == 0) return;
            var to = open ? 1 : 0;
            //var from = open ? 0 : 1;
            targetspacer.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty,
                new DoubleAnimation(to, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
            addButtonGrid.BeginAnimation(OpacityProperty,
                new DoubleAnimation(to, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
*/
        }

        private bool _isDragging;
        private void ItemDragStarted(object sender, DragablzDragStartedEventArgs e)
        {
            _isDragging = true;
            FocusManager.FocusTimer.Enabled = false;
            WindowManager.ForegroundManager.ForceVisible = true;
            WindowManager.ForegroundManager.ForceUndim = true;
        }

        private void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            if (CooldownWindowViewModel.Instance.MainSkills.Contains(e.DragablzItem.DataContext as FixedSkillCooldown))
            {
                if (_mainOrder == null) return;
                for (int j = 0; j < CooldownWindowViewModel.Instance.MainSkills.Count; j++)
                {
                    var newIndex = _mainOrder.ToList().IndexOf(CooldownWindowViewModel.Instance.MainSkills[j]);
                    var oldIndex = j;
                    CooldownWindowViewModel.Instance.MainSkills.Move(oldIndex, newIndex);
                }
            }
            else if (CooldownWindowViewModel.Instance.SecondarySkills.Contains(e.DragablzItem.DataContext as FixedSkillCooldown))
            {
                if (_secondaryOrder == null) return;
                for (int i = 0; i < CooldownWindowViewModel.Instance.SecondarySkills.Count; i++)
                {
                    var newIndex = _secondaryOrder.ToList().IndexOf(CooldownWindowViewModel.Instance.SecondarySkills[i]);
                    var oldIndex = i;
                    CooldownWindowViewModel.Instance.SecondarySkills.Move(oldIndex, newIndex);
                }
            }
            CooldownWindowViewModel.Instance.Save();
            FocusManager.FocusTimer.Enabled = true;
            WindowManager.ForegroundManager.ForceVisible = false;
            WindowManager.ForegroundManager.ForceUndim = false;
            _isDragging = false;
        }

        private void MainSkillOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _mainOrder = e.NewOrder;

            if (_mainOrder.Length < 2) return;
            if (!_isDragging)
            {
                //force it here
                //InstanceOnRefreshItemSourcesEvent();
            }
        }

        private void SecondarySkillOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _secondaryOrder = e.NewOrder;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void AddButtonPressed(object sender, MouseButtonEventArgs e)
        {
            //CooldownWindowViewModel.Instance.MainSkills.Add(new FixedSkillCooldown(new Skill(181100, Class.Warrior, "", ""), CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(), true));
            SelectionPopup.IsOpen = true;
            ChoiceListBox.ItemsSource = CooldownWindowViewModel.Instance.SkillChoiceList;
            _lastSender = (sender as Grid)?.Name;
        }

        private void MainSkillsGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateAddButton(true/*, Spacer, AddButtonGrid*/);
        }

        private void MainSkillsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            //_mainButtonTimer.Start();
            //AnimateAddButton(false, null, null);
            OnSkillsLoaded();
        }

//        private void SecondarySkillsGridMouseEnter(object sender, MouseEventArgs e)
//        {
//            AnimateAddButton(true/*, Spacer2, AddButtonGrid2*/);
//        }

/*
        private void SecSkillsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            _secButtonTimer.Start();
        }
*/

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox s && s.SelectedItems.Count > 0)
            {
                if (_lastSender == AddButtonGrid.Name)
                {
                    if (CooldownWindowViewModel.Instance.MainSkills.Any(x =>
                        x.Skill.IconName == (s.SelectedItems[0] as Skill)?.IconName)) return;
                    CooldownWindowViewModel.Instance.MainSkills.Add(new FixedSkillCooldown(s.SelectedItems[0] as Skill, false));
                }
                else if (_lastSender == AddButtonGrid2.Name)
                {
                    if (CooldownWindowViewModel.Instance.SecondarySkills.Any(x =>
                        x.Skill.IconName == (s.SelectedItems[0] as Skill)?.IconName)) return;
                    CooldownWindowViewModel.Instance.SecondarySkills.Add(new FixedSkillCooldown(
                        s.SelectedItems[0] as Skill,
                         false));
                }

                ChoiceListBox.ItemsSource = null;
            }

            SelectionPopup.IsOpen = false;
            CooldownWindowViewModel.Instance.Save();
        }

        public class SkillDropHandler : IDropTarget
        {
            public void DragOver(IDropInfo dropInfo)
            {
                dropInfo.Effects = DragDropEffects.Move;
            }

            public void Drop(IDropInfo dropInfo)
            {
                var target = ((SynchronizedObservableCollection<FixedSkillCooldown>)dropInfo.TargetCollection);
                switch (dropInfo.Data)
                {
                    case Skill sk:
                        if (target.All(x => x.Skill.IconName != sk.IconName))
                        {
                            target.Insert(dropInfo.InsertIndex, new FixedSkillCooldown((Skill)dropInfo.Data, false));
                        }
                        break;
                    case Abnormality ab:
                        if (target.All(x => x.Skill.IconName != ab.IconName))
                        {
                            target.Insert(dropInfo.InsertIndex,
                                new FixedSkillCooldown(new Skill(ab.Id, Class.None, ab.Name, ab.ToolTip) { IconName = ab.IconName },
                                    false, CooldownType.Passive));
                        }
                        break;
                    case Item i:
                        if (target.All(x => x.Skill.IconName != i.IconName))
                        {
                            SessionManager.ItemsDatabase.TryGetItemSkill(i.Id, out var s);
                            target.Insert(dropInfo.InsertIndex, new FixedSkillCooldown(s, false, CooldownType.Item));
                        }

                        break;
                }
                var tmp = new List<FixedSkillCooldown>();

                //force correct order as it's not preserved
                foreach (var fixedSkillCooldown in target)
                {
                    tmp.Add(fixedSkillCooldown);
                }
                target.Clear();
                tmp.ForEach(x =>
                {
                    target.Add(x);
                });

                const ulong delay = 500;
                //wait a bit and restart any running skill
                Task.Delay(TimeSpan.FromMilliseconds(delay)).ContinueWith(t =>
                {
                    CooldownWindowViewModel.Instance.MainSkills.ToList().ForEach(x =>
                    {
                        if (x.Seconds > 0)
                        {
                            x.Start((x.Seconds) * 1000 - delay);
                        }
                    });
                    CooldownWindowViewModel.Instance.SecondarySkills.ToList().ForEach(x =>
                    {
                        if (x.Seconds > 0)
                        {
                            x.Start((x.Seconds) * 1000 - delay);
                        }
                    });
                });

                CooldownWindowViewModel.Instance.Save();
            }
        }

        private void FixedSkillContainers_OnDragOver(object sender, DragEventArgs e)
        {
            if (SecSkills.ActualWidth == 0) SecSkills.MinWidth = 50;
        }

        private void FixedSkillContainers_OnDragLeave(object sender, DragEventArgs e)
        {
            SecSkills.MinWidth = 0;
        }

        private void OpenCooldownSettings(object sender, RoutedEventArgs e)
        {
            WindowManager.SkillConfigWindow.ShowWindow();
        }

        private void MainSkills_OnDrop(object sender, DragEventArgs e)
        {
            MainSkills.InvalidateArrange();
            MainSkills.InvalidateMeasure();
            MainSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        private void FixedSkillContainers_OnLoaded(object sender, RoutedEventArgs e)
        {
            //CooldownWindowViewModel.Instance.AddOrRefresh(new SkillCooldown(new Skill(0, Class.Warrior, "BD", "BD") { IconName = "icon_skills.dualslash_tex" }, 900000, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
        }
    }
}