using System;
using System.Windows.Input;
using Dragablz;
using Nostrum.WPF;
using TCC.ViewModels.Widgets;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.ViewModels;

public class TabViewModel : HeaderedItemViewModel
{
    public static event Action<Tab, ImportantRemovedArgs>? ImportantRemoved;
    public static event Action<TabViewModel>? TabOpened;

    public static void InvokeImportantRemoved(Tab source, ImportantRemovedArgs e)
    {
        ImportantRemoved?.Invoke(source, e);
    }

    private bool _showImportantPopup;
    public bool ShowImportantPopup
    {
        get => _showImportantPopup;
        set
        {
            if (_showImportantPopup == value) return;
            _showImportantPopup = value;
            FocusManager.PauseTopmost = _showImportantPopup;
            OnPropertyChanged();
        }
    }

    public ICommand TogglePopupCommand { get; }


    public TabViewModel()
    {
        TogglePopupCommand = new RelayCommand(SetPopupStatus); // todo: use RelayCommand<bool>
        TabOpened += OnTabOpened;
    }
    public TabViewModel(object header, object content, bool isSelected = false) : base(header, content, isSelected)
    {
        TogglePopupCommand = new RelayCommand(SetPopupStatus); // todo: use RelayCommand<bool>
        TabOpened += OnTabOpened;
    }

    private void SetPopupStatus(object? par)
    {
        var val = Convert.ToBoolean(par);
        ShowImportantPopup = val;
        if (val) TabOpened?.Invoke(this);
    }

    private void OnTabOpened(TabViewModel vm)
    {
        if (vm == this) return;
        ShowImportantPopup = false;
    }


}