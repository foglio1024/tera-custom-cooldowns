using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per GroupAbnormalConfigWindow.xaml
    /// </summary>
    public partial class GroupAbnormalConfigWindow
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
            var an = new DoubleAnimation(DC.ShowAll ? .2 : 1, TimeSpan.FromMilliseconds(200));
            MainGrid.BeginAnimation(OpacityProperty, an);
            MainGrid.IsHitTestVisible = !DC.ShowAll;
        }

        public void ShowWindow()
        {
            if (Settings.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
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
            an.Completed += (s, ev) =>
            {
                Hide();
                if (Settings.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            };
            BeginAnimation(OpacityProperty, an);
            SettingsWriter.Save();
        }

        private void FilterByClass(object sender, RoutedEventArgs e)
        {
            var c = (Class)((FrameworkElement) sender).DataContext;
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
                if (btn?.DataContext == null) continue;
                var dc = (Class)btn.DataContext;
                if (dc == _currentFilter) btn.Opacity = 1;
                else btn.Opacity = .3;
            }
        }
    }
}

namespace TCC.ViewModels
{
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
            Dispatcher = Dispatcher.CurrentDispatcher;
            Class = c;
            AbnormalityId = abId;
            ToggleCommand = new ToggleCommand(this);
        }
    }

    public class ToggleCommand : ICommand
    {
        private readonly ClassToggle _toggle;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _toggle.Selected = !_toggle.Selected;
            if (_toggle.Selected) Settings.Settings.GroupAbnormals[_toggle.Class].Add(_toggle.AbnormalityId);
            else Settings.Settings.GroupAbnormals[_toggle.Class].Remove(_toggle.AbnormalityId);
        }

        public event EventHandler CanExecuteChanged;

        public ToggleCommand(ClassToggle t)
        {
            _toggle = t;
        }
    }
}
