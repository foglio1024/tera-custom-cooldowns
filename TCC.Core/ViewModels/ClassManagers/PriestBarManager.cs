using System;
using System.Threading;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.ViewModels
{
    public class PriestBarManager : ClassManager
    {
        private DurationCooldownIndicator _energyStars;

        public DurationCooldownIndicator EnergyStars
        {
            get => _energyStars;
            set
            {
                if(_energyStars == value) return;
                _energyStars = value;
                NPC();
            }
        }

        public DurationCooldownIndicator Grace { get; set; }
        public DurationCooldownIndicator EdictOfJudgment { get; set; }
        public DurationCooldownIndicator DivineCharge { get; set; }

        public PriestBarManager() : base()
        {
        }


        public sealed override void LoadSpecialSkills()
        {
            //Energy Stars
            EnergyStars = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(350410, Class.Priest, out var es);
            EnergyStars.Buff = new FixedSkillCooldown(es,  false);
            EnergyStars.Cooldown = new FixedSkillCooldown(es,  true);

            Grace = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(390100, Class.Priest, out var gr);
            Grace.Buff = new FixedSkillCooldown(gr,  false);
            Grace.Cooldown = new FixedSkillCooldown(gr,  false);

            Grace.Buff.Started += OnGraceBuffStarted;
            Grace.Buff.Ended += OnGraceBuffEnded;

            // Edict Of Judgment
            EdictOfJudgment = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(430100, Class.Priest, out var ed);
            EdictOfJudgment.Buff = new FixedSkillCooldown(ed, false);
            EdictOfJudgment.Cooldown = new FixedSkillCooldown(ed, false);

            EdictOfJudgment.Buff.Started += OnEdictBuffStarted;
            EdictOfJudgment.Buff.Ended += OnEdictBuffEnded;

            // Divine Charge
            DivineCharge = new DurationCooldownIndicator(_dispatcher);
            SessionManager.SkillsDatabase.TryGetSkill(280200, Class.Priest, out var dc);
            DivineCharge.Buff = new FixedSkillCooldown(dc, false);
            DivineCharge.Cooldown = new FixedSkillCooldown(dc, false);
        }

        private void OnGraceBuffEnded(CooldownMode obj) => Grace.Cooldown.FlashOnAvailable = true;
        private void OnGraceBuffStarted(CooldownMode obj) => Grace.Cooldown.FlashOnAvailable = false;
        
        private void OnEdictBuffEnded(CooldownMode obj) => EdictOfJudgment.Cooldown.FlashOnAvailable = true;
        private void OnEdictBuffStarted(CooldownMode obj) => EdictOfJudgment.Cooldown.FlashOnAvailable = false;

        public override bool StartSpecialSkill(SkillCooldown sk)
        {
            if(sk.Skill.IconName == EnergyStars.Cooldown.Skill.IconName)
            {
                EnergyStars.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == Grace.Cooldown.Skill.IconName)
            {
                Grace.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == EdictOfJudgment.Cooldown.Skill.IconName)
            {
                EdictOfJudgment.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == DivineCharge.Cooldown.Skill.IconName)
            {
                DivineCharge.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }

        public override bool ChangeSpecialSkill(Skill skill, uint cd)
        {
            if (skill.IconName == EdictOfJudgment.Cooldown.Skill.IconName)
            {
                EdictOfJudgment.Cooldown.Refresh(cd);
                return true;
            }
            return false;
        }
    }
}