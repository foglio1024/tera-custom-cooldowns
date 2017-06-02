using System.Windows;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : BaseINPC
    {
        //visibility settings
        private bool isCooldownWindowVisible;
        public bool IsCooldownWindowVisible
        {
            get
            {
                if (SettingsManager.CooldownWindowSettings.Visibility == Visibility.Visible)
                {
                    isCooldownWindowVisible = true;
                }
                else
                {
                    isCooldownWindowVisible = false;
                }
                return isCooldownWindowVisible;
            }
            set
            {
                if (value == isCooldownWindowVisible) return;

                isCooldownWindowVisible = value;
                if (isCooldownWindowVisible)
                {
                    WindowManager.CooldownWindow.SetVisibility(Visibility.Visible);
                    SettingsManager.CooldownWindowSettings.Visibility = Visibility.Visible;

                }
                else
                {
                    WindowManager.CooldownWindow.SetVisibility(Visibility.Hidden);
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
                }
                else
                {
                    isBuffWindowVisible = false;
                }
                return isBuffWindowVisible;
            }
            set
            {
                if (value == isBuffWindowVisible) return;

                isBuffWindowVisible = value;
                if (isBuffWindowVisible)
                {
                    WindowManager.BuffBar.SetVisibility(Visibility.Visible);
                    SettingsManager.BuffBarWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.BuffBar.SetVisibility(Visibility.Hidden);
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
                }
                else
                {
                    isBossWindowVisible = false;
                }
                return isBossWindowVisible;
            }
            set
            {
                if (value == isBossWindowVisible) return;
                isBossWindowVisible = value;

                if (isBossWindowVisible)
                {
                    WindowManager.BossGauge.SetVisibility(Visibility.Visible);
                    SettingsManager.BossGaugeWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.BossGauge.SetVisibility(Visibility.Hidden);
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
                }
                else
                {
                    isCharacterWindowVisible = false;
                }
                return isCharacterWindowVisible;
            }
            set
            {
                if (isCharacterWindowVisible == value) return;

                isCharacterWindowVisible = value;
                if (isCharacterWindowVisible)
                {
                    WindowManager.CharacterWindow.SetVisibility(Visibility.Visible);
                    SettingsManager.CharacterWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.CharacterWindow.SetVisibility(Visibility.Hidden);
                    SettingsManager.CharacterWindowSettings.Visibility = Visibility.Hidden;
                }

                RaisePropertyChanged("IsCharacterWindowVisible");
            }
        }
        private bool isGroupWindowVisible;
        public bool IsGroupWindowVisible
        {
            get
            {
                if (SettingsManager.GroupWindowSettings.Visibility == Visibility.Visible)
                {
                    isGroupWindowVisible = true;
                }
                else
                {
                    isGroupWindowVisible = false;
                }
                return isGroupWindowVisible;
            }
            set
            {
                if (isGroupWindowVisible == value) return;
                isGroupWindowVisible = value;
                if (isGroupWindowVisible)
                {
                    WindowManager.GroupWindow.SetVisibility(Visibility.Visible);
                    SettingsManager.GroupWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.GroupWindow.SetVisibility(Visibility.Hidden);
                    SettingsManager.GroupWindowSettings.Visibility = Visibility.Hidden;
                }

                RaisePropertyChanged("IsGroupWindowVisible");
            }
        }
        private bool isClassWindowVisible;
        public bool IsClassWindowVisible
        {
            get
            {
                if (SettingsManager.ClassWindowSettings.Visibility == Visibility.Visible)
                {
                    isClassWindowVisible = true;
                }
                else
                {
                    isClassWindowVisible = false;
                }
                return isClassWindowVisible;
            }
            set
            {
                if (isClassWindowVisible == value) return;
                isClassWindowVisible = value;
                if (isClassWindowVisible)
                {
                    WindowManager.ClassWindow.SetVisibility(Visibility.Visible);
                    SettingsManager.ClassWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.ClassWindow.SetVisibility(Visibility.Hidden);
                    SettingsManager.ClassWindowSettings.Visibility = Visibility.Hidden;
                }

                RaisePropertyChanged("IsClassWindowVisible");
            }
        }

        //clickthru settings
        public bool IsCooldownWindowTransparent
        {
            get { return SettingsManager.CooldownWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.CooldownWindowSettings.ClickThru == value) return;
                SettingsManager.CooldownWindowSettings.ClickThru = value;
                WindowManager.CooldownWindow.SetClickThru(value);
                RaisePropertyChanged("IsCooldownWindowTransparent");
            }
        }
        public bool IsCharacterWindowTransparent
        {
            get { return SettingsManager.CharacterWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.CharacterWindowSettings.ClickThru == value) return;
                SettingsManager.CharacterWindowSettings.ClickThru = value;
                WindowManager.CharacterWindow.SetClickThru(value);
                RaisePropertyChanged("IsCharacterWindowTransparent");
            }
        }
        public bool IsBuffWindowTransparent
        {
            get { return SettingsManager.BuffBarWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.BuffBarWindowSettings.ClickThru == value) return;
                SettingsManager.BuffBarWindowSettings.ClickThru = value;
                WindowManager.BuffBar.SetClickThru(value);
                RaisePropertyChanged("IsBuffWindowTransparent");
            }
        }
        public bool IsBossWindowTransparent
        {
            get { return SettingsManager.BossGaugeWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.BossGaugeWindowSettings.ClickThru == value) return;
                SettingsManager.BossGaugeWindowSettings.ClickThru = value;
                WindowManager.BossGauge.SetClickThru(value);
                RaisePropertyChanged("IsBossWindowTransparent");
            }
        }
        public bool IsGroupWindowTransparent
        {
            get { return SettingsManager.GroupWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.GroupWindowSettings.ClickThru == value) return;
                SettingsManager.GroupWindowSettings.ClickThru = value;
                WindowManager.GroupWindow.SetClickThru(value);
                RaisePropertyChanged("IsGroupWindowTransparent");
            }
        }
        public bool IsClassWindowTransparent
        {
            get { return SettingsManager.ClassWindowSettings.ClickThru; }
            set
            {
                if (SettingsManager.ClassWindowSettings.ClickThru == value) return;
                SettingsManager.ClassWindowSettings.ClickThru = value;
                WindowManager.ClassWindow.SetClickThru(value);
                RaisePropertyChanged("IsClassWindowTransparent");
            }
        }


        //scale settings
        public double CooldownWindowScale
        {
            get { return SettingsManager.CooldownWindowSettings.Scale; }
            set
            {
                if (SettingsManager.CooldownWindowSettings.Scale == value) return;
                SettingsManager.CooldownWindowSettings.Scale = value;
                WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
                {
                    ((CooldownWindowViewModel)WindowManager.CooldownWindow.DataContext).Scale = value;
                });
                RaisePropertyChanged("CooldownWindowScale");
            }
        }
        public double GroupWindowScale
        {
            get { return SettingsManager.GroupWindowSettings.Scale; }
            set
            {
                if (SettingsManager.GroupWindowSettings.Scale == value) return;
                SettingsManager.GroupWindowSettings.Scale = value;
                WindowManager.GroupWindow.Dispatcher.Invoke(() =>
                {
                    ((GroupWindowViewModel)WindowManager.GroupWindow.DataContext).Scale = value;
                });
                RaisePropertyChanged("GroupWindowScale");
            }
        }
        public double CharacterWindowScale
        {
            get { return SettingsManager.CharacterWindowSettings.Scale; }
            set
            {
                if (SettingsManager.CharacterWindowSettings.Scale == value) return;
                SettingsManager.CharacterWindowSettings.Scale = value;
                WindowManager.CharacterWindow.Dispatcher.Invoke(() =>
                {
                    ((CharacterWindowViewModel)WindowManager.CharacterWindow.DataContext).Scale = value;
                });
                RaisePropertyChanged("CharacterWindowScale");
            }
        }
        public double BuffsWindowScale
        {
            get { return SettingsManager.BuffBarWindowSettings.Scale; }
            set
            {
                if (SettingsManager.BuffBarWindowSettings.Scale == value) return;
                SettingsManager.BuffBarWindowSettings.Scale = value;
                WindowManager.BuffBar.Dispatcher.Invoke(() =>
                {
                    ((AbnormalityWindowViewModel)WindowManager.BuffBar.DataContext).Scale = value;
                });
                RaisePropertyChanged("BuffsWindowScale");
            }
        }
        public double BossWindowScale
        {
            get { return SettingsManager.BossGaugeWindowSettings.Scale; }
            set
            {
                if (SettingsManager.BossGaugeWindowSettings.Scale == value) return;
                SettingsManager.BossGaugeWindowSettings.Scale = value;
                WindowManager.BossGauge.Dispatcher.Invoke(() =>
                {
                    ((BossGageWindowViewModel)WindowManager.BossGauge.DataContext).Scale = value;
                });
                RaisePropertyChanged("BossWindowScale");
            }
        }
        public double ClassWindowScale
        {
            get { return SettingsManager.ClassWindowSettings.Scale; }
            set
            {
                if (SettingsManager.ClassWindowSettings.Scale == value) return;
                SettingsManager.ClassWindowSettings.Scale = value;
                WindowManager.ClassWindow.Dispatcher.Invoke(() =>
                {
                    ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).Scale = value;
                });
                RaisePropertyChanged("ClassWindowScale");
            }
        }

        //other settings
        public bool HideMe
        {
            get { return SettingsManager.IgnoreMeInGroupWindow; }
            set { if (SettingsManager.IgnoreMeInGroupWindow == value) return;
                SettingsManager.IgnoreMeInGroupWindow = value;
                if (value == true) GroupWindowManager.Instance.RemoveMe();
                RaisePropertyChanged("HideMe");
            }
        }
        public bool HideMyBuffs
        {
            get { return SettingsManager.IgnoreMyBuffsInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreMyBuffsInGroupWindow == value) return;
                SettingsManager.IgnoreMyBuffsInGroupWindow = value;
                RaisePropertyChanged("HideMyBuffs");
                if(value == true) GroupWindowManager.Instance.ClearMyBuffs();

            }
        }
        public bool HideAllBuffs
        {
            get { return SettingsManager.IgnoreAllBuffsInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreAllBuffsInGroupWindow == value) return;
                SettingsManager.IgnoreAllBuffsInGroupWindow = value;
                RaisePropertyChanged("HideAllBuffs");
                if (value == true) GroupWindowManager.Instance.ClearAllBuffs();
            }
        }
        public bool HideRaidAbnormalities
        {
            get { return SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow == value) return;
                SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow = value;
                RaisePropertyChanged("HideRaidAbnormalities");
                if (value == true) GroupWindowManager.Instance.ClearAllAbnormalities();
            }
        }
        public bool IsLeftToRightOn
        {
            get { return SettingsManager.BuffsDirection == FlowDirection.LeftToRight ? true : false; }
            set
            {
                FlowDirection s;
                if (value == true) s = FlowDirection.LeftToRight;
                else s = FlowDirection.RightToLeft;
                if (SettingsManager.BuffsDirection == s) return;
                SettingsManager.BuffsDirection = s;
                WindowManager.BuffBar.Dispatcher.Invoke(() =>
                {
                    ((AbnormalityWindowViewModel)WindowManager.BuffBar.DataContext).Direction = s;
                });
                RaisePropertyChanged("IsLeftToRightOn");
            }
        }
        public bool ClassWindowOn
        {
            get { return SettingsManager.ClassWindowOn; }
            set
            {
                if (SettingsManager.ClassWindowOn == value) return;
                SettingsManager.ClassWindowOn = value;
                WindowManager.CooldownWindow.SwitchMode();
                //if(value == true)
                //{
                //    IsCooldownWindowVisible = false;
                //    IsClassWindowVisible = true;
                //}
                //else
                //{
                //    IsCooldownWindowVisible = true;
                //    IsClassWindowVisible = false;
                //}
                RaisePropertyChanged("ClassWindowOn");
            }
        }

        public SettingsWindowViewModel()
        {

            WindowManager.CooldownWindow.PropertyChanged += CooldownWindow_PropertyChanged;
            WindowManager.CharacterWindow.PropertyChanged += CharacterWindow_PropertyChanged;
            WindowManager.BossGauge.PropertyChanged += BossGauge_PropertyChanged;
            WindowManager.BuffBar.PropertyChanged += BuffBar_PropertyChanged;
            WindowManager.GroupWindow.PropertyChanged += GroupWindow_PropertyChanged;
            WindowManager.ClassWindow.PropertyChanged += ClassWindow_PropertyChanged;
        }

        private void ClassWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                IsClassWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
            }
            else if (e.PropertyName == "ClickThru")
            {
                IsClassWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
        private void GroupWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                IsGroupWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
            }
            else if (e.PropertyName == "ClickThru")
            {
                IsGroupWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
        private void BuffBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                IsBuffWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
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
                IsBossWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;

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
                IsCharacterWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
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
                IsCooldownWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
            }
            else if (e.PropertyName == "ClickThru")
            {
                IsCooldownWindowTransparent = ((TccWindow)sender).ClickThru;
            }
        }
    }
}
