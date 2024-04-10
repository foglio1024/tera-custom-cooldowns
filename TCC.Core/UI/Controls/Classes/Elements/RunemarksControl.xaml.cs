using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using TCC.Data;
using TCC.R;
using TCC.ViewModels.ClassManagers;

namespace TCC.UI.Controls.Classes.Elements;

public partial class RunemarksControl
{
    private ValkyrieLayoutViewModel? _dc;

    public RunemarksControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object _, RoutedEventArgs __)
    {
        _dc = (ValkyrieLayoutViewModel) DataContext;
        if (_dc == null) return;
        _dc.RunemarksCounter.PropertyChanged += OnRunemarksPropertyChanged;
    }

    private void OnRunemarksPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Counter.Val)) return;
        if (_dc == null) return;
        for (var i = 0; i < _dc.RunemarksCounter.MaxValue; i++)
        {
            if (i < _dc.RunemarksCounter.Val)
            {
                RunemarkContainer.Children[i].Opacity = 1;
                ((Shape)RunemarkContainer.Children[i]).Fill =
                    _dc.RunemarksCounter.Val == _dc.RunemarksCounter.MaxValue
                        ? Brushes.MaxRunemarkBrush : 
                        Brushes.RunemarkBrush; 

            }
            else
            {
                RunemarkContainer.Children[i].Opacity = .1;
                ((Shape)RunemarkContainer.Children[i]).Fill = System.Windows.Media.Brushes.White;
            }
        }

        Effect = _dc.RunemarksCounter.Val == _dc.RunemarksCounter.MaxValue ? new DropShadowEffect
        {
            BlurRadius = 10,
            ShadowDepth = 0,
            Color = Colors.MaxRunemarkColor 
        } : null;
    }
}