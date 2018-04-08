using System.Windows;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        private static BuffBarWindowViewModel _instance;
        public static BuffBarWindowViewModel Instance => _instance ?? (_instance = new BuffBarWindowViewModel());

        public FlowDirection Direction => SettingsManager.BuffsDirection;
        
        public BuffBarWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _scale = SettingsManager.BuffWindowSettings.Scale;
            Player = new Player();
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                //RaisePropertyChanged("IsTeraOnTop");
                if (WindowManager.IsTccVisible)
                {
                    WindowManager.BuffWindow.RefreshTopmost();
                }
            };
        }

        public void NotifyDirectionChanged()
        {
            NotifyPropertyChanged(nameof(Direction));
        }
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
    }
}
