using System.Windows;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public static class EntitiesManager
    {
        public static MonsterDatabase CurrentDatabase;
        private static ulong currentEncounter;
        public static void SpawnNPC(ushort zoneId, uint templateId, ulong entityId, Visibility v)
        {
            if (!Filter(zoneId, templateId)) return;

            if (CurrentDatabase.TryGetMonster(templateId, zoneId, out Monster m))
            {
                if (m.IsBoss)
                {
                    BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, m.MaxHP, m.MaxHP, m.IsBoss, templateId, zoneId, v);
                }
                else
                {
                    if (SettingsManager.ShowOnlyBosses) return;
                    BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, m.MaxHP, m.MaxHP, m.IsBoss, templateId, zoneId, Visibility.Collapsed);
                }
            }
        }

        static bool Filter(uint zoneId, uint templateId)
        {
            if (zoneId == 950 && templateId == 1002) return false; //skip HHP4 lament warriors

            return true;
        }

        public static void DespawnNPC(ulong target)
        {
            BossGageWindowViewModel.Instance.RemoveBoss(target);
            if (BossGageWindowViewModel.Instance.VisibleBossesCount == 0)
            {
                SessionManager.Encounter = false;
                GroupWindowViewModel.Instance.ResetAggro();
            }
        }
        public static void SetNPCStatus(ulong entityId, bool enraged)
        {
            BossGageWindowViewModel.Instance.SetBossEnrage(entityId, enraged);
        }
        public static void UpdateNPCbyGauge(ulong entityId, float curHP, float maxHP, ushort zoneId, uint templateId)
        {
            BossGageWindowViewModel.Instance.AddOrUpdateBoss(entityId, maxHP, curHP, true, templateId, zoneId, Visibility.Visible);
            if (maxHP > curHP)
            {
                currentEncounter = entityId;
                SessionManager.Encounter = true;
            }
            else if (maxHP == curHP || curHP == 0)
            {
                currentEncounter = 0;
                SessionManager.Encounter = false;
            }
        }
        public static void UpdateNPCbyCreatureChangeHP(ulong target, int currentHP, int maxHP)
        {
            BossGageWindowViewModel.Instance.AddOrUpdateBoss(target, maxHP, currentHP, false, 0, 0, Visibility.Visible);
            if (maxHP > currentHP)
            {
                currentEncounter = target;
                SessionManager.Encounter = true;
            }
            else if (maxHP == currentHP || currentHP == 0)
            {
                currentEncounter = 0;
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
