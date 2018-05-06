using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Dragablz;
using GongSolutions.Wpf.DragDrop;
using TCC.Data;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Controls
{
    /// <summary>
    ///     Logica di interazione per FixedSkillContainers.xaml
    /// </summary>
    public partial class FixedSkillContainers
    {
        private object[] _mainOrder;
        private object[] _secondaryOrder;
        private readonly DispatcherTimer _mainButtonTimer;
        private readonly DispatcherTimer _secButtonTimer;
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
            SelectionPopup.Closed += SelectionPopup_Closed;
            SelectionPopup.Opened += SelectionPopup_Opened;
            CooldownWindowViewModel.Instance.SecondarySkills.CollectionChanged += SecondarySkills_CollectionChanged;
            CooldownWindowViewModel.Instance.MainSkills.CollectionChanged += MainSkills_CollectionChanged;
        }

        public SkillDropHandler DropHandler => new SkillDropHandler();

        private void MainSkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                MainSkills.InvalidateMeasure();
                MainSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
        }

        private void SecondarySkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                SecSkills.InvalidateMeasure();
                SecSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }

            OtherSkills.Margin = ((SynchronizedObservableCollection<FixedSkillCooldown>)sender).Count == 0
                ? new Thickness(-60, 0, 0, 0)
                : new Thickness(0);
        }

        private void SelectionPopup_Opened(object sender, EventArgs e)
        {
            FocusManager.Running = false;
        }

        private void SelectionPopup_Closed(object sender, EventArgs e)
        {
            _mainButtonTimer.Start();
            _secButtonTimer.Start();
            ChoiceListBox.UnselectAll();
            FocusManager.Running = true;
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
                AnimateAddButton(false, Spacer, AddButtonGrid);
            }
        }

        private void SecButtonTimer_Tick(object sender, EventArgs e)
        {
            _secButtonTimer.Stop();
            if (!SecSkillsGrid.IsMouseOver && !SelectionPopup.IsMouseOver)
            {
                if (SelectionPopup.IsOpen) SelectionPopup.IsOpen = false;
                AnimateAddButton(false, Spacer2, AddButtonGrid2);
            }
        }

        private void AnimateAddButton(bool open, Grid targetspacer, Grid addButtonGrid)
        {
            if (open && ((ScaleTransform)targetspacer.LayoutTransform).ScaleX == 1) return;
            if (!open && ((ScaleTransform)targetspacer.LayoutTransform).ScaleX == 0) return;
            var to = open ? 1 : 0;
            //var from = open ? 0 : 1;
            targetspacer.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty,
                new DoubleAnimation(to, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
            addButtonGrid.BeginAnimation(OpacityProperty,
                new DoubleAnimation(to, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
        }

        private void ItemDragStarted(object sender, DragablzDragStartedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = false;
            WindowManager.IsFocused = true;
        }

        private void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            //var item = e.DragablzItem.DataContext;
            if (CooldownWindowViewModel.Instance.MainSkills.Contains(e.DragablzItem.DataContext as FixedSkillCooldown))
            {
                for (int j = 0; j < CooldownWindowViewModel.Instance.MainSkills.Count; j++)
                {
                    var newIndex = _mainOrder.ToList().IndexOf(CooldownWindowViewModel.Instance.MainSkills[j]);
                    var oldIndex = j;
                    CooldownWindowViewModel.Instance.MainSkills.Move(oldIndex, newIndex);
                }
            }
            else if (CooldownWindowViewModel.Instance.SecondarySkills.Contains(e.DragablzItem.DataContext as FixedSkillCooldown))
            {
                for (int i = 0; i < CooldownWindowViewModel.Instance.SecondarySkills.Count; i++)
                {
                    var newIndex = _secondaryOrder.ToList().IndexOf(CooldownWindowViewModel.Instance.SecondarySkills[i]);
                    var oldIndex = i;
                    CooldownWindowViewModel.Instance.SecondarySkills.Move(oldIndex, newIndex);
                }
            }

            CooldownWindowViewModel.Instance.Save();
            FocusManager.FocusTimer.Enabled = true;
        }

        private void MainSkillOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _mainOrder = e.NewOrder;
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
            AnimateAddButton(true, Spacer, AddButtonGrid);
            if (CooldownWindowViewModel.Instance.SecondarySkills.Count == 0)
                AnimateAddButton(true, Spacer2, AddButtonGrid2);
        }

        private void MainSkillsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            _mainButtonTimer.Start();
        }

        private void SecondarySkillsGridMouseEnter(object sender, MouseEventArgs e)
        {
            AnimateAddButton(true, Spacer2, AddButtonGrid2);
        }

        private void SecSkillsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            _secButtonTimer.Start();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox s && s.SelectedItems.Count > 0)
            {
                if (_lastSender == AddButtonGrid.Name)
                {
                    if (CooldownWindowViewModel.Instance.MainSkills.Any(x =>
                        x.Skill.IconName == (s.SelectedItems[0] as Skill)?.IconName)) return;
                    CooldownWindowViewModel.Instance.MainSkills.Add(new FixedSkillCooldown(s.SelectedItems[0] as Skill, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                }
                else if (_lastSender == AddButtonGrid2.Name)
                {
                    if (CooldownWindowViewModel.Instance.SecondarySkills.Any(x =>
                        x.Skill.IconName == (s.SelectedItems[0] as Skill)?.IconName)) return;
                    CooldownWindowViewModel.Instance.SecondarySkills.Add(new FixedSkillCooldown(
                        s.SelectedItems[0] as Skill,
                        CooldownWindowViewModel.Instance.GetDispatcher(), false));
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
                if (dropInfo.Data is Skill sk)
                {
                    if (!target.Any(x => x.Skill.IconName == sk.IconName))
                    {
                        target.Add(new FixedSkillCooldown((Skill)dropInfo.Data, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                    }
                }
                else if (dropInfo.Data is Abnormality ab)
                {
                    if (!target.Any(x => x.Skill.IconName == ab.IconName))
                    {
                        target.Add(
                            new FixedSkillCooldown(new Skill(ab.Id, Class.None, ab.Name, ab.ToolTip) { IconName = ab.IconName },
                                CooldownWindowViewModel.Instance.GetDispatcher(), false, CooldownType.Passive));
                    }
                }
                else if (dropInfo.Data is Item i)
                {
                    if (!target.Any(x => x.Skill.IconName == i.IconName))
                    {
                        SessionManager.ItemsDatabase.TryGetItemSkill(i.Id, out var s);
                        target.Add(new FixedSkillCooldown(s, CooldownWindowViewModel.Instance.GetDispatcher(), false, CooldownType.Item));
                    }

                }
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

        private bool a;
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (a) SessionManager.CurrentPlayer.Class = Class.Warrior;
            else SessionManager.CurrentPlayer.Class = Class.Archer;
            CooldownWindowViewModel.Instance.ClearSkills();
            CooldownWindowViewModel.Instance.LoadSkills($"{SessionManager.CurrentPlayer.Class.ToString().ToLower()}-skills.xml", SessionManager.CurrentPlayer.Class);
            a = !a;
        }
    }
}