using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Mystic
    {
        const int HurricaneId = 60010;
        const int HurricaneDuration = 120000;

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

        static readonly uint[] CritAuraIDs = { 700600, 700601, 700602, 700603 };
        static readonly uint[] ManaAuraIDs = { 700300 };
        static readonly uint[] CritResAuraIDs = { 700200, 700201, 700202, 700203 };
        static readonly uint[] SwiftAuraIDs = { 700700, 700701 };

        public static void CheckHurricane(S_ABNORMALITY_BEGIN msg)
        {
            if(msg.AbnormalityId == HurricaneId) Debug.WriteLine("Checking hurricane; id={0} caster={1} player={2}", msg.AbnormalityId, msg.CasterId, SessionManager.CurrentPlayer.EntityId);
            if (msg.AbnormalityId == HurricaneId && msg.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                SkillsDatabase.TryGetSkill(HurricaneId, Class.Common, out Skill hurricane);
                SkillManager.AddSkillDirectly(hurricane, HurricaneDuration);
            }

        }
        public static void CheckAura(S_ABNORMALITY_BEGIN p)
        {
            if(CritAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritAura = true;
            }
            else if (ManaAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.ManaAura = true;
            }
            else if (CritResAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritResAura = true;
            }
            else if (SwiftAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.SwiftAura = true;
            }
        }
        public static void CheckAura(S_ABNORMALITY_REFRESH p)
        {
            if (CritAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritAura = true;
            }
            else if (ManaAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.ManaAura = true;
            }
            else if (CritResAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritResAura = true;
            }
            else if (SwiftAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.SwiftAura = true;
            }
        }
        public static void CheckAuraEnd(S_ABNORMALITY_END p)
        {
            if (CritAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritAura = false;
            }
            else if (ManaAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.ManaAura = false;
            }
            else if (CritResAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.CritResAura = false;
            }
            else if (SwiftAuraIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((MysticBarManager)ClassManager.CurrentClassManager).Auras.SwiftAura = false;
            }
        }
    }
}
