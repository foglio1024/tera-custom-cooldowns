using System.ComponentModel;
using System.Threading;
using System.Windows;
using TCC.Data;

namespace TCC.UI.Controls.Classes.Elements;

public partial class MysticAurasIndicator
{
    private AurasTracker? _context;

    public MysticAurasIndicator()
    {
        InitializeComponent();
    }


    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(this)) return;
        //lazy way of making sure that DataContext is not null
        //TODO: find a better way to do this tho
        while (_context == null)
        {
            _context = (AurasTracker)DataContext;
            Thread.Sleep(500);
        }
        _context.PropertyChanged += OnContextPropertyChanged;
    }

    private void OnContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_context == null) return;

        if (e.PropertyName != "AuraChanged") return;
        Crit.Visibility = _context.CritAura ? Visibility.Visible : Visibility.Hidden;
        Mp.Visibility = _context.ManaAura ? Visibility.Visible : Visibility.Hidden;
        CritRes.Visibility = _context.CritResAura ? Visibility.Visible : Visibility.Hidden;
        Swift.Visibility = _context.SwiftAura ? Visibility.Visible : Visibility.Hidden;
    }
}