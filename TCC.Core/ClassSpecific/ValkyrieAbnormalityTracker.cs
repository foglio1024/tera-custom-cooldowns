using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ValkyrieAbnormalityTracker : ClassAbnormalityTracker
    {
        private const uint RagnarokId = 10155130;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckRagnarok(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckRagnarok(p);
        }

        private static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            ((ValkyrieBarManager)ClassWindowViewModel.Instance.CurrentManager).Ragnarok.Buff.Start(p.Duration);
        }
        private static void CheckRagnarok(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            ((ValkyrieBarManager)ClassWindowViewModel.Instance.CurrentManager).Ragnarok.Buff.Refresh(0);
        }
    }
}
