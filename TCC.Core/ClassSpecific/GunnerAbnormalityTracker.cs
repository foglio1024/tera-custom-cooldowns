using System.Collections.Generic;
using TCC.Data;
using TCC.Data.Skills;
using TCC.Parsing.Messages;
using FoglioUtils.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class GunnerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint DashingReloadId = 10152354;
        private static readonly List<uint> LaserTargetingIDs = new List<uint> { 10152340 };
        private static Skill _dashingReload;
        private static Skill _rollingReload;
        public GunnerAbnormalityTracker()
        {
            SessionManager.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.airdash_tex", SessionManager.CurrentPlayer.Class, out _dashingReload);
            SessionManager.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.ambushrolling_tex", SessionManager.CurrentPlayer.Class, out _rollingReload);

        }
        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckDashingReload(p);
            CheckLaserTargeting(p);
        }

        private static void CheckDashingReload(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != DashingReloadId) return;
            //TODO: choose icon based on gunner's status?
            StartPrecooldown(_dashingReload, p.Duration);
            StartPrecooldown(_rollingReload, p.Duration);
        }

        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckLaserTargeting(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckLaserTargeting(p);
        }

        private static void CheckLaserTargeting(S_ABNORMALITY_BEGIN p)
        {
            if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
            //Log.C($"[CheckLaserTargeting(S_ABNORMALITY_BEGIN)] id:{p.AbnormalityId} duration:{p.Duration}");
            TccUtils.CurrentClassVM<GunnerLayoutVM>().ModularSystem.Buff.Start(p.Duration);
        }
        private static void CheckLaserTargeting(S_ABNORMALITY_REFRESH p)
        {
            if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
            //Log.C($"[CheckLaserTargeting(S_ABNORMALITY_REFRESH)] id:{p.AbnormalityId} duration:{p.Duration}");
            TccUtils.CurrentClassVM<GunnerLayoutVM>().ModularSystem.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckLaserTargeting(S_ABNORMALITY_END p)
        {
            if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
            //Log.C($"[CheckLaserTargeting(S_ABNORMALITY_END)] id:{p.AbnormalityId}");

            TccUtils.CurrentClassVM<GunnerLayoutVM>().ModularSystem.Buff.Refresh(0, CooldownMode.Normal);
        }
    }
}
