using System;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.Controls;
using Nostrum.Factories;
using TCC.ViewModels;

namespace TCC.UI.Controls.Classes
{
    public partial class ValkyrieLayout
    {
        private ValkyrieLayoutVM? _dc;
        private readonly DoubleAnimation _an;
        private readonly DoubleAnimation _rag;

        public ValkyrieLayout()
        {
            _an = AnimationFactory.CreateDoubleAnimation(150, 0);
            _rag = AnimationFactory.CreateDoubleAnimation(0, from: 318, to: 42);
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = (ValkyrieLayoutVM)DataContext;
            _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;
            _dc.Ragnarok.Effect.Started += OnRagnarokStarted;
        }

        private void OnRagnarokStarted(ulong duration, Data.CooldownMode mode)
        {
            _rag.Duration = TimeSpan.FromMilliseconds(duration);
            RagArc.BeginAnimation(Arc.EndAngleProperty, _rag);
        }

        private void ST_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_dc == null) return;
            if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
            if (!_dc.Ragnarok.Effect.IsAvailable) return;
            var to = _dc.StaminaTracker.Factor * (359.99 - MainReArc.StartAngle * 2) + MainReArc.StartAngle;
            _an.To = double.IsNaN(to) ? 0 : to;
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
        }
    }
}
