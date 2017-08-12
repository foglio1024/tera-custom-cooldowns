using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Berserker
    {
        private static readonly int BloodlustId = 400701;
        private static readonly int FieryRageId = 400105;

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.AbnormalityId == BloodlustId)
                {
                    ((BerserkerBarManager)ClassManager.CurrentClassManager).Bloodlust.Buff.Start(p.Duration);
                }
                if (p.AbnormalityId == FieryRageId)
                {
                    ((BerserkerBarManager)ClassManager.CurrentClassManager).FieryRage.Buff.Start(p.Duration);
                }
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.AbnormalityId == BloodlustId)
                {
                    ((BerserkerBarManager)ClassManager.CurrentClassManager).Bloodlust.Buff.Refresh(p.Duration);
                }
                if (p.AbnormalityId == FieryRageId)
                {
                    ((BerserkerBarManager)ClassManager.CurrentClassManager).FieryRage.Buff.Refresh(p.Duration);
                }
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.AbnormalityId == BloodlustId)
                {
                    ((BerserkerBarManager)ClassManager.CurrentClassManager).Bloodlust.Buff.Refresh(0);
                }
                if (p.AbnormalityId == FieryRageId)
                {
                    ((BerserkerBarManager)ClassManager.CurrentClassManager).FieryRage.Buff.Refresh(0);
                }
            }
        }


    }
}
