using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TCC.Data;

namespace TCC
{
    public static class EntitiesManager
    {
        public static ObservableCollection<Boss> CurrentBosses = new ObservableCollection<Boss>();
        public static ObservableCollection<Player> CurrentUsers = new ObservableCollection<Player>();
        public static bool TryGetBossById(ulong id, out Boss b)
        {
            b = CurrentBosses.FirstOrDefault(x => x.EntityId == id);
            if (b == null)
            {
                b = new Boss(0, 0, 0, Visibility.Collapsed);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Dragon CurrentDragon = Dragon.None;

        public static void SpawnNPC(ushort zoneId, uint templateId, ulong entityId, Visibility v, bool force)
        {
            if (MonsterDatabase.TryGetMonster(templateId, zoneId, out Monster m))
            {
                //System.Console.WriteLine("[S_SPAWN_NPC] {0} {1} - {2}", zoneId,templateId, m.Name);

                if ((m.IsBoss || m.MaxHP >= 20000000) || force)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CurrentBosses.Add(new Boss(entityId, zoneId, templateId, v));
                        //System.Console.WriteLine("\t[ADDED] {0} {1} - {2}", zoneId, templateId, m.Name);

                    });
                }
                else
                {
                    //System.Console.WriteLine("\t[SKIPPED] {0} {1} - {2}", zoneId, templateId, m.Name);
                }
            }
            else
            {
                System.Console.WriteLine("\t[NOT FOUND] {0} {1}", zoneId, templateId);

            }

        }
        public static void DespawnNPC(ulong target)
        {
            if (TryGetBossById(target, out Boss b))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    CurrentBosses.Remove(b);
                });
            }
            if (WindowManager.BossGauge.HarrowholdMode)
            {
                UnsetDragonsContexts(target);
            }
        }
        public static void SetNPCStatus(ulong entityId, bool enraged)
        {
            if (TryGetBossById(entityId, out Boss b))
            {
                if (b.Enraged != enraged)
                {
                    b.Enraged = enraged;
                }
            }

        }
        public static void UpdateNPCbyGauge(ulong target, float curHP, float maxHP, ushort zoneId, uint templateId)
        {
            if (TryGetBossById(target, out Boss b))
            {
                b.MaxHP = maxHP;
                b.Visible = System.Windows.Visibility.Visible;

                if (b.CurrentHP != curHP)
                {
                    b.CurrentHP = curHP;
                }
            }
            else
            {
                SpawnNPC(zoneId, templateId, target, Visibility.Visible, true);
            }
        }
        public static void ClearNPC()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                CurrentBosses.Clear();
                Console.WriteLine("NPCs cleared");
            });
        }
        public static void ClearUsers()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                CurrentUsers.Clear();
            });
        }
        public static void CheckHarrowholdMode(ushort zoneId, uint templateId)
        {
            if (zoneId == 1023) return;
            if (zoneId == 63 && templateId >= 1960 && templateId <= 1963) return;
            if (zoneId != 950)
            {
                WindowManager.BossGauge.HarrowholdMode = false;
                //System.Console.WriteLine("{0} {1} spawned, exiting hh mode", zoneId, templateId);
            }
            else
            {
                if (templateId >= 1100 && templateId <= 1103)
                {
                    WindowManager.BossGauge.HarrowholdMode = true;
                    //System.Console.WriteLine("{0} {1} spawned, entering hh mode", zoneId, templateId);

                    SetDragonsContexts(templateId);
                }
                else if (templateId == 1000 || templateId == 2000 || templateId == 3000 || templateId == 4000)
                {
                    WindowManager.BossGauge.HarrowholdMode = false;
                    //System.Console.WriteLine("{0} {1} spawned, exiting hh mode", zoneId, templateId);
                }
            }

        }
        public static void CheckCurrentDragon(Point p)
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
            if (EntitiesManager.CurrentDragon != d)
            {
                EntitiesManager.CurrentDragon = d;
                WindowManager.BossGauge.HHBosses.Select(EntitiesManager.CurrentDragon);
            }

        }
        static void SetDragonsContexts(uint templateId)
        {
            if (templateId == 1100)
            {
                WindowManager.BossGauge.Dispatcher.Invoke(() =>
                {
                    WindowManager.BossGauge.HHBosses.ignidrax.Reset();
                    WindowManager.BossGauge.HHBosses.ignidrax.DataContext = CurrentBosses.FirstOrDefault(x => x.Name == "Ignidrax");
                });
            }
            if (templateId == 1101)
            {
                WindowManager.BossGauge.Dispatcher.Invoke(() =>
                {
                    WindowManager.BossGauge.HHBosses.terradrax.Reset();
                    WindowManager.BossGauge.HHBosses.terradrax.DataContext = CurrentBosses.FirstOrDefault(x => x.Name == "Terradrax");
                });
            }
            if (templateId == 1102)
            {
                WindowManager.BossGauge.Dispatcher.Invoke(() =>
                {
                    WindowManager.BossGauge.HHBosses.umbradrax.Reset();
                    WindowManager.BossGauge.HHBosses.umbradrax.DataContext = CurrentBosses.FirstOrDefault(x => x.Name == "Umbradrax");
                });
            }
            if (templateId == 1103)
            {
                WindowManager.BossGauge.Dispatcher.Invoke(() =>
                {
                    WindowManager.BossGauge.HHBosses.aquadrax.Reset();
                    WindowManager.BossGauge.HHBosses.aquadrax.DataContext = CurrentBosses.FirstOrDefault(x => x.Name == "Aquadrax");
                });
            }

        }

        public static void SpawnUser(ulong entityId, string name)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                CurrentUsers.Add(new Player(entityId, name));
            });
        }
        public static void DespawnUser(ulong entityId)
        {
            if(TryGetUserById(entityId, out Player p))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    CurrentUsers.Remove(p);
                });
            }
        }

        static void UnsetDragonsContexts(ulong target)
        {
            WindowManager.BossGauge.Dispatcher.Invoke(() =>
            {
                if (WindowManager.BossGauge.HHBosses.ignidrax.EntityId == target)
                {
                    WindowManager.BossGauge.HHBosses.ignidrax.DataContext = null;
                    WindowManager.BossGauge.HHBosses.ignidrax.ForceEnrageOff();
                    WindowManager.BossGauge.HHBosses.abnormalities.DataContext = null;
                    WindowManager.BossGauge.HHBosses.abnormalities.ItemsSource = null;
                }
                if (WindowManager.BossGauge.HHBosses.aquadrax.EntityId == target)
                {
                    WindowManager.BossGauge.HHBosses.aquadrax.DataContext = null;
                    WindowManager.BossGauge.HHBosses.aquadrax.ForceEnrageOff();
                    WindowManager.BossGauge.HHBosses.abnormalities.DataContext = null;
                    WindowManager.BossGauge.HHBosses.abnormalities.ItemsSource = null;

                }
                if (WindowManager.BossGauge.HHBosses.terradrax.EntityId == target)
                {
                    WindowManager.BossGauge.HHBosses.terradrax.DataContext = null;
                    WindowManager.BossGauge.HHBosses.terradrax.ForceEnrageOff();
                    WindowManager.BossGauge.HHBosses.abnormalities.DataContext = null;
                    WindowManager.BossGauge.HHBosses.abnormalities.ItemsSource = null;

                }
                if (WindowManager.BossGauge.HHBosses.umbradrax.EntityId == target)
                {
                    WindowManager.BossGauge.HHBosses.umbradrax.DataContext = null;
                    WindowManager.BossGauge.HHBosses.umbradrax.ForceEnrageOff();
                    WindowManager.BossGauge.HHBosses.abnormalities.DataContext = null;
                    WindowManager.BossGauge.HHBosses.abnormalities.ItemsSource = null;


                }
            });
        }

        public static bool TryGetUserById(ulong id, out Player p)
        {
            p = CurrentUsers.FirstOrDefault(x => x.EntityId == id);
            if (p == null)
            {
                p = new Player();
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
