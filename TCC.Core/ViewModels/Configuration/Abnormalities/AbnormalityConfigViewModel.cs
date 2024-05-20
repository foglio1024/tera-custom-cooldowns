using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace TCC.ViewModels.Configuration.Abnormalities;

public class AbnormalityConfigViewModel : ObservableObject, IDisposable
{
    private readonly DispatcherTimer _searchCooldown;

    private string _searchFilter = "";

    public string SearchFilter
    {
        get => _searchFilter;
        set
        {
            RaiseAndSetIfChanged(value, ref _searchFilter);
            _searchCooldown.Refresh();
        }
    }

    private bool _showAllOnSelf;

    public bool ShowAllOnSelf
    {
        get => _showAllOnSelf;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showAllOnSelf)) return;

            App.Settings.AbnormalitySettings.Self.ShowAll = value;
        }
    }

    private bool _showAllOnGroup;

    public bool ShowAllOnGroup
    {
        get => _showAllOnGroup;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showAllOnGroup)) return;

            App.Settings.AbnormalitySettings.Group.ShowAll = value;

            // triggers in XAML don't work as expected, so here we are
            _selectedAbnormality?.GroupConfigurator.ExN(nameof(AbnormalityConfigurator.CanBeWhitelisted));
        }
    }

    private bool _filterFavorites;

    public bool FilterFavorites
    {
        get => _filterFavorites;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _filterFavorites)) return;
            UpdateFilter();
        }
    }

    private bool _filterSelfWhitelisted;

    public bool FilterSelfWhitelisted
    {
        get => _filterSelfWhitelisted;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _filterSelfWhitelisted)) return;
            UpdateFilter();
        }
    }

    private bool _filterGroupWhitelisted;

    public bool FilterGroupWhitelisted
    {
        get => _filterGroupWhitelisted;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _filterGroupWhitelisted)) return;
            UpdateFilter();
        }
    }

    private bool _filterSelfCollapsible;

    public bool FilterSelfCollapsible
    {
        get => _filterSelfCollapsible;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _filterSelfCollapsible)) return;
            UpdateFilter();
        }
    }

    private bool _filterGroupCollapsible;

    public bool FilterGroupCollapsible
    {
        get => _filterGroupCollapsible;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _filterGroupCollapsible)) return;
            UpdateFilter();
        }
    }

    // needed only as a workaround for triggers not working in XAML, maybe use messages from mvvmtk one day
    private AbnormalityViewModel? _selectedAbnormality;
    public AbnormalityViewModel? SelectedAbnormality
    {
        get => _selectedAbnormality;
        set
        {
            RaiseAndSetIfChanged(value, ref _selectedAbnormality);
        }
    }

    public bool IsFiltering => FilterFavorites || FilterSelfWhitelisted || FilterGroupWhitelisted || FilterSelfCollapsible || FilterGroupCollapsible;

    public ICollectionView AbnormalitiesView { get; set; }

    public ICommand ToggleAllSelfAbnormalitiesCommand { get; }
    public ICommand ToggleAllGroupAbnormalitiesCommand { get; }

    public AbnormalityConfigViewModel()
    {
        _showAllOnSelf = App.Settings.AbnormalitySettings.Self.ShowAll;
        _showAllOnGroup = App.Settings.AbnormalitySettings.Group.ShowAll;

        _searchCooldown = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, OnSearchTriggered, Dispatcher.CurrentDispatcher);

        ObservableCollection<AbnormalityViewModel> abnormalities =
        [
            ..
             Game.DB!.AbnormalityDatabase.Abnormalities.Values
                .Where(a => a is { IsShow: true, CanShow: true })
                .Select(a => new AbnormalityViewModel(a))
                .ToArray()
        ];

        AbnormalitiesView = CollectionViewFactory.CreateCollectionView(abnormalities);

        ToggleAllSelfAbnormalitiesCommand = new RelayCommand(() => ShowAllOnSelf = !ShowAllOnSelf);
        ToggleAllGroupAbnormalitiesCommand = new RelayCommand(() => ShowAllOnGroup = !ShowAllOnGroup);
    }

    private void OnSearchTriggered(object? sender, EventArgs e)
    {
        _searchCooldown.Stop();
        UpdateFilter();
    }

    private void UpdateFilter()
    {
        AbnormalitiesView.Filter = o =>
        {
            if (o is not AbnormalityViewModel a) return false;

            var matchesName = string.IsNullOrWhiteSpace(SearchFilter)
                              || a.Abnormality.Name.Contains(SearchFilter, StringComparison.InvariantCultureIgnoreCase)
                              || a.Abnormality.Id.ToString().Contains(SearchFilter, StringComparison.InvariantCulture);

            var favOk = !FilterFavorites || a.IsFavorite;
            var swOk = !FilterSelfWhitelisted || a.SelfConfigurator.IsWhitelisted;
            var gwOk = !FilterGroupWhitelisted || a.GroupConfigurator.IsWhitelisted;
            var scOk = !FilterSelfCollapsible || a.SelfConfigurator.IsCollapsible;
            var gcOk = !FilterGroupCollapsible || a.GroupConfigurator.IsCollapsible;

            var matchesConditions = (favOk && swOk && gwOk && scOk && gcOk);

            return string.IsNullOrWhiteSpace(SearchFilter)
                ? matchesConditions
                : matchesName && matchesConditions;
        };

        AbnormalitiesView.Refresh();

        InvokePropertyChanged(nameof(IsFiltering));
    }

    public void Dispose()
    {
        AbnormalitiesView.Free();
    }
}