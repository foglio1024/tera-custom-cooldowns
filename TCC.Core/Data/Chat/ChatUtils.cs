using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using FoglioUtils.Extensions;
using TCC.Converters;
using TCC.Interop;
using TCC.R;
using TCC.Utils;

namespace TCC.Data.Chat
{
    public static class ChatUtils
    {
        public static string GradeToColorString(RareGrade g)
        {
            switch (g)
            {
                case RareGrade.Common: return Colors.ItemCommonColor.ToHex();
                case RareGrade.Uncommon: return Colors.ItemUncommonColor.ToHex();
                case RareGrade.Rare: return Colors.ItemRareColor.ToHex();
                case RareGrade.Superior: return Colors.ItemSuperiorColor.ToHex();
                default: return "";
            }
        }
        public static uint GetId(Dictionary<string, string> d, string paramName)
        {
            return UInt32.Parse(d[paramName]);
        }

        public static long GetItemUid(Dictionary<string, string> d)
        {
            return d.TryGetValue("dbid", out var value) ? Int64.Parse(value) : 0;
        }

        public static Dictionary<string, string> SplitDirectives(string m)
        {
            var parameters = m.Split('\v');
            return SplitDirectives(parameters);
        }
        public static Dictionary<string, string> SplitDirectives(params string[] parameters)
        {
            if (parameters.Length == 1) return null;
            var retDict = new Dictionary<string, string>();
            for (var i = 1; i < parameters.Length - 1; i += 2)
            {
                retDict.Add(parameters[i], parameters[i + 1]);
            }
            return retDict;
        }
        public static Dictionary<string, string> BuildParametersDictionary(string p)
        {
            //@464UserNameChippyAdded12ItemName@item:88176?dbid:273547775?masterpiece
            //@1613ItemAmount5ItemName@item:179072?dbid:254819647
            var stringPairs = p.Replace("@", "").Split('?');
            var retDict = new Dictionary<string, string>();
            foreach (var stringPair in stringPairs)
            {
                var paremterPair = stringPair.Split(':');
                if (paremterPair.Length == 1)
                {
                    if (paremterPair[0] == "masterpiece") retDict["masterpiece"] = "Masterwork";
                    if (paremterPair[0] == "awakened") retDict["awakened"] = "Awakened";
                    continue;
                }
                retDict[paremterPair[0]] = paremterPair[1];
            }
            return retDict;
        }

        public static string ReplaceParameters(string txt, Dictionary<string, string> pars, bool all)
        {
            var result = "";
            if (!all)
            {
                foreach (var keyVal in pars)
                {
                    var regex = new Regex(Regex.Escape($"{{{keyVal.Key}}}"));
                    result = regex.Replace(txt, $"{{{keyVal.Value}}}", 1);
                    if (txt == result) result = txt.ReplaceFirstOccurrenceCaseInsensitive($"{{{keyVal.Key}}}", $"{{{keyVal.Value}}}");
                    if (txt == result) result = txt.ReplaceFirstOccurrenceCaseInsensitive($"{{{keyVal.Key}", $"{{{keyVal.Value}");
                    txt = result;
                }
            }
            else
            {
                foreach (var keyVal in pars)
                {
                    var regex = new Regex(Regex.Escape($"{{{keyVal.Key}}}"));
                    result = regex.Replace(txt, $"{{{keyVal.Value}}}", Int32.MaxValue);
                    if (txt == result) result = txt.ReplaceCaseInsensitive($"{{{keyVal.Key}}}", $"{{{keyVal.Value}}}");
                    if (txt == result) result = txt.ReplaceCaseInsensitive($"{{{keyVal.Key}", $"{{{keyVal.Value}");
                    txt = result;
                }
            }
            return result;
        }

        public static string GetCustomColor(HtmlNode node)
        {
            var ret = "";
            if (!node.HasAttributes) return ret;
            ret = node.GetAttributeValue("color", "").Substring(1);
            while (ret.Length < 6) { ret = "0" + ret; }
            return ret;
        }

        public static string GetPlainText(string msg)
        {
            var sb = new StringBuilder();
            try
            {
                var html = new HtmlDocument(); html.LoadHtml(msg);
                var htmlPieces = html.DocumentNode.ChildNodes;

                foreach (var htmlPiece in htmlPieces)
                {
                    sb.Append(htmlPiece.InnerText);
                }
            }
            catch
            {
                sb.Append(msg);
            }

            return sb.ToString();
        }

        public static bool CheckMention(string text)
        {
            switch (App.Settings.MentionMode)
            {
                case MentionMode.Current:
                    try
                    {
                        return text.IndexOf(Game.Me.Name, StringComparison.InvariantCultureIgnoreCase) != -1;
                    }
                    catch { return false; }
                case MentionMode.All:
                    try
                    {
                        return Game.Account.Characters.Where(c => !c.Hidden).Any(ch => text.IndexOf(ch.Name, StringComparison.InvariantCultureIgnoreCase) != -1);
                    }
                    catch { return false; }
                default: return false;
            }
        }
        public static void CheckDiscordNotify(string message, string discordUsername)
        {
            if (FocusManager.IsForeground) return;
            if (!App.Settings.WebhookEnabledMentions) return;
            //var txt = GetPlainText(message).UnescapeHtml();
            //var chStr = new ChatChannelToName().Convert(ch, null, null, null);

            Discord.FireWebhook(App.Settings.WebhookUrlMentions, message, discordUsername); //string.IsNullOrEmpty(discordTextOverride) ? $"**{author}** `{chStr}`\n{txt}" : discordTextOverride);

        }
        public static void CheckWindowNotify(string message, string title)
        {
            if (FocusManager.IsForeground) return;
            if (!App.Settings.BackgroundNotifications) return;
            //var txt = GetPlainText(message).UnescapeHtml();
            //var chStr = new ChatChannelToName().Convert(ch, null, null, null);

            Log.N(title, message /*string.IsNullOrEmpty(titleOverride) ? $"{chStr} - {author}" : titleOverride, $"{txt}"*/, NotificationType.Normal, 6000);

        }
    }
}
