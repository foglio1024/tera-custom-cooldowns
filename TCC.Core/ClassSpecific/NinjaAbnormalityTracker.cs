using FoglioUtils.Extensions;

using TCC.Data;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.ClassSpecific
{
    public class NinjaAbnormalityTracker :ClassAbnormalityTracker
    {
        private const int FocusId = 10154030;
        private const int InnerHarmonyBuffId = 10154480;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckFocus(p);
            CheckInnerHarmony(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckInnerHarmony(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckFocus(p);
            CheckInnerHarmony(p);
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            TccUtils.CurrentClassVM<NinjaLayoutVM>().FocusOn = true;
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            TccUtils.CurrentClassVM<NinjaLayoutVM>().FocusOn = false;
        }

        private static void CheckInnerHarmony(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            TccUtils.CurrentClassVM<NinjaLayoutVM>().InnerHarmony.Buff.Start(p.Duration);

        }
        private static void CheckInnerHarmony(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            TccUtils.CurrentClassVM<NinjaLayoutVM>().InnerHarmony.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckInnerHarmony(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            TccUtils.CurrentClassVM<NinjaLayoutVM>().InnerHarmony.Buff.Refresh(0, CooldownMode.Normal);
        }
    }
}
