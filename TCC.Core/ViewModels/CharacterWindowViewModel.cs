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


        public bool CompactMode => Settings.Settings.CharacterWindowCompactMode;

        public bool ShowRe =>(
            !Settings.Settings.ClassWindowSettings.Visible ||
            !Settings.Settings.ClassWindowSettings.Enabled) &&
            (Player.Class == Class.Brawler  ||
            Player.Class == Class.Gunner ||
            Player.Class == Class.Ninja ||
            Player.Class == Class.Valkyrie);
        public bool ShowElements => Player.Class == Class.Sorcerer &&
            ((!Settings.Settings.ClassWindowSettings.Visible || !Settings.Settings.ClassWindowSettings.Enabled) || (!Settings.Settings.SorcererReplacesElementsInCharWindow));


        public CharacterWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            Settings.Settings.ClassWindowSettings.EnabledChanged += ClassWindowSettings_EnabledChanged;
            Settings.Settings.ClassWindowSettings.VisibilityChanged += ClassWindowSettings_EnabledChanged;
        }

        private void ClassWindowSettings_EnabledChanged()
        {
            NPC(nameof(ShowRe));
            NPC(nameof(ShowElements));
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NPC(e.PropertyName);
            if (e.PropertyName == nameof(Data.Pc.Player.Class))
            {
                NPC(nameof(ShowRe));
                NPC(nameof(ShowElements));
            }
        }
    }

}

