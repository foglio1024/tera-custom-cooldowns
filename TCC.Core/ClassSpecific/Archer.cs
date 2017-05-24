using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC
{
    public static class Archer
    {
        static readonly uint FOCUS_ID = 601400;
        static readonly uint FOCUS_X_ID = 601450;
        static readonly uint[] SNIPER_EYE_IDs = { 601100, 601101 };

        public static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.Id == FOCUS_ID && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StartFocus();
        }
        public static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == FOCUS_ID) 
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.SetFocusStacks(p.Stacks);
        }
        public static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.Id == FOCUS_X_ID && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StartFocusX();

        }
        public static void CheckFocusEnd(S_ABNORMALITY_END p)
        {
            if ((p.Id == FOCUS_ID || p.Id == FOCUS_X_ID) && p.Target == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StopFocus();
        }
        public static void CheckSniperEye(S_ABNORMALITY_BEGIN p)
        {
            if (SNIPER_EYE_IDs.Contains(p.Id) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        public static void CheckSniperEye(S_ABNORMALITY_REFRESH p)
        {
            if (SNIPER_EYE_IDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = Data.ArcherStance.SniperEye;
        }
        public static void CheckSniperEyeEnd(S_ABNORMALITY_END p)
        {
            if (SNIPER_EYE_IDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
                ((ArcherBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = Data.ArcherStance.None;
        }
    }
}
