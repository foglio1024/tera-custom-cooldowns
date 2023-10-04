using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Nostrum.Extensions;
using TCC.Data.Pc;
using TCC.UI;
using TCC.Utils;
using TCC.ViewModels;
using TeraPacketParser.Analysis;

namespace TCC.Data.Chat;

public static class SystemMessagesProcessor
{
    public static string Build(SystemMessageData template, params string[] parameters)
    {
        var pieces = new List<string>();
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
                pieces.Add(content);
            }
        }
        else
        {
            //more parameters
            foreach (var piece in htmlPieces)
            {
                if (piece.Name == "img") continue;
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
                                pieces[^1] = pieces.Last() + "s";
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
                        if (Game.DB!.AbnormalityDatabase.Abnormalities.TryGetValue(uint.Parse(inPiece.Split(':')[1]),
                                out var ab)) abName = ab.Name;
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

                    pieces.Add(mp);
                }
            }
        }

        pieces.ForEach(p => sb.Append(p));

        return sb.ToString();
    }

    public static void AnalyzeMessage(string fullParameters)
    {
        var opcodeStr = fullParameters.Split('\v')[0]; // "@opcode \v parameters"
        var opcode = ushort.Parse(opcodeStr.Substring(1));
        var opcodeName = PacketAnalyzer.Factory!.SystemMessageNamer.GetName(opcode);
        AnalyzeMessage(fullParameters, opcodeName);
    }
    public static void AnalyzeMessage(string parameters, string opcodeName)
    {
        if (!Game.DB!.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var template)) return;
        AnalyzeMessage(parameters, template, opcodeName);
    }

    static void AnalyzeMessage(string parameters, SystemMessageData template, string opcodeName)
    {
        if (!Pass(opcodeName)) return;

        if (Process(parameters, template, opcodeName)) return;

        ChatManager.Instance.AddSystemMessage(parameters, template);
    }

    static bool Pass(string opcodeName)
    {
        return !App.Settings.UserExcludedSysMsg.Contains(opcodeName);
    }

    //private static void HandleMaxEnchantSucceed(string parameters, SystemMessageData template)
    //{
    //    //"@464\vUserName\vHeve\vItemName\v@item:89607?dbid:327641239?enchantCount:11"
    //    var templ = new SystemMessageData( ChatUtils.Font(template.Template, R.Colors.ChatSystemGenericColor.ToHex()), template.ChatChannel);
    //    ChatWindowManager.Instance.AddSystemMessage(parameters, templ, ChatChannel.Enchant);
    //}
    static void HandleFriendLogin(string parameters, SystemMessageData template)
    {
        ChatManager.Instance.AddSystemMessage(parameters, template, ChatChannel.Friend, ChatUtils.SplitDirectives(parameters)?["UserName"] ?? "");
    }

    static void HandleClearedGuardianQuestsMessage(string parameters, SystemMessageData template)
    {
        var currChar = WindowManager.ViewModels.DashboardVM.CurrentCharacter;
        if (currChar == null) return;
        var cleared = currChar.GuardianInfo.Cleared;
        var standardCountString = ChatUtils.Font($"({cleared}/{GuardianInfo.MAX_DAILIES})", "cccccc");
        var maxedCountString = ChatUtils.Font("(", "cccccc")
                               + ChatUtils.Font($"{cleared}", "ff0000")
                               + ChatUtils.Font($"/{GuardianInfo.MAX_DAILIES})", "cccccc");
        var newMsg = new SystemMessageData($"{template.Template} {(cleared == GuardianInfo.MAX_DAILIES ? maxedCountString : standardCountString)}", template.ChatChannel);
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, newMsg, ChatChannel.Guardian);
        if (currChar.GuardianInfo.Cleared == GuardianInfo.MAX_DAILIES)
        {
            msg.ContainsPlayerName = true;
        }
        ChatManager.Instance.AddChatMessage(msg);
    }

    static void HandleNewGuildMasterMessage(string parameters, SystemMessageData template)
    {
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, ChatChannel.GuildNotice);
        Log.N("Guild", msg.ToString(), NotificationType.Success);
        ChatManager.Instance.AddChatMessage(msg);
        msg.ContainsPlayerName = true;

    }

    static void HandleGuilBamSpawn(string parameters, SystemMessageData template)
    {
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel);
        Log.N("Guild BAM", msg.ToString(), NotificationType.Warning, 2 * 60 * 1000);
        ChatManager.Instance.AddChatMessage(msg);

        GameEventManager.Instance.UploadGuildBamTimestamp();

        WindowManager.ViewModels.DashboardVM.SetGuildBamTime(true);
        GameEventManager.ExecuteGuildBamWebhook();
    }

    static void HandleDungeonEngagedMessage(string parameters, SystemMessageData template)
    {
        const string s = "dungeon:";
        var dgId = Convert.ToUInt32(parameters.Substring(parameters.IndexOf(s, StringComparison.Ordinal) + s.Length));
        WindowManager.ViewModels.DashboardVM.CurrentCharacter?.DungeonInfo.Engage(dgId);

        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel);
        ChatManager.Instance.AddChatMessage(msg);
    }

    static void HandleFriendInAreaMessage(string parameters, SystemMessageData template)
    {
        if (!App.Settings.ChatEnabled) return;
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, ChatChannel.Friend);
        var start = parameters.IndexOf("UserName\v", StringComparison.InvariantCultureIgnoreCase) + "UserName\v".Length;
        var end = parameters.IndexOf("\v", start, StringComparison.InvariantCultureIgnoreCase);
        var friendName = parameters.Substring(start, end - start);
        msg.Author = friendName;
        ChatManager.Instance.AddChatMessage(msg);
    }

    static void HandleRessMessage(string parameters, SystemMessageData template)
    {
        if (!App.Settings.ChatEnabled) return;

        var newSysMsg = new SystemMessageData(template.Template.Replace("{UserName}", ChatUtils.Font("{UserName}", "cccccc"))
            .Replace("{PartyPlayerName}", ChatUtils.Font("{PartyPlayerName}", "cccccc")), (int)ChatChannel.Ress);
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, newSysMsg, ChatChannel.Ress);
        ChatManager.Instance.AddChatMessage(msg);

    }

    static void HandleDeathMessage(string parameters, SystemMessageData template)
    {
        if (!App.Settings.ChatEnabled) return;

        var newSysMsg = new SystemMessageData(template.Template.Replace("{UserName}", ChatUtils.Font("{UserName}", "cccccc"))
            .Replace("{PartyPlayerName}", ChatUtils.Font("{PartyPlayerName}", "cccccc")), (int)ChatChannel.Death);
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, newSysMsg, ChatChannel.Death);
        ChatManager.Instance.AddChatMessage(msg);
    }

    static void HandleInvalidLink(string parameters, SystemMessageData template)
    {
        if (App.Settings.LfgWindowSettings.Enabled) WindowManager.ViewModels.LfgVM.RemoveDeadLfg();

        if (!App.Settings.ChatEnabled) return;
        ChatManager.Instance.RemoveDeadLfg();
        ChatManager.Instance.AddChatMessage(ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel));
    }

    static void HandleMerchantSpawn(string parameters, SystemMessageData template)
    {
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel);
        Log.N("Mystery Merchant", msg.ToString(), NotificationType.Info, 10000);
        ChatManager.Instance.AddChatMessage(msg);
    }

    static void HandleMerchantDespawn(string parameters, SystemMessageData template)
    {
        var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel);
        Log.N("Mystery Merchant", msg.ToString(), NotificationType.Error, 10000);
        ChatManager.Instance.AddChatMessage(msg);
    }

    static void HandleLfgNotListed(string parameters, SystemMessageData template)
    {
        ChatManager.Instance.AddSystemMessage(parameters, template);
        WindowManager.ViewModels.LfgVM.ForceStopPublicize();
    }

    static void Redirect(string parameters, SystemMessageData template, ChatChannel ch)
    {
        ChatManager.Instance.AddSystemMessage(parameters, template, ch);
    }

    //by HQ 20181224
    static void HandleFieldBossAppear(string parameters, SystemMessageData template)
    {
        string notificationText;
        if (App.Settings.ChatEnabled)
        {
            var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel);
            notificationText = msg.ToString();
            ChatManager.Instance.AddChatMessage(msg);
        }
        else
        {
            notificationText = Build(template, parameters.Split('\v'));
        }
        Log.N("Field Boss", notificationText, NotificationType.Success, 10000);

        if (!App.Settings.WebhookEnabledFieldBoss) return;

        // @4157 \v
        // regionName \v @rgn:213 \v
        // npcName \v @creature:26#5001

        var monsterName = GetFieldBossName(parameters);
        var regName = parameters.Split('\v')[2].Replace("@rgn:", "");
        var regId = uint.Parse(regName);

        var regionName = Game.DB!.RegionsDatabase.GetZoneName(regId);

        GameEventManager.ExecuteFieldBossSpawnWebhook(monsterName, regionName, notificationText);

    }

    static void HandleFieldBossDie(string parameters, SystemMessageData template)
    {
        string notificationText;

        if (App.Settings.ChatEnabled)
        {
            var msg = ChatManager.Instance.Factory.CreateSystemMessage(parameters, template, (ChatChannel)template.ChatChannel);
            notificationText = msg.ToString();
            ChatManager.Instance.AddChatMessage(msg);
        }
        else
        {
            notificationText = Build(template, parameters.Split('\v'));
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

        var monsterName = GetFieldBossName(parameters);
        var userName = GetFieldBossKillerName(parameters);
        var guildName = GetFieldBossKillerGuild(parameters);
        if (string.IsNullOrEmpty(guildName)) guildName = "-no guild-";
        GameEventManager.ExecuteFieldBossDieWebhook(monsterName, notificationText, userName, guildName);
    }

    static string GetFieldBossName(string parameters)
    {
        // only for 'SMT_FIELDBOSS_*'
        var srvMsgSplit = parameters.Split('\v');
        var npcName = srvMsgSplit.Last().Replace("@creature:", "");
        var zoneId = uint.Parse(npcName.Split('#')[0]);
        var templateId = uint.Parse(npcName.Split('#')[1]);
        Game.DB!.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m);
        return m.Name;
    }

    static string GetFieldBossKillerName(string parameters)
    {
        // only for 'SMT_FIELDBOSS_*'
        var ret = "";
        var srvMsgSplit = parameters.Split('\v').ToList();
        var idx = srvMsgSplit.IndexOf("userName") + 1;
        if (idx != -1 && idx < srvMsgSplit.Count) ret = srvMsgSplit[idx];
        return ret;
    }

    static string GetFieldBossKillerGuild(string parameters)
    {
        // only for 'SMT_FIELDBOSS_*'
        var ret = "";
        var srvMsgSplit = parameters.Split('\v').ToList();
        var idx = srvMsgSplit.IndexOf("guildName") + 1;
        if (idx != -1 && idx < srvMsgSplit.Count) ret = srvMsgSplit[idx];
        return ret;
    }

    #region Factory

    static readonly Dictionary<string, Delegate> Processor = new()
    {
        { "SMT_FRIEND_IS_CONNECTED",                    new Action<string, SystemMessageData>(HandleFriendLogin) },
        { "SMT_FRIEND_WALK_INTO_SAME_AREA",             new Action<string, SystemMessageData>(HandleFriendInAreaMessage) },
        { "SMT_CHAT_LINKTEXT_DISCONNECT",               new Action<string, SystemMessageData>(HandleInvalidLink) },

        { "SMT_BATTLE_PARTY_DIE",                       new Action<string, SystemMessageData>(HandleDeathMessage) },
        { "SMT_BATTLE_YOU_DIE",                         new Action<string, SystemMessageData>(HandleDeathMessage) },
        { "SMT_BATTLE_PARTY_RESURRECT",                 new Action<string, SystemMessageData>(HandleRessMessage) },
        { "SMT_BATTLE_RESURRECT",                       new Action<string, SystemMessageData>(HandleRessMessage) },

        { "SMT_MAX_ENCHANT_SUCCEED",                    new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Enchant)) },

        { "SMT_ACCEPT_QUEST",                           new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_CANT_START_QUEST",                       new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_COMPLATE_GUILD_QUEST",                   new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_COMPLETE_MISSION",                       new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_COMPLETE_QUEST",                         new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_FAILED_QUEST_COMPENSATION",              new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_FAILED_QUEST",                           new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_FAILED_QUEST_CANCLE",                    new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_QUEST_FAILED_GET_FLAG_PARTY_LEVEL",      new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_QUEST_ITEM_DELETED",                     new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_QUEST_RESET_MESSAGE",                    new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_UPDATE_QUEST_TASK",                      new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_QUEST_SHARE_MESSAGE2",                   new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_QUEST_USE_SKILL",                        new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },
        { "SMT_QUEST_USE_ITEM",                         new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Quest)) },

        { "SMT_GRANT_DUNGEON_COOLTIME_AND_COUNT",       new Action<string, SystemMessageData>(HandleDungeonEngagedMessage) },
        { "SMT_GQUEST_URGENT_NOTIFY",                   new Action<string, SystemMessageData>(HandleGuilBamSpawn) },

        { "SMT_PARTY_LOOT_ITEM_PARTYPLAYER",            new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Loot)) },

        { "SMT_GC_SYSMSG_GUILD_CHIEF_CHANGED",          new Action<string, SystemMessageData>(HandleNewGuildMasterMessage) },

        { "SMT_ACCOMPLISH_ACHIEVEMENT_GRADE_ALL",       new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Laurel)) },

        { "SMT_DROPDMG_DAMAGE",                         new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Damage)) },

        { "SMT_FIELD_EVENT_REWARD_AVAILABLE",           new Action<string, SystemMessageData>(HandleClearedGuardianQuestsMessage) },
        { "SMT_FIELD_EVENT_ENTER",                      new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guardian)) },
        { "SMT_FIELD_EVENT_LEAVE",                      new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guardian)) },
        { "SMT_FIELD_EVENT_COMPLETE",                   new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guardian)) },
        { "SMT_FIELD_EVENT_FAIL_OVERTIME",              new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guardian)) },
        { "SMT_FIELD_EVENT_CLEAR_REWARD_SENT",          new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guardian)) },
        { "SMT_FIELD_EVENT_WORLD_ANNOUNCE",             new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guardian)) },

        { "SMT_ITEM_DECOMPOSE_COMPLETE",                new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Loot)) },
        { "SMT_WAREHOUSE_ITEM_DRAW",                    new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Loot)) },
        { "SMT_WAREHOUSE_ITEM_INSERT",                  new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Loot)) },

        { "SMT_GC_MSGBOX_APPLYLIST_1",                  new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_MSGBOX_APPLYLIST_2",                  new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_MSGBOX_APPLYRESULT_1",                new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_MSGBOX_APPLYRESULT_2",                new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_SYSMSG_ACCEPT_GUILD_APPLY",           new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_SYSMSG_GUILD_MEMBER_BANNED",          new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_SYSMSG_GUILD_MEMBER_BANNED_2",        new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_SYSMSG_LEAVE_GUILD",                  new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_SYSMSG_LEAVE_GUILD_FRIEND",           new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },
        { "SMT_GC_SYSMSG_REFUSE_GUILD_APPLY",           new Action<string, SystemMessageData>((parameters, template) => Redirect(parameters, template, ChatChannel.Guild)) },

        { "SMT_FIELDBOSS_APPEAR",                       new Action<string, SystemMessageData>(HandleFieldBossAppear) },     //by HQ 20181224
        { "SMT_FIELDBOSS_DIE_GUILD",                    new Action<string, SystemMessageData>(HandleFieldBossDie) },        //by HQ 20181224
        { "SMT_FIELDBOSS_DIE_NOGUILD",                  new Action<string, SystemMessageData>(HandleFieldBossDie) },        //by HQ 20181224

        { "SMT_PARTY_MATCHING_CANT_PR_NO_INFORMATION",  new Action<string, SystemMessageData>(HandleLfgNotListed)},

        { "SMT_WORLDSPAWN_NOTIFY_SPAWN",                new Action<string, SystemMessageData>(HandleMerchantSpawn)},
        { "SMT_WORLDSPAWN_NOTIFY_DESPAWN",              new Action<string, SystemMessageData>(HandleMerchantDespawn)},
    };


    static bool Process(string parameters, SystemMessageData template, string opcodeName)
    {
        if (!Processor.TryGetValue(opcodeName, out var type)) return false;
        App.BaseDispatcher.InvokeAsync(() => type.DynamicInvoke(parameters, template));
        return true;
    }
    #endregion
}