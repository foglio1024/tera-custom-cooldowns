using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public partial class DragonControl
    {
        private Data.NPCs.NPC _dc;
        private DoubleAnimation _shieldArcAn;

        public DragonControl()
        {
            InitializeComponent();
            _shieldArcAn = new DoubleAnimation(0, 359.99, TimeSpan.FromSeconds(BossGageWindowViewModel.Ph1ShieldDuration));

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = (Data.NPCs.NPC)DataContext;
            _dc.PropertyChanged += Dc_PropertyChanged;
            _dc.DeleteEvent += Dc_DeleteEvent;
            EnrageLine.LayoutTransform = _dc.CurrentPercentage > _dc.EnragePattern.Percentage ? new RotateTransform((_dc.CurrentPercentage - _dc.EnragePattern.Percentage) * 3.6) : new RotateTransform(0);

        }

        private void Dc_DeleteEvent()
        {
            if (_dc == null) return;
            _dc.DeleteEvent -= Dc_DeleteEvent;
            _dc.PropertyChanged -= Dc_PropertyChanged;

            Dispatcher.Invoke(() =>
            {

                try
                {
                    WindowManager.BossWindow.VM.RemoveMe(_dc, 0);
                }
                catch
                {
                    // ignored
                }
            });
        }

        private void Dc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_dc.Shield))
            {
                if (_dc.Shield == ShieldStatus.On)
                {
                    _shieldArcAn = new DoubleAnimation(0, 359.99, TimeSpan.FromSeconds(BossGageWindowViewModel.Ph1ShieldDuration));
                    ShieldArc.BeginAnimation(Arc.EndAngleProperty, _shieldArcAn);
                }
                else
                {
                    _shieldArcAn.BeginTime = null;
                    ShieldArc.BeginAnimation(Arc.EndAngleProperty, _shieldArcAn);
                }
            }
            else if (e.PropertyName == nameof(_dc.Enraged))
            {
                if (_dc.Enraged)
                {
                    EnrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.99, 0, TimeSpan.FromSeconds(_dc.EnragePattern.Duration)));
                }
                else
                {
                    EnrageLine.LayoutTransform = _dc.CurrentPercentage > _dc.EnragePattern.Percentage ? new RotateTransform((_dc.CurrentPercentage - _dc.EnragePattern.Percentage) * 3.6) : new RotateTransform(0);
                }
            }
            else if (e.PropertyName == nameof(_dc.CurrentHP))
            {
                if (_dc.Enraged)
                {
                    EnrageLine.LayoutTransform = new RotateTransform(_dc.HPFactor * 359.9);
                }
            }
        }
    }
}
