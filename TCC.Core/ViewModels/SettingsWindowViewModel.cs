using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using TCC.Parsing;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        public WindowSettings CooldownWindowSettings => SettingsManager.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => SettingsManager.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => SettingsManager.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => SettingsManager.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => SettingsManager.CharacterWindowSettings;
        public WindowSettings ChatWindowSettings => SettingsManager.ChatWindowSettings;
        public WindowSettings BossWindowSettings => SettingsManager.BossWindowSettings;

        //enable settings
        //public bool IsCooldownWindowEnabled
        //{
        //    get { return SettingsManager.CooldownWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.CooldownWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.CooldownWindowSettings.Enabled = value;
        //            IsCooldownWindowVisible = value;
        //            WindowManager.CooldownWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.CooldownWindowSettings.Enabled = value;
        //            IsCooldownWindowVisible = value;
        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsCooldownWindowEnabled));
        //    }
        //}
        //public bool IsBuffWindowEnabled
        //{
        //    get { return SettingsManager.BuffWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.BuffWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.BuffWindowSettings.Enabled = value;
        //            IsBuffWindowVisible = value;
        //            WindowManager.BuffWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.BuffWindowSettings.Enabled = value;
        //            IsBuffWindowVisible = value;

        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsBuffWindowEnabled));
        //    }
        //}
        //public bool IsBossWindowEnabled
        //{
        //    get { return SettingsManager.BossWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.BossWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.BossWindowSettings.Enabled = value;
        //            IsBossWindowVisible = value;
        //            WindowManager.BossWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.BossWindowSettings.Enabled = value;
        //            IsBossWindowVisible = value;

        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsBossWindowEnabled));
        //    }
        //}
        //public bool IsCharacterWindowEnabled
        //{
        //    get { return SettingsManager.CharacterWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.CharacterWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.CharacterWindowSettings.Enabled = value;
        //            IsCharacterWindowVisible = value;
        //            WindowManager.CharacterWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.CharacterWindowSettings.Enabled = value;
        //            IsCharacterWindowVisible = value;

        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsCharacterWindowEnabled));
        //    }
        //}
        //public bool IsGroupWindowEnabled
        //{
        //    get { return SettingsManager.GroupWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.GroupWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.GroupWindowSettings.Enabled = value;
        //            IsGroupWindowVisible = value;
        //            WindowManager.GroupWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.GroupWindowSettings.Enabled = value;
        //            IsGroupWindowVisible = value;

        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsGroupWindowEnabled));
        //    }
        //}
        //public bool IsClassWindowEnabled
        //{
        //    get { return SettingsManager.ClassWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.ClassWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.ClassWindowSettings.Enabled = value;
        //            IsClassWindowVisible = value;
        //            WindowManager.ClassWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.ClassWindowSettings.Enabled = value;
        //            IsClassWindowVisible = value;

        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsClassWindowEnabled));
        //    }
        //}
        //public bool IsChatWindowEnabled
        //{
        //    get { return SettingsManager.ChatWindowSettings.Enabled; }
        //    set
        //    {
        //        if (SettingsManager.ChatWindowSettings.Enabled == value) return;
        //        if (value == false)
        //        {
        //            if (MessageBox.Show("Re-enabling this later will require TCC restart.\nDo you want to continue?", "TCC", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
        //                MessageBoxResult.Cancel) return;
        //            SettingsManager.ChatWindowSettings.Enabled = value;
        //            IsChatWindowVisible = value;
        //            WindowManager.ChatWindow.CloseWindowSafe();
        //        }
        //        else
        //        {
        //            MessageBox.Show("TCC will now be restarted.", "TCC", MessageBoxButton.OK, MessageBoxImage.Information);
        //            SettingsManager.ChatWindowSettings.Enabled = value;
        //            IsChatWindowVisible = value;

        //            App.Restart();
        //        }
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(IsChatWindowEnabled));
        //    }
        //}

        //visibility settings
        //private bool isCooldownWindowVisible;
        //public bool IsCooldownWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.CooldownWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isCooldownWindowVisible = true;
        //        }
        //        else
        //        {
        //            isCooldownWindowVisible = false;
        //        }
        //        return isCooldownWindowVisible;
        //    }
        //    set
        //    {
        //        if (value == isCooldownWindowVisible) return;

        //        isCooldownWindowVisible = value;
        //        if (isCooldownWindowVisible)
        //        {
        //            if (!IsCooldownWindowEnabled) return;

        //            WindowManager.CooldownWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.CooldownWindowSettings.Visibility = Visibility.Visible;

        //        }
        //        else
        //        {
        //            WindowManager.CooldownWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.CooldownWindowSettings.Visibility = Visibility.Hidden; ;
        //        }

        //        NotifyPropertyChanged("IsCooldownWindowVisible");
        //    }
        //}
        //private bool isBuffWindowVisible;
        //public bool IsBuffWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.BuffWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isBuffWindowVisible = true;
        //        }
        //        else
        //        {
        //            isBuffWindowVisible = false;
        //        }
        //        return isBuffWindowVisible;
        //    }
        //    set
        //    {
        //        if (value == isBuffWindowVisible) return;

        //        isBuffWindowVisible = value;
        //        if (isBuffWindowVisible)
        //        {
        //            WindowManager.BuffWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.BuffWindowSettings.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            WindowManager.BuffWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.BuffWindowSettings.Visibility = Visibility.Hidden;
        //        }
        //        NotifyPropertyChanged("IsBuffWindowVisible");


        //    }
        //}
        //public bool isBossWindowVisible;
        //public bool IsBossWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.BossWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isBossWindowVisible = true;
        //        }
        //        else
        //        {
        //            isBossWindowVisible = false;
        //        }
        //        return isBossWindowVisible;
        //    }
        //    set
        //    {
        //        if (value == isBossWindowVisible) return;
        //        isBossWindowVisible = value;

        //        if (isBossWindowVisible)
        //        {
        //            WindowManager.BossWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.BossWindowSettings.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            WindowManager.BossWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.BossWindowSettings.Visibility = Visibility.Hidden;
        //        }

        //        NotifyPropertyChanged("IsBossWindowVisible");
        //    }
        //}
        //private bool isCharacterWindowVisible;
        //public bool IsCharacterWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.CharacterWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isCharacterWindowVisible = true;
        //        }
        //        else
        //        {
        //            isCharacterWindowVisible = false;
        //        }
        //        return isCharacterWindowVisible;
        //    }
        //    set
        //    {
        //        if (isCharacterWindowVisible == value) return;

        //        isCharacterWindowVisible = value;
        //        if (isCharacterWindowVisible)
        //        {
        //            WindowManager.CharacterWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.CharacterWindowSettings.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            WindowManager.CharacterWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.CharacterWindowSettings.Visibility = Visibility.Hidden;
        //        }

        //        NotifyPropertyChanged("IsCharacterWindowVisible");
        //    }
        //}
        //private bool isGroupWindowVisible;
        //public bool IsGroupWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.GroupWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isGroupWindowVisible = true;
        //        }
        //        else
        //        {
        //            isGroupWindowVisible = false;
        //        }
        //        return isGroupWindowVisible;
        //    }
        //    set
        //    {
        //        if (isGroupWindowVisible == value) return;
        //        isGroupWindowVisible = value;
        //        if (isGroupWindowVisible)
        //        {
        //            WindowManager.GroupWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.GroupWindowSettings.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            WindowManager.GroupWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.GroupWindowSettings.Visibility = Visibility.Hidden;
        //        }

        //        NotifyPropertyChanged("IsGroupWindowVisible");
        //    }
        //}
        //private bool isClassWindowVisible;
        //public bool IsClassWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.ClassWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isClassWindowVisible = true;
        //        }
        //        else
        //        {
        //            isClassWindowVisible = false;
        //        }
        //        return isClassWindowVisible;
        //    }
        //    set
        //    {
        //        if (isClassWindowVisible == value) return;
        //        isClassWindowVisible = value;
        //        if (isClassWindowVisible)
        //        {
        //            WindowManager.ClassWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.ClassWindowSettings.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            WindowManager.ClassWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.ClassWindowSettings.Visibility = Visibility.Hidden;
        //        }

        //        NotifyPropertyChanged("IsClassWindowVisible");
        //    }
        //}
        //private bool isChatWindowVisible;
        //public bool IsChatWindowVisible
        //{
        //    get
        //    {
        //        if (SettingsManager.ChatWindowSettings.Visibility == Visibility.Visible)
        //        {
        //            isChatWindowVisible = true;
        //        }
        //        else
        //        {
        //            isChatWindowVisible = false;
        //        }
        //        return isChatWindowVisible;
        //    }
        //    set
        //    {
        //        if (isChatWindowVisible == value) return;
        //        isChatWindowVisible = value;
        //        if (isChatWindowVisible)
        //        {
        //            WindowManager.ChatWindow.SetVisibility(Visibility.Visible);
        //            SettingsManager.ChatWindowSettings.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            WindowManager.ChatWindow.SetVisibility(Visibility.Hidden);
        //            SettingsManager.ChatWindowSettings.Visibility = Visibility.Hidden;
        //        }

        //        NotifyPropertyChanged(nameof(IsChatWindowVisible));
        //    }
        //}

        //clickthru settings
        //public bool IsCooldownWindowTransparent
        //{
        //    get { return SettingsManager.CooldownWindowSettings.ClickThruMode; }
        //    set
        //    {
        //        if (SettingsManager.CooldownWindowSettings.ClickThruMode == value) return;
        //        SettingsManager.CooldownWindowSettings.ClickThruMode = value;
        //        WindowManager.CooldownWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsCooldownWindowTransparent");
        //    }
        //}
        //public bool IsCharacterWindowTransparent
        //{
        //    get { return SettingsManager.CharacterWindowSettings.ClickThruMode; }
        //    set
        //    {
        //        if (SettingsManager.CharacterWindowSettings.ClickThruMode == value) return;
        //        SettingsManager.CharacterWindowSettings.ClickThruMode = value;
        //        WindowManager.CharacterWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsCharacterWindowTransparent");
        //    }
        //}
        //public bool IsBuffWindowTransparent
        //{
        //    get { return SettingsManager.BuffWindowSettings.ClickThruMode; }
        //    set
        //    {
        //        if (SettingsManager.BuffWindowSettings.ClickThruMode == value) return;
        //        SettingsManager.BuffWindowSettings.ClickThruMode = value;
        //        WindowManager.BuffWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsBuffWindowTransparent");
        //    }
        //}
        //public bool IsBossWindowTransparent
        //{
        //    get { return SettingsManager.BossWindowSettings.ClickThruMode; }
        //    set
        //    {
        //        if (SettingsManager.BossWindowSettings.ClickThruMode == value) return;
        //        SettingsManager.BossWindowSettings.ClickThruMode = value;
        //        WindowManager.BossWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsBossWindowTransparent");
        //    }
        //}
        //public bool IsGroupWindowTransparent
        //{
        //    get { return SettingsManager.GroupWindowSettings.ClickThruMode; }
        //    set
        //    {
        //        if (SettingsManager.GroupWindowSettings.ClickThruMode == value) return;
        //        SettingsManager.GroupWindowSettings.ClickThruMode = value;
        //        WindowManager.GroupWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsGroupWindowTransparent");
        //    }
        //}
        //public bool IsClassWindowTransparent
        //{
        //    get { return SettingsManager.ClassWindowSettings.ClickThruMode; }
        //    set
        //    {
        //        if (SettingsManager.ClassWindowSettings.ClickThruMode == value) return;
        //        SettingsManager.ClassWindowSettings.ClickThruMode = value;
        //        WindowManager.ClassWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsClassWindowTransparent");
        //    }
        //}

        ////scale settings
        //public double CooldownWindowScale
        //{
        //    get { return SettingsManager.CooldownWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.CooldownWindowSettings.Scale == value) return;
        //        SettingsManager.CooldownWindowSettings.Scale = value;
        //        WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((CooldownWindowViewModel)WindowManager.CooldownWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged("CooldownWindowScale");
        //    }
        //}
        //public double GroupWindowScale
        //{
        //    get { return SettingsManager.GroupWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.GroupWindowSettings.Scale == value) return;
        //        SettingsManager.GroupWindowSettings.Scale = value;
        //        WindowManager.GroupWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((GroupWindowViewModel)WindowManager.GroupWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged("GroupWindowScale");
        //    }
        //}
        //public double CharacterWindowScale
        //{
        //    get { return SettingsManager.CharacterWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.CharacterWindowSettings.Scale == value) return;
        //        SettingsManager.CharacterWindowSettings.Scale = value;
        //        WindowManager.CharacterWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((CharacterWindowViewModel)WindowManager.CharacterWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged("CharacterWindowScale");
        //    }
        //}
        //public double BuffsWindowScale
        //{
        //    get { return SettingsManager.BuffWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.BuffWindowSettings.Scale == value) return;
        //        SettingsManager.BuffWindowSettings.Scale = value;
        //        WindowManager.BuffWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((BuffBarWindowViewModel)WindowManager.BuffWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged("BuffsWindowScale");
        //    }
        //}
        //public double BossWindowScale
        //{
        //    get { return SettingsManager.BossWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.BossWindowSettings.Scale == value) return;
        //        SettingsManager.BossWindowSettings.Scale = value;
        //        WindowManager.BossWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((BossGageWindowViewModel)WindowManager.BossWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged("BossWindowScale");
        //    }
        //}
        //public double ClassWindowScale
        //{
        //    get { return SettingsManager.ClassWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.ClassWindowSettings.Scale == value) return;
        //        SettingsManager.ClassWindowSettings.Scale = value;
        //        WindowManager.ClassWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged("ClassWindowScale");
        //    }
        //}
        //public double ChatWindowScale
        //{
        //    get { return SettingsManager.ChatWindowSettings.Scale; }
        //    set
        //    {
        //        if (SettingsManager.ChatWindowSettings.Scale == value) return;
        //        SettingsManager.ChatWindowSettings.Scale = value;
        //        WindowManager.ChatWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((ChatWindowViewModel)WindowManager.ChatWindow.DataContext).Scale = value;
        //        });
        //        NotifyPropertyChanged(nameof(ChatWindowScale));
        //    }
        //}

        //autodim settings
        //public bool IsCooldownWindowAutoDim
        //{
        //    get { return SettingsManager.CooldownWindowSettings.AutoDim; }
        //    set
        //    {
        //        if (SettingsManager.CooldownWindowSettings.AutoDim == value) return;
        //        SettingsManager.CooldownWindowSettings.AutoDim = value;
        //        WindowManager.CooldownWindow.RefreshSettings(SettingsManager.CooldownWindowSettings);
        //        WindowManager.RefreshDim();

        //        //WindowManager.CooldownWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsCooldownWindowAutoDim");
        //    }
        //}
        //public bool IsCharacterWindowAutoDim
        //{
        //    get { return SettingsManager.CharacterWindowSettings.AutoDim; }
        //    set
        //    {
        //        if (SettingsManager.CharacterWindowSettings.AutoDim == value) return;
        //        SettingsManager.CharacterWindowSettings.AutoDim = value;
        //        WindowManager.CharacterWindow.RefreshSettings(SettingsManager.CharacterWindowSettings);
        //        WindowManager.RefreshDim();
        //        //WindowManager.CharacterWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsCharacterWindowAutoDim");
        //    }
        //}
        //public bool IsBuffBarWindowAutoDim
        //{
        //    get { return SettingsManager.BuffWindowSettings.AutoDim; }
        //    set
        //    {
        //        if (SettingsManager.BuffWindowSettings.AutoDim == value) return;
        //        SettingsManager.BuffWindowSettings.AutoDim = value;
        //        WindowManager.BuffWindow.RefreshSettings(SettingsManager.BuffWindowSettings);

        //        WindowManager.RefreshDim();
        //        //WindowManager.BuffBarWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsBuffBarWindowAutoDim");
        //    }
        //}
        //public bool IsBossGaugeWindowAutoDim
        //{
        //    get { return SettingsManager.BossWindowSettings.AutoDim; }
        //    set
        //    {
        //        if (SettingsManager.BossWindowSettings.AutoDim == value) return;
        //        SettingsManager.BossWindowSettings.AutoDim = value;
        //        WindowManager.BossWindow.RefreshSettings(SettingsManager.BossWindowSettings);

        //        WindowManager.RefreshDim();
        //        //WindowManager.BossGaugeWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsBossGaugeWindowAutoDim");
        //    }
        //}
        //public bool IsGroupWindowAutoDim
        //{
        //    get { return SettingsManager.GroupWindowSettings.AutoDim; }
        //    set
        //    {
        //        if (SettingsManager.GroupWindowSettings.AutoDim == value) return;
        //        SettingsManager.GroupWindowSettings.AutoDim = value;
        //        WindowManager.GroupWindow.RefreshSettings(SettingsManager.GroupWindowSettings);

        //        WindowManager.RefreshDim();
        //        //WindowManager.GroupWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsGroupWindowAutoDim");
        //    }
        //}
        //public bool IsClassWindowAutoDim
        //{
        //    get { return SettingsManager.ClassWindowSettings.AutoDim; }
        //    set
        //    {
        //        if (SettingsManager.ClassWindowSettings.AutoDim == value) return;
        //        SettingsManager.ClassWindowSettings.AutoDim = value;
        //        WindowManager.ClassWindow.RefreshSettings(SettingsManager.ClassWindowSettings);

        //        WindowManager.RefreshDim();
        //        //WindowManager.ClassWindow.SetClickThru(value);
        //        NotifyPropertyChanged("IsClassWindowAutoDim");
        //    }
        //}

        //dimOpacity settings
        //public double CooldownWindowDimOpacity
        //{
        //    get { return SettingsManager.CooldownWindowSettings.DimOpacity; }
        //    set
        //    {
        //        if (SettingsManager.CooldownWindowSettings.DimOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;


        //        SettingsManager.CooldownWindowSettings.DimOpacity = val;
        //        if (WindowManager.IsTccDim)
        //        {
        //            WindowManager.SkillsEnded = false;
        //            WindowManager.SkillsEnded = true;
        //        }

        //        //WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
        //        //{
        //        //    ((CooldownWindowViewModel)WindowManager.CooldownWindow.DataContext).Scale = value;
        //        //});
        //        NotifyPropertyChanged("CooldownWindowDimOpacity");
        //    }
        //}
        //public double CharacterWindowDimOpacity
        //{
        //    get { return SettingsManager.CharacterWindowSettings.DimOpacity; }
        //    set
        //    {
        //        if (SettingsManager.CharacterWindowSettings.DimOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;


        //        SettingsManager.CharacterWindowSettings.DimOpacity = val;
        //        if (WindowManager.IsTccDim)
        //        {
        //            WindowManager.SkillsEnded = false;
        //            WindowManager.SkillsEnded = true;
        //        }

        //        //WindowManager.CharacterWindow.Dispatcher.Invoke(() =>
        //        //{
        //        //    ((CharacterWindowViewModel)WindowManager.CharacterWindow.DataContext).Scale = value;
        //        //});
        //        NotifyPropertyChanged("CharacterWindowDimOpacity");
        //    }
        //}
        //public double BuffBarWindowDimOpacity
        //{
        //    get { return SettingsManager.BuffWindowSettings.DimOpacity; }
        //    set
        //    {
        //        if (SettingsManager.BuffWindowSettings.DimOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;


        //        SettingsManager.BuffWindowSettings.DimOpacity = val;
        //        if (WindowManager.IsTccDim)
        //        {
        //            WindowManager.SkillsEnded = false;
        //            WindowManager.SkillsEnded = true;
        //        }

        //        //WindowManager.BuffBarWindow.Dispatcher.Invoke(() =>
        //        //{
        //        //    ((BuffBarWindowViewModel)WindowManager.BuffBarWindow.DataContext).Scale = value;
        //        //});
        //        NotifyPropertyChanged("BuffBarWindowDimOpacity");
        //    }
        //}
        //public double BossGaugeWindowDimOpacity
        //{
        //    get { return SettingsManager.BossWindowSettings.DimOpacity; }
        //    set
        //    {
        //        if (SettingsManager.BossWindowSettings.DimOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;
        //        SettingsManager.BossWindowSettings.DimOpacity = val;
        //        if (WindowManager.IsTccDim)
        //        {
        //            WindowManager.SkillsEnded = false;
        //            WindowManager.SkillsEnded = true;
        //        }


        //        //WindowManager.BossGaugeWindow.Dispatcher.Invoke(() =>
        //        //{
        //        //    ((BossGaugeWindowViewModel)WindowManager.BossGaugeWindow.DataContext).Scale = value;
        //        //});
        //        NotifyPropertyChanged("BossGaugeWindowDimOpacity");
        //    }
        //}
        //public double GroupWindowDimOpacity
        //{
        //    get { return SettingsManager.GroupWindowSettings.DimOpacity; }
        //    set
        //    {
        //        if (SettingsManager.GroupWindowSettings.DimOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;
        //        SettingsManager.GroupWindowSettings.DimOpacity = val;
        //        if (WindowManager.IsTccDim)
        //        {
        //            WindowManager.SkillsEnded = false;
        //            WindowManager.SkillsEnded = true;
        //        }


        //        //WindowManager.GroupWindow.Dispatcher.Invoke(() =>
        //        //{
        //        //    ((GroupWindowViewModel)WindowManager.GroupWindow.DataContext).Scale = value;
        //        //});
        //        NotifyPropertyChanged("GroupWindowDimOpacity");
        //    }
        //}
        //public double ClassWindowDimOpacity
        //{
        //    get { return SettingsManager.ClassWindowSettings.DimOpacity; }
        //    set
        //    {
        //        if (SettingsManager.ClassWindowSettings.DimOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;

        //        SettingsManager.ClassWindowSettings.DimOpacity = val;
        //        if (WindowManager.IsTccDim)
        //        {
        //            WindowManager.SkillsEnded = false;
        //            WindowManager.SkillsEnded = true;
        //        }

        //        //WindowManager.ClassWindow.Dispatcher.Invoke(() =>
        //        //{
        //        //    ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).Scale = value;
        //        //});
        //        NotifyPropertyChanged("ClassWindowDimOpacity");
        //    }
        //}
        //public double ChatWindowOpacity
        //{
        //    get { return SettingsManager.ChatWindowOpacity; }
        //    set
        //    {
        //        if (SettingsManager.ChatWindowOpacity == value) return;
        //        var val = value;
        //        if (val < 0) val = 0;
        //        if (val > 1) val = 1;

        //        SettingsManager.ChatWindowOpacity = val;
        //        ChatWindowViewModel.Instance.ChatWindowOpacity = SettingsManager.ChatWindowOpacity;
        //        NotifyPropertyChanged(nameof(ChatWindowOpacity));
        //    }
        //}

        //transparency settings
        //public bool DoesChatWindowAllowTransparency
        //{
        //    get => SettingsManager.ChatWindowSettings.AllowTransparency;
        //    set
        //    {
        //        if (SettingsManager.ChatWindowSettings.AllowTransparency == value) return;
        //        SettingsManager.ChatWindowSettings.AllowTransparency = value;
        //        NotifyPropertyChanged(nameof(DoesChatWindowAllowTransparency));
        //    }
        //}
        //public bool DoesCharacterWindowAllowTransparency
        //{
        //    get => SettingsManager.CharacterWindowSettings.AllowTransparency;
        //    set
        //    {
        //        if (SettingsManager.CharacterWindowSettings.AllowTransparency == value) return;
        //        SettingsManager.CharacterWindowSettings.AllowTransparency = value;
        //        NotifyPropertyChanged(nameof(DoesCharacterWindowAllowTransparency));
        //    }
        //}
        //public bool DoesCooldownWindowAllowTransparency
        //{
        //    get => SettingsManager.CooldownWindowSettings.AllowTransparency;
        //    set
        //    {
        //        if (SettingsManager.CooldownWindowSettings.AllowTransparency == value) return;
        //        SettingsManager.CooldownWindowSettings.AllowTransparency = value;
        //        NotifyPropertyChanged(nameof(DoesCooldownWindowAllowTransparency));
        //    }
        //}
        //public bool DoesBossWindowAllowTransparency
        //{
        //    get => SettingsManager.BossWindowSettings.AllowTransparency;
        //    set
        //    {
        //        if (SettingsManager.BossWindowSettings.AllowTransparency == value) return;
        //        SettingsManager.BossWindowSettings.AllowTransparency = value;
        //        NotifyPropertyChanged(nameof(DoesBossWindowAllowTransparency));
        //    }
        //}

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
        public bool HideBuffs
        {
            get => SettingsManager.IgnoreGroupBuffs;
            set
            {
                if (SettingsManager.IgnoreGroupBuffs == value) return;
                SettingsManager.IgnoreGroupBuffs = value;
                NotifyPropertyChanged(nameof(HideBuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool HideDebuffs
        {
            get => SettingsManager.IgnoreGroupDebuffs;
            set
            {
                if (SettingsManager.IgnoreGroupDebuffs == value) return;
                SettingsManager.IgnoreGroupDebuffs = value;
                NotifyPropertyChanged(nameof(HideDebuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool DisableAllPartyAbnormals
        {
            get { return SettingsManager.DisablePartyAbnormals; }
            set
            {
                if (SettingsManager.DisablePartyAbnormals == value) return;
                SettingsManager.DisablePartyAbnormals = value;
                NotifyPropertyChanged(nameof(DisableAllPartyAbnormals));
                MessageFactory.Update();
                if (value == true) GroupWindowViewModel.Instance.ClearAllAbnormalities();
            }
        }
        //public bool IsLeftToRightOn
        //{
        //    get { return SettingsManager.BuffsDirection == FlowDirection.LeftToRight ? true : false; }
        //    set
        //    {
        //        FlowDirection s;
        //        if (value == true) s = FlowDirection.LeftToRight;
        //        else s = FlowDirection.RightToLeft;
        //        if (SettingsManager.BuffsDirection == s) return;
        //        SettingsManager.BuffsDirection = s;
        //        WindowManager.BuffWindow.Dispatcher.Invoke(() =>
        //        {
        //            ((BuffBarWindowViewModel)WindowManager.BuffWindow.DataContext).Direction = s;
        //        });
        //        NotifyPropertyChanged("IsLeftToRightOn");
        //    }
        //}

        public FlowDirection BuffsDirection
        {
            get => SettingsManager.BuffsDirection;
            set
            {
                if (SettingsManager.BuffsDirection == value) return;
                SettingsManager.BuffsDirection = value;
                BuffBarWindowViewModel.Instance.NotifyDirectionChanged();
                NotifyPropertyChanged(nameof(BuffsDirection));
            }
        }
        //public bool ClassWindowOn
        //{
        //    get { return SettingsManager.ClassWindowOn; }
        //    set
        //    {
        //        if (SettingsManager.ClassWindowOn == value) return;
        //        SettingsManager.ClassWindowOn = value;
        //        CooldownWindowViewModel.Instance.IsClassWindowOn = value;
        //        //WindowManager.CooldownWindow.SwitchMode();
        //        //if(value == true)
        //        //{
        //        //    IsCooldownWindowVisible = false;
        //        //    IsClassWindowVisible = true;
        //        //}
        //        //else
        //        //{
        //        //    IsCooldownWindowVisible = true;
        //        //    IsClassWindowVisible = false;
        //        //}
        //        NotifyPropertyChanged("ClassWindowOn");
        //    }
        //}
        public CooldownBarMode CooldownBarMode
        {
            get => SettingsManager.CooldownBarMode;
            set
            {
                if (SettingsManager.CooldownBarMode == value) return;
                SettingsManager.CooldownBarMode = value;
                CooldownWindowViewModel.Instance.NotifyModeChanged();
                NotifyPropertyChanged(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => SettingsManager.EnrageLabelMode;
            set
            {
                if (SettingsManager.EnrageLabelMode == value) return;
                SettingsManager.EnrageLabelMode = value;
                NotifyPropertyChanged(nameof(EnrageLabelMode));
            }
        }

        public bool ChatFadeOut
        {
            get => SettingsManager.ChatFadeOut;
            set
            {
                if (SettingsManager.ChatFadeOut == value) return;
                SettingsManager.ChatFadeOut = value;
                if(value) ChatWindowViewModel.Instance.RefreshTimer();
                NotifyPropertyChanged(nameof(ChatFadeOut));
            }
        }
        public bool ClickThruWhenDim
        {
            get { return SettingsManager.ClickThruWhenDim; }
            set
            {
                if (SettingsManager.ClickThruWhenDim == value) return;
                SettingsManager.ClickThruWhenDim = value;

                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                NotifyPropertyChanged("ClickThruWhenDim");
            }
        }
        public bool ClickThruInCombat
        {
            get => SettingsManager.ClickThruInCombat;
            set
            {
                if (SettingsManager.ClickThruInCombat == value) return;
                SettingsManager.ClickThruInCombat = value;

                WindowManager.SkillsEnded = false;
                WindowManager.SkillsEnded = true;

                NotifyPropertyChanged(nameof(ClickThruInCombat));
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
        public bool DisableMP
        {
            get => SettingsManager.DisablePartyMP;
            set
            {
                if (SettingsManager.DisablePartyMP == value) return;
                SettingsManager.DisablePartyMP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NotifyPropertyChanged(nameof(DisableMP));
            }
        }
        public bool DisableHP
        {
            get => SettingsManager.DisablePartyHP;
            set
            {
                if (SettingsManager.DisablePartyHP == value) return;
                SettingsManager.DisablePartyHP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NotifyPropertyChanged(nameof(DisableHP));
            }
        }

        public bool ShowItemsCooldown
        {
            get => SettingsManager.ShowItemsCooldown;
            set
            {
                if(SettingsManager.ShowItemsCooldown == value) return;
                SettingsManager.ShowItemsCooldown = value;
                CooldownWindowViewModel.Instance.NotifyItemsDisplay();
                NotifyPropertyChanged(nameof(ShowItemsCooldown));
            }
        }
        public bool ShowMembersLaurels
        {
            get => SettingsManager.ShowMembersLaurels;
            set
            {
                if (SettingsManager.ShowMembersLaurels == value) return;
                SettingsManager.ShowMembersLaurels = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NotifyPropertyChanged(nameof(ShowMembersLaurels));
            }
        }
        public bool HhOnlyAggro
        {
            get => SettingsManager.ShowOnlyAggroStacks;
            set
            {
                if (SettingsManager.ShowOnlyAggroStacks == value) return;
                SettingsManager.ShowOnlyAggroStacks = value;
                NotifyPropertyChanged(nameof(HhOnlyAggro));
            }
        }
        public bool LfgOn
        {
            get => SettingsManager.LfgOn;
            set
            {
                if (SettingsManager.LfgOn == value) return;
                SettingsManager.LfgOn = value;
                ChatWindowViewModel.Instance.LfgOn = value;
                MessageFactory.Update();
                NotifyPropertyChanged(nameof(LfgOn));

            }
        }
        public string Webhook
        {
            get => SettingsManager.Webhook;
            set
            {
                if (value == SettingsManager.Webhook) return;
                SettingsManager.Webhook = value;
                NotifyPropertyChanged(nameof(Webhook));
            }
        }
        public string WebhookMessage
        {
            get => SettingsManager.WebhookMessage;
            set
            {
                if (value == SettingsManager.WebhookMessage) return;
                SettingsManager.WebhookMessage = value;
                NotifyPropertyChanged(nameof(WebhookMessage));
            }
        }
        public string TwitchUsername
        {
            get => SettingsManager.TwitchName;
            set
            {
                if (value == SettingsManager.TwitchName) return;
                SettingsManager.TwitchName = value;
                NotifyPropertyChanged(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => SettingsManager.TwitchToken;
            set
            {
                if (value == SettingsManager.TwitchToken) return;
                SettingsManager.TwitchToken = value;
                NotifyPropertyChanged(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => SettingsManager.TwitchChannelName;
            set
            {
                if (value == SettingsManager.TwitchChannelName) return;
                SettingsManager.TwitchChannelName = value;
                NotifyPropertyChanged(nameof(TwitchChannelName));
            }
        }
        public uint GroupSizeThreshold
        {
            get { return SettingsManager.GroupSizeThreshold; }
            set
            {
                if (SettingsManager.GroupSizeThreshold == value) return;
                SettingsManager.GroupSizeThreshold = value;
                GroupWindowViewModel.Instance.NotifyThresholdChanged();
                NotifyPropertyChanged(nameof(GroupSizeThreshold));
            }
        }

        public double ChatWindowOpacity
        {
            get => SettingsManager.ChatWindowOpacity;
            set
            {
                if (SettingsManager.ChatWindowOpacity == value) return;
                SettingsManager.ChatWindowOpacity = value;
                ChatWindowViewModel.Instance.NotifyOpacityChange();
                NotifyPropertyChanged(nameof(ChatWindowOpacity));
            }
        }
        public int FontSize
        {
            get => SettingsManager.FontSize;
            set
            {
                if (SettingsManager.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                SettingsManager.FontSize = val;
                NotifyPropertyChanged(nameof(FontSize));
            }
        }
        public SettingsWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //if (SettingsManager.CooldownWindowSettings.Enabled) WindowManager.CooldownWindow.PropertyChanged += CooldownWindow_PropertyChanged;
            //if (SettingsManager.CharacterWindowSettings.Enabled) WindowManager.CharacterWindow.PropertyChanged += CharacterWindow_PropertyChanged;
            //if (SettingsManager.BossWindowSettings.Enabled) WindowManager.BossWindow.PropertyChanged += BossGauge_PropertyChanged;
            //if (SettingsManager.BuffWindowSettings.Enabled) WindowManager.BuffWindow.PropertyChanged += BuffBar_PropertyChanged;
            //if (SettingsManager.GroupWindowSettings.Enabled) WindowManager.GroupWindow.PropertyChanged += GroupWindow_PropertyChanged;
            //if (SettingsManager.ClassWindowSettings.Enabled) WindowManager.ClassWindow.PropertyChanged += ClassWindow_PropertyChanged;
        }

        //private void ClassWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsClassWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsClassWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void GroupWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsGroupWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsGroupWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void BuffBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsBuffWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsBuffWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void BossGauge_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsBossWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;

        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsBossWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void CharacterWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsCharacterWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsCharacterWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void CooldownWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsCooldownWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsCooldownWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}

        public List<ClickThruMode> ClickThruModes => Utils.ListFromEnum<ClickThruMode>();
        public List<CooldownBarMode> CooldownBarModes => Utils.ListFromEnum<CooldownBarMode>();
        public List<FlowDirection> FlowDirections => Utils.ListFromEnum<FlowDirection>();
        public List<EnrageLabelMode> EnrageLabelModes => Utils.ListFromEnum<EnrageLabelMode>();
    }
}
