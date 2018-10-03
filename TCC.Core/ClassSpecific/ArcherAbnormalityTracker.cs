using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ArcherAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint FocusId = 601400;
        private static readonly uint FocusXId = 601450;
        private static readonly uint[] SniperEyeIDs = { 601100, 601101 };
        private static readonly uint[] VelikMarkIDs = { 600500, 600501, 600502 };

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            CheckVelikMark(p);
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckSniperEye(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            CheckVelikMark(p);
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckSniperEye(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            CheckVelikMark(p);
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckSniperEye(p);
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocus(p.Duration);
        }
        private static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.SetFocusStacks(p.Stacks, p.Duration);
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StopFocus();
        }

        private static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusXId) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocusX(p.Duration);
        }
        private static void CheckFocusX(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != FocusXId) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocusX(p.Duration);
        }
        private static void CheckFocusX(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusXId) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StopFocusX();
        }

        private static void CheckSniperEye(S_ABNORMALITY_BEGIN p)
        {
            if (!SniperEyeIDs.Contains(p.AbnormalityId)) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        private static void CheckSniperEye(S_ABNORMALITY_REFRESH p)
        {
            if (!SniperEyeIDs.Contains(p.AbnormalityId)) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        private static void CheckSniperEye(S_ABNORMALITY_END p)
        {
            if (!SniperEyeIDs.Contains(p.AbnormalityId)) return;
            ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = Data.ArcherStance.None;
        }

        private static void CheckVelikMark(S_ABNORMALITY_BEGIN p)
        {
            if (!VelikMarkIDs.Contains(p.AbnormalityId)) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                InvokeMarkingRefreshed(p.Duration);
            }
        }
        private static void CheckVelikMark(S_ABNORMALITY_REFRESH p)
        {
            if (!VelikMarkIDs.Contains(p.AbnormalityId)) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                InvokeMarkingRefreshed(p.Duration);
            }
        }
        private static void CheckVelikMark(S_ABNORMALITY_END p)
        {
            if (!VelikMarkIDs.Contains(p.AbnormalityId)) return;
            if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
            if (MarkedTargets.Count == 0) InvokeMarkingExpired();
        }

    }
}
