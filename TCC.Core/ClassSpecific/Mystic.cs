using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
using TCC.ViewModels;

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

        static uint[] critAuraIDs = { 700600, 700601, 700602, 700603 };
        static uint[] manaAuraIDs = { 700300 };
        static uint[] critResAuraIDs = { 700200, 700201, 700202, 700203 };
        static uint[] swiftAuraIDs = { 700700, 700701 };

        public static void CheckHurricane(S_ABNORMALITY_BEGIN msg)
        {
            if(msg.Id == HURRICANE_ID) Debug.WriteLine("Checking hurricane; id={0} caster={1} player={2}", msg.Id, msg.CasterId, SessionManager.CurrentPlayer.EntityId);
            if (msg.Id == HURRICANE_ID && msg.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                SkillsDatabase.TryGetSkill(HURRICANE_ID, Class.Common, out Skill hurricane);
                SkillManager.AddSkillDirectly(hurricane, HURRICANE_DURATION);
            }

        }
        public static void CheckAura(S_ABNORMALITY_BEGIN p)
        {
            if(critAuraIDs.Contains(p.Id) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritAura = true;
            }
            else if (manaAuraIDs.Contains(p.Id) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.ManaAura = true;
            }
            else if (critResAuraIDs.Contains(p.Id) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritResAura = true;
            }
            else if (swiftAuraIDs.Contains(p.Id) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.SwiftAura = true;
            }
        }
        public static void CheckAura(S_ABNORMALITY_REFRESH p)
        {
            if (critAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritAura = true;
            }
            else if (manaAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.ManaAura = true;
            }
            else if (critResAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritResAura = true;
            }
            else if (swiftAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.SwiftAura = true;
            }
        }
        public static void CheckAuraEnd(S_ABNORMALITY_END p)
        {
            if (critAuraIDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritAura = false;
            }
            else if (manaAuraIDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.ManaAura = false;
            }
            else if (critResAuraIDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritResAura = false;
            }
            else if (swiftAuraIDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.SwiftAura = false;
            }
        }
    }
}
