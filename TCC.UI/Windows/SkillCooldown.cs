namespace TCC
{
    public class SkillCooldown
    {
        public Skill Skill { get; set; }
        public int Cooldown { get; set; }
        public CooldownType Type { get; set; }
        public System.Timers.Timer Timer { get; set; }

        public SkillCooldown(Skill sk, int cd, CooldownType t)
        {
            Skill = sk;
            Cooldown = cd;
            if (t == CooldownType.Item)
            {
                Cooldown = Cooldown * 1000;
            }

            if (cd != 0)
            {
                Timer = new System.Timers.Timer(Cooldown);
            }

        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/