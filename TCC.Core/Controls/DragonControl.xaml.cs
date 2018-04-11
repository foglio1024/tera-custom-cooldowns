using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Annotations;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for DragonControl.xaml
    /// </summary>
    public partial class DragonControl : UserControl, INotifyPropertyChanged
    {
        public DragonControl()
        {
            InitializeComponent();
            shieldArcAn = new DoubleAnimation(0, 359.99, TimeSpan.FromSeconds(BossGageWindowViewModel.Ph1ShieldDuration));

        }

        Npc dc;
        private string _enrageLabel;
        public string EnrageLabel
        {
            get => _enrageLabel;
            set
            {
                if (_enrageLabel == value) return;
                _enrageLabel = value;
                NotifyPropertyChanged(nameof(EnrageLabel));
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dc = (Npc)DataContext;
            dc.PropertyChanged += Dc_PropertyChanged;
            dc.DeleteEvent += Dc_DeleteEvent;
            enrageLine.LayoutTransform = dc.CurrentPercentage > dc.EnragePattern.Percentage ? new RotateTransform((dc.CurrentPercentage - dc.EnragePattern.Percentage) * 3.6) : new RotateTransform(0);

        }

        DoubleAnimation shieldArcAn;
        private void Dc_DeleteEvent() => Dispatcher.Invoke(() =>
        {
            try
            {
                BossGageWindowViewModel.Instance.RemoveMe((Npc)DataContext);
            }
            catch { }
        });

        private void Dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(dc.Shield))
            {
                if (dc.Shield == ShieldStatus.On)
                {
                    shieldArcAn = new DoubleAnimation(0, 359.99, TimeSpan.FromSeconds(BossGageWindowViewModel.Ph1ShieldDuration));
                    shieldArc.BeginAnimation(Arc.EndAngleProperty, shieldArcAn);
                }
                else
                {
                    shieldArcAn.BeginTime = null;
                    shieldArc.BeginAnimation(Arc.EndAngleProperty, shieldArcAn);
                }
            }
            else if(e.PropertyName == nameof(dc.Enraged))
            {
                if (dc.Enraged)
                {
                    enrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.99, 0, TimeSpan.FromSeconds(dc.EnragePattern.Duration)));
                }
                else
                {
                    enrageLine.LayoutTransform = dc.CurrentPercentage > dc.EnragePattern.Percentage ? new RotateTransform((dc.CurrentPercentage - dc.EnragePattern.Percentage) * 3.6) : new RotateTransform(0);
                }
            }
            else if(e.PropertyName == nameof(dc.CurrentHP))
            {
                if (dc.Enraged)
                {
                    enrageLine.LayoutTransform = new RotateTransform(dc.CurrentFactor * 359.9);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
