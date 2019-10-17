using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoglioUtils.Extensions;
using HtmlAgilityPack;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Utils;
using TCC.ViewModels;
using VMs = TCC.WindowManager.ViewModels;
namespace TCC.Parsing
{
    public static class SystemMessagesProcessor
    {
        public static string Build(SystemMessageData template, params string[] parameters)
        {
            var Pieces = new List<string>();
            var sb = new StringBuilder();

            var prm = ChatUtils.SplitDirectives(parameters);
            var txt = template.Template.UnescapeHtml().Replace("<BR>", "\r\n");
            var html = new HtmlDocument(); html.LoadHtml(txt);
            var htmlPieces = html.DocumentNode.ChildNodes;
            if (prm == null)
            {
                //only one parameter (opcode) so just add text

                foreach (var htmlPiece in htmlPieces)
                {
                    var content = htmlPiece.InnerText;
                    AddPiece(content);
                }
            }
            else
            {
                //more parameters
                foreach (var htmlPiece in htmlPieces)
                {
                    ParseSysHtmlPiece(htmlPiece);
                }
            }

            Pieces.ForEach(p => sb.Append(p));

            return sb.ToString();

            ///////////////////////////////////////////////////////////////

            void AddPiece(string p)
            {
                Pieces.Add(p);
            }
            void ParseSysHtmlPiece(HtmlNode piece)
            {
                if (piece.Name == "img")
                {
                }
                else
                {
                    var content = ChatUtils.ReplaceParameters(piece.InnerText, prm, true);
                    var innerPieces = content.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                    var plural = false;
                    var selectionStep = 0;

                    foreach (var inPiece in innerPieces)
                    {
                        switch (selectionStep)
                        {
                            case 1:
                                if (int.Parse(inPiece) != 1) plural = true;
                                selectionStep++;
                                continue;
                            case 2:
                                if (inPiece == "/s//s" && plural)
                                {
                                    Pieces[Pieces.Count - 1] = Pieces.Last() + "s";
                                    plural = false;
                                }
                                selectionStep = 0;
                                continue;
                        }

                        string mp;
                        if (inPiece.StartsWith("@select"))
                        {
                            selectionStep++;
                            continue;
                        }
                        if (inPiece.StartsWith("@item"))
                        {
                            mp = SystemMessageParser.ParseSysMsgItem(inPiece);
                        }
                        else if (inPiece.StartsWith("@abnormal"))
                        {
                            var abName = "Unknown";
                            if (Game.DB.AbnormalityDatabase.Abnormalities.TryGetValue(uint.Parse(inPiece.Split(':')[1]), out var ab)) abName = ab.Name;
                            mp = abName;
                        }
                        else if (inPiece.StartsWith("@achievement"))
                        {
                            mp = SystemMessageParser.ParseSysMsgAchi(inPiece);
                        }
                        else if (inPiece.StartsWith("@GuildQuest"))
                        {
                            mp = SystemMessageParser.ParseSysMsgGuildQuest(inPiece);
                        }
                        else if (inPiece.StartsWith("@dungeon"))
                        {
                            mp = SystemMessageParser.ParseSysMsgDungeon(inPiece);
                        }
                        else if (inPiece.StartsWith("@accountBenefit"))
                        {
                            mp = SystemMessageParser.ParseSysMsgAccBenefit(inPiece);
                        }
                        else if (inPiece.StartsWith("@AchievementGradeInfo"))
                        {
                            mp = SystemMessageParser.ParseSysMsgAchiGrade(inPiece);
                        }
                        else if (inPiece.StartsWith("@quest"))
                        {
                            mp = SystemMessageParser.ParseSysMsgQuest(inPiece);
                        }
                        else if (inPiece.StartsWith("@creature"))
                        {
                            mp = SystemMessageParser.ParseSysMsgCreature(inPiece);
                        }
                        else if (inPiece.StartsWith("@rgn"))
                        {
                            mp = SystemMessageParser.ParseSysMsgRegion(inPiece);
                        }
                        else if (inPiece.StartsWith("@zoneName"))
                        {
                            mp = SystemMessageParser.ParseSysMsgZone(inPiece);
                        }
                        else if (inPiece.Contains("@money"))
                        {
                            var t = inPiece.Replace("@money", "");
                            mp = new Money(t).ToString();
                        }
                        else
                        {
                            mp = inPiece.UnescapeHtml();
                        }
                        AddPiece(mp);
                    }
                }
            }
        }
        public static void AnalyzeMessage(string srvMsg, SystemMessageData sysMsg, string opcodeName)
        {
            if (!Pass(opcodeName)) return;

            if (!Process(srvMsg, sysMsg, opcodeName))
            {
                ChatWindowManager.Instance.AddSystemMessage(srvMsg, sysMsg);
            }
        }

