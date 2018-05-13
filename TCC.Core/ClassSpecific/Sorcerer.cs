using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Sorcerer
    {
        private const int ManaBoostId = 500150;

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (ManaBoostId != p.AbnormalityId || p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Start(p.Duration);
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (ManaBoostId != p.AbnormalityId || p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Refresh(p.Duration);
        }

        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (ManaBoostId != p.AbnormalityId || p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Refresh(0);
        }
    }
}
