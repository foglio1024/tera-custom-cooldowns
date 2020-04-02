using System.Linq;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class ArcherAbnormalityTracker : AbnormalityTracker
    {
        private static readonly uint FocusId = 601400;
        private static readonly uint FocusXId = 601450;
        //private static readonly uint[] WindsongIds = { 602101, 602107, 602108, 602221, 602227 };
        private static readonly uint[] WindWalkIds = { 602102, 602103 };
        private const string WindsongIconName = "icon_skills.breeze_tex";

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;  
            CheckFocus(p);
            CheckFocusX(p);
            CheckWindsong(p);
            CheckGaleSteps(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckWindsong(p);
            CheckWindWalk(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckFocus(p);
            CheckFocusX(p);
            CheckWindsong(p);
            CheckGaleSteps(p);
        }

        private static void CheckWindsong(S_ABNORMALITY_BEGIN p)
        {
            //if (!WindsongIds.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, WindsongIconName)) return; //TODO: temporary
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Windsong.Buff.Start(p.Duration);
        }
        private static void CheckWindsong(S_ABNORMALITY_REFRESH p)
        {
            //if (!WindsongIds.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, WindsongIconName)) return; //TODO: temporary
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;

            vm.Windsong.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckWindsong(S_ABNORMALITY_END p)
        {
            //if (!WindsongIds.Contains(p.AbnormalityId)) return;☺
            if (!CheckByIconName(p.AbnormalityId, WindsongIconName)) return; //TODO: temporary
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;

            vm.Windsong.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckGaleSteps(S_ABNORMALITY_BEGIN p)
        {
            if (!WindWalkIds.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;

            vm.WindWalk.Start(p.Duration);
            vm.WindWalkProc = true;
        }
        private static void CheckWindWalk(S_ABNORMALITY_REFRESH p)
        {
            if (!WindWalkIds.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;

            vm.WindWalk.Refresh(p.Duration, CooldownMode.Normal);
            vm.WindWalkProc = true;
        }
        private static void CheckGaleSteps(S_ABNORMALITY_END p)
        {
            if (!WindWalkIds.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;

            vm.WindWalk.Refresh(0, CooldownMode.Normal);
            vm.WindWalkProc = false;
        }

        private static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusId) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Focus.StartFocus(p.Duration);
        }
        private static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != FocusId) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Focus.SetFocusStacks(p.Stacks, p.Duration);
        }
        private static void CheckFocus(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusId) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Focus.StopFocus();
        }

        private static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != FocusXId) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Focus.StartFocusX(p.Duration);
        }
        private static void CheckFocusX(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != FocusXId) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Focus.StartFocusX(p.Duration);
        }
        private static void CheckFocusX(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != FocusXId) return;
            if (!IsViewModelAvailable<ArcherLayoutVM>(out var vm)) return;
            vm.Focus.StopFocusX();
        }
    }
}
