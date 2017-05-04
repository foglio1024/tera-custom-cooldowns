using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : BaseINPC
    {
        private bool isCooldownWindowVisible;
        public bool IsCooldownWindowVisible
        {
            get
            {
                if (WindowManager.CooldownWindow.Visibility == System.Windows.Visibility.Visible)
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
                if (value != isCooldownWindowVisible)
                {
                    isCooldownWindowVisible = value;
                    Console.WriteLine("Cooldown window set to {0}", value);

                    if (value)
                    {
                        WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
                        WindowManager.CooldownWindow.Visibility = System.Windows.Visibility.Visible
                        );
                    }
                    else
                    {
                        WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
                        WindowManager.CooldownWindow.Visibility = System.Windows.Visibility.Collapsed
                        );
                    }
                    RaisePropertyChanged("IsCooldownWindowVisible");
                }
            }
        }

        private bool isBuffWindowVisible;
        public bool IsBuffWindowVisible
        {
            get
            {
                if (WindowManager.BuffBar.Visibility == System.Windows.Visibility.Visible)
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
                if (value != isBuffWindowVisible)
                {
                    isBuffWindowVisible = value;
                    Console.WriteLine("Buff window set to {0}", value);

                    if (value)
                    {
                        WindowManager.BuffBar.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        WindowManager.BuffBar.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    RaisePropertyChanged("IsBuffWindowVisible");

                }
            }
        }

        public bool isBossWindowVisible;
        public bool IsBossWindowVisible
        {
            get
            {
                if (WindowManager.BossGauge.Visibility == System.Windows.Visibility.Visible)
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
                if (value != isBossWindowVisible)
                {
                    isBossWindowVisible = value;
                    Console.WriteLine("Boss window set to {0}", value);

                    if (value)
                    {
                        WindowManager.BossGauge.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        WindowManager.BossGauge.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    RaisePropertyChanged("IsBossWindowVisible");

                }
            }
        }

        private bool isCharacterWindowVisible;
        public bool IsCharacterWindowVisible
        {
            get
            {
                if (WindowManager.CharacterWindow.Visibility == System.Windows.Visibility.Visible)
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
                if (isCharacterWindowVisible != value)
                {
                    isCharacterWindowVisible = value;
                    Console.WriteLine("Char window set to {0}", value);
                    if (value)
                    {
                        WindowManager.CharacterWindow.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        WindowManager.CharacterWindow.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    RaisePropertyChanged("IsCharacterWindowVisible");

                }
            }

        }

        public bool IsClickThruOn
        {
            get
            {
                return WindowManager.ClickThru;
            }
            set
            {
                if (value != WindowManager.ClickThru)
                {
                    WindowManager.ClickThru = value;
                }
            }

        }

        public SettingsWindowViewModel()
        {
            WindowManager.CooldownWindow.VisibilityChanged += (s, ev) => RaisePropertyChanged("IsCooldownWindowVisible");
            WindowManager.CharacterWindow.VisibilityChanged += (s, ev) => RaisePropertyChanged("IsCharacterWindowVisible");
            WindowManager.BossGauge.VisibilityChanged += (s, ev) => RaisePropertyChanged("IsBossWindowVisible");
            WindowManager.BuffBar.VisibilityChanged += (s, ev) => RaisePropertyChanged("IsBuffWindowVisible");
            WindowManager.ClickThruChanged += (s, ev) => RaisePropertyChanged("IsClickThruOn");

        }
    }
}
