using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Brawler
    {
        private static readonly int GrowingFuryId = 10153040;
        private static readonly int CounterGlyphId = 31020;
        
            
        public static void CheckBrawlerAbnormal(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == GrowingFuryId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsGfOn = true;
                return;
            }

            if (p.AbnormalityId == CounterGlyphId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).Counter.Start(p.Duration);
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).CounterProc = true;
            }
        }
        public static void CheckBrawlerAbnormal(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == GrowingFuryId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsGfOn = true;
                return;
            }

            if (p.AbnormalityId == CounterGlyphId || p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).Counter.Start(p.Duration);
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).CounterProc = true;
            }
        }
        public static void CheckBrawlerAbnormalEnd(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == GrowingFuryId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsGfOn = false;
                return;
            }
            if (p.AbnormalityId == CounterGlyphId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).Counter.Refresh(0);
                ((BrawlerBarManager)ClassWindowViewModel.Instance.CurrentManager).CounterProc = false;
                return;
            }
        }
    }
}
