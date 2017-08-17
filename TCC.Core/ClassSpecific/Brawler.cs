using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ((BrawlerBarManager)ClassManager.CurrentClassManager).IsGfOn = true;
                return;
            }
            if (p.AbnormalityId == CounterGlyphId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassManager.CurrentClassManager).Counter.Start(p.Duration);
                ((BrawlerBarManager)ClassManager.CurrentClassManager).CounterProc = true;

                return;
            }
        }
        public static void CheckBrawlerAbnormal(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == GrowingFuryId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassManager.CurrentClassManager).IsGfOn = true;
                return;
            }
            if (p.AbnormalityId == CounterGlyphId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassManager.CurrentClassManager).CounterProc = true;
                ((BrawlerBarManager)ClassManager.CurrentClassManager).Counter.Start(p.Duration);

                return;
            }
        }
        public static void CheckBrawlerAbnormalEnd(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == GrowingFuryId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassManager.CurrentClassManager).IsGfOn = false;
                return;
            }
            if (p.AbnormalityId == CounterGlyphId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BrawlerBarManager)ClassManager.CurrentClassManager).CounterProc = false;
                ((BrawlerBarManager)ClassManager.CurrentClassManager).Counter.Refresh(0);

            }
        }
    }
}
