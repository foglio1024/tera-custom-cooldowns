using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Archer
    {
        private static readonly List<ulong> MarkedTargets = new List<ulong>();

        private static readonly uint FocusId = 601400;
        private static readonly uint FocusXId = 601450;
        private static readonly uint[] SniperEyeIDs = { 601100, 601101 };
        private static readonly uint[] VelikMarkIDs = { 600500, 600501, 600502 };

        public static event Action<ulong> VelikMarkRefreshed;
        public static event Action VelikMarkExpired;

        public static void CheckVelikMark(ulong target)
        {
            if (MarkedTargets.Contains(target))
            {
                MarkedTargets.Remove(target);
                if (MarkedTargets.Count == 0) VelikMarkExpired?.Invoke();
            }
        }

        public static void CheckVelikMark(S_ABNORMALITY_BEGIN p)
        {
            if (!VelikMarkIDs.Contains(p.AbnormalityId)) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                VelikMarkRefreshed?.Invoke(p.Duration);
            }
        }
        public static void CheckVelikMark(S_ABNORMALITY_REFRESH p)
        {
            if (!VelikMarkIDs.Contains(p.AbnormalityId)) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                VelikMarkRefreshed?.Invoke(p.Duration);
            }
        }
        public static void CheckVelikMark(S_ABNORMALITY_END p)
        {
            if(!VelikMarkIDs.Contains(p.AbnormalityId)) return;
            if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
                if (MarkedTargets.Count == 0) VelikMarkExpired?.Invoke();
        }

        public static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocus(p.Duration);
            }
        }
        public static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.SetFocusStacks(p.Stacks, p.Duration);
            }
        }
        public static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == FocusId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StopFocus();
            }
        }
        public static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == FocusXId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                Console.WriteLine("Begin Focus X");
                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocusX(p.Duration);
            }
        }
        public static void CheckFocusX(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == FocusXId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                Console.WriteLine("Refresh Focus X");

                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StartFocusX(p.Duration);
            }
        }
        public static void CheckFocusX(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == FocusXId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                Console.WriteLine("End Focus X");

                ((ArcherBarManager)ClassWindowViewModel.Instance.CurrentManager).Focus.StopFocusX();
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

        public static void ClearMarkedTargets()
        {
            App.BaseDispatcher.Invoke(() => MarkedTargets.Clear());
        }
    }
}
