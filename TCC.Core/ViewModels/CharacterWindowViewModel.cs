using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TSPropertyChanged
    {
        private static CharacterWindowViewModel _instance;
        public static CharacterWindowViewModel Instance => _instance ?? (_instance = new CharacterWindowViewModel());

        public Player Player => SessionManager.CurrentPlayer;

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Class")
            {
                NotifyPropertyChanged("STname");
            }
        }


        private double scale = SettingsManager.CharacterWindowSettings.Scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale == value) return;
                scale = value;
                NotifyPropertyChanged("Scale");
            }
        }
        public CharacterWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.CharacterWindow.RefreshTopmost();
                }
            };
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
    }

}

