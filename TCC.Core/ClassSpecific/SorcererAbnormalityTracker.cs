using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class SorcererAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int ManaBoostId = 500150;

        private static void CheckManaBoost(S_ABNORMALITY_BEGIN p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Start(p.Duration);

        }
        private static void CheckManaBoost(S_ABNORMALITY_REFRESH p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Refresh(p.Duration);

        }
        private static void CheckManaBoost(S_ABNORMALITY_END p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Refresh(0);
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckManaBoost(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckManaBoost(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckManaBoost(p);
        }
    }
}
