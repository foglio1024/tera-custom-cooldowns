using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TCC.Controls;
using TCC.Data.Databases;
using TCC.Parsing;
using TCC.ViewModels;

namespace TCC.Data
{
    public class ChatMessage : TSPropertyChanged
    {
        protected readonly string pieceOpeningTag = "<FONT";
        protected readonly string pieceClosingTag = "</FONT>";

        protected ChatChannel channel;
        public ChatChannel Channel
        {
            get => channel;
            set
            {
                if (channel == value) return;
                channel = value;
                NotifyPropertyChanged(nameof(Channel));
            }
        }

        protected string timestamp;
        public string Timestamp
        {
            get => timestamp;
            set
            {
                if (timestamp == value) return;
                timestamp = value;
                NotifyPropertyChanged(nameof(timestamp));
            }
        }

        protected string rawMessage;
        public string RawMessage
        {
            get => rawMessage;
            set
            {
                if (rawMessage == value) return;
                rawMessage = value;
                NotifyPropertyChanged(nameof(RawMessage));
            }
        }

        protected string author;
        public string Author
        {
            get => author;
            set
            {
                if (author == value) return;
                author = value;
                NotifyPropertyChanged(nameof(Author));
            }
        }

        public ulong AuthorId { get; private set; }

        protected bool containsPlayerName;
        public bool ContainsPlayerName
        {
            get { return containsPlayerName; }
            set
            {
                if (containsPlayerName == value) return;
                containsPlayerName = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return true;// ChatWindowViewModel.Instance.VisibleChannels.Contains(Channel);            
            }
        }

        protected SynchronizedObservableCollection<MessagePiece> pieces;
        public SynchronizedObservableCollection<MessagePiece> Pieces
        {
            get => pieces;
            set
            {
                if (pieces == value) return;
                pieces = value;
                NotifyPropertyChanged(nameof(Pieces));
            }
        }

        protected void AddPiece(MessagePiece mp)
        {
            _dispatcher.Invoke(() => Pieces.Add(mp));
        }
        protected string ReplaceGtLt(string msg)
        {
            msg = msg.Replace("&lt;", "<");
            msg = msg.Replace("&gt;", ">");
            return msg;
        }
        protected void ParseFormattedMessage(string msg)
        {
            var piecesCount = Regex.Matches(msg, pieceClosingTag).Count;
            for (int i = 0; i < piecesCount; i++)
            {
                try
                {
                    msg = ParsePiece(msg); //adds piece to list and cuts msg

                }
                catch (Exception)
                {

                }
            }
        }
        protected void ParseDirectMessage(string msg, ChatChannel ch)
        {
            AddPiece(new MessagePiece(msg, MessagePieceType.Simple, ch));
        }

