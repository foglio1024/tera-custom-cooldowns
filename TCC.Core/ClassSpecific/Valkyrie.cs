using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC
{
    public static class Valkyrie
    {
        public static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
        {
            if(p.CasterId == SessionManager.CurrentPlayer.EntityId && p.Id == 10155130)
            {
                ((ValkyrieBarManager)ClassManager.CurrentClassManager).RagnarokBuff.Start(p.Duration);
            }
        }
    }
}
