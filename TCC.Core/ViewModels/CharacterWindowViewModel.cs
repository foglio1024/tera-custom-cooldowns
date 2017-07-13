using System;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TSPropertyChanged
    {
        private static CharacterWindowViewModel _instance;
        public static CharacterWindowViewModel Instance => _instance ?? (_instance = new CharacterWindowViewModel());

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set
            {
                if (_player == value) return;
                _player = value;
            }
        }

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
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.CharacterWindow.RefreshTopmost();
                }
            };
        }
    }

}

