using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class DurationCooldownIndicator : TSPropertyChanged
    {
        private Cooldown _cooldown;
        private Cooldown _buff;

        public Cooldown Cooldown
        {
            get => _cooldown;
            set
            {
                if(_cooldown == value) return;
                _cooldown = value;
                NPC();
            }
        }
        public Cooldown Buff
        {
            get => _buff;
            set
            {
                if(_buff == value) return;
                _buff = value;
                NPC();
            }
        }

        public DurationCooldownIndicator(Dispatcher d)
        {
            Dispatcher = d;
            Cooldown = new Cooldown();
            Buff = new Cooldown();
        }
    }
}