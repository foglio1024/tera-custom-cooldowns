using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC
{
    public static class Priest
    {
        public static List<uint> CommonBuffs = new List<uint>
        {
            805100, 805101, 805102,
            805600, 805601, 805602, 805603, 805604,
            800300, 800301, 800302,800303, 800304
        };

        static readonly uint[] energyStarsIDs = { 801500, 801501 , 801502 , 801503, 98000107 };
        
        internal static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if(p.TargetId == SessionManager.CurrentPlayer.EntityId && energyStarsIDs.Contains(p.Id))
            {
                ((PriestBarManager)ClassManager.CurrentClassManager).EnergyStars.Buff.Start(p.Duration);
            }
        }

        internal static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && energyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassManager.CurrentClassManager).EnergyStars.Buff.Refresh(p.Duration);
            }
        }
    }
}
