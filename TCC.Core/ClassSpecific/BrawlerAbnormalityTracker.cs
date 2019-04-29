using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Utilities.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class BrawlerAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int GrowingFuryId = 10153040;
        private const int CounterGlyphId = 31020;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }

        private static void CheckGrowingFury(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            Utils.CurrentClassVM<BrawlerLayoutVM>().IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            Utils.CurrentClassVM<BrawlerLayoutVM>().IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            Utils.CurrentClassVM<BrawlerLayoutVM>().IsGfOn = false;
        }

        private static void CheckCounterProc(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            Utils.CurrentClassVM<BrawlerLayoutVM>().Counter.Start(p.Duration);
            Utils.CurrentClassVM<BrawlerLayoutVM>().CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            Utils.CurrentClassVM<BrawlerLayoutVM>().Counter.Start(p.Duration);
            Utils.CurrentClassVM<BrawlerLayoutVM>().CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            Utils.CurrentClassVM<BrawlerLayoutVM>().Counter.Refresh(0, CooldownMode.Normal);
            Utils.CurrentClassVM<BrawlerLayoutVM>().CounterProc = false;
        }
    }
}
