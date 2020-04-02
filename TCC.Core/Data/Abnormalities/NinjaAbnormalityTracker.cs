using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class NinjaAbnormalityTracker :AbnormalityTracker
    {
        private const int FocusId = 10154030;
        private const int InnerHarmonyBuffId = 10154480;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckFocus(p);
            CheckInnerHarmony(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckInnerHarmony(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckFocus(p);
            CheckInnerHarmony(p);
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            if (!IsViewModelAvailable<NinjaLayoutVM>(out var vm)) return;

            vm.FocusOn = true;
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            if (!IsViewModelAvailable<NinjaLayoutVM>(out var vm)) return;

            vm.FocusOn = false;
        }

        private static void CheckInnerHarmony(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            if (!IsViewModelAvailable<NinjaLayoutVM>(out var vm)) return;

            vm.InnerHarmony.Buff.Start(p.Duration);

        }
        private static void CheckInnerHarmony(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            if (!IsViewModelAvailable<NinjaLayoutVM>(out var vm)) return;

            vm.InnerHarmony.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckInnerHarmony(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != InnerHarmonyBuffId) return;
            if (!IsViewModelAvailable<NinjaLayoutVM>(out var vm)) return;

            vm.InnerHarmony.Buff.Refresh(0, CooldownMode.Normal);
        }
    }
}
