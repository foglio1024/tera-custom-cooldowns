using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.ViewModels.ClassManagers;

namespace TCC.UI.Controls.Classes
{
    public partial class GunnerLayout
    {
        private GunnerLayoutVM? _dc;
        private readonly DoubleAnimation _an;
        public GunnerLayout()
        {
            _an = AnimationFactory.CreateDoubleAnimation(150, to: 400);
            InitializeComponent();
        }
        private void GunnerLayout_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (GunnerLayoutVM) DataContext;
            _an.To = _dc.StaminaTracker.Factor * 359.99 + 40;
            _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;

        }

        private void ST_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
            if (_dc == null) return;
            _an.To = _dc.StaminaTracker.Factor*(359.99 - MainReArc.StartAngle*2) + MainReArc.StartAngle;
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
        }
    }
}
