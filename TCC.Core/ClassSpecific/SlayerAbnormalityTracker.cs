using System.Linq;

using TCC.Data;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.ClassSpecific
{
    public class SlayerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint[] IcbIds = { 300800, 300801, 300805 };

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckInColdBlood(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckInColdBlood(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckInColdBlood(p);
        }

        private static void CheckInColdBlood(S_ABNORMALITY_BEGIN p)
        {
            if (!IcbIds.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<SlayerLayoutVM>().InColdBlood.Buff.Start(p.Duration);
        }
        private static void CheckInColdBlood(S_ABNORMALITY_REFRESH p)
        {
            if (!IcbIds.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<SlayerLayoutVM>().InColdBlood.Buff.Start(p.Duration);
        }
        private static void CheckInColdBlood(S_ABNORMALITY_END p)
        {
            if (!IcbIds.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<SlayerLayoutVM>().InColdBlood.Buff.Refresh(0, CooldownMode.Normal);
        }
    }
}
