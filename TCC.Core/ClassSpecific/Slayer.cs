using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Slayer
    {
        private static readonly int IcbId = 300801;

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == IcbId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SlayerBarManager)ClassManager.CurrentClassManager).InColdBlood.Buff.Start(p.Duration);
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId== IcbId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SlayerBarManager)ClassManager.CurrentClassManager).InColdBlood.Buff.Refresh(p.Duration);
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == IcbId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SlayerBarManager)ClassManager.CurrentClassManager).InColdBlood.Buff.Refresh(0);
            }
        }
    }
}
