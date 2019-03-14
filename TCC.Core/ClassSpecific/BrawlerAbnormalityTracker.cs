using TCC.Data;
using TCC.Parsing.Messages;
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
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).IsGfOn = false;
        }

        private static void CheckCounterProc(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Counter.Start(p.Duration);
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Counter.Start(p.Duration);
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Counter.Refresh(0, CooldownMode.Normal);
            ((BrawlerLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).CounterProc = false;
        }
    }
}
