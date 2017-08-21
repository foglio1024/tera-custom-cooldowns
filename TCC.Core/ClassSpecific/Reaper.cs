using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Reaper
    {
        private static readonly int ShadowReapingId = 10151010;

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (ShadowReapingId == p.AbnormalityId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ReaperBarManager)ClassManager.CurrentClassManager).ShadowReaping.Buff.Start(p.Duration);
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (ShadowReapingId == p.AbnormalityId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ReaperBarManager)ClassManager.CurrentClassManager).ShadowReaping.Buff.Refresh(p.Duration);
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (ShadowReapingId == p.AbnormalityId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ReaperBarManager)ClassManager.CurrentClassManager).ShadowReaping.Buff.Refresh(0);
            }
        }
    }
}
