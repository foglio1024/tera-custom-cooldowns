using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TccWindowViewModel
    {
        private static CharacterWindowViewModel _instance;
        public static CharacterWindowViewModel Instance => _instance ?? (_instance = new CharacterWindowViewModel());

        public Player Player => SessionManager.CurrentPlayer;

        public bool CompactMode => SettingsManager.CharacterWindowCompactMode;

        public bool ShowRe =>(
            !SettingsManager.ClassWindowSettings.Visible ||
            !SettingsManager.ClassWindowSettings.Enabled) &&
            (Player.Class == Class.Brawler  ||
            Player.Class == Class.Gunner ||
            Player.Class == Class.Ninja ||
            Player.Class == Class.Valkyrie);
        //public bool IsTeraOnTop
        //{
        //    get => WindowManager.IsTccVisible;
        //}


        public CharacterWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            SettingsManager.ClassWindowSettings.EnabledChanged += ClassWindowSettings_EnabledChanged;
            SettingsManager.ClassWindowSettings.VisibilityChanged += ClassWindowSettings_EnabledChanged;
        }

        private void ClassWindowSettings_EnabledChanged()
        {
            NPC(nameof(ShowRe));
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NPC(e.PropertyName);
            if (e.PropertyName == nameof(Player.Class))
            {
                NPC(nameof(ShowRe));
            }
        }
    }

}

