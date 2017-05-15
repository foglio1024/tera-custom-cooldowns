using System.Windows;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BuffBarWindowManager : DependencyObject
    {
        private static BuffBarWindowManager _instance;
        public static BuffBarWindowManager Instance => _instance ?? (_instance = new BuffBarWindowManager());

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
