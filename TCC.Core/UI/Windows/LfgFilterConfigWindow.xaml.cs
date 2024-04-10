using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using TCC.ViewModels;

namespace TCC.UI.Windows;

public partial class LfgFilterConfigWindow
{
    private readonly LfgListViewModel _dc;

    public LfgFilterConfigWindow(LfgListViewModel vm)
    {
        DataContext = vm;
        _dc = vm;
        RemoveEntryCommand = new RelayCommand<string>(entry =>
        {
            vm.BlacklistedWords.Remove(entry ?? string.Empty);
        });
        InitializeComponent();
    }

    public ICommand RemoveEntryCommand { get; }

    private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.TryDragMove();
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        App.Settings.Save();
        Close();
    }

    private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        var content = ((TextBox)sender).Text.Trim();
        if (string.IsNullOrWhiteSpace(content)) return;
        if (_dc.BlacklistedWords.Contains(content)) return;
        _dc.BlacklistedWords.Add(content);
        ((TextBox) sender).Text = "";
    }
}