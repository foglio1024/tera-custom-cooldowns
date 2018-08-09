using System.Windows;
using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public static class EntitiesManager
    {
        private static ulong _currentEncounter;

        public static void SpawnNPC(ushort zoneId, uint templateId, ulong entityId, Visibility v)
        {
            if (IsWorldBoss(zoneId, templateId))
            {
                SessionManager.MonsterDatabase.TryGetMonster(templateId, zoneId, out var monst);
                //TimeManager.Instance.SendWebhookMessage(monst.Name);
                if (monst.IsBoss)
                {

                    var msg = new ChatMessage(ChatChannel.WorldBoss, "System", $"<font>{monst.Name}</font><font size=\"15\" color=\"#cccccc\"> is nearby.</font>");
                    ChatWindowManager.Instance.AddChatMessage(msg);
                }
            }
            if (!Filter(zoneId, templateId)) return;

            if (SessionManager.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
            {
                if (m.IsBoss)
                {
                    BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp , templateId, zoneId, v);
                }
                else
                {
                    if (SettingsManager.ShowOnlyBosses) return;
                    BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, Visibility.Collapsed);
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

            return true;
        }

        public static void DespawnNPC(ulong target, DespawnType type)
        {
            BossGageWindowViewModel.Instance.RemoveBoss(target, type);
            if (BossGageWindowViewModel.Instance.VisibleBossesCount == 0)
            {
                SessionManager.Encounter = false;
                GroupWindowViewModel.Instance.SetAggro(0);
            }
        }
        public static void SetNPCStatus(ulong entityId, bool enraged)
        {
            BossGageWindowViewModel.Instance.SetBossEnrage(entityId, enraged);
        }
        public static void UpdateNPCbyGauge(ulong entityId, float curHP, float maxHP, ushort zoneId, uint templateId)
        {
            BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, maxHP, curHP, true, HpChangeSource.BossGage, templateId, zoneId);
            if (maxHP > curHP)
            {
                _currentEncounter = entityId;
                SessionManager.Encounter = true;
            }
            else if (maxHP == curHP || curHP == 0)
            {
                _currentEncounter = 0;
                SessionManager.Encounter = false;
            }
        }
        public static void UpdateNPCbyCreatureChangeHP(ulong target, long currentHP, long maxHP)
        {
            BossGageWindowViewModel.Instance.AddOrUpdateBoss(target, maxHP, currentHP, false, HpChangeSource.CreatureChangeHp);
            if (maxHP > currentHP)
            {
                _currentEncounter = target;
                SessionManager.Encounter = true;
            }
            else if (maxHP == currentHP || currentHP == 0)
            {
                _currentEncounter = 0;
                SessionManager.Encounter = false;
            }
        }
        public static void ClearNPC()
        {
            BossGageWindowViewModel.Instance.ClearBosses();
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
            {
                if (rel.Y > -1.3 * rel.X - 94)
                {
                    d = Dragon.Aquadrax;

                }
                else
                {
                    d = Dragon.Umbradrax;
                }
            }
            else
            {
                if (rel.Y > -1.3 * rel.X - 94)
                {
                    d = Dragon.Terradrax;
                }
                else
                {
                    d = Dragon.Ignidrax;
                }
            }
            return d;
        }
    }
}
