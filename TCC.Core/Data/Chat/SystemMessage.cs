using System;
using System.Collections.Generic;
using System.Linq;
using FoglioUtils.Extensions;
using HtmlAgilityPack;
using TCC.Utils;

namespace TCC.Data.Chat
{
    public class SystemMessage : ChatMessage
    {
        public SystemMessage(string parameters, SystemMessageData template, ChatChannel ch)
        {
            Channel = ch;
            RawMessage = parameters;
            Author = "System";
            try
            {
                var prm = ChatUtils.SplitDirectives(parameters);
                var txt = template.Template.UnescapeHtml().Replace("<BR>", "\r\n");
                var html = new HtmlDocument(); html.LoadHtml(txt);
                var htmlPieces = html.DocumentNode.ChildNodes;


                if (prm == null)
                {
                    //only one parameter (opcode) so just add text

                    foreach (var htmlPiece in htmlPieces)
                    {
                        var customColor = ChatUtils.GetCustomColor(htmlPiece);
                        var content = htmlPiece.InnerText;
                        RawMessage = content;
                        AddPiece(new SimpleMessagePiece(content, App.Settings.FontSize, false, customColor));
                    }
                }
                else
                {
                    //more parameters

                    foreach (var htmlPiece in htmlPieces)
                    {
                        ParseSysHtmlPiece(htmlPiece, prm);
                    }
                }

            }
            catch
            {
                Log.F($"Failed to parse system message: {parameters} -- {template.Template}");
                // ignored
            }
        }
        private void ParseSysHtmlPiece(HtmlNode piece, Dictionary<string, string> prm)
        {
            if (piece.Name == "img")
            {
                var source = piece.GetAttributeValue("src", "")
                    .Replace("img://__", "")
                    .ToLower();
                var mp = new IconMessagePiece(source, App.Settings.FontSize, false);
                AddPiece(mp);
            }
            else
            {
                var col = ChatUtils.GetCustomColor(piece);

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
                                Pieces.Last().Text = Pieces.Last().Text + "s";
                                plural = false;
                            }
                            selectionStep = 0;
                            continue;
                    }

                    MessagePieceBase mp;
                    if (inPiece.StartsWith("@select"))
                    {
                        selectionStep++;
                        continue;
                    }
                    if (inPiece.StartsWith("@item"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgItem(inPiece);
                    }
                    else if (inPiece.StartsWith("@abnormal"))
                    {
                        var abName = "Unknown";
                        if (Game.DB.AbnormalityDatabase.Abnormalities.TryGetValue(
                            uint.Parse(inPiece.Split(':')[1]), out var ab)) abName = ab.Name;
                        mp = new SimpleMessagePiece(abName, App.Settings.FontSize, false);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@achievement"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgAchi(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@GuildQuest"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgGuildQuest(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@dungeon"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgDungeon(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@accountBenefit"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgAccBenefit(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@AchievementGradeInfo"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgAchiGrade(inPiece);
                    }
                    else if (inPiece.StartsWith("@quest"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgQuest(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@creature"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgCreature(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@rgn"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgRegion(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.StartsWith("@zoneName"))
                    {
                        mp = MessagePieceBuilder.BuildSysMsgZone(inPiece);
                        mp.Color = col;
                    }
                    else if (inPiece.Contains("@money"))
                    {
                        var t = inPiece.Replace("@money", "");
                        mp = new MoneyMessagePiece(new Money(t));
                        Channel = ChatChannel.Money;
                    }
                    else
                    {
                        mp = new SimpleMessagePiece(inPiece.UnescapeHtml(), App.Settings.FontSize, false, col);
                    }
                    AddPiece(mp);
                }
            }
        }

    }
}