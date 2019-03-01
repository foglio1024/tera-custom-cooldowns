using System.Linq;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class LancerAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint[] ARushIDs = { 200700, 200701, 200731 };
        private static readonly uint[] GShoutIDs = { 200200, 200201, 200202 };
        private static readonly uint LineHeldId = 201701;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckArush(p);
            CheckGshout(p);
            CheckLineHeld(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckArush(p);
            CheckGshout(p);
            CheckLineHeld(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckArush(p);
            CheckGshout(p);
            CheckLineHeld(p);
        }

        private static void CheckArush(S_ABNORMALITY_BEGIN p)
        {
            if (!ARushIDs.Contains(p.AbnormalityId)) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).AdrenalineRush.Buff.Start(p.Duration);
        }
        private static void CheckArush(S_ABNORMALITY_REFRESH p)
        {
            if (!ARushIDs.Contains(p.AbnormalityId)) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).AdrenalineRush.Buff.Start(p.Duration);
        }
        private static void CheckArush(S_ABNORMALITY_END p)
        {
            if (!ARushIDs.Contains(p.AbnormalityId)) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).AdrenalineRush.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckGshout(S_ABNORMALITY_BEGIN p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).GuardianShout.Buff.Start(p.Duration);
        }
        private static void CheckGshout(S_ABNORMALITY_REFRESH p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).GuardianShout.Buff.Start(p.Duration);
        }
        private static void CheckGshout(S_ABNORMALITY_END p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).GuardianShout.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckLineHeld(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).LH.Val = p.Stacks;
        }
        private static void CheckLineHeld(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).LH.Val = p.Stacks;
        }
        private static void CheckLineHeld(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            ((LancerBarManager)WindowManager.ClassWindow.VM.CurrentManager).LH.Val = 0;
        }
    }
}
