using Dragablz;
using GongSolutions.Wpf.DragDrop;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.Factories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;
using TeraDataLite;

namespace TCC.UI.Controls.Skills
{
    //TODO: refactor this
    public partial class FixedSkillContainers 
    {
        private object[] _mainOrder = { };
        private object[] _secondaryOrder = { };
        private readonly DoubleAnimation _opacityUp;
        private readonly DoubleAnimation _opacityDown;
        private static readonly Action EmptyDelegate = delegate { };
        private CooldownWindowViewModel? VM { get; set; }

        public SkillDropHandler DropHandler { get; private set; }


        public FixedSkillContainers()
        {
            DropHandler = new SkillDropHandler();
            InitializeComponent();
            AddHandler(DragablzItem.DragStarted, new DragablzDragStartedEventHandler(ItemDragStarted), true);
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
            _opacityUp = AnimationFactory.CreateDoubleAnimation(250, 1, easing: true);
            _opacityDown = AnimationFactory.CreateDoubleAnimation(250, 0, easing: true, delay: 1000);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            VM = (CooldownWindowViewModel?)Window.GetWindow(this)?.DataContext;
            if (VM == null) return;
            VM.SecondarySkills.CollectionChanged += SecondarySkills_CollectionChanged;
            VM.MainSkills.CollectionChanged += MainSkills_CollectionChanged;
            VM.SkillsLoaded += OnSkillsLoaded;

            OnSkillShapeChanged();
            SettingsWindowViewModel.SkillShapeChanged += OnSkillShapeChanged;
        }

        private void OnUnloaded(object _, RoutedEventArgs __)
        {
            SettingsWindowViewModel.AbnormalityShapeChanged -= OnSkillShapeChanged;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;

            if (VM == null) return;
            VM.SecondarySkills.CollectionChanged -= SecondarySkills_CollectionChanged;
            VM.MainSkills.CollectionChanged -= MainSkills_CollectionChanged;
            VM.SkillsLoaded -= OnSkillsLoaded;
        }

        private void OnSkillsLoaded()
        {
            //really absurd way of fixing order issue
            if (VM == null) return;
            Dispatcher?.Invoke(() =>
            {
                ReorderSkillContainer(MainSkills, VM.MainSkills);
                ReorderSkillContainer(SecSkills, VM.SecondarySkills);
            });
        }

        private void ReorderSkillContainer(ItemsControl container, IEnumerable<Cooldown> collection)
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
                if (!(positions[j] > positions[j + 1])) continue;
                needsReorder = true;
                break;
            }

