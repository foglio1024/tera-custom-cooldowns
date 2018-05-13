using System;
using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Archer
    {
        private static readonly uint FocusId = 601400;
        private static readonly uint FocusXId = 601450;
        private static readonly uint[] SniperEyeIDs = { 601100, 601101 };

        public static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager) ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocus();
            }
        }
        public static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.SetFocusStacks(p.Stacks);
            }
        }
        public static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusXId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocusX();
            }
        }
        public static void CheckFocusX(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == FocusXId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocusX();
            }
        }
        public static void CheckFocusEnd(S_ABNORMALITY_END p)
        {
            if ((p.AbnormalityId == FocusId || p.AbnormalityId == FocusXId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StopFocus();
            }

        }
        public static void CheckSniperEye(S_ABNORMALITY_BEGIN p)
        {
            if (SniperEyeIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        public static void CheckSniperEye(S_ABNORMALITY_REFRESH p)
        {
            if (SniperEyeIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        public static void CheckSniperEyeEnd(S_ABNORMALITY_END p)
        {
            if (SniperEyeIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = Data.ArcherStance.None;
        }
    }
}
