using System.Linq;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ArcherAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint FocusId = 601400;
        private static readonly uint FocusXId = 601450;
        private static readonly uint[] WindsongIds = { 602101, 602107, 602108 };
        private static readonly uint[] WindWalkIds = { 602102, 602103 };

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;       
            CheckFocus(p);
            CheckFocusX(p);
            CheckWindsong(p);
            CheckGaleSteps(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckWindsong(p);
            CheckWindWalk(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {

            if (!p.TargetId.IsMe()) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckWindsong(p);
            CheckGaleSteps(p);
        }

        private static void CheckWindsong(S_ABNORMALITY_BEGIN p)
        {
            if (!WindsongIds.Contains(p.AbnormalityId)) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Windsong.Buff.Start(p.Duration);
        }
        private static void CheckWindsong(S_ABNORMALITY_REFRESH p)
        {
            if (!WindsongIds.Contains(p.AbnormalityId)) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Windsong.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckWindsong(S_ABNORMALITY_END p)
        {
            if (!WindsongIds.Contains(p.AbnormalityId)) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Windsong.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckGaleSteps(S_ABNORMALITY_BEGIN p)
        {
            if (!WindWalkIds.Contains(p.AbnormalityId)) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).WindWalk.Start(p.Duration);
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).WindWalkProc = true;
        }
        private static void CheckWindWalk(S_ABNORMALITY_REFRESH p)
        {
            if (!WindWalkIds.Contains(p.AbnormalityId)) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).WindWalk.Refresh(p.Duration, CooldownMode.Normal);
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).WindWalkProc = true;
        }
        private static void CheckGaleSteps(S_ABNORMALITY_END p)
        {
            if (!WindWalkIds.Contains(p.AbnormalityId)) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).WindWalk.Refresh(0, CooldownMode.Normal);
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).WindWalkProc = false;
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Focus.StartFocus(p.Duration);
        }
        private static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Focus.SetFocusStacks(p.Stacks, p.Duration);
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Focus.StopFocus();
        }

        private static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusXId) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Focus.StartFocusX(p.Duration);
        }
        private static void CheckFocusX(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != FocusXId) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Focus.StartFocusX(p.Duration);
        }
        private static void CheckFocusX(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusXId) return;
            ((ArcherLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Focus.StopFocusX();
        }
    }
}
