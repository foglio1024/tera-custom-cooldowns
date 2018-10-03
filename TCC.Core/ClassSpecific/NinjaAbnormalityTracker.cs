using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class NinjaAbnormalityTracker :ClassAbnormalityTracker
    {
        private const int FocusId = 10154030;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckFocus(p);
        }

        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckFocus(p);
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((NinjaBarManager)ClassWindowViewModel.Instance.CurrentManager).FocusOn = true;
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((NinjaBarManager)ClassWindowViewModel.Instance.CurrentManager).FocusOn = false;
        }
    }
}
