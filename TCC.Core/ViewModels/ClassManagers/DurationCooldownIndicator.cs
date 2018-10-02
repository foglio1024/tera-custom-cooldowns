using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class DurationCooldownIndicator : TSPropertyChanged
    {
        private FixedSkillCooldown _cooldown;
        private FixedSkillCooldown _buff;

        public FixedSkillCooldown Cooldown
        {
            get => _cooldown;
            set
            {
                if(_cooldown == value) return;
                _cooldown = value;
                NPC();
            }
        }
        public FixedSkillCooldown Buff
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
            Cooldown = new FixedSkillCooldown();
            Buff = new FixedSkillCooldown();
        }
    }
}