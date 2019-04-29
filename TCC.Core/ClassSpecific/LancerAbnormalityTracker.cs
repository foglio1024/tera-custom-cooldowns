using System.Linq;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Utilities.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class LancerAbnormalityTracker : ClassAbnormalityTracker
    {
        public static readonly uint[] ARushIDs = { 200700, 200701, 200731 };
        public static readonly uint[] GShoutIDs = { 200200, 200201, 200202 };
        public static readonly uint LineHeldId = 201701;

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
            Utils.CurrentClassVM<LancerLayoutVM>().AdrenalineRush.Buff.Start(p.Duration);
        }
        private static void CheckArush(S_ABNORMALITY_REFRESH p)
        {
            if (!ARushIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<LancerLayoutVM>().AdrenalineRush.Buff.Start(p.Duration);
        }
        private static void CheckArush(S_ABNORMALITY_END p)
        {
            if (!ARushIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<LancerLayoutVM>().AdrenalineRush.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckGshout(S_ABNORMALITY_BEGIN p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<LancerLayoutVM>().GuardianShout.Buff.Start(p.Duration);
        }
        private static void CheckGshout(S_ABNORMALITY_REFRESH p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<LancerLayoutVM>().GuardianShout.Buff.Start(p.Duration);
        }
        private static void CheckGshout(S_ABNORMALITY_END p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<LancerLayoutVM>().GuardianShout.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckLineHeld(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            Utils.CurrentClassVM<LancerLayoutVM>().LH.StartBaseBuff(p.Duration);
        }
        private static void CheckLineHeld(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            Utils.CurrentClassVM<LancerLayoutVM>().LH.RefreshBaseBuff(p.Stacks, p.Duration);
        }
        private static void CheckLineHeld(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            Utils.CurrentClassVM<LancerLayoutVM>().LH.Stop();
        }
    }
}
