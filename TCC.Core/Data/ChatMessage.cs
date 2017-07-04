using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
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
        #region Fields
        protected readonly string O_TAG = "<FONT";
        protected readonly string C_TAG = "</FONT>";
        #endregion

        #region Properties
        protected ChatChannel channel;
        public ChatChannel Channel
        {
            get => channel;
            private set
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
            private set
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
            private set
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
            private set
            {
                if (author == value) return;
                author = value;
                NotifyPropertyChanged(nameof(Author));
            }
        }

        protected bool containsPlayerName;
        public bool ContainsPlayerName
        {
            get { return containsPlayerName; }
            private set
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
            private set
            {
                if (pieces == value) return;
                pieces = value;
                NotifyPropertyChanged(nameof(Pieces));
            }
        }
        #endregion

        #region Constructors
        public ChatMessage()
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            Pieces = new SynchronizedObservableCollection<MessagePiece>(_dispatcher);
            Timestamp = DateTime.Now.ToShortTimeString();
            RawMessage = "";
        }
        public ChatMessage(ChatChannel ch, string auth, string msg) : this()
        {
            Channel = ch;
            RawMessage = msg;
            Author = auth;
            if (Channel == ChatChannel.Raid && GroupWindowManager.Instance.IsLeader(Author)) Channel = ChatChannel.RaidLeader;

            try
            {
                switch (ch)
                {
                    case ChatChannel.Greet:
                        ParseDirectMessage(ReplaceGtLt(RawMessage), ch);
                        break;
                    case ChatChannel.Emote:
                        ParseEmoteMessage(msg);
                        break;
                    default:
                        ParseFormattedMessage(msg);
                        break;
                }
            }
            catch (Exception)
            {
                return;
            }
            File.AppendAllText("chat.log", "[CHAT] " + RawMessage + "\n");
        }
        public ChatMessage(string systemMessage, SystemMessage m, string opcodeName) : this()
        {
            Channel = (ChatChannel)m.ChatChannel;

            RawMessage = systemMessage;
            Author = "System";

            var prm = SplitDirectives(systemMessage);
            var txt = ReplaceGtLt(m.Message);

            if (prm == null)
            {
                //only one parameter (opcode) so just add text
                AddPiece(new MessagePiece(txt, MessagePieceType.Simple, Channel));
            }
            else
            {
                //more parameters
                txt = ReplaceParameters(txt, prm);
                var textPieces = SplitByFontTags(txt);

                foreach (var piece in textPieces)
                {
                    string content;
                    string customColor;
                    if (piece.StartsWith("<font"))
                    {
                        //formatted piece: get color and content
                        customColor = piece.Substring(piece.IndexOf('#') + 1, 6);
                        var s = piece.IndexOf('>') + 1;
                        var e = piece.IndexOf('<', s);
                        content = piece.Substring(s, e - s);
                    }
                    else
                    {
                        //normal piece
                        content = piece;
                        customColor = "";
                    }
                    var innerPieces = content.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var inPiece in innerPieces)
                    {
                        if (inPiece.StartsWith("@item"))
                        {
                            AddPiece(ParseSysMsgItem(BuildParametersDictionary(inPiece)));
                        }
                        else if (inPiece.StartsWith("@achievement"))
                        {
                            var mp = ParseSysMsgAchi(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.StartsWith("@GuildQuest"))
                        {
                            var mp = ParseSysMsgGuildQuest(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.StartsWith("@dungeon"))
                        {
                            var mp = ParseSysMsgDungeon(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.StartsWith("@accountBenefit"))
                        {
                            var mp = ParseSysMsgAccBenefit(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.StartsWith("@AchievementGradeInfo"))
                        {
                            var mp = ParseSysMsgAchiGrade(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.StartsWith("@quest"))
                        {
                            var mp = ParseSysMsgQuest(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.StartsWith("@creature"))
                        {
                            var mp = ParseSysMsgCreature(BuildParametersDictionary(inPiece));
                            mp.SetColor(customColor);
                            AddPiece(mp);
                        }
                        else if (inPiece.Contains("@money"))
                        {
                            Channel = ChatChannel.Money;
                            var t = inPiece.Replace("@money", "");
                            AddPiece(new MessagePiece(new Money(t), ChatChannel.Money));
                        }
                        else
                        {
                            AddPiece(new MessagePiece(inPiece, MessagePieceType.Simple, Channel, customColor));
                        }
                    }
                }
            }

            File.AppendAllText("chat.log", "[SYST] " + RawMessage + "\n");
        }
        private static MessagePiece ParseSysMsgCreature(Dictionary<string, string> dictionary)
        {
            var creatureId = dictionary["creature"];
            var creatureSplit = creatureId.Split('#');
            var zoneId = uint.Parse(creatureSplit[0]);
            var templateId = uint.Parse(creatureSplit[1]);

            string txt = creatureId;

            if (EntitiesManager.CurrentDatabase.TryGetMonster(templateId, zoneId, out Monster m))
            {
                txt = m.Name;
            }

            MessagePiece mp = new MessagePiece(txt)
            {
                Type = MessagePieceType.Simple
            };
            return mp;
        }
        #endregion

        #region Generic Methods
        private void AddPiece(MessagePiece mp)
        {
            _dispatcher.Invoke(() => Pieces.Add(mp));
        }
        private string ReplaceGtLt(string msg, string left = "<", string right = ">")
        {
            msg = msg.Replace("&lt;", left);
            msg = msg.Replace("&gt;", right);
            return msg;
        }
        #endregion

        #region Chat Methods
        private void ParseDirectMessage(string msg, ChatChannel ch)
        {
            AddPiece(new MessagePiece(msg, MessagePieceType.Simple, ch));
        }
        private void ParseEmoteMessage(string msg)
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
        private void ParseFormattedMessage(string msg)
        {
            var piecesCount = Regex.Matches(msg, C_TAG).Count;
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
        private string ParsePiece(string msg)
        {
            var start = msg.IndexOf(O_TAG) + O_TAG.Length;
            if (msg[start] == '>')
            {
                //it's not formatted: just take the value and add it to pieces
                start++;
                var end = msg.IndexOf(C_TAG, start);

                //get the message text
                var text = msg.Substring(start, end - start);

                //check if player is mentioned
                try
                {
                    foreach (var item in SessionManager.CurrentAccountCharacters)
                    {
                        if (text.IndexOf(item.Name, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            ContainsPlayerName = true;
                            break;
                        }
                    }
                }
                catch (Exception) { }

                //redirect trading message if it's in global
                if (text.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
                if (text.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
                if (text.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;

                //add piece to list
                AddPiece(new MessagePiece(ReplaceGtLt(text), MessagePieceType.Simple, Channel));

                //cut message
                return msg = msg.Substring(end + C_TAG.Length);
            }
            else
            {
                File.AppendAllText("chat2.log", "[CHAT] " + RawMessage + "\n");
                //it's formatted: parse then add

                //get custom color
                var colorIndex = msg.IndexOf("COLOR=");
                var customColor = colorIndex == -1 ? "" : msg.Substring(colorIndex + 8, 6);

                //get link type
                var linkIndex = msg.IndexOf("#####");
                var t = msg.Substring(linkIndex - 1, 1);
                int type = Int32.Parse(t);

                var aStart = msg.IndexOf("<A");
                var aEnd = msg.IndexOf("</A>");

                var a = msg.Substring(aStart, aEnd - aStart + 1);

                MessagePiece mp;

                if (type == 1)
                {
                    mp = ParseItemLink(a);
                }
                else if (type == 2)
                {
                    mp = ParseQuestLink(a);
                }
                else if (type == 3)
                {
                    mp = ParseLocationLink(a);
                }
                else
                {
                    throw new Exception();
                }

                mp.SetColor(customColor);

                AddPiece(mp);

                //cut message
                return msg = msg.Substring(msg.IndexOf(C_TAG) + C_TAG.Length);
            }
        }
        private string[] ParseLinkedParameters(string a)
        {
            var parStart = a.IndexOf("#####") + 5;
            var parEnd = a.IndexOf('"', parStart);
            var parString = a.Substring(parStart, parEnd - parStart);

            return parString.Split('@');
        }
        private MessagePiece ParseItemLink(string a)
        {
            var pars = ParseLinkedParameters(a);
            var id = UInt32.Parse(pars[0]);
            var uid = Int32.Parse(pars[1]);
            var owner = pars[2];

            var textStart = a.IndexOf('>', a.IndexOf(owner)) + 1;
            var textEnd = a.IndexOf('<', textStart);

            var text = a.Substring(textStart, textEnd - textStart);

            var result = new MessagePiece(ReplaceGtLt(text))
            {
                ItemId = id,
                ItemUid = uid,
                OwnerName = owner,
                Type = MessagePieceType.Item
            };
            ItemsDatabase.Items.TryGetValue(id, out Item i);
            if (i != null)
            {
                result.BoundType = i.BoundType;
            }
            return result;
        }
        private MessagePiece ParseQuestLink(string a)
        {
            //parsing only name
            var textStart = a.IndexOf('>', a.IndexOf("#####")) + 1;
            var textEnd = a.IndexOf('<', textStart);

            var text = a.Substring(textStart, textEnd - textStart);
            text = ReplaceGtLt(text);

            var result = new MessagePiece(text)
            {
                Type = MessagePieceType.Simple
            };

            return result;
        }
        private MessagePiece ParseLocationLink(string a)
        {
            var pars = ParseLinkedParameters(a);
            var locTree = pars[0].Split('_');
            var world = Int32.Parse(locTree[0]);
            var guard = Int32.Parse(locTree[1]);
            var zone = Int32.Parse(locTree[2]);
            var continent = Int32.Parse(pars[1]);
            var coords = pars[2].Split(',');
            var x = Double.Parse(coords[0], CultureInfo.InvariantCulture);
            var y = Double.Parse(coords[1], CultureInfo.InvariantCulture);
            var z = Double.Parse(coords[2], CultureInfo.InvariantCulture);

            var textStart = a.IndexOf('>', a.IndexOf("#####")) + 1;
            var textEnd = a.IndexOf('<', textStart);
            var text = a.Substring(textStart, textEnd - textStart);
            text = ReplaceGtLt(text);
            var result = new MessagePiece(text)
            {
                Type = MessagePieceType.Point_of_interest
            };
            return result;
        }
        #endregion

        #region System Methods
        private static MessagePiece ParseSysMsgItem(Dictionary<string, string> info)
        {
            var id = GetId(info, "item");
            var uid = GetItemUid(info);
            string username = SessionManager.CurrentPlayer.Name;
            if (info.ContainsKey("UserName")) username = info["UserName"];
            MessagePiece mp = new MessagePiece(id.ToString());
            if (ItemsDatabase.Items.TryGetValue(id, out Item i))
            {
                var txt = String.Format("<{0}>", i.Name);
                mp = new MessagePiece(txt)
                {
                    Type = MessagePieceType.Item,
                    BoundType = i.BoundType,
                    ItemId = id,
                    ItemUid = uid,
                    OwnerName = username
                };
                mp.SetColor(GetItemColor(i));
            }
            return mp;
        }
        private MessagePiece ParseSysMsgAchi(Dictionary<string, string> info)
        {
            var id = GetId(info, "achievement");
            var achiName = id.ToString();
            if (AchievementDatabase.Achievements.TryGetValue(id, out string g))
            {
                achiName = String.Format("[{0}]", g);
            }
            return new MessagePiece(achiName, MessagePieceType.Simple, Channel);
        }
        private MessagePiece ParseSysMsgQuest(Dictionary<string, string> info)
        {
            var id = GetId(info, "quest");
            string txt = id.ToString();
            if (QuestDatabase.Quests.TryGetValue(id, out string q))
            {
                txt = q;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        private MessagePiece ParseSysMsgAchiGrade(Dictionary<string, string> info)
        {
            var id = GetId(info, "AchievementGradeInfo");
            var txt = id.ToString();
            if (AchievementGradeDatabase.Grades.TryGetValue(id, out string g))
            {
                txt = g;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        private MessagePiece ParseSysMsgDungeon(Dictionary<string, string> info)
        {
            var id = GetId(info, "dungeon");
            string txt = id.ToString();
            if (DungeonDatabase.Dungeons.TryGetValue(id, out string dngName))
            {
                txt = dngName;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        private MessagePiece ParseSysMsgAccBenefit(Dictionary<string, string> info)
        {
            var id = GetId(info, "accountBenefit");
            string txt = id.ToString();
            if (AccountBenefitDatabase.Benefits.TryGetValue(id, out string ab))
            {
                txt = ab;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        private MessagePiece ParseSysMsgGuildQuest(Dictionary<string, string> info)
        {
            var id = GetId(info, "GuildQuest");
            string questName = id.ToString();
            if (GuildQuestDatabase.GuildQuests.TryGetValue(id, out GuildQuest q))
            {
                questName = q.Title.Replace("{HuntingZone1}", EntitiesManager.CurrentDatabase.GetZoneName(q.ZoneId));
            }
            return new MessagePiece(questName, MessagePieceType.Simple, Channel);
        }
        private static Dictionary<string, string> SplitDirectives(string m)
        {
            var parameters = m.Split('\v');
            if (parameters.Length == 1)
            {
                return null;
            }
            var dict = new Dictionary<string, string>();
            for (int i = 1; i < parameters.Length - 1; i = i + 2)
            {
                dict.Add(parameters[i], parameters[i + 1]);
            }
            return dict;
        }
        private static Dictionary<string, string> BuildParametersDictionary(string p)
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
                    if (keyVal[0] == "awakened")
                    {
                        itemParsDict.Add("awakened", "Awakened");
                    }
                    continue;
                }
                itemParsDict.Add(keyVal[0], keyVal[1]);
            }
            return itemParsDict;
        }
        private string[] SplitByFontTags(string txt)
        {
            //formatted text
            var result = new List<string>();
            while (true)
            {
                var s = txt.IndexOf("<font");
                string x;
                if (s == 0)
                {
                    //piece begins with opening tag
                    var e = txt.IndexOf("</font>", s);
                    x = txt.Substring(s, e - s + 7);
                }
                else if (s == -1)
                {
                    //piece doesen't contain opening tag (end of string)
                    x = txt.Substring(0);
                }
                else
                {
                    //opening tag is not at the beginning
                    x = txt.Substring(0, s);
                }
                result.Add(x);

                txt = txt.Replace(x, "");

                if (txt.Length == 0) break;
            }

            return result.ToArray();
        }
        private static string ReplaceParameters(string txt, Dictionary<string, string> pars)
        {
            string result = "";
            foreach (var keyVal in pars)
            {
                result  = txt.Replace('{' + keyVal.Key, '{' + keyVal.Value);
                if(txt == result)
                {
                    result = Utils.ReplaceCaseInsensitive(txt, '{' + keyVal.Key, '{' + keyVal.Value);
                }
                txt = result;
            }
            return result;
        }
        private static string GetItemColor(Item i)
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
        private static uint GetId(Dictionary<string, string> d, string paramName)
        {
            return uint.Parse(d[paramName]);
        }
        private static long GetItemUid(Dictionary<string, string> d)
        {
            if (d.ContainsKey("dbid")) return long.Parse(d["dbid"]);
            else return 0;
        }
        #endregion

        #region Builders
        public static ChatMessage BuildEnchantSystemMessage(string systemMessage)
        {
            var msg = new ChatMessage();
            string mw = " Masterwork ";
            string e = "+12";
            if (systemMessage.Contains("Added12"))
            {
                msg.Channel = ChatChannel.Enchant12;
            }
            else if (systemMessage.Contains("Added15"))
            {
                msg.Channel = ChatChannel.Enchant15;
                mw = " Awakened ";
                e = "+15";
            }
            var prm = SplitDirectives(systemMessage);

            msg.Author = prm["UserName"];
            var txt = "{ItemName}";
            txt = ReplaceParameters(txt, prm);
            txt = txt.Replace("{", "");
            txt = txt.Replace("}", "");
            var mp = ParseSysMsgItem(BuildParametersDictionary(txt));

            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(e);
            sb.Append(mw);
            sb.Append(mp.Text.Substring(1));
            mp.Text = sb.ToString();
            msg.AddPiece(mp);

            return msg;
        }
        #endregion
    }
}
