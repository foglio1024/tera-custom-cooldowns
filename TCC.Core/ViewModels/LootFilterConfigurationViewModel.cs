using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using System;
using System.ComponentModel;
using TCC.Data;
using TCC.Settings.WindowSettings;

namespace TCC.ViewModels;

public class LootFilterConfigurationViewModel : ThreadSafeObservableObject
{
    public ICollectionViewLiveShaping? ItemsView { get; }
    public LootDistributionWindowSettings Settings { get; }

    string _searchFilter = "";
    public string SearchFilter
    {
        get => _searchFilter;
        set
        {
            if (_searchFilter == value) return;
            _searchFilter = value;
            N();

            var view = (ICollectionView?)ItemsView;
            if (view == null) return;

            view.Filter = o => ((Item)o).Name.IndexOf(_searchFilter, StringComparison.InvariantCultureIgnoreCase) != -1;
            view.Refresh();
        }
    }


    // todo: make 3 lists: items list, pass items list, drop items list, use drag&drop

    protected LootFilterConfigurationViewModel(LootDistributionWindowSettings settings)
    {
        ItemsView = CollectionViewFactory.CreateLiveCollectionView(Game.DB!.ItemsDatabase.Items);

        Settings = settings;
    }
}
