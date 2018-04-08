using Dragablz;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per FixedSkillContainers.xaml
    /// </summary>
    public partial class FixedSkillContainers : UserControl
    {
        private object[] _mainOrder;
        private object[] _secondaryOrder;
        private DispatcherTimer mainButtonTimer;
        private DispatcherTimer secButtonTimer;
        private static Action EmptyDelegate = delegate () { };
        private string lastSender = "";
        public FixedSkillContainers()
        {
            InitializeComponent();
            AddHandler(DragablzItem.DragStarted, new DragablzDragStartedEventHandler(ItemDragStarted), true);
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
            mainButtonTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            secButtonTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            mainButtonTimer.Tick += MainButtonTimer_Tick;
            secButtonTimer.Tick += SecButtonTimer_Tick;
            SelectionPopup.Closed += SelectionPopup_Closed;
            CooldownWindowViewModel.Instance.SecondarySkills.CollectionChanged += SecondarySkills_CollectionChanged;
        }


        private void SecondarySkills_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            secSkills.InvalidateMeasure();
            secSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        private void MainButtonTimer_Tick(object sender, EventArgs e)
        {
            mainButtonTimer.Stop();
            if (!MainSkillsGrid.IsMouseOver && !SelectionPopup.IsMouseOver)
            {
                if (SelectionPopup.IsOpen == true) SelectionPopup.IsOpen = false;
                AnimateAddButton(false, spacer, AddButtonGrid);
            }
        }
        private void SecButtonTimer_Tick(object sender, EventArgs e)
        {
            secButtonTimer.Stop();
            if (!SecSkillsGrid.IsMouseOver && !SelectionPopup.IsMouseOver)
            {
                if (SelectionPopup.IsOpen == true) SelectionPopup.IsOpen = false;
                AnimateAddButton(false, spacer2, AddButtonGrid2);
            }

        }
        private void AnimateAddButton(bool open, Grid targetspacer, Grid addButtonGrid)
        {
            if(open && (targetspacer.LayoutTransform as ScaleTransform).ScaleX == 1) return;
            if(!open && (targetspacer.LayoutTransform as ScaleTransform).ScaleX == 0) return;
            var to = open ? 1 : 0;
            var from = open ? 0 : 1;
            targetspacer.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty,
                new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
            addButtonGrid.BeginAnimation(OpacityProperty,
                new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
        }
        private void SelectionPopup_Closed(object sender, EventArgs e)
        {
            mainButtonTimer.Start();
            secButtonTimer.Start();
            ChoiceListBox.UnselectAll();
        }
        private void ItemDragStarted(object sender, DragablzDragStartedEventArgs e)
        {
            var item = e.DragablzItem.DataContext;
            FocusManager.FocusTimer.Enabled = false;
            WindowManager.IsFocused = true;


        }
        private void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            //var item = e.DragablzItem.DataContext;
            if ((sender as FrameworkElement).Name == "mainSkills")
            {
                CooldownWindowViewModel.Instance.MainSkills.Clear();
                foreach (var i in _mainOrder)
                {
                    CooldownWindowViewModel.Instance.MainSkills.Add(i as FixedSkillCooldown);
                }
            }
            else if ((sender as FrameworkElement).Name == "secSkills")
                foreach (var i in _secondaryOrder)
                {
                    CooldownWindowViewModel.Instance.SecondarySkills.Add(i as FixedSkillCooldown);
                }
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
        private void UserControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainSkills.InvalidateMeasure();
            mainSkills.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        private void AddButtonPressed(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //CooldownWindowViewModel.Instance.MainSkills.Add(new FixedSkillCooldown(new Skill(181100, Class.Warrior, "", ""), CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(), true));
            SelectionPopup.IsOpen = true;
            if (ChoiceListBox.ItemsSource == null) ChoiceListBox.ItemsSource = CooldownWindowViewModel.Instance.ChoiceList;
            lastSender = (sender as Grid).Name;
        }
        private void MainSkillsGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateAddButton(true, spacer, AddButtonGrid);
            if(CooldownWindowViewModel.Instance.SecondarySkills.Count == 0) AnimateAddButton(true, spacer2, AddButtonGrid2);
        }
        private void MainSkillsGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mainButtonTimer.Start();
        }
        private void SelectionPopup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SelectionPopup.IsOpen = false;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = sender as ListBox;
            if (s.SelectedItems.Count > 0)
            {
                if (lastSender == AddButtonGrid.Name)
                {
                    if (_mainOrder.Any(x => ((FixedSkillCooldown)x).Skill.IconName == (s.SelectedItems[0] as Skill).IconName)) return;
                    CooldownWindowViewModel.Instance.MainSkills.Add(new FixedSkillCooldown(s.SelectedItems[0] as Skill, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                }
                else if (lastSender == AddButtonGrid2.Name)
                {
                    if (_secondaryOrder.Any(x => ((FixedSkillCooldown)x).Skill.IconName == (s.SelectedItems[0] as Skill).IconName)) return;
                    CooldownWindowViewModel.Instance.SecondarySkills.Add(new FixedSkillCooldown(s.SelectedItems[0] as Skill, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                }
                ChoiceListBox.ItemsSource = null;
            }
            SelectionPopup.IsOpen = false;
        }
        private void SecondarySkillsGridMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateAddButton(true, spacer2, AddButtonGrid2);
        }
        private void SecSkillsGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            secButtonTimer.Start();

        }
    }
}
