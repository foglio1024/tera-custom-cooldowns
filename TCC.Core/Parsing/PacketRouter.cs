using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
using TCC.ViewModels;
using Tera.Game;

namespace TCC.Parsing
{


    public static class PacketProcessor
    {
        public static uint Version;
        public static string Region;
        public static uint ServerId;
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        static ConcurrentQueue<Tera.Message> Packets = new ConcurrentQueue<Tera.Message>();

        public static void Init()
        {
            TeraSniffer.Instance.MessageReceived += MessageReceived;
            var analysisThread = new Thread(PacketAnalysisLoop);
            analysisThread.Start();
        }

        private static void InitDB(uint serverId)
        {
            var server = BasicTeraData.Instance.Servers.GetServer(serverId);
            Region = server.Region;
            var td = new TeraData(Region);
            var lang = td.GetLanguage(Region);

            EntitiesManager.CurrentDatabase = new MonsterDatabase(lang);
            SocialDatabase.Load();
            SystemMessages.Load();
            ItemsDatabase.Load();
            GuildQuestDatabase.Load();
            AccountBenefitDatabase.Load();
            AchievementDatabase.Load();
            AchievementGradeDatabase.Load();
            DungeonDatabase.Load();
            MapDatabase.Load();
            QuestDatabase.Load();

        }

        public static void MessageReceived(global::Tera.Message obj)
        {
            if (obj.Direction == Tera.MessageDirection.ClientToServer && obj.OpCode == 19900)
            {
                var msg = new C_CHECK_VERSION_CUSTOM(new CustomReader(obj));
                Version = msg.Versions[0];
                OpCodeNamer = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/{Version}.txt"));
                SystemMessageNamer = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/smt_{Version}.txt"));
                MessageFactory.Init();
                TeraSniffer.Instance.Connected = true;
            }
            Packets.Enqueue(obj);
        }
        private static void PacketAnalysisLoop()
        {
            while (true)
            {
                Tera.Message msg;
                var successDequeue = Packets.TryDequeue(out msg);
                if (!successDequeue)
                {
                    Thread.Sleep(1);
                    continue;
                }
                var message = MessageFactory.Create(msg);
                //PacketInspector.InspectPacket(msg);
                if (message.GetType() == typeof(Tera.Game.Messages.UnknownMessage))
                {

                    continue;
                }

                if (!MessageFactory.Process(message))
                {
                }
            }
        }

        public static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
        {
            WindowManager.SkillsEnded = false;
            SkillManager.AddSkill(p.SkillId, p.Cooldown);
        }
        public static void HandleNewItemCooldown(S_START_COOLTIME_ITEM p)
        {
            WindowManager.SkillsEnded = false;
            SkillManager.AddBrooch(p.ItemId, p.Cooldown);
        }
        public static void HandleDecreaseSkillCooldown(S_DECREASE_COOLTIME_SKILL p)
        {
            SkillManager.ChangeSkillCooldown(p.SkillId, p.Cooldown);
        }

        public static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
            SessionManager.CurrentPlayer.ItemLevel = p.Ilvl;

            CharacterWindowViewModel.Instance.Player.ItemLevel = p.Ilvl;

            SessionManager.SetPlayerMaxHP(SessionManager.CurrentPlayer.EntityId, p.MaxHP);
            SessionManager.SetPlayerMaxMP(SessionManager.CurrentPlayer.EntityId, p.MaxMP);
            SessionManager.SetPlayerMaxST(SessionManager.CurrentPlayer.EntityId, p.MaxST + p.BonusST);
            
