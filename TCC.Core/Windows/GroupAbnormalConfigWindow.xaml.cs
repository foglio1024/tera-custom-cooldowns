using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per GroupAbnormalConfigWindow.xaml
    /// </summary>
    public partial class GroupAbnormalConfigWindow : Window
    {
        private Class _currentFilter;

        public GroupConfigVM DC => Dispatcher.Invoke(() => DataContext as GroupConfigVM);

        public GroupAbnormalConfigWindow()
        {
            InitializeComponent();
            Dispatcher.Invoke(() => DataContext = new GroupConfigVM());
            DC.ShowAllChanged += OnShowAllChanged;
            OnShowAllChanged();
        }

        private void OnShowAllChanged()
        {
            var an = new DoubleAnimation(DC.ShowAll? .2 : 1, TimeSpan.FromMilliseconds(200));
            MainGrid.BeginAnimation(OpacityProperty, an);
            MainGrid.IsHitTestVisible = !DC.ShowAll;
        }

        public void ShowWindow()
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

        private void PassivitySearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = ((TextBox)sender).Text;
            var view = DC.AbnormalitiesView;
            view.Filter = o => ((GroupAbnormalityVM)o).Abnormality.Name.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            an.Completed += (s, ev) => Hide();
            BeginAnimation(OpacityProperty, an);
            SettingsWriter.Save();
        }

        private void FilterByClass(object sender, RoutedEventArgs e)
        {
            var c = (Class)(sender as FrameworkElement).DataContext;
            var view = DC.AbnormalitiesView;
            if (SearchBox.Text.Length > 0)
            {
                SearchBox.Clear();
                view.Filter = null;
            }
            if (view.Filter == null || c != _currentFilter)
            {
                view.Filter = o => ((GroupAbnormalityVM)o).Classes.Any(x => x.Class == c && x.Selected);
                _currentFilter = c;
            }
            else
            {
                view.Filter = null;
                _currentFilter = Class.None;
            }
            view.Refresh();
            foreach (var x in ClassesButtons.Items)
            {
                var cp = (ContentPresenter)ClassesButtons.ItemContainerGenerator.ContainerFromItem(x);
                var btn = cp.ContentTemplate.FindName("Btn", cp) as Button;
                var dc = ((Class)btn.DataContext);
                if (dc == _currentFilter) btn.Opacity = 1;
                else btn.Opacity = .3;
            }
        }
    }
}

namespace TCC.ViewModels
{
    public class GroupConfigVM : TSPropertyChanged
    {

        public event Action ShowAllChanged;

        public SynchronizedObservableCollection<GroupAbnormalityVM> GroupAbnormals;
        public IEnumerable<Abnormality> Abnormalities => SessionManager.AbnormalityDatabase.Abnormalities.Values.ToList();
        public ICollectionView AbnormalitiesView { get; set; }

        public bool ShowAll
        {
            get => Settings.ShowAllGroupAbnormalities;
            set
            {
                if (Settings.ShowAllGroupAbnormalities == value) return;
                Settings.ShowAllGroupAbnormalities = value;
                _dispatcher.Invoke(() => ShowAllChanged?.Invoke());
                SettingsWriter.Save();
                NPC();
            }
        }
        public List<Class> Classes
        {
            get
            {
                var l = Utils.ListFromEnum<Class>();
                l.Remove(Class.None);
                return l;
            }
        }
        public GroupConfigVM()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            GroupAbnormals = new SynchronizedObservableCollection<GroupAbnormalityVM>(_dispatcher);
            foreach (var abnormality in Abnormalities)
            {
                var abVM = new GroupAbnormalityVM(abnormality);

                GroupAbnormals.Add(abVM);
            }
            AbnormalitiesView = new CollectionViewSource { Source = GroupAbnormals }.View;
            AbnormalitiesView.CurrentChanged += OnAbnormalitiesViewOnCurrentChanged;
            AbnormalitiesView.Filter = null;
        }
        //to keep view referenced
        private void OnAbnormalitiesViewOnCurrentChanged(object s, EventArgs ev)
        {
        }
    }

    public class GroupAbnormalityVM : TSPropertyChanged
    {
        public Abnormality Abnormality { get; }
        public SynchronizedObservableCollection<ClassToggle> Classes { get; }

        public GroupAbnormalityVM(Abnormality ab)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Abnormality = ab;
            Classes = new SynchronizedObservableCollection<ClassToggle>(_dispatcher);
            for (int i = 0; i < 13; i++)
            {
                var ct = new ClassToggle((Class)i, ab.Id);
                if (Settings.GroupAbnormals.ContainsKey(ct.Class)) ct.Selected = Settings.GroupAbnormals[ct.Class].Contains(ab.Id);
                Classes.Add(ct);
            }
            Classes.Add(new ClassToggle(Class.Common, ab.Id)
            {
                Selected = Settings.GroupAbnormals[Class.Common].Contains(ab.Id)
            });

        }
    }

    public class ClassToggle : TSPropertyChanged
    {
        private bool _selected;

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                _selected = value;
                NPC();

            }
        }
        public Class Class { get; }
        public ToggleCommand ToggleCommand { get; set; }
        public uint AbnormalityId { get; }
        public ClassToggle(Class c, uint abId)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Class = c;
            AbnormalityId = abId;
            ToggleCommand = new ToggleCommand(this);
        }
    }

    public class ToggleCommand : ICommand
    {
        private ClassToggle _toggle;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _toggle.Selected = !_toggle.Selected;
            if (_toggle.Selected) Settings.GroupAbnormals[_toggle.Class].Add(_toggle.AbnormalityId);
            else Settings.GroupAbnormals[_toggle.Class].Remove(_toggle.AbnormalityId);
        }

        public event EventHandler CanExecuteChanged;

        public ToggleCommand(ClassToggle t)
        {
            _toggle = t;
        }
    }
}
