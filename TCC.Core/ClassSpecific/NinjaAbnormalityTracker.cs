using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Utilities.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class NinjaAbnormalityTracker :ClassAbnormalityTracker
    {
        private const int FocusId = 10154030;
        private const int InnerHarmonyBuffId = 10154480;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckFocus(p);
            CheckInnerHarmony(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckInnerHarmony(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckFocus(p);
            CheckInnerHarmony(p);
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            Utils.CurrentClassVM<NinjaLayoutVM>().FocusOn = true;
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            Utils.CurrentClassVM<NinjaLayoutVM>().FocusOn = false;
        }

        private static void CheckInnerHarmony(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            Utils.CurrentClassVM<NinjaLayoutVM>().InnerHarmony.Buff.Start(p.Duration);

        }
        private static void CheckInnerHarmony(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            Utils.CurrentClassVM<NinjaLayoutVM>().InnerHarmony.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckInnerHarmony(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            Utils.CurrentClassVM<NinjaLayoutVM>().InnerHarmony.Buff.Refresh(0, CooldownMode.Normal);
        }
    }
}
