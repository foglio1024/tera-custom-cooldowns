using System.Windows;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowManager :DependencyObject
    {
        private static CharacterWindowManager _instance;
        public static CharacterWindowManager Instance => _instance ?? (_instance = new CharacterWindowManager());

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
