using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;

namespace TCC
{
    public static class Mystic
    {
        const int HURRICANE_ID = 60010;
        const int HURRICANE_DURATION = 120000;

        public static List<uint> CommonBuffs = new List<uint>
        {
            700100,                 //vow
            27120,                  //united thrall of protection
            601, 602, 603,          //titanic wrath
            700630,700631,          //titanic fury
            700230, 700231, 700232, 700233, //aura unyelding
            700330,                 //aura mp
            700730, 700731          //aura swift
        };

        public static void CheckHurricane(S_ABNORMALITY_BEGIN msg)
        {
            if(msg.Id == HURRICANE_ID) Console.WriteLine("Checking hurricane; id={0} caster={1} player={2}", msg.Id, msg.CasterId, SessionManager.CurrentPlayer.EntityId);
            if (msg.Id == HURRICANE_ID && msg.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                SkillsDatabase.TryGetSkill(HURRICANE_ID, Class.Common, out Skill hurricane);
                SkillManager.AddSkillDirectly(hurricane, HURRICANE_DURATION);
            }

        }
    }
}
