using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.Factories;
using TCC.ViewModels;
using TeraDataLite;

namespace TCC.Windows
{
    public partial class GroupAbnormalConfigWindow
    {
        private Class _currentFilter;
        private readonly DispatcherTimer _searchCooldown;
        private string _searchText;

        public GroupConfigVM DC { get; private set; }

        public GroupAbnormalConfigWindow() : base(true)
        {
            InitializeComponent();
            Dispatcher?.Invoke(() => DataContext = new GroupConfigVM());
            DC = (GroupConfigVM) DataContext;
            _searchCooldown = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, OnSearchTriggered, Dispatcher ?? throw new InvalidOperationException());

        }
        private void OnSearchTriggered(object sender, EventArgs e)
        {
            _searchCooldown.Stop();
            if (_searchText == null) return;
            var view = DC.AbnormalitiesView;
            view.Filter = o => ((GroupAbnormalityVM)o).Abnormality.Name.IndexOf(_searchText, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }



        private void PassivitySearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = ((TextBox)sender).Text;
            if (_searchText == null) return;
            _searchCooldown.Refresh();
        }


        private void Close(object sender, RoutedEventArgs e)
        {
            App.Settings.Save();
            var anim = AnimationFactory.CreateDoubleAnimation(200, 1, completed: (_, __) =>
             {
                 Close();
                 if (App.Settings.ForceSoftwareRendering)
                     RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
             });
            BeginAnimation(OpacityProperty, anim);
        }

        private void FilterByClass(object sender, RoutedEventArgs e)
        {
            var c = (Class)((FrameworkElement)sender).DataContext;
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

            //TODO: this is bad and i should feel bad
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
                N();

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
            if (_toggle.Selected) App.Settings.GroupWindowSettings.GroupAbnormals[_toggle.Class].Add(_toggle.AbnormalityId);
            else App.Settings.GroupWindowSettings.GroupAbnormals[_toggle.Class].Remove(_toggle.AbnormalityId);
        }
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        public ToggleCommand(ClassToggle t)
        {
            _toggle = t;
        }
    }
}