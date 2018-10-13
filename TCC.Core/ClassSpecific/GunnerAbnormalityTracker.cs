using System.Collections.Generic;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class GunnerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint DashingReloadId = 10152354;
        private static readonly List<uint> LaserTargetingIDs = new List<uint> { 10152340, 10152342, 10152345 };
        private static Skill _dashingReload;
        private static Skill _rollingReload;
        public GunnerAbnormalityTracker()
        {
            SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.airdash_tex", SessionManager.CurrentPlayer.Class, out _dashingReload);
            SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.ambushrolling_tex", SessionManager.CurrentPlayer.Class, out _rollingReload);

        }
        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
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
            if (!p.TargetId.IsMe()) return;
            CheckLaserTargeting(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckLaserTargeting(p);
        }

        private static void CheckLaserTargeting(S_ABNORMALITY_BEGIN p)
        {
            if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
            ((GunnerBarManager)ClassWindowViewModel.Instance.CurrentManager).ModularSystem.Buff.Start(p.Duration);
        }
        private static void CheckLaserTargeting(S_ABNORMALITY_REFRESH p)
        {
            if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
            ((GunnerBarManager)ClassWindowViewModel.Instance.CurrentManager).ModularSystem.Buff.Refresh(p.Duration);
        }
        private static void CheckLaserTargeting(S_ABNORMALITY_END p)
        {
            if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
            ((GunnerBarManager)ClassWindowViewModel.Instance.CurrentManager).ModularSystem.Buff.Refresh(0);
        }
    }
}