        private static bool Pass(string opcodeName)
        {
            return !App.Settings.UserExcludedSysMsg.Contains(opcodeName);
        }

        private static void HandleMaxEnchantSucceed(string x)
        {
            var msg = ChatWindowManager.Instance.Factory.CreateEnchantSystemMessage(x);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleFriendLogin(string friendName, SystemMessageData sysMsg)
        {
            var template = "@0\vUserName\v" + friendName;
            ChatWindowManager.Instance.AddSystemMessage(template, sysMsg, ChatChannel.Friend, friendName);
        }
        private static void HandleClearedGuardianQuestsMessage(string srvMsg, SystemMessageData sysMsg)
        {
            var currChar = VMs.DashboardVM.CurrentCharacter;
            var cleared = currChar.GuardianInfo.Cleared;
            var standardCountString = $"<font color =\"#cccccc\">({cleared}/40)</font>";
            var maxedCountString = $"<font color=\"#cccccc\">(</font><font color =\"#ff0000\">{cleared}</font><font color=\"#cccccc\">/40)</font>";
            var newMsg = new SystemMessageData($"{sysMsg.Template} {(cleared == 40 ? maxedCountString : standardCountString)}", sysMsg.ChatChannel);
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, newMsg, ChatChannel.Guardian);
            if (currChar.GuardianInfo.Cleared == 40)
            {
                
                msg.ContainsPlayerName = true;
            }
            ChatWindowManager.Instance.AddChatMessage(msg);

        }
        private static void HandleNewGuildMasterMessage(string template, SystemMessageData sysMsg)
        {
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(template, sysMsg, ChatChannel.GuildNotice);
            Log.N("Guild", msg.ToString(), NotificationType.Success);
            ChatWindowManager.Instance.AddChatMessage(msg);
            msg.ContainsPlayerName = true;

        }
        private static void HandleGuilBamSpawn(string srvMsg, SystemMessageData sysMsg)
        {
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
            Log.N("Guild BAM", msg.ToString(), NotificationType.Normal);
            ChatWindowManager.Instance.AddChatMessage(msg);

            TimeManager.Instance.UploadGuildBamTimestamp();

            TimeManager.Instance.SetGuildBamTime(true);
            TimeManager.Instance.ExecuteGuildBamWebhook();
        }
        private static void HandleDungeonEngagedMessage(string srvMsg, SystemMessageData sysMsg)
        {
            const string s = "dungeon:";
            var dgId = Convert.ToUInt32(srvMsg.Substring(srvMsg.IndexOf(s, StringComparison.Ordinal) + s.Length));
            VMs.DashboardVM.CurrentCharacter.DungeonInfo.Engage(dgId);

            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleFriendInAreaMessage(string srvMsg, SystemMessageData sysMsg)
        {
            if (!App.Settings.ChatEnabled) return;
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, ChatChannel.Friend);
            var start = srvMsg.IndexOf("UserName\v", StringComparison.InvariantCultureIgnoreCase) + "UserName\v".Length;
            var end = srvMsg.IndexOf("\v", start, StringComparison.InvariantCultureIgnoreCase);
            var friendName = srvMsg.Substring(start, end - start);
            msg.Author = friendName;
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleRessMessage(string srvMsg, SystemMessageData sysMsg)
        {
            if (!App.Settings.ChatEnabled) return;

            var newSysMsg = new SystemMessageData(sysMsg.Template.Replace("{UserName}", "<font color='#cccccc'>{UserName}</font>")
                .Replace("{PartyPlayerName}", "<font color='#cccccc'>{PartyPlayerName}</font>"), (int)ChatChannel.Ress);
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, newSysMsg, ChatChannel.Ress);
            ChatWindowManager.Instance.AddChatMessage(msg);

        }
        private static void HandleDeathMessage(string srvMsg, SystemMessageData sysMsg)
        {
            if (!App.Settings.ChatEnabled) return;

            var newSysMsg = new SystemMessageData(sysMsg.Template.Replace("{UserName}", "<font color='#cccccc'>{UserName}</font>")
                .Replace("{PartyPlayerName}", "<font color='#cccccc'>{PartyPlayerName}</font>"), (int)ChatChannel.Death);
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, newSysMsg, ChatChannel.Death);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleInvalidLink(string srvMsg, SystemMessageData sysMsg)
        {
            if (App.Settings.LfgWindowSettings.Enabled) VMs.LfgVM.RemoveDeadLfg();

            if (!App.Settings.ChatEnabled) return;
            ChatWindowManager.Instance.RemoveDeadLfg();
            ChatWindowManager.Instance.AddChatMessage(ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel));
        }
        private static void HandleMerchantSpawn(string srvMsg, SystemMessageData sysMsg)
        {
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
            Log.N("Mystery Merchant", msg.ToString(), NotificationType.Normal, 10000);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleMerchantDespawn(string srvMsg, SystemMessageData sysMsg)
        {
            var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
            Log.N("Mystery Merchant", msg.ToString(), NotificationType.Normal, 10000);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleLfgNotListed(string srvMsg, SystemMessageData sysMsg)
        {
            ChatWindowManager.Instance.AddSystemMessage(srvMsg, sysMsg);
            VMs.LfgVM.ForceStopPublicize();
        }
        private static void Redirect(string srvMsg, SystemMessageData sysMsg, ChatChannel ch)
        {
            ChatWindowManager.Instance.AddSystemMessage(srvMsg, sysMsg, ch);
        }

        //by HQ 20181224
        private static void HandleFieldBossAppear(string srvMsg, SystemMessageData sysMsg)
        {
            string notificationText;
            if (App.Settings.ChatEnabled)
            {
                var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
                notificationText = msg.ToString();
                ChatWindowManager.Instance.AddChatMessage(msg);
            }
            else
            {
                notificationText = Build(sysMsg, srvMsg.Split('\v'));
            }
            Log.N("Field Boss", notificationText, NotificationType.Success, 10000);

            if (!App.Settings.WebhookEnabledFieldBoss) return;

            // @4157 \v
            // regionName \v @rgn:213 \v
            // npcName \v @creature:26#5001

            var monsterName = GetFieldBossName(srvMsg);
            var regName = srvMsg.Split('\v')[2].Replace("@rgn:", "");
            var regId = uint.Parse(regName);

            Game.DB.RegionsDatabase.Names.TryGetValue(regId, out var regionName);

            TimeManager.Instance.ExecuteFieldBossSpawnWebhook(monsterName, regionName, notificationText);

        }
        private static void HandleFieldBossDie(string srvMsg, SystemMessageData sysMsg)
        {
            string notificationText;

            if (App.Settings.ChatEnabled)
            {
                var msg = ChatWindowManager.Instance.Factory.CreateSystemMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
                notificationText = msg.ToString();
                ChatWindowManager.Instance.AddChatMessage(msg);
            }
            else
            {
                notificationText = Build(sysMsg, srvMsg.Split('\v'));
            }
            Log.N("Field Boss", notificationText, NotificationType.Error, 10000);
            if (!App.Settings.WebhookEnabledFieldBoss) return;

            //@4158
            //guildNameWish
            //userName쿤
            //npcname@creature:26#5001

            //@????
            //userName쿤
            //npcname@creature:26#5001

            var monsterName = GetFieldBossName(srvMsg);
            var userName = GetFieldBossKillerName(srvMsg);
            var guildName = GetFieldBossKillerGuild(srvMsg);
            if (string.IsNullOrEmpty(guildName)) guildName = "-no guild-";
            TimeManager.Instance.ExecuteFieldBossDieWebhook(monsterName, notificationText, userName, guildName);
        }
        private static string GetFieldBossName(string srvMsg)
        {
            // only for 'SMT_FIELDBOSS_*'
            var srvMsgSplit = srvMsg.Split('\v');
            var npcName = srvMsgSplit.Last().Replace("@creature:", "");
            var zoneId = uint.Parse(npcName.Split('#')[0]);
            var templateId = uint.Parse(npcName.Split('#')[1]);
            Game.DB.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m);
            return m.Name;
        }
        private static string GetFieldBossKillerName(string srvMsg)
        {
            // only for 'SMT_FIELDBOSS_*'
            var ret = "";
            var srvMsgSplit = srvMsg.Split('\v').ToList();
            var idx = srvMsgSplit.IndexOf("userName") + 1;
            if (idx != -1 && idx < srvMsgSplit.Count) ret = srvMsgSplit[idx];
            return ret;
        }
        private static string GetFieldBossKillerGuild(string srvMsg)
        {
            // only for 'SMT_FIELDBOSS_*'
            var ret = "";
            var srvMsgSplit = srvMsg.Split('\v').ToList();
            var idx = srvMsgSplit.IndexOf("guildName") + 1;
            if (idx != -1 && idx < srvMsgSplit.Count) ret = srvMsgSplit[idx];
            return ret;
        }

        #region Factory

        private static readonly Dictionary<string, Delegate> Processor = new Dictionary<string, Delegate>
        {
            { "SMT_MAX_ENCHANT_SUCCEED",                    new Action<string, SystemMessageData>((srvMsg, sysMsg) => HandleMaxEnchantSucceed(srvMsg)) },
            { "SMT_FRIEND_IS_CONNECTED",                    new Action<string, SystemMessageData>(HandleFriendLogin) },
            { "SMT_FRIEND_WALK_INTO_SAME_AREA",             new Action<string, SystemMessageData>(HandleFriendInAreaMessage) },
            { "SMT_CHAT_LINKTEXT_DISCONNECT",               new Action<string, SystemMessageData>(HandleInvalidLink) },

            { "SMT_BATTLE_PARTY_DIE",                       new Action<string, SystemMessageData>(HandleDeathMessage) },
            { "SMT_BATTLE_YOU_DIE",                         new Action<string, SystemMessageData>(HandleDeathMessage) },
            { "SMT_BATTLE_PARTY_RESURRECT",                 new Action<string, SystemMessageData>(HandleRessMessage) },
            { "SMT_BATTLE_RESURRECT",                       new Action<string, SystemMessageData>(HandleRessMessage) },

            { "SMT_ACCEPT_QUEST",                           new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_CANT_START_QUEST",                       new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_COMPLATE_GUILD_QUEST",                   new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_COMPLETE_MISSION",                       new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_COMPLETE_QUEST",                         new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_FAILED_QUEST_COMPENSATION",              new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_FAILED_QUEST",                           new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_FAILED_QUEST_CANCLE",                    new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_QUEST_FAILED_GET_FLAG_PARTY_LEVEL",      new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_QUEST_ITEM_DELETED",                     new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_QUEST_RESET_MESSAGE",                    new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_UPDATE_QUEST_TASK",                      new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_QUEST_SHARE_MESSAGE2",                   new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_QUEST_USE_SKILL",                        new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },
            { "SMT_QUEST_USE_ITEM",                         new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Quest)) },

            { "SMT_GRANT_DUNGEON_COOLTIME_AND_COUNT",       new Action<string, SystemMessageData>(HandleDungeonEngagedMessage) },
            { "SMT_GQUEST_URGENT_NOTIFY",                   new Action<string, SystemMessageData>(HandleGuilBamSpawn) },

            { "SMT_PARTY_LOOT_ITEM_PARTYPLAYER",            new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Loot)) },

            { "SMT_GC_SYSMSG_GUILD_CHIEF_CHANGED",          new Action<string, SystemMessageData>(HandleNewGuildMasterMessage) },

            { "SMT_ACCOMPLISH_ACHIEVEMENT_GRADE_ALL",       new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Laurel)) },

            { "SMT_DROPDMG_DAMAGE",                         new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Damage)) },

            { "SMT_FIELD_EVENT_REWARD_AVAILABLE",           new Action<string, SystemMessageData>(HandleClearedGuardianQuestsMessage) },
            { "SMT_FIELD_EVENT_ENTER",                      new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guardian)) },
            { "SMT_FIELD_EVENT_LEAVE",                      new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guardian)) },
            { "SMT_FIELD_EVENT_COMPLETE",                   new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guardian)) },
            { "SMT_FIELD_EVENT_FAIL_OVERTIME",              new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guardian)) },
            { "SMT_FIELD_EVENT_CLEAR_REWARD_SENT",          new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guardian)) },
            { "SMT_FIELD_EVENT_WORLD_ANNOUNCE",             new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guardian)) },

            { "SMT_ITEM_DECOMPOSE_COMPLETE",                new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Loot)) },
            { "SMT_WAREHOUSE_ITEM_DRAW",                    new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Loot)) },
            { "SMT_WAREHOUSE_ITEM_INSERT",                  new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Loot)) },

            { "SMT_GC_MSGBOX_APPLYLIST_1",                  new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_MSGBOX_APPLYLIST_2",                  new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_MSGBOX_APPLYRESULT_1",                new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_MSGBOX_APPLYRESULT_2",                new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_SYSMSG_ACCEPT_GUILD_APPLY",           new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_SYSMSG_GUILD_MEMBER_BANNED",          new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_SYSMSG_GUILD_MEMBER_BANNED_2",        new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_SYSMSG_LEAVE_GUILD",                  new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_SYSMSG_LEAVE_GUILD_FRIEND",           new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },
            { "SMT_GC_SYSMSG_REFUSE_GUILD_APPLY",           new Action<string, SystemMessageData>((sys, srv) => Redirect(sys, srv, ChatChannel.Guild)) },

            { "SMT_FIELDBOSS_APPEAR",                       new Action<string, SystemMessageData>(HandleFieldBossAppear) },     //by HQ 20181224
            { "SMT_FIELDBOSS_DIE_GUILD",                    new Action<string, SystemMessageData>(HandleFieldBossDie) },        //by HQ 20181224
            { "SMT_FIELDBOSS_DIE_NOGUILD",                  new Action<string, SystemMessageData>(HandleFieldBossDie) },        //by HQ 20181224

            { "SMT_PARTY_MATCHING_CANT_PR_NO_INFORMATION",  new Action<string, SystemMessageData>(HandleLfgNotListed)},

            { "SMT_WORLDSPAWN_NOTIFY_SPAWN",                new Action<string, SystemMessageData>(HandleMerchantSpawn)},
            { "SMT_WORLDSPAWN_NOTIFY_DESPAWN",              new Action<string, SystemMessageData>(HandleMerchantDespawn)},
        };


        private static bool Process(string parameters, SystemMessageData template, string opcodeName)
        {
            if (!Processor.TryGetValue(opcodeName, out var type) || type == null) return false;
            App.BaseDispatcher.InvokeAsync(() => type.DynamicInvoke(parameters, template));
            return true;
        }
        #endregion
    }
}