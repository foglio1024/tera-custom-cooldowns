using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ReaperAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int ShadowReapingId = 10151010;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckShadowReaping(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckShadowReaping(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckShadowReaping(p);
        }

        private static void CheckShadowReaping(S_ABNORMALITY_BEGIN p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            ((ReaperBarManager)ClassWindowViewModel.Instance.CurrentManager).ShadowReaping.Buff.Start(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_REFRESH p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            ((ReaperBarManager)ClassWindowViewModel.Instance.CurrentManager).ShadowReaping.Buff.Refresh(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_END p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            ((ReaperBarManager)ClassWindowViewModel.Instance.CurrentManager).ShadowReaping.Buff.Refresh(0);
        }

    }
}
