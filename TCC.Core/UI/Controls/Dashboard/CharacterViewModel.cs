using Nostrum;
using TCC.Data.Pc;

namespace TCC.UI.Controls.Dashboard
{
    public class CharacterViewModel : TSPropertyChanged
    {
        private bool _hilight;

        public bool Hilight
        {
            get => _hilight;
            set
            {
                if (_hilight == value) return;
                _hilight = value;
                N();
            }
        }
        public Character Character { get; }

        public CharacterViewModel(Character c)
        {
            Character = c;
        }

    }
}