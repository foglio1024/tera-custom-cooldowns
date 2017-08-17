using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Archer
    {
        static readonly uint FocusId = 601400;
        static readonly uint FocusXId = 601450;
        static readonly uint[] SniperEyeIDs = { 601100, 601101 };

        public static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StartFocus();
        }
        public static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId) 
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.SetFocusStacks(p.Stacks);
        }
        public static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusXId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StartFocusX();

        }
        public static void CheckFocusEnd(S_ABNORMALITY_END p)
        {
            if ((p.AbnormalityId == FocusId || p.AbnormalityId == FocusXId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StopFocus();
        }
        public static void CheckSniperEye(S_ABNORMALITY_BEGIN p)
        {
            if (SniperEyeIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        public static void CheckSniperEye(S_ABNORMALITY_REFRESH p)
        {
            if (SniperEyeIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        public static void CheckSniperEyeEnd(S_ABNORMALITY_END p)
        {
            if (SniperEyeIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = Data.ArcherStance.None;
        }
    }
}
