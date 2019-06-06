using System;
using System.Windows.Threading;
using TCC.Data.Pc;
using TCC.Settings;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TccWindowViewModel
    {
        public Player Player => SessionManager.CurrentPlayer;


        public bool CompactMode => App.Settings.CharacterWindowCompactMode;

        public bool ShowRe => (!App.Settings.ClassWindowSettings.Visible || !App.Settings.ClassWindowSettings.Enabled) &&
                              (Player.Class == Class.Brawler || Player.Class == Class.Gunner ||
                               Player.Class == Class.Ninja || Player.Class == Class.Valkyrie);

        public bool ShowElements => Player.Class == Class.Sorcerer &&
                                 (!App.Settings.ClassWindowSettings.Visible
                                 || !App.Settings.ClassWindowSettings.Enabled
                                 || !App.Settings.SorcererReplacesElementsInCharWindow);

        public CharacterWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            App.Settings.ClassWindowSettings.EnabledChanged += ClassWindowSettings_EnabledChanged;
            App.Settings.ClassWindowSettings.VisibilityChanged += ClassWindowSettings_EnabledChanged;
        }

        private void ClassWindowSettings_EnabledChanged()
        {
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            N(e.PropertyName);
            if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        public void InvokeCompactModeChanged()
        {
            N(nameof(CompactMode));
            Dispatcher.BeginInvoke(new Action(() =>
            {
                WindowManager.CharacterWindow.Left = App.Settings.CharacterWindowCompactMode
                ? WindowManager.CharacterWindow.Left + 175
                : WindowManager.CharacterWindow.Left - 175;
            }), DispatcherPriority.Background);
        }
    }

}

