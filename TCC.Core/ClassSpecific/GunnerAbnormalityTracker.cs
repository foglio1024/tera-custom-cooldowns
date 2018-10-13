using System.Collections.Generic;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class GunnerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint DashingReloadId = 10152354;
        private static readonly List<uint> LaserTargetingIDs = new List<uint> { 10152340, 10152342,10152345 };

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
            StartPrecooldown("icon_skills.airdash_tex", p.Duration);
            StartPrecooldown("icon_skills.ambushrolling_tex", p.Duration);
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
