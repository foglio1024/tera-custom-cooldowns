using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.Controls;
using Nostrum.Factories;
using TCC.Data;
using TCC.Data.Npc;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Controls.NPCs
{
    public partial class DragonControl
    {
        private NPC? _dc;
        private readonly DoubleAnimation _shieldArcAn;
        private readonly DoubleAnimation _enrageEndAnim;

        public DragonControl()
        {
            InitializeComponent();
            _shieldArcAn = AnimationFactory.CreateDoubleAnimation(NpcWindowViewModel.Ph1ShieldDuration, from:0, to: 359.99);
            _enrageEndAnim = AnimationFactory.CreateDoubleAnimation(0, from: 359.99, to: 0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = (NPC)DataContext;
            _dc.PropertyChanged += Dc_PropertyChanged;
            _dc.DeleteEvent += Dc_DeleteEvent;
            EnrageLine.LayoutTransform = _dc.EnragePattern != null && _dc.CurrentPercentage > _dc.EnragePattern.Percentage 
                ? new RotateTransform((_dc.CurrentPercentage - _dc.EnragePattern.Percentage) * 3.6) 
                : new RotateTransform(0);
        }
        private void Dc_DeleteEvent()
        {
            if (_dc == null) return;
            _dc.DeleteEvent -= Dc_DeleteEvent;
            _dc.PropertyChanged -= Dc_PropertyChanged;

            Dispatcher?.Invoke(() =>
            {
                try
                {
                    WindowManager.ViewModels.NpcVM.RemoveNPC(_dc, 0);
                }
                catch
                {
                    // ignored
                }
            });
        }
        private void Dc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_dc == null) return;
            switch (e.PropertyName)
            {
                case nameof(_dc.Shield) when _dc.Shield == ShieldStatus.On:
                    ShieldArc.BeginAnimation(Arc.EndAngleProperty, _shieldArcAn);
                    break;
                case nameof(_dc.Shield):
                    _shieldArcAn.BeginTime = null;
                    ShieldArc.BeginAnimation(Arc.EndAngleProperty, _shieldArcAn);
                    break;
                case nameof(_dc.Enraged) when _dc.Enraged:
                    if (_dc.EnragePattern != null)
                        _enrageEndAnim.Duration = TimeSpan.FromMilliseconds(_dc.EnragePattern.Duration);
                    EnrageArc.BeginAnimation(Arc.EndAngleProperty, _enrageEndAnim);
                    break;
                case nameof(_dc.Enraged):
                    EnrageLine.LayoutTransform = _dc.EnragePattern != null && _dc.CurrentPercentage > _dc.EnragePattern.Percentage 
                        ? new RotateTransform((_dc.CurrentPercentage - _dc.EnragePattern.Percentage) * 3.6) 
                        : new RotateTransform(0);
                    break;
                case nameof(_dc.CurrentHP):
                {
                    if (_dc.Enraged)
                    {
                        EnrageLine.LayoutTransform = new RotateTransform(_dc.HPFactor * 359.9);
                    }

                    break;
                }
            }
        }
    }
}
