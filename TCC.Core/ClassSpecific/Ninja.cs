using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Ninja
    {
        private const int FocusId = 10154030;
        public static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((NinjaBarManager)ClassManager.CurrentClassManager).FocusOn = true;
        }
        public static void CheckFocusEnd(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((NinjaBarManager)ClassManager.CurrentClassManager).FocusOn = false;
        }
    }
}
