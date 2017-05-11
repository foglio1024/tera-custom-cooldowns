using System.Windows;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : BaseINPC
    {
        private bool isCooldownWindowVisible;
        public bool IsCooldownWindowVisible
        {
            get
            {
                if (SettingsManager.CooldownWindowSettings.Visibility == Visibility.Visible)
                {
                    isCooldownWindowVisible = true;
                    return isCooldownWindowVisible;
                }
                else
                {
                    isCooldownWindowVisible = false;
                    return isCooldownWindowVisible;
                }
            }
            set
            {
                if (value == isCooldownWindowVisible) return;

                isCooldownWindowVisible = value;
                if (isCooldownWindowVisible)
                {
                    WindowManager.CooldownWindow.Dispatcher.Invoke(() => WindowManager.CooldownWindow.Visibility = Visibility.Visible);
                    SettingsManager.CooldownWindowSettings.Visibility = Visibility.Visible;

                }
                else
                {
                    WindowManager.CooldownWindow.Dispatcher.Invoke(() => WindowManager.CooldownWindow.Visibility = Visibility.Hidden);
                    SettingsManager.CooldownWindowSettings.Visibility = Visibility.Hidden; ;
                }

                RaisePropertyChanged("IsCooldownWindowVisible");
            }
        }
        private bool isBuffWindowVisible;
        public bool IsBuffWindowVisible
        {
            get
            {
                if (SettingsManager.BuffBarWindowSettings.Visibility == Visibility.Visible)
                {
                    isBuffWindowVisible = true;
                    return isBuffWindowVisible;
                }
                else
                {
                    isBuffWindowVisible = false;
                    return isBuffWindowVisible;
                }
            }
            set
            {
                if (value == isBuffWindowVisible) return;

                isBuffWindowVisible = value;
                if (isBuffWindowVisible)
                {
                    WindowManager.BuffBar.Dispatcher.Invoke(() => WindowManager.BuffBar.Visibility = Visibility.Visible);
                    SettingsManager.BuffBarWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.BuffBar.Dispatcher.Invoke(() => WindowManager.BuffBar.Visibility = Visibility.Hidden);
                    SettingsManager.BuffBarWindowSettings.Visibility = Visibility.Hidden;
                }
                RaisePropertyChanged("IsBuffWindowVisible");


            }
        }
        public bool isBossWindowVisible;
        public bool IsBossWindowVisible
        {
            get
            {
                if (SettingsManager.BossGaugeWindowSettings.Visibility == Visibility.Visible)
                {
                    isBossWindowVisible = true;
                    return isBossWindowVisible;
                }
                else
                {
                    isBossWindowVisible = false;
                    return isBossWindowVisible;
                }
            }
            set
            {
                if (value == isBossWindowVisible) return;
                isBossWindowVisible = value;

                if (isBossWindowVisible)
                {
                    WindowManager.BossGauge.Dispatcher.Invoke(() => WindowManager.BossGauge.Visibility = Visibility.Visible);
                    SettingsManager.BossGaugeWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.BossGauge.Dispatcher.Invoke(() => WindowManager.BossGauge.Visibility = Visibility.Hidden);
                    SettingsManager.BossGaugeWindowSettings.Visibility = Visibility.Hidden;
                }

                RaisePropertyChanged("IsBossWindowVisible");
            }
        }
        private bool isCharacterWindowVisible;
        public bool IsCharacterWindowVisible
        {
            get
            {
                if (SettingsManager.CharacterWindowSettings.Visibility == Visibility.Visible)
                {
                    isCharacterWindowVisible = true;
                    return isCharacterWindowVisible;
                }
                else
                {
                    isCharacterWindowVisible = false;
                    return isCharacterWindowVisible;
                }
            }
            set
            {
                if (isCharacterWindowVisible == value) return;

                isCharacterWindowVisible = value;
                if (isCharacterWindowVisible)
                {
                    WindowManager.CharacterWindow.Dispatcher.Invoke(() => WindowManager.CharacterWindow.Visibility = Visibility.Visible);
                    SettingsManager.CharacterWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.CharacterWindow.Dispatcher.Invoke(() => WindowManager.CharacterWindow.Visibility = Visibility.Hidden);
                    SettingsManager.CharacterWindowSettings.Visibility = Visibility.Hidden;
                }

                RaisePropertyChanged("IsCharacterWindowVisible");
            }
        }

        private bool isCooldownWindowTransparent;
        public bool IsCooldownWindowTransparent
        {
            get { return SettingsManager.CooldownWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.CooldownWindowSettings.ClickThru == value) return;
                SettingsManager.CooldownWindowSettings.ClickThru = value;
                WindowManager.CooldownWindow.Dispatcher.Invoke(() => WindowManager.CooldownWindow.SetClickThru(SettingsManager.CooldownWindowSettings.ClickThru));
                RaisePropertyChanged("IsCooldownWindowTransparent");
            }
        }
        private bool isCharacterWindowTransparent;
        public bool IsCharacterWindowTransparent
        {
            get { return SettingsManager.CharacterWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.CharacterWindowSettings.ClickThru == value) return;
                SettingsManager.CharacterWindowSettings.ClickThru = value;
                WindowManager.CharacterWindow.Dispatcher.Invoke(() => WindowManager.CharacterWindow.SetClickThru(SettingsManager.CharacterWindowSettings.ClickThru));
                RaisePropertyChanged("IsCharacterWindowTransparent");
            }
        }
        private bool isBuffWindowTransparent;
        public bool IsBuffWindowTransparent
        {
            get { return SettingsManager.BuffBarWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.BuffBarWindowSettings.ClickThru == value) return;
                SettingsManager.BuffBarWindowSettings.ClickThru = value;
                WindowManager.BuffBar.Dispatcher.Invoke(() =>
                WindowManager.BuffBar.SetClickThru(SettingsManager.BuffBarWindowSettings.ClickThru));
                RaisePropertyChanged("IsBuffWindowTransparent");
            }
        }
        private bool isBossWindowTransparent;
        public bool IsBossWindowTransparent
        {
            get { return SettingsManager.BossGaugeWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.BossGaugeWindowSettings.ClickThru == value) return;
                SettingsManager.BossGaugeWindowSettings.ClickThru = value;
                WindowManager.BossGauge.Dispatcher.Invoke(() => WindowManager.BossGauge.SetClickThru(SettingsManager.BossGaugeWindowSettings.ClickThru));
                RaisePropertyChanged("IsBossWindowTransparent");
            }
        }

        public SettingsWindowViewModel()
        {

            WindowManager.CooldownWindow.PropertyChanged += CooldownWindow_PropertyChanged;
            WindowManager.CharacterWindow.PropertyChanged += CharacterWindow_PropertyChanged;
            WindowManager.BossGauge.PropertyChanged += BossGauge_PropertyChanged;
            WindowManager.BuffBar.PropertyChanged += BuffBar_PropertyChanged;
        }

        private void BuffBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                RaisePropertyChanged("IsBuffWindowVisible");
            }
            else if (e.PropertyName == "ClickThru")
            {
                IsBuffWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
        private void BossGauge_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                RaisePropertyChanged("IsBossWindowVisible");

            }
            else if (e.PropertyName == "ClickThru")
            {
                IsBossWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
        private void CharacterWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                RaisePropertyChanged("IsCharacterWindowVisible");
            }
            else if (e.PropertyName == "ClickThru")
            {
                IsCharacterWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
        private void CooldownWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                RaisePropertyChanged("IsCooldownWindowVisible");
            }
            else if (e.PropertyName == "ClickThru")
            {
                IsCooldownWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
    }
}
