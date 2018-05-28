using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TccWindowViewModel
    {
        private static CharacterWindowViewModel _instance;
        public static CharacterWindowViewModel Instance => _instance ?? (_instance = new CharacterWindowViewModel());

        public Player Player => SessionManager.CurrentPlayer;

        //public bool IsTeraOnTop
        //{
        //    get => WindowManager.IsTccVisible;
        //}


        public CharacterWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            // WindowManager.TccVisibilityChanged += (s, ev) =>
            //{
            //    NPC("IsTeraOnTop");
            //    if (IsTeraOnTop)
            //    {
            //        WindowManager.CharacterWindow.RefreshTopmost();
            //    }
            //};
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NPC(e.PropertyName);
        }
    }

}

