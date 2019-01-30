using System.Collections.Generic;
using System.Windows;
using TCC.ClassSpecific;
using TCC.Data;
using TCC.Data.Chat;
using TCC.ViewModels;

namespace TCC
{
    public static class EntityManager
    {
        private static readonly Dictionary<ulong, string> NearbyNPC = new Dictionary<ulong, string>();
        private static readonly Dictionary<ulong, string> NearbyPlayers = new Dictionary<ulong, string>();
        public static void SpawnNPC(ushort zoneId, uint templateId, ulong entityId, bool v, bool villager, int remainingEnrageTime)
        {
            CheckHarrowholdMode(zoneId, templateId);
            if (IsWorldBoss(zoneId, templateId))
            {
                SessionManager.CurrentDatabase.MonsterDatabase.TryGetMonster(templateId, zoneId, out var monst);
                if (monst.IsBoss)
                {
                    var msg = new ChatMessage(ChatChannel.WorldBoss, "System", $"<font>{monst.Name}</font><font size=\"15\" color=\"#cccccc\"> is nearby.</font>");
                    ChatWindowManager.Instance.AddChatMessage(msg);
                }
            }
            if (!Filter(zoneId, templateId)) return;

            if (SessionManager.CurrentDatabase.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
            {
                NearbyNPC[entityId] = m.Name;
                FlyingGuardianDataProvider.InvokeProgressChanged();
                if (villager) return;
                if (m.IsBoss)
                {
                    BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, v);
                    BossGageWindowViewModel.Instance.SetBossEnrageTime(entityId, remainingEnrageTime);
                }
                else
                {
                    if (Settings.SettingsHolder.ShowOnlyBosses) return;
                    BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, false);
                    BossGageWindowViewModel.Instance.SetBossEnrageTime(entityId, remainingEnrageTime);
                }
            }
        }

        private static bool IsWorldBoss(ushort zoneId, uint templateId)
        {
            return (zoneId == 10 && templateId == 99) ||
                   (zoneId == 4 && templateId == 5011) ||
                   (zoneId == 51 && templateId == 7011) ||
                   (zoneId == 52 && templateId == 9050) ||
                   (zoneId == 57 && templateId == 33) ||
                   (zoneId == 38 && templateId == 35);
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

            BossGageWindowViewModel.Instance.RemoveBoss(target, type);
            if (BossGageWindowViewModel.Instance.VisibleBossesCount == 0)
            {
                SessionManager.Encounter = false;
                GroupWindowViewModel.Instance.SetAggro(0);
            }
            ClassAbnormalityTracker.CheckMarkingOnDespawn(target);
            FlyingGuardianDataProvider.InvokeProgressChanged();
        }
        public static void SetNPCStatus(ulong entityId, bool enraged, int remainingEnrageTime)
        {
            BossGageWindowViewModel.Instance.SetBossEnrageTime(entityId, remainingEnrageTime);
            BossGageWindowViewModel.Instance.SetBossEnrage(entityId, enraged);
        }
        public static void UpdateNPC(ulong entityId, float curHP, float maxHP, ushort zoneId, uint templateId)
        {
            BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, maxHP, curHP, true, HpChangeSource.BossGage, templateId, zoneId);
            SetEncounter(curHP, maxHP);
        }
        public static void UpdateNPC(ulong target, long currentHP, long maxHP, ulong source)
        {
            BossGageWindowViewModel.Instance.AddOrUpdateBoss(target, maxHP, currentHP, false, source.IsMe() ? HpChangeSource.Me : HpChangeSource.CreatureChangeHp);
            SetEncounter(currentHP, maxHP);
        }
        private static void SetEncounter(float curHP, float maxHP)
        {
            if (maxHP > curHP)
            {
                SessionManager.Encounter = true;
            }
            else if (maxHP == curHP || curHP == 0)
            {
                SessionManager.Encounter = false;
            }
        }
        public static void ClearNPC()
        {
            if(Settings.SettingsHolder.BossWindowSettings.Enabled) BossGageWindowViewModel.Instance.ClearBosses();
            NearbyNPC.Clear();
            NearbyPlayers.Clear();
            ClassAbnormalityTracker.ClearMarkedTargets();
        }
        public static void CheckHarrowholdMode(ushort zoneId, uint templateId)
        {
            if (zoneId != 950) return;
            if (templateId >= 1100 && templateId <= 1103)
            {
                BossGageWindowViewModel.Instance.CurrentHHphase = HarrowholdPhase.Phase1;
            }
            else if (templateId == 1000)
            {
                BossGageWindowViewModel.Instance.CurrentHHphase = HarrowholdPhase.Phase2;
            }
            else if (templateId == 2000)
            {
                BossGageWindowViewModel.Instance.CurrentHHphase = HarrowholdPhase.Balistas;
            }
            else if (templateId == 3000)
            {
                BossGageWindowViewModel.Instance.CurrentHHphase = HarrowholdPhase.Phase3;
            }
            else if (templateId == 4000)
            {
                BossGageWindowViewModel.Instance.CurrentHHphase = HarrowholdPhase.Phase4;
            }
        }
        public static Dragon CheckCurrentDragon(Point p)
        {
            var rel = Utils.GetRelativePoint(p.X, p.Y, -7672, -84453);

            Dragon d;
            if (rel.Y > .8 * rel.X - 78)
                d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Aquadrax : Dragon.Umbradrax;
            else d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Terradrax : Dragon.Ignidrax;

            return d;
        }

        public static string GetNpcName(ulong eid)
        {
            return NearbyNPC.TryGetValue(eid, out var name) ? name : "unknown";
        }
        public static string GetUserName(ulong eid)
        {
            return NearbyPlayers.TryGetValue(eid, out var name) ? name : "unknown";
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
            var name = SessionManager.CurrentDatabase.MonsterDatabase.GetName(templateId, zoneId);
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
