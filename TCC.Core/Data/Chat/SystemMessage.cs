using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Nostrum.Extensions;
using TCC.Utils;

namespace TCC.Data.Chat;

public class SystemMessage : ChatMessage
{
    static readonly char[] CurlyBrackets = ['{', '}'];

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

            foreach (var p in Pieces)
            {
                PlainMessage += p.Text;
            }
        }
        catch (Exception e)
        {
            Log.F($"Failed to parse system message: {parameters} -- {template.Template}\n {e}");
        }
    }

    void ParseSysHtmlPiece(HtmlNode piece, Dictionary<string, string> prm)
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
            var innerPieces = content.Split(CurlyBrackets, StringSplitOptions.RemoveEmptyEntries);
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
                            Pieces.Last().Text += "s";
                            plural = false;
                        }
                        selectionStep = 0;
                        continue;
                }

                MessagePieceBase mp;

                if (inPiece.StartsWith("@select", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectionStep++;
                    continue;
                }

                if (inPiece.Contains("@item", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgItem(inPiece);
                }
                else if (inPiece.Contains("@abnormal", StringComparison.InvariantCultureIgnoreCase))
                {
                    var abName = "Unknown";
                    if (Game.DB!.AbnormalityDatabase.Abnormalities.TryGetValue(
                            uint.Parse(inPiece.Split(':')[1]), out var ab)) abName = ab.Name;
                    mp = new SimpleMessagePiece(abName, App.Settings.FontSize, false) { Color = col };
                }
                else if (inPiece.Contains("@guildquest", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgGuildQuest(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@dungeon", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgDungeon(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@accountbenefit", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgAccBenefit(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@achievementgradeinfo", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgAchiGrade(inPiece);
                }
                else if (inPiece.Contains("@achievement", StringComparison.InvariantCultureIgnoreCase)) // todo: this has to be here, find a better way pls
                {
                    mp = MessagePieceBuilder.BuildSysMsgAchi(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@quest", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgQuest(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@creature", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgCreature(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@rgn", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgRegion(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@zonename", StringComparison.InvariantCultureIgnoreCase))
                {
                    mp = MessagePieceBuilder.BuildSysMsgZone(inPiece);
                    mp.Color = col;
                }
                else if (inPiece.Contains("@money", StringComparison.InvariantCultureIgnoreCase))
                {
                    var t = inPiece.Replace("@money", "", StringComparison.InvariantCultureIgnoreCase);
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