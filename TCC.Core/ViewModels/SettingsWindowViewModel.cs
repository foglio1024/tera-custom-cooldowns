using System.Windows;
using System.Windows.Threading;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
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

                NotifyPropertyChanged("IsCooldownWindowVisible");
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
                NotifyPropertyChanged("IsBuffWindowVisible");


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

                NotifyPropertyChanged("IsBossWindowVisible");
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

                NotifyPropertyChanged("IsCharacterWindowVisible");
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

                NotifyPropertyChanged("IsGroupWindowVisible");
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

                NotifyPropertyChanged("IsClassWindowVisible");
            }
        }
        private bool isChatWindowVisible;
        public bool IsChatWindowVisible
        {
            get
            {
                if (SettingsManager.ChatWindowSettings.Visibility == Visibility.Visible)
                {
                    isChatWindowVisible = true;
                }
                else
                {
                    isChatWindowVisible = false;
                }
                return isChatWindowVisible;
            }
            set
            {
                if (isChatWindowVisible == value) return;
                isChatWindowVisible = value;
                if (isChatWindowVisible)
                {
                    WindowManager.ChatWindow.SetVisibility(Visibility.Visible);
                    SettingsManager.ChatWindowSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowManager.ChatWindow.SetVisibility(Visibility.Hidden);
                    SettingsManager.ChatWindowSettings.Visibility = Visibility.Hidden;
                }

                NotifyPropertyChanged(nameof(IsChatWindowVisible));
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
                NotifyPropertyChanged("IsCooldownWindowTransparent");
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
                NotifyPropertyChanged("IsCharacterWindowTransparent");
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
                NotifyPropertyChanged("IsBuffWindowTransparent");
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
                NotifyPropertyChanged("IsBossWindowTransparent");
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
                NotifyPropertyChanged("IsGroupWindowTransparent");
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
                NotifyPropertyChanged("IsClassWindowTransparent");
            }
        }
        public bool ChatWindowShowAlways
        {
            get { return SettingsManager.ChatWindowSettings.ShowAlways; }
            set
            {
                if (SettingsManager.ChatWindowSettings.ShowAlways == value) return;
                SettingsManager.ChatWindowSettings.ShowAlways = value;
                NotifyPropertyChanged(nameof(ChatWindowShowAlways));
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
                NotifyPropertyChanged("CooldownWindowScale");
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
                NotifyPropertyChanged("GroupWindowScale");
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
                NotifyPropertyChanged("CharacterWindowScale");
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
                    ((BuffBarWindowViewModel)WindowManager.BuffBar.DataContext).Scale = value;
                });
                NotifyPropertyChanged("BuffsWindowScale");
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
                NotifyPropertyChanged("BossWindowScale");
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
                NotifyPropertyChanged("ClassWindowScale");
            }
        }
        public double ChatWindowScale
        {
            get { return SettingsManager.ChatWindowSettings.Scale; }
            set
            {
                if (SettingsManager.ChatWindowSettings.Scale == value) return;
                SettingsManager.ChatWindowSettings.Scale = value;
                WindowManager.ChatWindow.Dispatcher.Invoke(() =>
                {
                    ((ChatWindowViewModel)WindowManager.ChatWindow.DataContext).Scale = value;
                });
                NotifyPropertyChanged(nameof(ChatWindowScale));
            }
        }

        //autodim settings
        public bool IsCooldownWindowAutoDim
        {
            get { return SettingsManager.CooldownWindowSettings.AutoDim; }
            set
            {
                if (SettingsManager.CooldownWindowSettings.AutoDim == value) return;
                SettingsManager.CooldownWindowSettings.AutoDim = value;
                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                //WindowManager.CooldownWindow.SetClickThru(value);
                NotifyPropertyChanged("IsCooldownWindowAutoDim");
            }
        }
        public bool IsCharacterWindowAutoDim
        {
            get { return SettingsManager.CharacterWindowSettings.AutoDim; }
            set
            {
                if (SettingsManager.CharacterWindowSettings.AutoDim == value) return;
                SettingsManager.CharacterWindowSettings.AutoDim = value;
                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                //WindowManager.CharacterWindow.SetClickThru(value);
                NotifyPropertyChanged("IsCharacterWindowAutoDim");
            }
        }
        public bool IsBuffBarWindowAutoDim
        {
            get { return SettingsManager.BuffBarWindowSettings.AutoDim; }
            set
            {
                if (SettingsManager.BuffBarWindowSettings.AutoDim == value) return;
                SettingsManager.BuffBarWindowSettings.AutoDim = value;
                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                //WindowManager.BuffBarWindow.SetClickThru(value);
                NotifyPropertyChanged("IsBuffBarWindowAutoDim");
            }
        }
        public bool IsBossGaugeWindowAutoDim
        {
            get { return SettingsManager.BossGaugeWindowSettings.AutoDim; }
            set
            {
                if (SettingsManager.BossGaugeWindowSettings.AutoDim == value) return;
                SettingsManager.BossGaugeWindowSettings.AutoDim = value;
                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                //WindowManager.BossGaugeWindow.SetClickThru(value);
                NotifyPropertyChanged("IsBossGaugeWindowAutoDim");
            }
        }
        public bool IsGroupWindowAutoDim
        {
            get { return SettingsManager.GroupWindowSettings.AutoDim; }
            set
            {
                if (SettingsManager.GroupWindowSettings.AutoDim == value) return;
                SettingsManager.GroupWindowSettings.AutoDim = value;
                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                //WindowManager.GroupWindow.SetClickThru(value);
                NotifyPropertyChanged("IsGroupWindowAutoDim");
            }
        }
        public bool IsClassWindowAutoDim
        {
            get { return SettingsManager.ClassWindowSettings.AutoDim; }
            set
            {
                if (SettingsManager.ClassWindowSettings.AutoDim == value) return;
                SettingsManager.ClassWindowSettings.AutoDim = value;
                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                //WindowManager.ClassWindow.SetClickThru(value);
                NotifyPropertyChanged("IsClassWindowAutoDim");
            }
        }

        //dimOpacity settings
        public double CooldownWindowDimOpacity
        {
            get { return SettingsManager.CooldownWindowSettings.DimOpacity; }
            set
            {
                if (SettingsManager.CooldownWindowSettings.DimOpacity == value) return;
                var val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;


                SettingsManager.CooldownWindowSettings.DimOpacity = val;
                if (WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }

                //WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
                //{
                //    ((CooldownWindowViewModel)WindowManager.CooldownWindow.DataContext).Scale = value;
                //});
                NotifyPropertyChanged("CooldownWindowDimOpacity");
            }
        }
        public double CharacterWindowDimOpacity
        {
            get { return SettingsManager.CharacterWindowSettings.DimOpacity; }
            set
            {
                if (SettingsManager.CharacterWindowSettings.DimOpacity == value) return;
                var val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;


                SettingsManager.CharacterWindowSettings.DimOpacity = val;
                if (WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }

                //WindowManager.CharacterWindow.Dispatcher.Invoke(() =>
                //{
                //    ((CharacterWindowViewModel)WindowManager.CharacterWindow.DataContext).Scale = value;
                //});
                NotifyPropertyChanged("CharacterWindowDimOpacity");
            }
        }
        public double BuffBarWindowDimOpacity
        {
            get { return SettingsManager.BuffBarWindowSettings.DimOpacity; }
            set
            {
                if (SettingsManager.BuffBarWindowSettings.DimOpacity == value) return;
                var val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;


                SettingsManager.BuffBarWindowSettings.DimOpacity = val;
                if (WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }

                //WindowManager.BuffBarWindow.Dispatcher.Invoke(() =>
                //{
                //    ((BuffBarWindowViewModel)WindowManager.BuffBarWindow.DataContext).Scale = value;
                //});
                NotifyPropertyChanged("BuffBarWindowDimOpacity");
            }
        }
        public double BossGaugeWindowDimOpacity
        {
            get { return SettingsManager.BossGaugeWindowSettings.DimOpacity; }
            set
            {
                if (SettingsManager.BossGaugeWindowSettings.DimOpacity == value) return;
                var val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;
                SettingsManager.BossGaugeWindowSettings.DimOpacity = val;
                if (WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }


                //WindowManager.BossGaugeWindow.Dispatcher.Invoke(() =>
                //{
                //    ((BossGaugeWindowViewModel)WindowManager.BossGaugeWindow.DataContext).Scale = value;
                //});
                NotifyPropertyChanged("BossGaugeWindowDimOpacity");
            }
        }
        public double GroupWindowDimOpacity
        {
            get { return SettingsManager.GroupWindowSettings.DimOpacity; }
            set
            {
                if (SettingsManager.GroupWindowSettings.DimOpacity == value) return;
                var val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;
                SettingsManager.GroupWindowSettings.DimOpacity = val;
                if (WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }


                //WindowManager.GroupWindow.Dispatcher.Invoke(() =>
                //{
                //    ((GroupWindowViewModel)WindowManager.GroupWindow.DataContext).Scale = value;
                //});
                NotifyPropertyChanged("GroupWindowDimOpacity");
            }
        }
        public double ClassWindowDimOpacity
        {
            get { return SettingsManager.ClassWindowSettings.DimOpacity; }
            set
            {
                if (SettingsManager.ClassWindowSettings.DimOpacity == value) return;
                var val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;

                SettingsManager.ClassWindowSettings.DimOpacity = val;
                if (WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }

                //WindowManager.ClassWindow.Dispatcher.Invoke(() =>
                //{
                //    ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).Scale = value;
                //});
                NotifyPropertyChanged("ClassWindowDimOpacity");
            }
        }

        //transparency settings
        public bool DoesChatWindowAllowTransparency
        {
            get => SettingsManager.ChatWindowSettings.AllowTransparency;
            set
            {
                if (SettingsManager.ChatWindowSettings.AllowTransparency == value) return;
                SettingsManager.ChatWindowSettings.AllowTransparency = value;
                NotifyPropertyChanged(nameof(DoesChatWindowAllowTransparency));
            }
        }
        public bool DoesCharacterWindowAllowTransparency
        {
            get => SettingsManager.CharacterWindowSettings.AllowTransparency;
            set
            {
                if (SettingsManager.CharacterWindowSettings.AllowTransparency == value) return;
                SettingsManager.CharacterWindowSettings.AllowTransparency = value;
                NotifyPropertyChanged(nameof(DoesCharacterWindowAllowTransparency));
            }
        }
        public bool DoesCooldownWindowAllowTransparency
        {
            get => SettingsManager.CooldownWindowSettings.AllowTransparency;
            set
            {
                if (SettingsManager.CooldownWindowSettings.AllowTransparency == value) return;
                SettingsManager.CooldownWindowSettings.AllowTransparency = value;
                NotifyPropertyChanged(nameof(DoesCooldownWindowAllowTransparency));
            }
        }
        public bool DoesBossWindowAllowTransparency
        {
            get => SettingsManager.BossGaugeWindowSettings.AllowTransparency;
            set
            {
                if (SettingsManager.BossGaugeWindowSettings.AllowTransparency == value) return;
                SettingsManager.BossGaugeWindowSettings.AllowTransparency = value;
                NotifyPropertyChanged(nameof(DoesBossWindowAllowTransparency));
            }
        }

        //other settings
        public bool HideMe
        {
            get { return SettingsManager.IgnoreMeInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreMeInGroupWindow == value) return;
                SettingsManager.IgnoreMeInGroupWindow = value;
                if (value == true) GroupWindowViewModel.Instance.RemoveMe();
                NotifyPropertyChanged("HideMe");
            }
        }
        public bool HideMyBuffs
        {
            get { return SettingsManager.IgnoreMyBuffsInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreMyBuffsInGroupWindow == value) return;
                SettingsManager.IgnoreMyBuffsInGroupWindow = value;
                NotifyPropertyChanged("HideMyBuffs");
                if (value == true) GroupWindowViewModel.Instance.ClearMyBuffs();

            }
        }
        public bool HideAllBuffs
        {
            get { return SettingsManager.IgnoreAllBuffsInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreAllBuffsInGroupWindow == value) return;
                SettingsManager.IgnoreAllBuffsInGroupWindow = value;
                NotifyPropertyChanged("HideAllBuffs");
                if (value == true) GroupWindowViewModel.Instance.ClearAllBuffs();
            }
        }
        public bool HideRaidAbnormalities
        {
            get { return SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow; }
            set
            {
                if (SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow == value) return;
                SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow = value;
                NotifyPropertyChanged("HideRaidAbnormalities");
                if (value == true) GroupWindowViewModel.Instance.ClearAllAbnormalities();
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
                    ((BuffBarWindowViewModel)WindowManager.BuffBar.DataContext).Direction = s;
                });
                NotifyPropertyChanged("IsLeftToRightOn");
            }
        }
        public bool ClassWindowOn
        {
            get { return SettingsManager.ClassWindowOn; }
            set
            {
                if (SettingsManager.ClassWindowOn == value) return;
                SettingsManager.ClassWindowOn = value;
                CooldownWindowViewModel.Instance.IsClassWindowOn = value;
                //WindowManager.CooldownWindow.SwitchMode();
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
                NotifyPropertyChanged("ClassWindowOn");
            }
        }
        public bool ClickThruWhenDim
        {
            get { return SettingsManager.ClickThruWhenDim; }
            set
            {
                if (SettingsManager.ClickThruWhenDim == value) return;
                SettingsManager.ClickThruWhenDim = value;
                if (value && WindowManager.IsTccDim)
                {
                    WindowManager.SkillsEnded = false;
                    WindowManager.SkillsEnded = true;
                }
                NotifyPropertyChanged("ClickThruWhenDim");
            }
        }
        public int MaxMessages
        {
            get { return SettingsManager.MaxMessages; }
            set
            {
                if (SettingsManager.MaxMessages == value) return;
                var val = value;
                if (val < 20)
                {
                    val = 20;
                }
                SettingsManager.MaxMessages = val;
                NotifyPropertyChanged("MaxMessages");
            }
        }
        public int SpamThreshold
        {
            get { return SettingsManager.SpamThreshold; }
            set
            {
                if (SettingsManager.SpamThreshold == value) return;
                SettingsManager.SpamThreshold = value;
                NotifyPropertyChanged("SpamThreshold");
            }
        }
        public bool ShowTimestamp
        {
            get { return SettingsManager.ShowTimestamp; }
            set
            {
                if (SettingsManager.ShowTimestamp == value) return;
                SettingsManager.ShowTimestamp = value;
                NotifyPropertyChanged(nameof(ShowTimestamp));
            }

        }
        public bool ShowChannel
        {
            get { return SettingsManager.ShowChannel; }
            set
            {
                if (SettingsManager.ShowChannel == value) return;
                SettingsManager.ShowChannel = value;
                NotifyPropertyChanged(nameof(ShowChannel));
            }

        }
        public bool ShowOnlyBosses
        {
            get => SettingsManager.ShowOnlyBosses;
            set
            {
                if (SettingsManager.ShowOnlyBosses == value) return;
                SettingsManager.ShowOnlyBosses = value;
                NotifyPropertyChanged(nameof(ShowOnlyBosses));
            }
        }
        public SettingsWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
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
