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
        public static void CheckFocus(S_ABNORMALITY_BEGIN p)
        {
            if (p.Id == 601400 && p.CasterId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StartFocus();
        }
        public static void CheckFocus(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == 601400) 
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.SetFocusStacks(p.Stacks);
        }
        public static void CheckFocusX(S_ABNORMALITY_BEGIN p)
        {
            if (p.Id == 601450 && p.CasterId == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StartFocusX();

        }
        public static void CheckFocusEnd(S_ABNORMALITY_END p)
        {
            if ((p.Id == 601400 || p.Id == 601450) && p.Target == SessionManager.CurrentPlayer.EntityId)
            ((ArcherBarManager)ClassManager.CurrentClassManager).Focus.StopFocus();
        }
    }
}
