using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC
{
    public static class Lancer
    {
        static readonly uint[] aRushIDs = { 200700, 200701, 200731, 200732 };
        static readonly uint[] gShoutIDs = { 200200, 200201, 200202 };
        static readonly uint lineHeldID = 201701;

        public static void CheckArush(S_ABNORMALITY_BEGIN p)
        {
            if (aRushIDs.Contains(p.Id) && p.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).AdrenalineRush.Buff.Start(p.Duration);
            }
        }
        public static void CheckGshout(S_ABNORMALITY_BEGIN p)
        {
            if (gShoutIDs.Contains(p.Id) && p.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).GuardianShout.Buff.Start(p.Duration);
            }
        }
        public static void CheckLineHeld(S_ABNORMALITY_BEGIN p)
        {
            if(p.Id == lineHeldID && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).LH.Val = p.Stacks;
            }
        }
        public static void CheckLineHeld(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == lineHeldID && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).LH.Val = p.Stacks;
            }
        }

        public static void CheckLineHeldEnd(S_ABNORMALITY_END p)
        {
            if (p.Id == lineHeldID && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((LancerBarManager)ClassManager.CurrentClassManager).LH.Val = 0;
            }
        }
    }
}
