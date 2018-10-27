using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Annotations;
using TCC.ViewModels;

namespace TCC.Controls.Npc
{
    /// <summary>
    /// Interaction logic for SmallMobControl.xaml
    /// </summary>
    public partial class SmallMobControl : INotifyPropertyChanged
    {
        private const uint DeleteDelay = 0;
        private DispatcherTimer _t;
        private DoubleAnimation _hpAnim;
        private Data.Npc _dc;
        public SmallMobControl()
        {
            InitializeComponent();
        }
        public bool Compact => BossGageWindowViewModel.Instance.IsCompact;

        private void SmallMobControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (Data.Npc)DataContext;
            _dc.DeleteEvent += Dc_DeleteEvent;
            _dc.PropertyChanged += OnDcPropertyChanged;
            _t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(DeleteDelay) };
            _t.Tick += (s, ev) =>
            {
                RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                        new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)));
                _t.Stop();
                _dc = null;
                _t = null;
            };
            RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            BossGageWindowViewModel.Instance.NpcListChanged += OnNpcListChanged;
            SettingsWindowViewModel.AbnormalityShapeChanged += OnViewModelPropertyChanged;

            _hpAnim = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase()
            };
        }

        private void OnViewModelPropertyChanged()
        {
            Abnormalities.ItemTemplateSelector = null;
            Abnormalities.ItemTemplateSelector = Application.Current.FindResource("RaidAbnormalityTemplateSelector") as DataTemplateSelector;
        }

        private void OnDcPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Data.Npc.CurrentFactor)) return;
            if (Compact)
            {
                _hpAnim.To = _dc.CurrentFactor * 359.9;
                ExternalArc.BeginAnimation(Arc.EndAngleProperty, _hpAnim);
            }
            else
            {
                _hpAnim.To = _dc.CurrentFactor;
                HpBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            }
        }

        private void OnNpcListChanged()
        {
            NPC(nameof(Compact));
        }

        private void Dc_DeleteEvent()
        {
            SettingsWindowViewModel.AbnormalityShapeChanged -= OnViewModelPropertyChanged;
            BossGageWindowViewModel.Instance.NpcListChanged -= OnNpcListChanged;
            if (_dc == null) return;
            _dc.DeleteEvent -= Dc_DeleteEvent;
            _dc.PropertyChanged -= OnDcPropertyChanged;

            Dispatcher.Invoke(() =>
            {
                _t.Start();

                try
                {
                    BossGageWindowViewModel.Instance.RemoveMe(_dc, DeleteDelay);
                }
                catch
                {
                    // ignored
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