        private string ParsePiece(string msg)
        {
            var start = msg.IndexOf(pieceOpeningTag) + pieceOpeningTag.Length;
            if (msg[start] == '>')
            {
                start++;
                var end = msg.IndexOf(pieceClosingTag, start);
                var text = msg.Substring(start, end - start);

                if (text.IndexOf(SessionManager.CurrentPlayer.Name, StringComparison.InvariantCultureIgnoreCase) >= 0) ContainsPlayerName = true;


                if (text.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
                if (text.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
                if (text.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
                _dispatcher.Invoke(() => Pieces.Add(new MessagePiece(ReplaceGtLt(text), MessagePieceType.Simple, Channel)));

                return msg = msg.Substring(end + pieceClosingTag.Length);
            }
            else
            {
                try
                {
                    var colorString = msg.Substring(msg.IndexOf("COLOR=") + 8, 6);

                    var textStart = msg.IndexOf("&lt;");
                    var textEnd = msg.IndexOf("&gt;") + 4;
                    if (textStart == -1)
                    {
                        textStart = msg.IndexOf("<");
                        textEnd = msg.IndexOf(">") + 4;
                    }
                    var text = ReplaceGtLt(msg.Substring(textStart, textEnd - textStart));
                    MessagePieceType t;
                    if (msg[msg.IndexOf("#####") - 1] == '1')
                    {
                        t = MessagePieceType.Item;
                    }
                    else
                    {
                        t = MessagePieceType.Quest;
                    }

                    uint id = 0;
                    long uid = 0;
                    string ownerName = "";
                    if (t != MessagePieceType.Quest)
                    {
                        var idStart = msg.IndexOf("#####") + 5;
                        var idEnd = msg.IndexOf("@", idStart);
                        id = UInt32.Parse(msg.Substring(idStart, idEnd - idStart));

                        var uidStart = msg.IndexOf("@", msg.IndexOf("#####") + 6) + 1;
                        var uidEnd = msg.IndexOf("@", uidStart);
                        var uidLength = uidEnd != -1 ? uidEnd - uidStart : 2;
                        uid = long.Parse(msg.Substring(uidStart, uidLength));
                        if (uid == -1) uid = 0;
                        var ownerNameStart = msg.IndexOf("@", uidStart + uidLength) + 1;
                        var ownerNameEnd = msg.IndexOf(">", ownerNameStart) - 1;
                        ownerName = msg.Substring(ownerNameStart, ownerNameEnd - ownerNameStart);
                    }
                    BoundType b = BoundType.None;
                    if (id != 0)
                    {
                        try
                        {
                            b = ItemsDatabase.Items[id].BoundType;
                        }
                        catch (Exception)
                        {

                        }
                    }
                    AddPiece(new MessagePiece(text, t, Channel, colorString, uid, id, ownerName, b));

                    return msg = msg.Substring(msg.IndexOf(pieceClosingTag) + pieceClosingTag.Length);

                }
                catch (Exception)
                {
                    return "";
                }

            }

        }
        public ChatMessage(ChatChannel ch, string auth, string msg, bool priv = false, uint privChannelNumber = 0)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            Pieces = new SynchronizedObservableCollection<MessagePiece>(_dispatcher);

            if (!priv)
            {
                Channel = ch;

            }
            else
            {
                var i = ChatWindowViewModel.Instance.PrivateChannels.FirstOrDefault(x => x.Id == privChannelNumber).Index;
                Channel = (ChatChannel)(ChatWindowViewModel.Instance.PrivateChannels[i].Index + 11);
            }

            RawMessage = msg;
            Author = auth;
            //AuthorId = authId;
            Timestamp = DateTime.Now.ToShortTimeString();
            if (Channel == ChatChannel.Greet)
            {
                ParseDirectMessage(ReplaceGtLt(RawMessage), ch);
            }
            else if (Channel == ChatChannel.Emote)
            {
                ParseSocialMessage(msg);
            }
            else
            {
                ParseFormattedMessage(msg);
            }
        }
        public ChatMessage(string systemMessage, SystemMessage m)
        {

            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            Pieces = new SynchronizedObservableCollection<MessagePiece>(_dispatcher);

            var msg = systemMessage.Split('\v');
            var ch = (ChatChannel)m.ChatChannel;
            var text = ReplaceGtLt(m.Message);

            Channel = ch;
            RawMessage = systemMessage;
            Author = "System";
            Timestamp = DateTime.Now.ToShortTimeString();
            if (text.Contains("Amount@money")) Channel = ChatChannel.Money;
            if (msg.Length == 1)
            {
                AddPiece(new MessagePiece(text, MessagePieceType.Simple, ch));
            }
            else if (Channel == ChatChannel.Money)
            {
                if (text.Contains("Amount@money") || text.Contains("amount@money"))
                {
                    text = text.Replace("Amount@", "Money@");
                    text = text.Replace("amount@", "Money@");
                }
                var textPieces = text.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> param = new Dictionary<string, string>();
                for (int i = 1; i < msg.Length; i += 2)
                {
                    var key = msg[i];
                    var value = msg[i + 1];
                    param.Add(key, value);

                }
                foreach (string p in textPieces)
                {
                    if (p.StartsWith("Money"))
                    {
                        string amount = "";
                        if (param.ContainsKey("Amount")) amount = param["Amount"];
                        else amount = param["Money"];
                        AddPiece(new MessagePiece(new Money(amount), ch));

                    }
                    else
                    {
                        AddPiece(new MessagePiece(p, MessagePieceType.Simple, ch));
                    }
                }



            }
            else
            {
                var textPieces = text.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> param = new Dictionary<string, string>();
                for (int i = 1; i < msg.Length; i += 2)
                {
                    var key = msg[i];
                    var value = msg[i + 1];
                    param.Add(key, value);
                }

                string formattedColor = "";
                bool nextIsFormatted = false;
                foreach (string p in textPieces)
                {
                    var pieceText = p;
                    int fontStart = 0;
                    int fontEnd = 0;
                    bool currentIsFormatted = nextIsFormatted;
                    if (pieceText.Contains("<font"))
                    {
                        nextIsFormatted = true;
                        fontStart = p.IndexOf("<font");
                        fontEnd = p.IndexOf(">", fontStart);
                        formattedColor = p.Substring(p.IndexOf("#", fontStart) + 1, 6);
                        var fontString = p.Substring(fontStart, fontEnd - fontStart + 1);
                        pieceText = p.Replace(fontString, "");
                    }
                    else
                    {
                        nextIsFormatted = false;
                    }

                    if (p.Contains("</font>") && p.Contains("<font>") && p.IndexOf("<font") < p.IndexOf("</font>"))
                    {
                        currentIsFormatted = true;
                        nextIsFormatted = false;
                    }
                    if (p.Contains("</font>")) pieceText = pieceText.Replace("</font>", "");

                    if (pieceText == "") continue;

                    //
                    if (!param.ContainsKey(pieceText))
                    {
                        string c = currentIsFormatted ? formattedColor : "";
                        AddPiece(new MessagePiece(pieceText, MessagePieceType.Simple, ch, c));
                    }
                    else
                    {
                        if (param[pieceText].StartsWith("@item"))
                        {
                            var itemDict = Decode(param[pieceText]);
                            if (ItemsDatabase.Items.TryGetValue(GetId(itemDict, "item"), out Item i))
                            {
                                string username = SessionManager.CurrentPlayer.Name;
                                if (param.ContainsKey("UserName")) username = param["UserName"];
                                var name = i.Name;
                                if (itemDict.ContainsKey("masterpiece"))
                                {
                                    name = itemDict["masterpiece"] + " " + i.Name;
                                }
                                if (itemDict.ContainsKey("enchantCount"))
                                {
                                    name = "+" + itemDict["enchantCount"] + " " + name;
                                }
                                name = "<" + name + ">";
                                AddPiece(new MessagePiece(name, MessagePieceType.Item, ch, GetItemColor(i), GetItemUid(itemDict), GetId(itemDict, "item"), username, i.BoundType));

                            }
                        }
                        else if (param[pieceText].StartsWith("@GuildQuest"))
                        {
                            //"@3817\vguildQuestName\v@GuildQuest:31007001"
                            var gquestDict = Decode(param[pieceText]);
                            if (GuildQuestDatabase.GuildQuests.TryGetValue(GetId(gquestDict, "GuildQuest"), out GuildQuest q))
                            {
                                var questName = q.Title.Replace("{HuntingZone1}", EntitiesManager.CurrentDatabase.GetZoneName(q.ZoneId));
                                string c = currentIsFormatted ? formattedColor : "";
                                AddPiece(new MessagePiece(questName, MessagePieceType.Simple, ch, c));
                            }
                        }
                        else if (param[pieceText].StartsWith("@accountBenefit"))
                        {
                            var d = Decode(param[pieceText]);
                            if (AccountBenefitDatabase.Benefits.TryGetValue(GetId(d, "accountBenefit"), out string ab))
                            {
                                AddPiece(new MessagePiece(ab, MessagePieceType.Simple, ch));
                            }
                        }
                        else if (param[pieceText].StartsWith("@dungeon"))
                        {
                            var d = Decode(param[pieceText]);
                            if (DungeonDatabase.Dungeons.TryGetValue(GetId(d, "dungeon"), out string dngName))
                            {
                                AddPiece(new MessagePiece(dngName, MessagePieceType.Simple, ch));
                            }
                        }
                        else if (param[pieceText].StartsWith("@quest"))
                        {
                            var d = Decode(param[pieceText]);
                            if (QuestDatabase.Quests.TryGetValue(GetId(d, "quest"), out string q))
                            {
                                AddPiece(new MessagePiece(q, MessagePieceType.Simple, ch));
                            }
                        }
                        else if (param[pieceText].StartsWith("@AchievementGradeInfo"))
                        {
                            var d = Decode(param[pieceText]);
                            if (AchievementGradeDatabase.Grades.TryGetValue(GetId(d, "AchievementGradeInfo"), out string g))
                            {
                                AddPiece(new MessagePiece(g, MessagePieceType.Simple, ch));
                            }
                        }
                        else if (param[pieceText].StartsWith("@achievement"))
                        {
                            var d = Decode(param[pieceText]);
                            if (AchievementDatabase.Achievements.TryGetValue(GetId(d, "achievement"), out string g))
                            {
                                string c = currentIsFormatted ? formattedColor : "";
                                var sb = new StringBuilder("<");
                                sb.Append(g);
                                sb.Append(">");

                                AddPiece(new MessagePiece(sb.ToString(), MessagePieceType.Simple, ch, c));
                            }
                        }
                        else
                        {
                            string c = currentIsFormatted ? formattedColor : "";
                            AddPiece(new MessagePiece(param[pieceText], MessagePieceType.Simple, ch, c));
                        }
                    }
                }
            }

            foreach (var item in Pieces)
            {
                Console.WriteLine(item.Text);
            }
        }
        private uint GetId(Dictionary<string, string> d, string paramName)
        {
            return uint.Parse(d[paramName]);
        }
        private long GetItemUid(Dictionary<string, string> d)
        {
            if (d.ContainsKey("dbid")) return long.Parse(d["dbid"]);
            else return 0;
        }
        private string GetItemColor(Item i)
        {
            string CommonColor = "FFFFFF";
            string UncommonColor = "4DCB30";
            string RareColor = "009ED9";
            string SuperiorColor = "EEBE00";

            switch (i.RareGrade)
            {
                case RareGrade.Common:
                    return CommonColor;
                case RareGrade.Uncommon:
                    return UncommonColor;
                case RareGrade.Rare:
                    return RareColor;
                case RareGrade.Superior:
                    return SuperiorColor;
                default:
                    return "";
            }
        }
        private Dictionary<string, string> Decode(string p)
        {
            //@464UserNameChippyAdded12ItemName@item:88176?dbid:273547775?masterpiece
            //@1613ItemAmount5ItemName@item:179072?dbid:254819647
            string[] itemPars = p.Replace("@", "").Split('?');
            Dictionary<string, string> itemParsDict = new Dictionary<string, string>();
            foreach (var i in itemPars)
            {
                var keyVal = i.Split(':');
                if (keyVal.Length == 1)
                {
                    if (keyVal[0] == "masterpiece")
                    {
                        itemParsDict.Add("masterpiece", "Masterwork");
                    }
                    continue;
                }
                itemParsDict.Add(keyVal[0], keyVal[1]);
            }
            return itemParsDict;


        }
        private void ParseSocialMessage(string msg)
        {
            string header = "@social:";
            var start = msg.IndexOf(header);
            if (start == -1)
            {
                AddPiece(new MessagePiece(Author + " " + msg, MessagePieceType.Simple, Channel));
                return;
            }
            start += header.Length;
            var id = UInt32.Parse(msg.Substring(start));
            var text = SocialDatabase.Social[id].Replace("{Name}", Author);
            AddPiece(new MessagePiece(text, MessagePieceType.Simple, Channel));

        }

    }
}
