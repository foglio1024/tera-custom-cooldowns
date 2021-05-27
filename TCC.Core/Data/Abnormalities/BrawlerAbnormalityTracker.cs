using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class BrawlerAbnormalityTracker : AbnormalityTracker
    {
        private const int GrowingFuryId = 10153040;
        private const int CounterGlyphId = 31020;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;

            CheckGrowingFury(p);
            CheckCounterProc(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckGrowingFury(p);
            CheckCounterProc(p);
        }

        private static void CheckGrowingFury(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            if (!IsViewModelAvailable<BrawlerLayoutVM>(out var vm)) return;

            vm!.IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            if (!IsViewModelAvailable<BrawlerLayoutVM>(out var vm)) return;

            vm!.IsGfOn = true;
        }
        private static void CheckGrowingFury(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GrowingFuryId) return;
            if (!IsViewModelAvailable<BrawlerLayoutVM>(out var vm)) return;

            vm!.IsGfOn = false;
        }

        private static void CheckCounterProc(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            if (!IsViewModelAvailable<BrawlerLayoutVM>(out var vm)) return;

            vm!.Counter.Start(p.Duration);
            vm!.CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            if (!IsViewModelAvailable<BrawlerLayoutVM>(out var vm)) return;

            vm!.Counter.Start(p.Duration);
            vm!.CounterProc = true;
        }
        private static void CheckCounterProc(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != CounterGlyphId) return;
            if (!IsViewModelAvailable<BrawlerLayoutVM>(out var vm)) return;

            vm!.Counter.Stop();
            vm!.CounterProc = false;
        }
    }
}
