using System.Linq;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Utilities.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class SlayerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint[] IcbIds = { 300800, 300801, 300805 };

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckInColdBlood(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckInColdBlood(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckInColdBlood(p);
        }

        private static void CheckInColdBlood(S_ABNORMALITY_BEGIN p)
        {
            if (!IcbIds.Contains(p.AbnormalityId)) return;
            ((SlayerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).InColdBlood.Buff.Start(p.Duration);
        }
        private static void CheckInColdBlood(S_ABNORMALITY_REFRESH p)
        {
            if (!IcbIds.Contains(p.AbnormalityId)) return;
            ((SlayerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).InColdBlood.Buff.Start(p.Duration);
        }
        private static void CheckInColdBlood(S_ABNORMALITY_END p)
        {
            if (!IcbIds.Contains(p.AbnormalityId)) return;
            ((SlayerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).InColdBlood.Buff.Refresh(0, CooldownMode.Normal);
        }
    }
}
