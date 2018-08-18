using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Slayer
    {
        private static readonly uint[] IcbIds = { 300800, 300801, 300805 };

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (IcbIds.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SlayerBarManager)ClassWindowViewModel.Instance.CurrentManager).InColdBlood.Buff.Start(p.Duration);
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (IcbIds.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SlayerBarManager)ClassWindowViewModel.Instance.CurrentManager).InColdBlood.Buff.Start(p.Duration);
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (IcbIds.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((SlayerBarManager)ClassWindowViewModel.Instance.CurrentManager).InColdBlood.Buff.Refresh(0);
            }
        }
    }
}
