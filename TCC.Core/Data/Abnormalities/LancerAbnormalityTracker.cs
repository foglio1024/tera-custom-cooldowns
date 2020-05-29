using System.Linq;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class LancerAbnormalityTracker : AbnormalityTracker
    {
        public static readonly uint[] ARushIDs = { 200700, 200701, 200731 };
        public static readonly uint[] GShoutIDs = { 200200, 200201, 200202 };
        public static readonly uint LineHeldId = 201701;
        public const string AdrenalineRushIconName = "icon_skills.fightingwill_tex";

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckArush(p);
            CheckGshout(p);
            CheckLineHeld(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckArush(p);
            CheckGshout(p);
            CheckLineHeld(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckArush(p);
            CheckGshout(p);
            CheckLineHeld(p);
        }

        private static void CheckArush(S_ABNORMALITY_BEGIN p)
        {
            //if (!ARushIDs.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, AdrenalineRushIconName)) return; //temporary
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.AdrenalineRush.Buff.Start(p.Duration);
        }
        private static void CheckArush(S_ABNORMALITY_REFRESH p)
        {
            //if (!ARushIDs.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, AdrenalineRushIconName)) return; //temporary
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.AdrenalineRush.Buff.Start(p.Duration);
        }
        private static void CheckArush(S_ABNORMALITY_END p)
        {
            //if (!ARushIDs.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, AdrenalineRushIconName)) return; //temporary
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.AdrenalineRush.Buff.Stop();
        }

        private static void CheckGshout(S_ABNORMALITY_BEGIN p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.GuardianShout.Buff.Start(p.Duration);
        }
        private static void CheckGshout(S_ABNORMALITY_REFRESH p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.GuardianShout.Buff.Start(p.Duration);
        }
        private static void CheckGshout(S_ABNORMALITY_END p)
        {
            if (!GShoutIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.GuardianShout.Buff.Stop();
        }

        private static void CheckLineHeld(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.LH.StartBaseBuff(p.Duration);
        }
        private static void CheckLineHeld(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.LH.RefreshBaseBuff(p.Stacks, p.Duration);
        }
        private static void CheckLineHeld(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != LineHeldId) return;
            if (!IsViewModelAvailable<LancerLayoutVM>(out var vm)) return;

            vm.LH.Stop();
        }
    }
}
