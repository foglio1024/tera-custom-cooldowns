using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Messages;

namespace TCC
{
    public static class Mystic
    {
        const int HURRICANE_ID = 60010;
        const int HURRICANE_DURATION = 120000;

        public static void CheckHurricane(S_ABNORMALITY_BEGIN msg)
        {
            if (msg.id == HURRICANE_ID && msg.casterId == SessionManager.CurrentPlayer.EntityId)
            {
                SkillsDatabase.TryGetSkill(HURRICANE_ID, Class.Common, out Skill hurricane);
                SkillManager.AddSkill(hurricane, HURRICANE_DURATION);
            }

        }
    }
}
