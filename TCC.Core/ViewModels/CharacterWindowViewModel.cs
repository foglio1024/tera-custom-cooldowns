using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TccWindowViewModel
    {
        private static CharacterWindowViewModel _instance;
        public static CharacterWindowViewModel Instance => _instance ?? (_instance = new CharacterWindowViewModel());

        public Player Player => SessionManager.CurrentPlayer;


        public bool CompactMode => Settings.SettingsStorage.CharacterWindowCompactMode;

        public bool ShowRe =>(
            !Settings.SettingsStorage.ClassWindowSettings.Visible ||
            !Settings.SettingsStorage.ClassWindowSettings.Enabled) &&
            (Player.Class == Class.Brawler  ||
            Player.Class == Class.Gunner ||
            Player.Class == Class.Ninja ||
            Player.Class == Class.Valkyrie);
        public bool ShowElements => Player.Class == Class.Sorcerer &&
            ((!Settings.SettingsStorage.ClassWindowSettings.Visible || !Settings.SettingsStorage.ClassWindowSettings.Enabled) || (!Settings.SettingsStorage.SorcererReplacesElementsInCharWindow));


        public CharacterWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            Settings.SettingsStorage.ClassWindowSettings.EnabledChanged += ClassWindowSettings_EnabledChanged;
            Settings.SettingsStorage.ClassWindowSettings.VisibilityChanged += ClassWindowSettings_EnabledChanged;
        }

        private void ClassWindowSettings_EnabledChanged()
        {
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            N(e.PropertyName);
            if (e.PropertyName == nameof(Data.Pc.Player.Class))
            {
                N(nameof(ShowRe));
                N(nameof(ShowElements));
            }
        }
    }

}

