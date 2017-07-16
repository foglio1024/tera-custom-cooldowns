using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TCC.Parsing.Messages;
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
            protected set
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
            protected set
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
            protected set
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
            protected set
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
            protected set
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


        protected bool isContracted;
        public bool IsContracted
        {
            get
            {
                return isContracted;
            }
            set
            {
                if (isContracted == value) return;
                isContracted = value;
                NotifyPropertyChanged(nameof(IsContracted));
            }
        }

        private int rows;
        public int Rows
        {
            get { return rows; }
            set
            {
                if (rows == value) return;
                rows = value;
                NotifyPropertyChanged(nameof(Rows));
            }
        }
        public bool ShowTimestamp
        {
            get => SettingsManager.ShowTimestamp;
        }
        public bool ShowChannel
        {
            get => SettingsManager.ShowChannel;
        }
        protected SynchronizedObservableCollection<MessagePiece> pieces;
        public SynchronizedObservableCollection<MessagePiece> Pieces
        {
            get => pieces;
            protected set
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
            WindowManager.Settings.Dispatcher.Invoke(() => ((SettingsWindowViewModel)WindowManager.Settings.DataContext).PropertyChanged += VM_PropChanged);
            RawMessage = "";
        }
        public ChatMessage(ChatChannel ch, string auth, string msg) : this()
        {
            Channel = ch;
            RawMessage = msg;
            Author = auth;
            try
            {
                if (Channel == ChatChannel.Raid && GroupWindowViewModel.Instance.IsLeader(Author)) Channel = ChatChannel.RaidLeader;
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
            catch (Exception) { }
        }
        public ChatMessage(string systemMessage, SystemMessage m) : this()
        {
            Channel = (ChatChannel)m.ChatChannel;
            RawMessage = systemMessage;
            Author = "System";
            try
            {
                var prm = SplitDirectives(systemMessage);
                var txt = ReplaceGtLt(m.Message);
                txt = txt.Replace("<BR>", " ");
                if (prm == null)
                {
                    //only one parameter (opcode) so just add text
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
                        RawMessage = content;
                        AddPiece(new MessagePiece(content, MessagePieceType.Simple, Channel, customColor));
                    }
                }
                else
                {
                    //more parameters
                    txt = ReplaceParameters(txt, prm, true);
                    RawMessage = txt;
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
                        bool plural = false;
                        int selectionStep = 0;

                        foreach (var inPiece in innerPieces)
                        {
                            if (selectionStep == 1)
                            {
                                var n = Int32.Parse(inPiece);
                                if (n != 1) plural = true;

                                selectionStep++;
                                continue;
                            }
                            if (selectionStep == 2)
                            {
                                if (inPiece == "/s//s")
                                {
                                    if (plural)
                                    {
                                        Pieces.Last().Text = Pieces.Last().Text + "s";
                                        plural = false;
                                    }
                                }

                                selectionStep = 0;
                                continue;
                            }

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
                            else if (inPiece.StartsWith("@select"))
                            {
                                selectionStep++;
                            }
                            else if (inPiece.StartsWith("@zoneName"))
                            {
                                var mp = ParseSysMsgZone(BuildParametersDictionary(inPiece));
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
            }
            catch (Exception) { }
        }

        #endregion

        #region Generic Methods
        protected void AddPiece(MessagePiece mp)
        {
            _dispatcher.Invoke(() => Pieces.Add(mp));
        }
        protected void InsertPiece(MessagePiece mp, int index)
        {
            _dispatcher.Invoke(() => Pieces.Insert(index, mp));
        }
        protected void RemovePiece(MessagePiece mp)
        {
            _dispatcher.Invoke(() => Pieces.Remove(mp));
        }
        public static string ReplaceGtLt(string msg, string left = "<", string right = ">")
        {
            msg = msg.Replace("&lt;", left);
            msg = msg.Replace("&gt;", right);
            return msg;
        }
        public static void SetChannel(ChatMessage msg, ChatChannel ch)
        {
            msg.Channel = ch;
        }
        internal static void SplitSimplePieces(ChatMessage chatMessage)
        {
            var simplePieces = new List<MessagePiece>();
            bool onlySimple = true;
            foreach (var item in chatMessage.Pieces)
            {
                if (item.Type == MessagePieceType.Simple)
                {
                    simplePieces.Add(item);
                }
                else if (item.Type == MessagePieceType.Item)
                {
                    simplePieces.Add(item);
                    onlySimple = false;
                }
                else
                {
                    onlySimple = false;
                }
            }
            if (onlySimple) return;

            for (int i = 0; i < simplePieces.Count; i++)
            {
                simplePieces[i].Text = simplePieces[i].Text.Replace(" ", " [[");
                var split = simplePieces[i].Text.Split(new string[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);

                int index = chatMessage.Pieces.IndexOf(simplePieces[i]);
                for (int j = 0; j < split.Length; j++)
                {
                    var mp = new MessagePiece(split[j])
                    {
                        Color = simplePieces[i].Color,
                        Type = simplePieces[i].Type,
                        ItemId = simplePieces[i].ItemId,
                        ItemUid = simplePieces[i].ItemUid,
                        BoundType = simplePieces[i].BoundType,
                        OwnerName = simplePieces[i].OwnerName,
                        RawLink = simplePieces[i].RawLink
                    };
                    chatMessage.InsertPiece(mp, index);
                    index = chatMessage.Pieces.IndexOf(mp) + 1;
                }
                chatMessage.RemovePiece(simplePieces[i]);
            }
        }
        private void VM_PropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShowChannel))
            {
                NotifyPropertyChanged(nameof(ShowChannel));
            }
            else if (e.PropertyName == nameof(ShowTimestamp))
            {
                NotifyPropertyChanged(nameof(ShowTimestamp));
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in Pieces)
            {
                sb.Append(item.Text);
            }
            return sb.ToString();
        }
        #endregion

        #region Chat Methods
        protected void ParseDirectMessage(string msg, ChatChannel ch)
        {
            AddPiece(new MessagePiece(msg, MessagePieceType.Simple, ch));
        }


        protected void ParseEmoteMessage(string msg)
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
        protected void ParseFormattedMessage(string msg)
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
        protected string ParsePiece(string msg)
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

                var t2 = text.Replace(" ", " [[");
                var split = t2.Split(new string[] { "[[" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                StringBuilder content = new StringBuilder("");
                foreach (var item in split)
                {
                    Regex RgxUrl = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
                    if (RgxUrl.IsMatch(item))
                    {
                        if (content.ToString() != "")
                        {
                            AddPiece(new MessagePiece(ReplaceGtLt(content.ToString()), MessagePieceType.Simple, Channel));
                            content = new StringBuilder("");
                        }
                        AddPiece(new MessagePiece(ReplaceGtLt(item), MessagePieceType.Url, Channel, "7289da"));
                    }
                    else
                    {
                        content.Append(item);
                    }
                }
                if (content.ToString() != "")
                {
                    AddPiece(new MessagePiece(ReplaceGtLt(content.ToString()), MessagePieceType.Simple, Channel));
                }

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
        protected string[] ParseLinkedParameters(string a)
        {
            var parStart = a.IndexOf("#####") + 5;
            var parEnd = a.IndexOf('"', parStart);
            var parString = a.Substring(parStart, parEnd - parStart);

            return parString.Split('@');
        }
        protected MessagePiece ParseItemLink(string a)
        {
            var linkData = a.Substring(a.IndexOf("#####") - 1);
            linkData = linkData.Substring(0, linkData.IndexOf(">") - 1);
            var pars = ParseLinkedParameters(a);
            var id = UInt32.Parse(pars[0]);
            var uid = Int32.Parse(pars[1]);
            string owner = "";
            try { owner = pars[2]; }
            catch (Exception)
            {
            }

            var textStart = a.IndexOf('>') + 1;
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
            result.RawLink = linkData;
            return result;
        }
        protected MessagePiece ParseQuestLink(string a)
        {
            var linkData = a.Substring(a.IndexOf("#####") - 1);
            linkData = linkData.Substring(0, linkData.IndexOf(">") - 1);

            //parsing only name
            var textStart = a.IndexOf('>', a.IndexOf("#####")) + 1;
            var textEnd = a.IndexOf('<', textStart);

            var text = a.Substring(textStart, textEnd - textStart);
            text = ReplaceGtLt(text);

            var result = new MessagePiece(text)
            {
                Type = MessagePieceType.Quest
            };
            result.RawLink = linkData;

            return result;
        }
        protected MessagePiece ParseLocationLink(string a)
        {
            var linkData = a.Substring(a.IndexOf("#####") - 1);
            linkData = linkData.Substring(0, linkData.IndexOf(">") - 1);

            var pars = ParseLinkedParameters(a);
            var locTree = pars[0].Split('_');
            var worldId = UInt32.Parse(locTree[0]);
            var guardId = UInt32.Parse(locTree[1]);
            var sectionId = UInt32.Parse(locTree[2]);
            if (worldId == 1 && guardId == 2 && sectionId == 9) sectionId = 7;
            var continent = UInt32.Parse(pars[1]);
            continent = continent == 0 && worldId == 1 && guardId == 24 && sectionId == 183001 ? 7031 : continent;
            var coords = pars[2].Split(',');
            var x = Double.Parse(coords[0], CultureInfo.InvariantCulture);
            var y = Double.Parse(coords[1], CultureInfo.InvariantCulture);
            var z = Double.Parse(coords[2], CultureInfo.InvariantCulture);

            var textStart = a.IndexOf('>', a.IndexOf("#####")) + 1;
            var textEnd = a.IndexOf('<', textStart);
            var text = a.Substring(textStart, textEnd - textStart); //get actual map name from database
            text = ReplaceGtLt(text);

            var world = MapDatabase.Worlds[worldId];
            var guard = world.Guards[guardId];
            var section = guard.Sections[sectionId];
            var sb = new StringBuilder();

            var guardName = guard.NameId != 0 ? MapDatabase.Names[guard.NameId] : "";
            var sectionName = MapDatabase.Names[section.NameId];
            //sb.Append(MapDatabase.Names[world.NameId]);
            sb.Append("<");

            sb.Append(guardName);
            if (guardName != sectionName)
            {
                if (guardName != "") sb.Append(" - ");
                sb.Append(sectionName);
            }
            sb.Append(">");


            var result = new MessagePiece(sb.ToString())
            {
                Type = MessagePieceType.Point_of_interest
            };
            result.Location = new Location(worldId, guardId, sectionId, x, y);
            result.RawLink = linkData;// String.Format("{0}_{1}_{2}@{3}@{4},{5},{6}", worldId, guardId, sectionId, continent == 0 && worldId==1 && guardId ==24 && sectionId==183001? 7031 : continent, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture), z.ToString(CultureInfo.InvariantCulture));
            return result;
        }
        #endregion

        #region System Methods
        protected MessagePiece ParseSysMsgZone(Dictionary<string, string> dictionary)
        {
            var zoneId = uint.Parse(dictionary["zoneName"]);
            var zoneName = EntitiesManager.CurrentDatabase.GetZoneName(zoneId);
            string txt = zoneId.ToString();
            if (zoneName != null) txt = zoneName;
            MessagePiece mp = new MessagePiece(txt)
            {
                Type = MessagePieceType.Simple
            };
            return mp;
        }
        protected MessagePiece ParseSysMsgCreature(Dictionary<string, string> dictionary)
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
        protected static MessagePiece ParseSysMsgItem(Dictionary<string, string> info)
        {
            var id = GetId(info, "item");
            var uid = GetItemUid(info);

            var rawLink = new StringBuilder("1#####");
            rawLink.Append(id.ToString());
            if (uid != 0)
            {
                rawLink.Append("@" + uid.ToString());
            }

            string username = SessionManager.CurrentPlayer.Name;
            if (info.ContainsKey("UserName"))
            {
                username = info["UserName"];
                rawLink.Append("@" + username);
            }
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
                    OwnerName = username,
                    RawLink = rawLink.ToString()
                };
                mp.SetColor(GetItemColor(i));
            }
            return mp;
        }
        protected MessagePiece ParseSysMsgAchi(Dictionary<string, string> info)
        {
            var id = GetId(info, "achievement");
            var achiName = id.ToString();
            if (AchievementDatabase.Achievements.TryGetValue(id, out string g))
            {
                achiName = String.Format("[{0}]", g);
            }
            return new MessagePiece(achiName, MessagePieceType.Simple, Channel);
        }
        protected MessagePiece ParseSysMsgQuest(Dictionary<string, string> info)
        {
            var id = GetId(info, "quest");
            string txt = id.ToString();
            if (QuestDatabase.Quests.TryGetValue(id, out string q))
            {
                txt = q;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        protected MessagePiece ParseSysMsgAchiGrade(Dictionary<string, string> info)
        {
            var id = GetId(info, "AchievementGradeInfo");
            var txt = id.ToString();
            if (AchievementGradeDatabase.Grades.TryGetValue(id, out string g))
            {
                txt = g;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        protected MessagePiece ParseSysMsgDungeon(Dictionary<string, string> info)
        {
            var id = GetId(info, "dungeon");
            string txt = id.ToString();
            if (DungeonDatabase.Dungeons.TryGetValue(id, out string dngName))
            {
                txt = dngName;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        protected MessagePiece ParseSysMsgAccBenefit(Dictionary<string, string> info)
        {
            var id = GetId(info, "accountBenefit");
            string txt = id.ToString();
            if (AccountBenefitDatabase.Benefits.TryGetValue(id, out string ab))
            {
                txt = ab;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, Channel);
        }
        protected MessagePiece ParseSysMsgGuildQuest(Dictionary<string, string> info)
        {
            var id = GetId(info, "GuildQuest");
            string questName = id.ToString();
            if (GuildQuestDatabase.GuildQuests.TryGetValue(id, out GuildQuest q))
            {
                questName = q.Title;
            }
            return new MessagePiece(questName, MessagePieceType.Simple, Channel);
        }
        protected static Dictionary<string, string> SplitDirectives(string m)
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
        protected static Dictionary<string, string> BuildParametersDictionary(string p)
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
        protected string[] SplitByFontTags(string txt)
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
                var regex = new Regex(Regex.Escape(x));
                txt = regex.Replace(txt, "", 1);

                if (txt.Length == 0) break;
            }

            return result.ToArray();
        }
        protected static string ReplaceParameters(string txt, Dictionary<string, string> pars, bool all)
        {
            string result = "";
            if (!all)
            {
                foreach (var keyVal in pars)
                {
                    var regex = new Regex(Regex.Escape('{' + keyVal.Key + '}'));

                    result = regex.Replace(txt, '{' + keyVal.Value + '}', 1);
                    if (txt == result)
                    {
                        result = Utils.ReplaceFirstOccurrenceCaseInsensitive(txt, '{' + keyVal.Key + '}', '{' + keyVal.Value + '}');
                    }
                    if (txt == result)
                    {
                        result = Utils.ReplaceFirstOccurrenceCaseInsensitive(txt, '{' + keyVal.Key, '{' + keyVal.Value);
                    }
                    txt = result;
                }
            }
            else
            {
                foreach (var keyVal in pars)
                {
                    var regex = new Regex(Regex.Escape('{' + keyVal.Key + '}'));

                    result = regex.Replace(txt, '{' + keyVal.Value + '}');
                    if (txt == result)
                    {
                        result = Utils.ReplaceCaseInsensitive(txt, '{' + keyVal.Key + '}', '{' + keyVal.Value + '}');
                    }
                    if (txt == result)
                    {
                        result = Utils.ReplaceCaseInsensitive(txt, '{' + keyVal.Key, '{' + keyVal.Value);
                    }
                    txt = result;
                }

            }
            return result;
        }
        protected static string GetItemColor(Item i)
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
        protected static uint GetId(Dictionary<string, string> d, string paramName)
        {
            return uint.Parse(d[paramName]);
        }
        protected static long GetItemUid(Dictionary<string, string> d)
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
            txt = ReplaceParameters(txt, prm, true);
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