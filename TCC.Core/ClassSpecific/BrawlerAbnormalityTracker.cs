using TCC.Data;
using TCC.ViewModels;

using TeraPacketParser.Messages;

namespace TCC.ClassSpecific
{
    public class BrawlerAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int GrowingFuryId = 10153040;
        private const int CounterGlyphId = 31020;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!SessionManager.IsMe(p.TargetId)) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }

        private static void CheckGrowingFury(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().IsGfOn = false;
        }

        private static void CheckCounterProc(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().Counter.Start(p.Duration);
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().Counter.Start(p.Duration);
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().Counter.Refresh(0, CooldownMode.Normal);
            TccUtils.CurrentClassVM<BrawlerLayoutVM>().CounterProc = false;
        }
    }
}