            SessionManager.SetPlayerHP(SessionManager.CurrentPlayer.EntityId, p.CurrentHP);
            SessionManager.SetPlayerMP(SessionManager.CurrentPlayer.EntityId, p.CurrentMP);
            SessionManager.SetPlayerST(SessionManager.CurrentPlayer.EntityId, p.CurrentST);


            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    ((WarriorBarManager)ClassManager.CurrentClassManager).EdgeCounter.Val = p.Edge;
                    break;
                default:
                    break;
            }

        }
        public static void HandleCreatureChangeHP(S_CREATURE_CHANGE_HP p)
        {
            SessionManager.SetPlayerMaxHP(p.Target, p.MaxHP);
            if (p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                SessionManager.SetPlayerHP(p.Target, p.CurrentHP);
            }
            else
            {
                EntitiesManager.UpdateNPCbyCreatureChangeHP(p.Target, p.CurrentHP, p.MaxHP);
            }
        }
        public static void HandlePlayerChangeMP(S_PLAYER_CHANGE_MP p)
        {
            SessionManager.SetPlayerMaxMP(p.Target, p.MaxMP);
            SessionManager.SetPlayerMP(p.Target, p.CurrentMP);
        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            SessionManager.SetPlayerST(SessionManager.CurrentPlayer.EntityId, p.CurrentST);
        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            SessionManager.SetPlayerFE(p.Energy);
        }
        public static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            SessionManager.SetCombatStatus(p.EntityId, p.IsInCombat);
        }

        public static void HandleGageReceived(S_BOSS_GAGE_INFO p)
        {
            EntitiesManager.UpdateNPCbyGauge(p.EntityId, p.CurrentHP, p.MaxHP, (ushort)p.HuntingZoneId, (uint)p.TemplateId);
        }
        public static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            EntitiesManager.SetNPCStatus(p.EntityId, p.IsEnraged);
            if (p.Target == 0)
            {
                BossGageWindowViewModel.Instance.UnsetBossTarget(p.EntityId);
            }
        }
        public static void HandleUserEffect(S_USER_EFFECT p)
        {
            BossGageWindowViewModel.Instance.SetBossAggro(p.Source, p.Circle, p.User);
        }

        public static void HandleCharList(S_GET_USER_LIST p)
        {
            SessionManager.CurrentAccountCharacters = p.CharacterList;
        }
        public static void HandleLogin(S_LOGIN p)
        {
            ProxyInterop.ConnectToProxy();

            CooldownWindowViewModel.Instance.ClearSkills();
            CooldownWindowViewModel.Instance.LoadSkills(Utils.ClassEnumToString(p.CharacterClass).ToLower() + "-skills.xml", p.CharacterClass);
            WindowManager.ClassWindow.Context.CurrentClass = p.CharacterClass;

            ServerId = p.ServerId;
            InitDB(p.ServerId);

            EntitiesManager.ClearNPC();
            GroupWindowViewModel.Instance.ClearAll();

            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            SkillManager.Clear();

            SessionManager.LoadingScreen = true;
            SessionManager.Logged = true;
            SessionManager.Encounter = false;
            SessionManager.CurrentPlayer.Class = p.CharacterClass;
            SessionManager.CurrentPlayer.EntityId = p.entityId;
            SessionManager.CurrentPlayer.PlayerId = p.PlayerId;
            SessionManager.CurrentPlayer.ServerId = p.ServerId;
            SessionManager.CurrentPlayer.Name = p.Name;
            SessionManager.CurrentPlayer.Level = p.Level;
            SessionManager.SetPlayerLaurel(SessionManager.CurrentPlayer);

            CharacterWindowViewModel.Instance.Player.Class = p.CharacterClass;
            CharacterWindowViewModel.Instance.Player.Name = p.Name;
            CharacterWindowViewModel.Instance.Player.Level = p.Level;
            SessionManager.SetPlayerLaurel(CharacterWindowViewModel.Instance.Player);
        }
        public static void SendTestMessage()
        {
            //var str = "@3947questNameDefeat HumedraszoneName@zoneName:181npcName@creature:181#2050";
            //var str = "@3789cityname@cityWar:20guildFated";
            var str = "@1773ItemName@item:152141ItemName1@item:447ItemCount5";
            byte[] toBytes = Encoding.Unicode.GetBytes(str);
            byte[] arr = new byte[toBytes.Length + 2 + 4];
            for (int i = 0; i < toBytes.Length - 1; i++)
            {
                arr[i+4] = toBytes[i];
            }

            var seg = new ArraySegment<byte>(arr);
            
            var sysMsg = new S_SYSTEM_MESSAGE(new TeraMessageReader(new Tera.Message(DateTime.Now, Tera.MessageDirection.ServerToClient, seg), OpCodeNamer, Version, SystemMessageNamer));
            HandleSystemMessage(sysMsg);

        }
        public static void HandleReturnToLobby(S_RETURN_TO_LOBBY p)
        {
            SessionManager.Logged = false;
            SessionManager.CurrentPlayer.ClearAbnormalities();
            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            SkillManager.Clear();
            EntitiesManager.ClearNPC();
            GroupWindowViewModel.Instance.ClearAll();
        }

        public static void HandleLoadTopo(S_LOAD_TOPO x)
        {
            SessionManager.LoadingScreen = true;
            SessionManager.Encounter = false;
            GroupWindowViewModel.Instance.ClearAllAbnormalities();
            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
        }
        public static void HandleLoadTopoFin(C_LOAD_TOPO_FIN x)
        {
            //SessionManager.LoadingScreen = false;
        }

        public static void HandleStartRoll(S_ASK_BIDDING_RARE_ITEM x)
        {
            GroupWindowViewModel.Instance.StartRoll();
        }
        public static void HandleRollResult(S_RESULT_BIDDING_DICE_THROW x)
        {
            GroupWindowViewModel.Instance.SetRoll(x.EntityId, x.RollResult);
        }
        public static void HandleEndRoll(S_RESULT_ITEM_BIDDING x)
        {
            GroupWindowViewModel.Instance.EndRoll();
        }

        public static void HandleSpawnMe(S_SPAWN_ME p)
        {
            //WindowManager.ShowWindow(WindowManager.CharacterWindow);
            EntitiesManager.ClearNPC();
            EntitiesManager.ClearUsers();
            System.Timers.Timer t = new System.Timers.Timer(2000);
            t.Elapsed += (s, ev) =>
            {
                SessionManager.LoadingScreen = false;
                t.Stop();
            };
            t.Enabled = true;
        }
        public static void HandleSpawnNpc(S_SPAWN_NPC p)
        {
            if(p.HuntingZoneId == 950)
            {
                if (p.TemplateId == 1007) Debug.WriteLine("Begin phase");
                if (p.TemplateId == 1005) Debug.WriteLine("Begin phase 0");
                if (p.TemplateId == 1006) Debug.WriteLine("Begin phase 1");
                if (p.TemplateId == 1008) Debug.WriteLine("Begin phase 3"); 
                if (p.TemplateId == 1003) Debug.WriteLine("Begin phase 4"); 
            }
            EntitiesManager.CheckHarrowholdMode(p.HuntingZoneId, p.TemplateId);
            EntitiesManager.SpawnNPC(p.HuntingZoneId, p.TemplateId, p.EntityId, System.Windows.Visibility.Collapsed, false);
        }
        public static void HandleSpawnUser(S_SPAWN_USER p)
        {
            EntitiesManager.SpawnUser(p.EntityId, p.Name);
        }

        public static void HandlePartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE x)
        {
            foreach (var buff in x.Abnormals)
            {
                AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(x.PlayerId, x.ServerId, buff.Id, buff.Duration, buff.Stacks);
            }
        }
        public static void HandlePartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD x)
        {
            AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }
        public static void HandlePartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL x)
        {
            AbnormalityManager.EndPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id);
        }

        public static void HandleRunemark(S_WEAK_POINT x)
        {
            if (SessionManager.CurrentPlayer.Class != Class.Glaiver) return;
            if (ClassManager.CurrentClassManager.GetType() != typeof(ValkyrieBarManager)) return;
            ((ValkyrieBarManager)ClassManager.CurrentClassManager).RunemarksCounter.Val = (int)x.TotalRunemarks;
        }

        public static void HandleSkillResult(S_EACH_SKILL_RESULT x)
        {
            //bool sourceInParty = GroupWindowViewModel.Instance.UserExists(x.Source);
            //bool targetInParty = GroupWindowViewModel.Instance.UserExists(x.Target);
            //if (x.Target == x.Source) return;
            //if (sourceInParty && targetInParty) return;
            //if (sourceInParty || targetInParty) WindowManager.SkillsEnded = false;
            //if (x.Source == SessionManager.CurrentPlayer.EntityId) WindowManager.SkillsEnded = false;
        }

        public static void HandleChangeLeader(S_CHANGE_PARTY_MANAGER x)
        {
            GroupWindowViewModel.Instance.SetNewLeader(x.EntityId, x.Name);
        }

        public static void HandlePartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR x)
        {
            GroupWindowViewModel.Instance.ClearUserAbnormality(x.PlayerId, x.ServerId);
        }
        public static void HandlePartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH x)
        {
            AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }

        public static void HandleChat(S_CHAT x)
        {
            ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(x.Channel, x.AuthorName, x.Message));
        }

        public static void HandlePrivateChat(S_PRIVATE_CHAT x)
        {
            var i = ChatWindowViewModel.Instance.PrivateChannels.FirstOrDefault(y => y.Id == x.Channel).Index;
            var ch = (ChatChannel)(ChatWindowViewModel.Instance.PrivateChannels[i].Index + 11);

            ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ch, x.AuthorName, x.Message));
        }
        public static void HandleJoinPrivateChat(S_JOIN_PRIVATE_CHANNEL x)
        {
            ChatWindowViewModel.Instance.PrivateChannels[x.Index] = new PrivateChatChannel(x.Id, x.Name, x.Index);
        }
        public static void HandleLeavePrivateChat(S_LEAVE_PRIVATE_CHANNEL x)
        {
            var i = ChatWindowViewModel.Instance.PrivateChannels.FirstOrDefault(c => c.Id == x.Id).Index;
            ChatWindowViewModel.Instance.PrivateChannels[i].Joined = false;
        }
        public static void HandleWhisper(S_WHISPER x)
        {
            if (x.Author == SessionManager.CurrentPlayer.Name)
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.SentWhisper, x.Recipient, x.Message));
            }
            else
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.ReceivedWhisper, x.Author, x.Message));
            }
        }

        internal static void HandleBrokerOffer(S_TRADE_BROKER_DEAL_SUGGESTED x)
        {
            ChatWindowViewModel.Instance.AddChatMessage(new BrokerChatMessage(x));
        }

        internal static void HandleUserApplyToParty(S_OTHER_USER_APPLY_PARTY x)
        {
            ChatWindowViewModel.Instance.AddChatMessage(new ApplyMessage(x));
        }

        internal static void HandleFriendStatus(S_UPDATE_FRIEND_INFO x)
        {
            var opcodeName = "SMT_FRIEND_IS_CONNECTED";
            if (!x.Online) return;
            if (SystemMessages.Messages.TryGetValue(opcodeName, out SystemMessage m))
            {
                SystemMessagesProcessor.AnalyzeMessage(x.Name, m, opcodeName);
            }
        }

        internal static void HandleLfgSpam(S_PARTY_MATCH_LINK x)
        {
            if (x.Message.IndexOf("WTB", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            if (x.Message.IndexOf("WTS", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            if (x.Message.IndexOf("WTT", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            ChatWindowViewModel.Instance.AddOrRefreshLfg(x);
        }

        public static void HandleSystemMessage(S_SYSTEM_MESSAGE x)
        {
            try
            {
                var msg = x.Message.Split('\v');
                var opcode = UInt16.Parse(msg[0].Substring(1));
                var opcodeName = SystemMessageNamer.GetName(opcode);

                if (SystemMessages.Messages.TryGetValue(opcodeName, out SystemMessage m))
                {
                    SystemMessagesProcessor.AnalyzeMessage(x.Message, m, opcodeName);
                }

            }
            catch (Exception)
            {
                File.AppendAllText("chat-errors.log", x.Message + "\n");
            }
        }

        internal static void HandleAccomplishAchievement(S_ACCOMPLISH_ACHIEVEMENT x)
        {
            if(AchievementDatabase.Achievements.TryGetValue(x.AchievementId, out string name))
            {
                if (SystemMessages.Messages.TryGetValue("SMT_ACHIEVEMENT_GRADE0_CLEAR_MESSAGE", out SystemMessage m))
                {
                    var sysMsg = new ChatMessage("@0\vAchievementName\v@achievement:"+x.AchievementId, m);
                    ChatWindowViewModel.Instance.AddChatMessage(sysMsg);
                }
            }
        }

        public static void HandleBlockList(S_USER_BLOCK_LIST x)
        {
            ChatWindowViewModel.Instance.BlockedUsers = x.BlockedUsers;
        }

        internal static void HandleFriendList(S_FRIEND_LIST x)
        {
            ChatWindowViewModel.Instance.Friends = x.Friends;
        }

        internal static void HandleAnswerInteractive(S_ANSWER_INTERACTIVE x)
        {
            EntitiesManager.CurrentDatabase.TryGetMonster(x.Model, 0, out Monster m);
            ChatWindowViewModel.Instance.TooltipInfo.Name = x.Name;
            ChatWindowViewModel.Instance.TooltipInfo.Info = m.Name;
            ChatWindowViewModel.Instance.TooltipInfo.Level = (int)x.Level;
            ChatWindowViewModel.Instance.TooltipInfo.SetInfo(x.Model);
            if (x.Name == SessionManager.CurrentPlayer.Name)
            {
                ChatWindowViewModel.Instance.TooltipInfo.ShowGuildInvite = false;
                ChatWindowViewModel.Instance.TooltipInfo.ShowPartyInvite = false;
            }
            else
            {
                ChatWindowViewModel.Instance.TooltipInfo.ShowGuildInvite = !x.HasGuild;
                ChatWindowViewModel.Instance.TooltipInfo.ShowPartyInvite = !x.HasParty;
            }
            if (!ProxyInterop.IsConnected)  return; 
            WindowManager.ChatWindow.OpenTooltip();
        }

        internal static void HandleCrestMessage(S_CREST_MESSAGE x)
        {

        }

        internal static void HandleSystemMessageLoot(S_SYSTEM_MESSAGE_LOOT_ITEM x)
        {
            try
            {
                var msg = x.SysMessage.Split('\v');
                var opcode = UInt16.Parse(msg[0].Substring(1));
                var opcodeName = SystemMessageNamer.GetName(opcode);

                if (SystemMessages.Messages.TryGetValue(opcodeName, out SystemMessage m))
                {
                    var sysMsg = new ChatMessage(x.SysMessage, m);
                    ChatWindowViewModel.Instance.AddChatMessage(sysMsg);
                }

            }
            catch (Exception)
            {

                File.AppendAllText("chat-errors.log", x.SysMessage + "\n");
            }
        }

        public static void HandleShowItemTooltipEx(C_SHOW_ITEM_TOOLTIP_EX x)
        {
        }
        public static void HandleRequestNondbItemInfo(C_REQUEST_NONDB_ITEM_INFO x)
        {
        }


        public static void HandleDespawnNpc(S_DESPAWN_NPC p)
        {
            EntitiesManager.DespawnNPC(p.Target);
        }


        public static void HandleDespawnUser(S_DESPAWN_USER p)
        {
            EntitiesManager.DespawnUser(p.EntityId);
        }

        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            AbnormalityManager.BeginAbnormality(p.Id, p.TargetId, p.Duration, p.Stacks);

            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Elementalist:
                    Mystic.CheckHurricane(p);
                    Mystic.CheckAura(p);
                    break;
                case Class.Warrior:
                    Warrior.CheckBuff(p);
                    break;
                case Class.Glaiver:
                    Valkyrie.CheckRagnarok(p);
                    break;
                case Class.Archer:
                    Archer.CheckFocus(p);
                    Archer.CheckFocusX(p);
                    Archer.CheckSniperEye(p);
                    break;
                case Class.Lancer:
                    Lancer.CheckArush(p);
                    Lancer.CheckGshout(p);
                    Lancer.CheckLineHeld(p);
                    break;
                case Class.Priest:
                    Priest.CheckBuff(p);
                    break;
            }
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.Duration, p.Stacks);
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    Warrior.CheckBuff(p);
                    break;
                case Class.Archer:
                    Archer.CheckFocus(p);
                    Archer.CheckSniperEye(p);
                    break;
                case Class.Lancer:
                    Lancer.CheckLineHeld(p);
                    break;
                case Class.Priest:
                    Priest.CheckBuff(p);
                    break;
                case Class.Elementalist:
                    Mystic.CheckAura(p);
                    break;
            }
        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            AbnormalityManager.EndAbnormality(p.Target, p.Id);
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Archer:
                    Archer.CheckFocusEnd(p);
                    Archer.CheckSniperEyeEnd(p);
                    break;
                case Class.Warrior:
                    Warrior.CheckBuffEnd(p);
                    break;
                case Class.Lancer:
                    Lancer.CheckLineHeldEnd(p);
                    Lancer.CheckArushEnd(p);
                    Lancer.CheckGshoutEnd(p);
                    break;
                case Class.Elementalist:
                    Mystic.CheckAuraEnd(p);
                    break;
            }
        }

        public static void HandlePlayerLocation(C_PLAYER_LOCATION p)
        {
            if (SessionManager.HarrowholdMode)
            {
                EntitiesManager.CheckCurrentDragon(new System.Windows.Point(p.X, p.Y));
            }
        }

        public static void HandlePartyMemberList(S_PARTY_MEMBER_LIST p)
        {
            GroupWindowViewModel.Instance.SetRaid(p.Raid);
            foreach (var user in p.Members)
            {
                Task.Delay(200).ContinueWith(t => {
                    GroupWindowViewModel.Instance.AddOrUpdateMember(user);
                });
            }
        }
        public static void HandlePartyMemberLeave(S_LEAVE_PARTY_MEMBER p)
        {
            GroupWindowViewModel.Instance.RemoveMember(p.PlayerId, p.ServerId);
        }
        public static void HandlePartyMemberLogout(S_LOGOUT_PARTY_MEMBER p)
        {
            GroupWindowViewModel.Instance.LogoutMember(p.PlayerId, p.ServerId);
            GroupWindowViewModel.Instance.ClearUserAbnormality(p.PlayerId, p.ServerId);

        }
        public static void HandlePartyMemberKick(S_BAN_PARTY_MEMBER p)
        {
            GroupWindowViewModel.Instance.RemoveMember(p.PlayerId, p.ServerId, true);
        }
        public static void HandlePartyMemberHP(S_PARTY_MEMBER_CHANGE_HP p)
        {
            GroupWindowViewModel.Instance.UpdateMemberHP(p.PlayerId, p.ServerId, p.CurrentHP, p.MaxHP);
        }
        public static void HandlePartyMemberMP(S_PARTY_MEMBER_CHANGE_MP p)
        {
            GroupWindowViewModel.Instance.UpdateMemberMP(p.PlayerId, p.ServerId, p.CurrentMP, p.MaxMP);
        }
        public static void HandlePartyMemberStats(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            GroupWindowViewModel.Instance.UpdateMember(p);
        }
        public static void HandleLeaveParty(S_LEAVE_PARTY x)
        {
            GroupWindowViewModel.Instance.ClearAll();

        }

        public static void HandleReadyCheck(S_CHECK_TO_READY_PARTY p)
        {
            foreach (var item in p.Party)
            {
                GroupWindowViewModel.Instance.SetReadyStatus(item);
            }
        }
        public static void HandleReadyCheckFin(S_CHECK_TO_READY_PARTY_FIN x)
        {
            GroupWindowViewModel.Instance.EndReadyCheck();
        }

        //for lfg, not used
        public static void HandlePartyMemberInfo(S_PARTY_MEMBER_INFO p)
        {
            ChatWindowViewModel.Instance.UpdateLfgMembers(p);
        }

        //public static void Debug(bool x)
        //{
        //    SessionManager.TryGetBossById(10, out Boss b);
        //    if (x)
        //    {
        //        b.CurrentHP = b.MaxHP/2;
        //    }
        //    else
        //    {
        //        b.CurrentHP = b.MaxHP;
        //    }

        //}
        //public static void DebugEnrage(bool e)
        //{
        //    SessionManager.TryGetBossById(10, out Boss b);

        //    b.Enraged = e;
        //    //EnragedChanged?.Invoke(10, e);

        //}

    }

}
