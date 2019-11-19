using TCC.Data;
using TCC.Data.Skills;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.ViewModels
{

    public class WarriorLayoutVM : BaseClassLayoutVM
    {

        public DurationCooldownIndicator DeadlyGamble { get; set; }
        public DurationCooldownIndicator AdrenalineRush { get; set; }
        public DurationCooldownIndicator Swift { get; set; }
        public Counter EdgeCounter { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }
        public StatTracker TraverseCut { get; set; }

        private bool _warningStance;
        public bool WarningStance
        {
            get => _warningStance;
            set
            {
                if (_warningStance == value) return;
                _warningStance = value;
                N();
            }
        }

        //public StatTracker TempestAura { get; set; }
        private bool _atkSpeedProc;

        public bool AtkSpeedProc
        {
            get => _atkSpeedProc;
            set
            {
                if (_atkSpeedProc == value) return; _atkSpeedProc = value;
                N();
            }
        }

        public WarriorLayoutVM()
        {
            EdgeCounter = new Counter(10, true);
            TraverseCut = new StatTracker { Max = 13, Val = 0 };
            //TempestAura = new StatTracker { Max = 50, Val = 0 };
            Stance = new StanceTracker<WarriorStance>();
            Game.Me.Death += OnDeath;
            Game.CombatChanged += CheckStanceWarning;
            Stance.PropertyChanged += (_, __) => CheckStanceWarning(); // StanceTracker has only one prop

        }

        private void OnDeath()
        {
            DeadlyGamble.Buff.Refresh(0, CooldownMode.Normal);
        }

        public bool ShowEdge => App.Settings.ClassWindowSettings.WarriorShowEdge;
        public bool ShowTraverseCut => App.Settings.ClassWindowSettings.WarriorShowTraverseCut;
        public WarriorEdgeMode WarriorEdgeMode => App.Settings.ClassWindowSettings.WarriorEdgeMode;

        public sealed override void LoadSpecialSkills()
        {
            //Deadly gamble
            Game.DB.SkillsDatabase.TryGetSkill(200200, Class.Warrior, out var dg);
            DeadlyGamble = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(dg, false),
                Cooldown = new Cooldown(dg, true) { CanFlash = true }
            };

            Game.DB.SkillsDatabase.TryGetSkill(170250, Class.Lancer, out var ar);
            AdrenalineRush = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(ar, false),
                Cooldown = new Cooldown(ar, false) { CanFlash = false }
            };
            var ab = Game.DB.AbnormalityDatabase.Abnormalities[21010];//21070 dfa
            //Swift = new Cooldown(new Skill(ab), false);
            Swift = new DurationCooldownIndicator(Dispatcher)
            {
                Buff = new Cooldown(new Skill(ab), false),
                Cooldown = new Cooldown(new Skill(ab), false) { CanFlash = false }
            };

        }

        public override void Dispose()
        {
            DeadlyGamble.Cooldown.Dispose();
        }

        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == DeadlyGamble.Cooldown.Skill.IconName)
            {
                DeadlyGamble.Cooldown.Start(sk.Duration);
                return true;
            }
            return false;
        }

        private void CheckStanceWarning()
        {
            WarningStance = Stance.CurrentStance == WarriorStance.None && Game.Combat;
        }

        public void SetSwift(uint duration)
        {
            if (duration == 0)
            {
                Swift.Buff.Refresh(0, CooldownMode.Normal);
                AtkSpeedProc = false;
            }
            Swift.Buff.Start(duration);
            AtkSpeedProc = true;
        }

        public void SetArush(uint duration)
        {
            if (duration == 0)
            {
                AdrenalineRush.Buff.Refresh(0, CooldownMode.Normal);
                AtkSpeedProc = false;
            }
            AdrenalineRush.Buff.Start(duration);
            AtkSpeedProc = true;

        }

    }
}