            if (!needsReorder) return;
            container.ItemsSource = null;
            container.ItemsSource = collection;
        }

        private void MainSkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) return;
            RefreshMeasure(MainSkills);
            VM?.SaveConfig();
        }

        private void SecondarySkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) return;
            RefreshMeasure(SecSkills);
            VM?.SaveConfig();
        }

        private void ItemDragStarted(object sender, DragablzDragStartedEventArgs e)
        {
        }

        private void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            var cd = (Cooldown)e.DragablzItem.DataContext;
            if (VM == null) return;
            if (VM.MainSkills.Contains(cd))
            {
                Reorder(VM.MainSkills, _mainOrder);
            }
            else if (VM.SecondarySkills.Contains(cd))
            {
                Reorder(VM.SecondarySkills, _secondaryOrder);
            }
            VM.SaveConfig();
        }

        private void Reorder(ObservableCollection<Cooldown> list, object[] order)
        {
            if (order == null) return;
            for (var j = 0; j < list.Count; j++)
            {
                var newIndex = order.ToList().IndexOf(list[j]);
                var oldIndex = j;
                list.Move(oldIndex, newIndex);
            }
        }

        private void MainSkillOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _mainOrder = e.NewOrder;
        }

        private void SecondarySkillOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _secondaryOrder = e.NewOrder;
        }

        private void MainSkillsGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateSettingsButton(true);
        }

        private void MainSkillsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateSettingsButton(false);
            OnSkillsLoaded();
        }

        private void FixedSkillContainers_OnDragOver(object sender, DragEventArgs e)
        {
            if (SecSkills.ActualWidth != 0) return;
            SecSkills.MinWidth = 59;
            //VM.IsDragging = true;
        }

        private void FixedSkillContainers_OnDragLeave(object sender, DragEventArgs e)
        {
            //Task.Delay(500).ContinueWith(t =>
            //{
            //    Dispatcher.InvokeAsync(() =>
            //    {

            //        VM.IsDragging = false;

            //    });
            //});
            SecSkills.MinWidth = 0;
        }

        private void MainSkills_OnDrop(object sender, DragEventArgs e)
        {
            MainSkills.InvalidateArrange();
            RefreshMeasure(MainSkills);
        }

        private void OpenCooldownSettings(object sender, RoutedEventArgs e)
        {
            VM?.OnShowSkillConfigHotkeyPressed();
        }

        private void OnSkillShapeChanged()
        {
            OtherSkills.RefreshTemplate("NormalSkillTemplateSelector");
            ItemSkills.RefreshTemplate("NormalSkillTemplateSelector");
            // NOTE: the above can't be done for fixed skill ICs,
            // because they use ControlTemplate and not DataTemplate
            RefreshControlTemplate(MainSkills);
            RefreshControlTemplate(SecSkills);
        }

        private void RefreshControlTemplate(ItemsControl ic)
        {
            Dispatcher?.InvokeAsync(() =>
            {
                ic.ItemContainerStyle =
                    FindResource(App.Settings.SkillShape == ControlShape.Round
                        ? "RoundDragableStyle"
                        : "SquareDragableStyle") as Style;
            }, DispatcherPriority.Background);
        }

        private static void RefreshMeasure(UIElement ic)
        {
            ic.InvalidateMeasure();
            ic.Dispatcher?.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        private void AnimateSettingsButton(bool open)
        {
            SettingsButton.BeginAnimation(OpacityProperty, open ? _opacityUp : _opacityDown);
        }

        public class SkillDropHandler : IDropTarget
        {
            public void DragOver(IDropInfo dropInfo)
            {
                dropInfo.Effects = DragDropEffects.Move;
            }

            public void Drop(IDropInfo dropInfo)
            {
                var target = (TSObservableCollection<Cooldown>)dropInfo.TargetCollection;
                switch (dropInfo.Data)
                {
                    case Skill sk:
                        if (target.All(x => x.Skill.IconName != sk.IconName))
                            target.Insert(dropInfo.InsertIndex, new Cooldown(sk, false));
                        break;
                    case Abnormality ab:
                        if (target.All(x => x.Skill.IconName != ab.IconName))
                            target.Insert(dropInfo.InsertIndex,
                                new Cooldown(new Skill(ab.Id, Class.None, ab.Name, ab.ToolTip) { IconName = ab.IconName },
                                    false, CooldownType.Passive));
                        break;
                    case Item i:
                        if (target.All(x => x.Skill.IconName != i.IconName))
                        {
                            Game.DB.ItemsDatabase.TryGetItemSkill(i.Id, out var s);
                            target.Insert(dropInfo.InsertIndex, new Cooldown(s, false, CooldownType.Item));
                        }

                        break;
                }

                var tmp = target.ToList();

                //force correct order as it's not preserved
                target.Clear();
                tmp.ForEach(x => target.Add(x));

                const ulong delay = 500;
                //wait a bit and restart any running skill
                Task.Delay(TimeSpan.FromMilliseconds(delay)).ContinueWith(t =>
                {
                    WindowManager.ViewModels.CooldownsVM.MainSkills.ToList().ForEach(x =>
                    {
                        if (x.Seconds > 0) x.Start(Convert.ToUInt64(x.Seconds * 1000 - delay));
                    });
                    WindowManager.ViewModels.CooldownsVM.SecondarySkills.ToList().ForEach(x =>
                    {
                        if (x.Seconds > 0) x.Start(Convert.ToUInt64(x.Seconds * 1000 - delay));
                    });
                });

                WindowManager.ViewModels.CooldownsVM.SaveConfig();
            }
        }


    }
}