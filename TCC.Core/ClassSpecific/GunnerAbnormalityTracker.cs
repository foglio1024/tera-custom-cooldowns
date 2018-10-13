using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class GunnerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint DashingReloadId = 10152354;
        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckDashingReload(p);
        }

        private static void CheckDashingReload(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != DashingReloadId) return;
            //TODO: choose icon based on gunner's status?
            if (!SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.airdash_tex", SessionManager.CurrentPlayer.Class, out var sk)) return;
            if (!SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.ambushrolling_tex", SessionManager.CurrentPlayer.Class, out var sk1)) return;
            CooldownWindowViewModel.Instance.AddOrRefresh(new SkillCooldown(sk, p.Duration, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(), true, true));
            CooldownWindowViewModel.Instance.AddOrRefresh(new SkillCooldown(sk1, p.Duration, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(), true, true));
        }
    }
}
