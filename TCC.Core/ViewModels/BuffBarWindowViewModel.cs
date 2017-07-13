using System.Windows;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BuffBarWindowViewModel : TSPropertyChanged
    {
        private static BuffBarWindowViewModel _instance;
        public static BuffBarWindowViewModel Instance => _instance ?? (_instance = new BuffBarWindowViewModel());
        private double scale = SettingsManager.BuffBarWindowSettings.Scale;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale == value) return;
                scale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        private FlowDirection direction = SettingsManager.BuffsDirection;
        public FlowDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction == value) return;
                direction = value;
                NotifyPropertyChanged("Direction");
            }
        }

        public BuffBarWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                //RaisePropertyChanged("IsTeraOnTop");
                if (WindowManager.IsTccVisible)
                {
                    WindowManager.BuffBar.RefreshTopmost();
                }
            };
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
