using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Nostrum;
using Newtonsoft.Json.Linq;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Parsing;
using Nostrum.Extensions;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;
using TCC.Windows;
using TeraDataLite;
using TeraPacketParser;
using TeraPacketParser.Data;
using TeraPacketParser.Messages;

namespace TCC.Test
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public static class Tester
    {
        public static bool Enabled = false;

        public static void Deadlock()
        {
            WindowManager.CharacterWindow.Dispatcher?.Invoke(() =>
            {
                App.BaseDispatcher.Invoke(() =>
                {
                    WindowManager.CharacterWindow.Dispatcher.Invoke(() => { Console.WriteLine("Deadlock"); });
                });
            });
        }
        public static void ParsePacket()
        {
            var packet = "3b6112000000000022000000d207000040004700750069006c006400510075006500730074003a0036003000300032003000300031000000";
            ParsePacketFromHexString<S_NOTIFY_GUILD_QUEST_URGENT>(packet, 356368);
        }
        public static void ShowDebugWindow()
        {
            new DebugWindow().Show();
        }
        public static void SendFakeUsageStat(string region = "EU", int server = 27, string account = "foglio", string version = "TCC v1.3.19")
        {
            try
            {
                using (var c = MiscUtils.GetDefaultWebClient())
                {
                    c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var data = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(new JObject
                    {
                        { "region", region },
                        { "server", server},
                        { "account", account },
                        { "tcc_version", version}
                    }.ToString()));

                    c.Encoding = Encoding.UTF8;
                    c.UploadStringAsync(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/usage_stat"), data);
                }
            }
            catch { }
        }
        public static void AddTccTestMessages(int count)
        {
            var i = 0;
            while (i < count)
            {
                ChatManager.Instance.AddTccMessage($"Test {i++}");
            }
        }
        public static void ParsePacketFromHexString<PacketType>(string hex, uint version)
        {

            var msg = new Message(DateTime.Now, MessageDirection.ServerToClient, new ArraySegment<byte>(hex.ToByteArrayHex()));
            var opcNamer = new OpCodeNamer(Path.Combine(App.DataPath, "opcodes", $"protocol.{version}.map"));
            var fac = new MessageFactory(version, opcNamer) {ReleaseVersion = 9901};
            var del = MessageFactory.Contructor<Func<TeraMessageReader, PacketType>>();
            var reader = new TeraMessageReader(msg, opcNamer, fac, null);
            del.DynamicInvoke(reader);
        }
        public static void Login(Class c)
        {
            Game.Logged = true;
            Game.LoadingScreen = false;
            Game.Me.Class = c;
            WindowManager.ViewModels.ClassVM.CurrentClass = Game.Me.Class;
            WindowManager.ViewModels.CooldownsVM.LoadConfig(c);
        }
        public static void ForceEncounter(bool val = true)
        {
            Game.Combat = val;
            Game.Encounter = val;
        }
        public static void StartDeadlyGambleCooldown(uint cd)
        {
            TccUtils.CurrentClassVM<WarriorLayoutVM>()?.DeadlyGamble.Cooldown.Start(cd);
        }
        public static void AddFakeGroupMember(int id, Class c, Laurel l, bool leader = false)
        {
            WindowManager.ViewModels.GroupVM.AddOrUpdateMember(new User(WindowManager.ViewModels.GroupVM.GetDispatcher())
            {
                Alive = true,
                Awakened = true,
                CurrentHp = 150000,
                MaxHp = 150000,
                EntityId = Convert.ToUInt64(id),
                ServerId = Convert.ToUInt32(id),
                PlayerId = Convert.ToUInt32(id),
                UserClass = c,
                Online = true,
                Laurel = l,
                InRange = App.Random.Next(0, 10) >= 5,
                IsLeader = leader
            });

        }
        public static void SpawnNpcAndUpdateHP(ushort zoneId, uint templateId, ulong eid)
        {
            SpawnNPC(zoneId, templateId, eid, true, false, 36);
            var t = new System.Timers.Timer { Interval = 1000 };
            var hp = 1000;
            t.Elapsed += (_, __) =>
            {
                hp -= 10;
                if (hp <= 900) hp = 1000;
                UpdateNPC(eid, hp, 1000, 0);
            };
            //t.Start();

        }
        public static void UpdateNPC(ulong target, long currentHP, long maxHP, ulong source)
        {
            WindowManager.ViewModels.NpcVM.AddOrUpdateNpc(target, maxHP, currentHP, false, Game.IsMe(source) ? HpChangeSource.Me : HpChangeSource.CreatureChangeHp);
            Game.SetEncounter(currentHP, maxHP);
        }
        public static void AddFakeCuGuilds()
        {
            var r = new Random();
            for (var i = 0; i < 30; i++)
            {
                WindowManager.ViewModels.CivilUnrestVM.AddGuild(new CityWarGuildData(1, (uint)i, 0, 0, (float)r.Next(0, 100) / 100));
                WindowManager.ViewModels.CivilUnrestVM.SetGuildName((uint)i, "Guild " + i);
                WindowManager.ViewModels.CivilUnrestVM.AddDestroyedGuildTower((uint)r.Next(0, 29));
            }
        }

        internal static void AddFakeSystemMessage(string opcodeNname, params string[] p)
        {
            var srvMsg = $"@0";
            p.ToList().ForEach(par => srvMsg += $"\v{par}");
            SystemMessagesProcessor.AnalyzeMessage(srvMsg, opcodeNname);
            //ChatWindowManager.Instance.AddSystemMessage(srvMsg, sysmsg);
        }

        public static void AddFakeLfgAndShowWindow()
        {
            Game.Me.PlayerId = 10;
            WindowManager.LfgListWindow.ShowWindow();
            var party = new Listing
            {
                LeaderId = 10,
                Message = "SJG exp only",
                LeaderName = "Foglio",
                IsExpanded = true
            };
            var idx = 20U;
            foreach (var cl in EnumUtils.ListFromEnum<Class>().Where(x => x != Class.None && x != Class.Common))
            {
                party.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = idx, IsLeader = true, Online = true, Name = $"Member{cl}", UserClass = cl, Location = "Sirjuka Gallery" });
                party.Applicants.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = idx + 100, Name = $"Applicant{cl}", Online = true, UserClass = cl, Location = "Sirjuka Gallery" });
                idx++;
            }

            var raid = new Listing
            {
                LeaderId = 11,
                Message = "WHHM 166+",
                LeaderName = "Foglio",
                IsExpanded = true,
                IsRaid = true
            };
            raid.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 11, IsLeader = true, Online = true, Name = "Foglio" });
            raid.Applicants.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 2, Name = "Applicant", Online = true, UserClass = Class.Priest });

            var trade = new Listing
            {
                LeaderId = 12,
                Message = "WTS stuff",
                LeaderName = "Foglio",
                IsExpanded = true
            };
            trade.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 12, IsLeader = true, Online = true, Name = "Foglio" });

            var trade2 = new Listing
            {
                LeaderId = 13,
                Message = "WTS more stuff",
                LeaderName = "Foglio",
                IsExpanded = true
            };
            trade2.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 13, IsLeader = true, Online = true, Name = "Foglio" });
            var twitch = new Listing
            {
                LeaderId = 14,
                Message = "twitch.tv/FoglioTera",
                LeaderName = "Foglio",
                IsExpanded = true
            };
            twitch.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 14, IsLeader = true, Online = true, Name = "Foglio" });


            WindowManager.ViewModels.LfgVM.AddOrRefreshListing(party);
            WindowManager.ViewModels.LfgVM.AddOrRefreshListing(raid);
            WindowManager.ViewModels.LfgVM.AddOrRefreshListing(trade);
            WindowManager.ViewModels.LfgVM.AddOrRefreshListing(trade2);
            WindowManager.ViewModels.LfgVM.AddOrRefreshListing(twitch);
        }
        public static void AddFakeGroupMembers(int count)
        {
            var r = App.Random;
            for (var i = 0; i <= count; i++)
            {
                AddFakeGroupMember(i, (Class)r.Next(0, 12), (Laurel)r.Next(0, 6), i == 0);
            }
        }
        public static void UpdateFakeMember(ulong eid)
        {
            WindowManager.ViewModels.GroupVM.TryGetUser(eid, out User l);
            var ut = new User(WindowManager.ViewModels.GroupVM.GetDispatcher())
            {
                Name = l.Name,
                PlayerId = l.PlayerId,
                ServerId = l.ServerId,
                EntityId = l.EntityId,
                Online = true,
                Laurel = l.Laurel,
                HasAggro = l.HasAggro,
                Alive = l.Alive,
                UserClass = l.UserClass,
                Awakened = l.Awakened,
                CurrentHp = 120000,
                MaxHp = 120000
            };
            Task.Delay(2000).ContinueWith(t => WindowManager.ViewModels.GroupVM.AddOrUpdateMember(ut));
        }
        //public static void ProfileThreadsUsage()
        //{
        //    var t          = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        //    var dispatchers = new List<Dispatcher>
        //    {
        //         App.BaseDispatcher                                ,
        //         WindowManager.BossWindow.Dispatcher               ,
        //         WindowManager.BuffWindow.Dispatcher               ,
        //         WindowManager.CharacterWindow.Dispatcher          ,
        //         WindowManager.GroupWindow.Dispatcher              ,
        //         WindowManager.CooldownWindow.Dispatcher           ,
        //         WindowManager.ClassWindow.Dispatcher              ,
        //    };
        //    var threadIdToName = new ConcurrentDictionary<int, string>();
        //    foreach (var disp in dispatchers)
        //    {
        //        disp.Invoke(() =>
        //        {
        //            var myId = MiscUtils.GetCurrentThreadId();
        //            threadIdToName[myId] = disp.Thread.ManagedThreadId == 1 ? "Main" : disp.Thread.Name;
        //        });
        //    }
        //    //threadIdToName[PacketAnalyzer.AnalysisThreadId] = PacketAnalyzer.AnalysisThread.Name;

        //    var stats = new Dictionary<int, ThreadInfo>();
        //    t.Tick += (_, __) =>
        //    {
        //        var p = Process.GetCurrentProcess();
        //        foreach (ProcessThread th in p.Threads)
        //        {
        //            if (threadIdToName.ContainsKey(th.Id))
        //            {
        //                if (!stats.ContainsKey(th.Id))
        //                {
        //                    stats.Add(th.Id, new ThreadInfo
        //                    {
        //                        Name = threadIdToName[th.Id],
        //                        Id = th.Id,
        //                        TotalTime = th.TotalProcessorTime.TotalMilliseconds,
        //                        Priority = threadIdToName[th.Id] == "Anls"
        //                            ? PacketAnalyzer.AnalysisThread.Priority
        //                            : dispatchers.FirstOrDefault(d => d.Thread.Name == threadIdToName[th.Id]).Thread
        //                                .Priority
        //                    });
        //                }
        //                else stats[th.Id].TotalTime = th.TotalProcessorTime.TotalMilliseconds;
        //            }
        //        }
        //        foreach (var item in stats)
        //        {
        //            Console.WriteLine($"{threadIdToName[item.Key]} ({(int)item.Value.Priority}):\t\t{item.Value.TotalTime:0}\t\t{item.Value.DiffTime / 1000:P}\t");
        //        }
        //        Console.WriteLine("----------------------------------");
        //    };
        //    _t.Start();

        //}
        public static void TestWebhook()
        {
            var url = "";
            var msg = new JObject
            {
                {"content", $"**Markdown** `or not?`"},
                {"username", "TCC Update" },
                {"avatar_url", "http://i.imgur.com/8IltuVz.png" }
            };

            using (var client = MiscUtils.GetDefaultWebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.UploadString(url, "POST", msg.ToString());
            }
        }

        public static async void FireWebhook(string username)
        {
            var canFire = true;
            var wh = "";
            var whHash = HashUtils.GenerateHash(wh);
            var req = new JObject
            {
                { "webhook" , whHash},
                {"user", username }

            };
            using (var c = MiscUtils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {

                    var res = await c.UploadStringTaskAsync(new Uri("https://us-central1-tcc-global-events.cloudfunctions.net/fire_webhook"),
                                                Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));

                    var jRes = JObject.Parse(res);
                    canFire = jRes["canFire"].Value<bool>();
                }
                catch (WebException)
                {
                    Console.WriteLine($"{username} failed");
                }

                if (!canFire)
                {
                    Console.WriteLine($"- Webhook refused for {username}");
                    return;
                }
                Console.WriteLine($"+ Webhook fired for {username}");
                var msg = new JObject
                {
                    {"content", $"Test from {username}"},
                    {"username", "TCC" },
                    {"avatar_url", "http://i.imgur.com/8IltuVz.png" }
                };
                using (var client = MiscUtils.GetDefaultWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.UploadString(wh, "POST", msg.ToString());
                }

            }

        }

        public static async void RegisterWebhook(string username)
        {
            var wh = "";
            var whHash = HashUtils.GenerateHash(wh);
            var r = new Random(DateTime.Now.Millisecond);
            var req = new JObject
            {
                {"webhook", whHash},
                {"user", username},
                {"online", r.NextDouble() >= 0.9 }
            };
            using (var c = MiscUtils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {
                    await c.UploadStringTaskAsync(
                        new Uri("http://localhost:5001/tcc-global-events/us-central1/register_webhook"),
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));
                }
                catch
                {
                    Console.WriteLine($"Failed to register webhook for {username}");
                }
            }
        }

        public static void AddWhispers(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                ChatManager.Instance.AddChatMessage(
                    ChatManager.Instance.Factory.CreateMessage(ChatChannel.ReceivedWhisper, "Test", $"Test {i}"));
            }
        }

        private static System.Timers.Timer _t;
        public static void AddMobs()
        {
            _t = new System.Timers.Timer { Interval = 1 };
            _t.Elapsed += T_Elapsed;
            _t.Start();
        }

        public static void AddNpcAndAbnormalities()
        {
            SpawnNPC(950, 4000, 1, true, false, 36);
            var t = new System.Timers.Timer(1);
            var a = true;
            t.Elapsed += (_, __) =>
            {
                //if (true) AbnormalityUtils.BeginAbnormality(id++, 1, 0, 500000, 1);
                //else AbnormalityUtils.EndAbnormality(1, 100800);
                a = !a;
            };
            t.Start();
        }
        private static ulong _eid = 1;
        private static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            SpawnNPC(9, 700, _eid++, true, false, 0);
            if (_eid != 10000) return;
            WindowManager.ViewModels.NpcVM.Clear();
            _t.Stop();
        }


        public static void CheckDelegateSubs()
        {
            var t = new System.Timers.Timer(1000);
            t.Elapsed += (_, __) =>
            {
                SettingsWindowViewModel.PrintEventsData();
                Log.CW($"Messages: {ChatManager.Instance.ChatMessages.Count}");
                Log.CW("----------------------------------------------------------");
            };
            t.Start();
        }

        public static void SpawnNPC(ushort zoneId, uint templateId, ulong entityId, bool v, bool villager, int remainingEnrageTime)
        {
            if (Game.DB.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
            {
                if (TccUtils.IsWorldBoss(zoneId, templateId))
                {
                    if (m.IsBoss)
                    {
                        var msg = ChatManager.Instance.Factory.CreateMessage(ChatChannel.WorldBoss, "System", $"<font>{m.Name}</font><font size=\"15\" color=\"#cccccc\"> is nearby.</font>");
                        ChatManager.Instance.AddChatMessage(msg);
                    }
                }

                Game.NearbyNPC[entityId] = m.Name;
                FlyingGuardianDataProvider.InvokeProgressChanged();
                if (villager) return;
                if (m.IsBoss)
                {
                    WindowManager.ViewModels.NpcVM.AddOrUpdateNpc(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, v, remainingEnrageTime);
                }
                else
                {
                    if (App.Settings.NpcWindowSettings.HideAdds) return;
                    WindowManager.ViewModels.NpcVM.AddOrUpdateNpc(entityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, templateId, zoneId, false, remainingEnrageTime);
                }
            }
        }

        public static void AddAbnormality(uint id)
        {
            if (!AbnormalityUtils.Exists(id, out var ab) || !AbnormalityUtils.Pass(ab)) return;
            ab.Infinity = false;
            Game.Me.UpdateAbnormality(ab, int.MaxValue, 1);

        }

        public static void ReadyCheck()
        {
            var status = new[] {ReadyStatus.Ready, ReadyStatus.NotReady, ReadyStatus.Undefined};
            foreach (var m in WindowManager.ViewModels.GroupVM.Members)
            {
                WindowManager.ViewModels.GroupVM.SetReadyStatus(new ReadyPartyMember
                {
                    ServerId = m.ServerId,
                    PlayerId = m.PlayerId,
                    Status = status[App.Random.Next(0,3)]
                });

            }
        }

        public static void StartRoll()
        {
            WindowManager.ViewModels.GroupVM.StartRoll();
            WindowManager.ViewModels.GroupVM.Members[0].IsWinning = true;
        }
    }
}
