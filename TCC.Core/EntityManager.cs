using FoglioUtils;
using System.Collections.Generic;
using System.Windows;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Chat;
using TCC.ViewModels;
using TeraDataLite;

namespace TCC
{
    public static class EntityManager
    {
        public static ulong FoglioEid = 0;
        private static readonly Dictionary<ulong, string> NearbyNPC = new Dictionary<ulong, string>();
        private static readonly Dictionary<ulong, string> NearbyPlayers = new Dictionary<ulong, string>();
        public static void SpawnNPC(ushort zoneId, uint templateId, ulong entityId, bool v, bool villager, int remainingEnrageTime)
        {
            CheckHarrowholdMode(zoneId, templateId);
            if (IsWorldBoss(zoneId, templateId))
            {
                Game.DB.MonsterDatabase.TryGetMonster(templateId, zoneId, out var monst);
                if (monst.IsBoss)
                {
                    var msg = new ChatMessage(ChatChannel.WorldBoss, "System", $"<font>{monst.Name}</font><font size=\"15\" color=\"#cccccc\"> is nearby.</font>");
                    ChatWindowManager.Instance.AddChatMessage(msg);
                }
            }
            if (!Filter(zoneId, templateId)) return;

            if (Game.DB.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
            {
                NearbyNPC[entityId] = m.Name;
                FlyingGuardianDataProvider.InvokeProgressChanged();
                if (villager) return;
                if (m.IsBoss)
                {
                    WindowManager.ViewModels.NPC.AddOrUpdateNpc(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, v, remainingEnrageTime);
                    //WindowManager.ViewModels.NPC.SetEnrageTime(entityId, remainingEnrageTime);
                }
                else
                {
                    if (App.Settings.NpcWindowSettings.ShowOnlyBosses) return;
                    WindowManager.ViewModels.NPC.AddOrUpdateNpc(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, false, remainingEnrageTime);
                    //WindowManager.ViewModels.NPC.SetEnrageTime(entityId, remainingEnrageTime);
                }
            }
        }

        private static bool IsWorldBoss(ushort zoneId, uint templateId)
        {
            return zoneId == 10 && templateId == 99 ||
                   zoneId == 4 && templateId == 5011 ||
                   zoneId == 51 && templateId == 7011 ||
                   zoneId == 52 && templateId == 9050 ||
                   zoneId == 57 && templateId == 33 ||
                   zoneId == 38 && templateId == 35;
        }

        private static bool Filter(uint zoneId, uint templateId)
        {
            if (zoneId == 950 && templateId == 1002) return false; //skip HHP4 lament warriors
            if (zoneId == 210 && templateId == 4000) return false; //skip goddamn toy tanks

            return true;
        }

        public static void DespawnNPC(ulong target, DespawnType type)
        {
            NearbyNPC.Remove(target);

            WindowManager.ViewModels.NPC.RemoveNpc(target, type);
            if (WindowManager.ViewModels.NPC.VisibleBossesCount == 0)
            {
                Game.Encounter = false;
                WindowManager.ViewModels.Group.SetAggro(0);
            }
            AbnormalityTracker.CheckMarkingOnDespawn(target);
            FlyingGuardianDataProvider.InvokeProgressChanged();
        }
        public static void SetNPCStatus(ulong entityId, bool enraged, int remainingEnrageTime)
        {
            WindowManager.ViewModels.NPC.SetEnrageTime(entityId, remainingEnrageTime);
            WindowManager.ViewModels.NPC.SetEnrageStatus(entityId, enraged);
        }
        public static void UpdateNPC(ulong entityId, float curHP, float maxHP, ushort zoneId, uint templateId)
        {
            WindowManager.ViewModels.NPC.AddOrUpdateNpc(entityId, maxHP, curHP, true, HpChangeSource.BossGage, templateId, zoneId);
            SetEncounter(curHP, maxHP);
        }
        public static void UpdateNPC(ulong target, long currentHP, long maxHP, ulong source)
        {
            WindowManager.ViewModels.NPC.AddOrUpdateNpc(target, maxHP, currentHP, false, Game.IsMe(source) ? HpChangeSource.Me : HpChangeSource.CreatureChangeHp);
            SetEncounter(currentHP, maxHP);
        }
        private static void SetEncounter(float curHP, float maxHP)
        {
            if (maxHP > curHP)
            {
                Game.Encounter = true;
            }
            else if (maxHP == curHP || curHP == 0)
            {
                Game.Encounter = false;
            }
        }
        public static void ClearNPC()
        {
            if(App.Settings.NpcWindowSettings.Enabled) WindowManager.ViewModels.NPC.Clear();
            NearbyNPC.Clear();
            NearbyPlayers.Clear();
            AbnormalityTracker.ClearMarkedTargets();
        }
        public static void CheckHarrowholdMode(ushort zoneId, uint templateId)
        {
            if (zoneId != 950) return;
            if (templateId >= 1100 && templateId <= 1103)
            {
                WindowManager.ViewModels.NPC.CurrentHHphase = HarrowholdPhase.Phase1;
            }
            else if (templateId == 1000)
            {
                WindowManager.ViewModels.NPC.CurrentHHphase = HarrowholdPhase.Phase2;
            }
            else if (templateId == 2000)
            {
                WindowManager.ViewModels.NPC.CurrentHHphase = HarrowholdPhase.Balistas;
            }
            else if (templateId == 3000)
            {
                WindowManager.ViewModels.NPC.CurrentHHphase = HarrowholdPhase.Phase3;
            }
            else if (templateId == 4000)
            {
                WindowManager.ViewModels.NPC.CurrentHHphase = HarrowholdPhase.Phase4;
            }
        }
        public static Dragon CheckCurrentDragon(Point p)
        {
            var rel = MathUtils.GetRelativePoint(p.X, p.Y, -7672, -84453);

            Dragon d;
            if (rel.Y > .8 * rel.X - 78)
                d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Aquadrax : Dragon.Umbradrax;
            else d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Terradrax : Dragon.Ignidrax;

            return d;
        }

        public static string GetNpcName(ulong eid)
        {
            return NearbyNPC.TryGetValue(eid, out var name) ? name : "Unknown";
        }
        public static string GetUserName(ulong eid)
        {
            return NearbyPlayers.TryGetValue(eid, out var name) ? name : "Unknown";
        }

        internal static void DepawnUser(ulong entityId)
        {
            NearbyPlayers.Remove(entityId);
        }

        internal static void SpawnUser(ulong entityId, string name)
        {
            NearbyPlayers[entityId] = name;
        }

        public static bool IsEntitySpawned(ulong pSource)
        {
            return NearbyNPC.ContainsKey(pSource) || NearbyPlayers.ContainsKey(pSource);
        }

        public static bool IsEntitySpawned(uint zoneId, uint templateId)
        {
            var name = Game.DB.MonsterDatabase.GetName(templateId, zoneId);
            return name != "Unknown" && NearbyNPC.ContainsValue(name);
        }

        public static string GetEntityName(ulong pSource)
        {
            return NearbyNPC.ContainsKey(pSource)
                ? NearbyNPC[pSource]
                : NearbyPlayers.ContainsKey(pSource)
                    ? NearbyPlayers[pSource]
                    : "unknown";
        }
    }
}
