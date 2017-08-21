using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Sorcerer
    {
        private static readonly int ManaBoostId = 500150;

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (ManaBoostId == p.AbnormalityId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SorcererBarManager)ClassManager.CurrentClassManager).ManaBoost.Buff.Start(p.Duration);
                return;
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (ManaBoostId == p.AbnormalityId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SorcererBarManager)ClassManager.CurrentClassManager).ManaBoost.Buff.Refresh(p.Duration);
                return;
            }
        }

        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (ManaBoostId == p.AbnormalityId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SorcererBarManager)ClassManager.CurrentClassManager).ManaBoost.Buff.Refresh(0);
                return;
            }
        }
    }
}
