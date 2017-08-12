using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Lancer
    {
        static readonly uint[] ARushIDs = { 200700, 200701, 200731, 200732 };
        static readonly uint[] GShoutIDs = { 200200, 200201, 200202 };
        static readonly uint LineHeldId = 201701;

        public static void CheckArush(S_ABNORMALITY_BEGIN p)
        {
            if (ARushIDs.Contains(p.AbnormalityId) && p.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).AdrenalineRush.Buff.Start(p.Duration);
            }
        }
        public static void CheckGshout(S_ABNORMALITY_BEGIN p)
        {
            if (GShoutIDs.Contains(p.AbnormalityId) && p.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).GuardianShout.Buff.Start(p.Duration);
            }
        }
        public static void CheckArushEnd(S_ABNORMALITY_END p)
        {
            if (ARushIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).AdrenalineRush.Buff.Refresh(0);
            }
        }
        public static void CheckGshoutEnd(S_ABNORMALITY_END p)
        {
            if (GShoutIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).GuardianShout.Buff.Refresh(0);
            }
        }
        public static void CheckLineHeld(S_ABNORMALITY_BEGIN p)
        {
            if(p.AbnormalityId == LineHeldId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).LH.Val = p.Stacks;
            }
        }
        public static void CheckLineHeld(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == LineHeldId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).LH.Val = p.Stacks;
            }
        }

        public static void CheckLineHeldEnd(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == LineHeldId && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).LH.Val = 0;
            }
        }
    }
}
