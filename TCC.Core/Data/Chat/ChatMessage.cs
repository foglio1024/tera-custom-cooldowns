using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TCC.Data.Map;
using TCC.ViewModels;

namespace TCC.Data.Chat
{
    public class ChatMessage : TSPropertyChanged
    {
        #region Fields

        private const string OTag = "<FONT";
        private const string CTag = "</FONT>";

        #endregion

        #region Properties

        private ChatChannel _channel;
        public ChatChannel Channel
        {
            get => _channel;
            protected set
            {
                if (_channel == value) return;
                _channel = value;
                NPC(nameof(Channel));
            }
        }

        private string _timestamp;
        public string Timestamp
        {
            get => _timestamp;
            protected set
            {
                if (_timestamp == value) return;
                _timestamp = value;
                NPC(nameof(Timestamp));
            }
        }

        private string _rawMessage;
        public string RawMessage
        {
            get => _rawMessage;
            private set
            {
                if (_rawMessage == value) return;
                _rawMessage = value;
                NPC(nameof(RawMessage));
            }
        }

        private string _author;
        public string Author
        {
            get => _author;
            set
            {
                if (_author == value) return;
                _author = value;
                NPC(nameof(Author));
            }
        }

        public bool ContainsPlayerName { get; set; }

        private bool _animate = true;

        public bool Animate
        {
            get => _animate && Settings.Settings.AnimateChatMessages;
            set => _animate = value;
        }

        public bool ShowTimestamp => Settings.Settings.ShowTimestamp;
        public bool ShowChannel => Settings.Settings.ShowChannel;

        private SynchronizedObservableCollection<MessagePiece> _pieces;
        private bool _isVisible;

        public SynchronizedObservableCollection<MessagePiece> Pieces
        {
            get => _pieces;
            protected set
            {
                if (_pieces == value) return;
                _pieces = value;
                NPC(nameof(Pieces));
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                Pieces.ToList().ForEach(p => p.IsVisible = value);
                if (value)
                {
                    SettingsWindowViewModel.ChatShowChannelChanged += ShowChannelNPC;
                    SettingsWindowViewModel.ChatShowTimestampChanged += ShowTimestampNPC;
                    SettingsWindowViewModel.FontSizeChanged += FontSizeNPC;
                }
                else
                {
                    SettingsWindowViewModel.ChatShowChannelChanged -= ShowChannelNPC;
                    SettingsWindowViewModel.ChatShowTimestampChanged -= ShowTimestampNPC;
                    SettingsWindowViewModel.FontSizeChanged -= FontSizeNPC;
                }

                if (_isVisible == value) return;
                _isVisible = value;
                NPC();
            }
        }

        public int Size => Settings.Settings.FontSize;
        private void ShowChannelNPC()
        {
            NPC(nameof(ShowChannel));
        }

        private void ShowTimestampNPC()
        {
            NPC(nameof(ShowTimestamp));
        }
        private void FontSizeNPC()
        {
            NPC(nameof(Size));
        }
        #endregion

        #region Constructors

        protected ChatMessage()
        {
            Dispatcher = ChatWindowManager.Instance.GetDispatcher();
            Pieces = new SynchronizedObservableCollection<MessagePiece>(Dispatcher);
            Timestamp = DateTime.Now.ToShortTimeString();
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
                        ParseDirectMessage(ReplaceEscapes(RawMessage), ch);
                        break;
                    case ChatChannel.Emote:
                        ParseEmoteMessage(msg);
                        break;
                    default:
                        ParseFormattedMessage(msg);
                        break;
                }
            }
            catch
            {
                // ignored
            }
        }
        public ChatMessage(string systemMessage, SystemMessage m, ChatChannel ch) : this()
        {
            Channel = ch;
            RawMessage = systemMessage;
            Author = "System";
            try
            {
                var prm = SplitDirectives(systemMessage);
                var txt = ReplaceEscapes(m.Message);
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
                        AddPiece(new MessagePiece(content, MessagePieceType.Simple, Channel, Settings.Settings.FontSize, false, customColor));
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
                        var fontSize = Settings.Settings.FontSize;
                        if (piece.StartsWith("<font"))
                        {
                            //formatted piece: get color and content
                            customColor = piece.Substring(piece.IndexOf('#') + 1, 6);
                            //var fStart = piece.IndexOf("size=", StringComparison.InvariantCultureIgnoreCase) == -1 ?
                                //piece.IndexOf("size=", StringComparison.InvariantCultureIgnoreCase) :
                                //piece.IndexOf("size=", StringComparison.InvariantCultureIgnoreCase) + 6;

                            //var fEnd = fStart == -1 ? 0 : piece.IndexOf("\"", fStart, piece.Length, StringComparison.InvariantCultureIgnoreCase);
                            //fontSize = fStart == -1 ? fontSize : int.Parse(piece.Substring(fStart, fEnd - fStart));
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
                        var innerPieces = content.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                        var plural = false;
                        var selectionStep = 0;

                        foreach (var inPiece in innerPieces)
                        {
                            if (selectionStep == 1)
                            {
                                var n = int.Parse(inPiece);
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
                                AddPiece(new MessagePieceBuilder(Channel).ParseSysMsgItem(BuildParametersDictionary(inPiece)));
                            }
                            else if (inPiece.StartsWith("@achievement"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgAchi(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@GuildQuest"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgGuildQuest(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@dungeon"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgDungeon(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@accountBenefit"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgAccBenefit(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@AchievementGradeInfo"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgAchiGrade(BuildParametersDictionary(inPiece));

                                //mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@quest"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgQuest(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@creature"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgCreature(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.StartsWith("@select"))
                            {
                                selectionStep++;
                            }
                            else if (inPiece.StartsWith("@zoneName"))
                            {
                                var mp = new MessagePieceBuilder(Channel).ParseSysMsgZone(BuildParametersDictionary(inPiece));
                                mp.SetColor(customColor);
                                AddPiece(mp);
                            }
                            else if (inPiece.Contains("@money"))
                            {
                                Channel = ChatChannel.Money;
                                var t = inPiece.Replace("@money", "");
                                AddPiece(new MessagePiece(new Money(t)));
                            }
                            else
                            {
                                AddPiece(new MessagePiece(ReplaceEscapes(inPiece), MessagePieceType.Simple, Channel, fontSize, fontSize != 18, customColor));
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region Generic Methods
        protected void AddPiece(MessagePiece mp)
        {
            Dispatcher.Invoke(() => Pieces.Add(mp));
        }
        protected void InsertPiece(MessagePiece mp, int index)
        {
            Dispatcher.Invoke(() => Pieces.Insert(index, mp));
        }
        protected void RemovePiece(MessagePiece mp)
        {
            Dispatcher.Invoke(() => Pieces.Remove(mp));
        }
        public static string ReplaceEscapes(string msg, string left = "<", string right = ">")
        {
            msg = msg.Replace("&lt;", left);
            msg = msg.Replace("&gt;", right);
            msg = msg.Replace("&#xA", "\n");
            msg = msg.Replace("&quot;", "\"");
            return msg;
        }
        public static void SetChannel(ChatMessage msg, ChatChannel ch)
        {
            msg.Channel = ch;
        }
        internal static void SplitSimplePieces(ChatMessage chatMessage)
        {
            var simplePieces = new List<MessagePiece>();
            //var onlySimple = true;
            foreach (var item in chatMessage.Pieces)
            {
                if (item.Type == MessagePieceType.Simple)
                {
                    simplePieces.Add(item);
                }
                else if (item.Type == MessagePieceType.Item)
                {
                    simplePieces.Add(item);
                    //onlySimple = false;
                }
            }
            //if (onlySimple) return;

            for (var i = 0; i < simplePieces.Count; i++)
            {
                simplePieces[i].Text = simplePieces[i].Text.Replace(" ", " [[");
                var split = simplePieces[i].Text.Split(new[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);
                var index = chatMessage.Pieces.IndexOf(simplePieces[i]);
                for (var j = 0; j < split.Length; j++)
                {

                    var endsWithK = split[j].ToLower().EndsWith("k ", StringComparison.InvariantCultureIgnoreCase) ||
                                    split[j].ToLower().EndsWith("k", StringComparison.InvariantCultureIgnoreCase);
                    var endsWithG = split[j].ToLower().EndsWith("g ", StringComparison.InvariantCultureIgnoreCase) ||
                                    split[j].ToLower().EndsWith("g", StringComparison.InvariantCultureIgnoreCase);
                    var isNumber = int.TryParse(split[j].ToLower().Replace("k ", "").Replace("k", "").Replace("g ", "").Replace("g", ""), out var money);

                    //var isEmoji = split[j].StartsWith(":") && (split[j].EndsWith(":") || split[j].EndsWith(": "));

                    var mp = (endsWithK || endsWithG)&& isNumber && (chatMessage.Channel == ChatChannel.Trade ||
                                                        chatMessage.Channel == ChatChannel.TradeRedirect ||
                                                        chatMessage.Channel == ChatChannel.Megaphone ||
                                                        chatMessage.Channel == ChatChannel.Global)?
                        new MessagePiece(new Money(endsWithK ? money*1000 : money, 0, 0)) 
                        //: 
                        //isEmoji?
                        //new MessagePiece(split[j]) { Type = MessagePieceType.Emoji} 
                        :
                        new MessagePiece(split[j])
                        {
                            Color = simplePieces[i].Color,
                            Type = simplePieces[i].Type,
                            ItemId = simplePieces[i].ItemId,
                            ItemUid = simplePieces[i].ItemUid,
                            BoundType = simplePieces[i].BoundType,
                            OwnerName = simplePieces[i].OwnerName,
                            RawLink = simplePieces[i].RawLink,
                            Size = simplePieces[i].Size
                        };
                    chatMessage.InsertPiece(mp, index);
                    index = chatMessage.Pieces.IndexOf(mp) + 1;
                }
                chatMessage.RemovePiece(simplePieces[i]);
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
            AddPiece(new MessagePiece(msg, MessagePieceType.Simple, ch, Settings.Settings.FontSize, false));
        }
        protected void ParseEmoteMessage(string msg)
        {
            const string header = "@social:";
            var start = msg.IndexOf(header, StringComparison.Ordinal);
            if (start == -1)
            {
                AddPiece(new MessagePiece(Author + " " + msg, MessagePieceType.Simple, Channel, Settings.Settings.FontSize, false));
                return;
            }
            start += header.Length;
            var id = uint.Parse(msg.Substring(start));
            var text = SessionManager.SocialDatabase.Social[id].Replace("{Name}", Author);
            AddPiece(new MessagePiece(text, MessagePieceType.Simple, Channel, Settings.Settings.FontSize, false));

        }
        protected void ParseFormattedMessage(string msg)
        {

            var piecesCount = Regex.Matches(msg, CTag, RegexOptions.IgnoreCase).Count;
            for (var i = 0; i < piecesCount; i++)
            {
                try
                {
                    msg = ParsePiece(msg); //adds piece to list and cuts msg
                }
                catch
                {
                    // ignored
                }
            }
        }

        protected string ParsePiece(string msg)
        {
            var start = msg.IndexOf(OTag, StringComparison.InvariantCultureIgnoreCase) + OTag.Length;
            if (msg[start] == '>')
            {
                //it's not formatted: just take the value and add it to pieces
                start++;
                var end = msg.IndexOf(CTag, start, StringComparison.InvariantCultureIgnoreCase);
                //get the message text
                var text = msg.Substring(start, end - start);

                CheckMention(text);
                CheckRedirect(text);
                var content = GetPieceContent(text);
                if (content != "")
                {
                    AddPiece(new MessagePiece(ReplaceEscapes(content.Replace("<a href=\"asfunction:chatLinkAction\">", "").Replace("</a>", "")),
                                                MessagePieceType.Simple, Channel, Settings.Settings.FontSize, false));
                }

                //cut message
                return msg.Substring(end + CTag.Length);
            }
            else
            {
                //it's formatted: parse then add
                var customColor = GetCustomColor(msg);
                var fontSize = GetPieceSize(); //msg);
                //get link type
                var linkIndex = msg.IndexOf("#####", StringComparison.Ordinal);
                if (linkIndex > -1)
                {
                    var t = msg.Substring(linkIndex - 1, 1);
                    var type = int.Parse(t);

                    var aStart = msg.IndexOf("<ChatLinkAction", StringComparison.Ordinal);
                    var aEnd = msg.IndexOf("</ChatLinkAction>", StringComparison.Ordinal);

                    var a = msg.Substring(aStart, aEnd - aStart + 1);

                    MessagePiece mp;

                    switch (type)
                    {
                        case 1:
                            mp = ParseItemLink(a);
                            break;
                        case 2:
                            mp = ParseQuestLink(a);
                            break;
                        case 3:
                            mp = ParseLocationLink(a);
                            break;
                        default:
                            throw new Exception();
                    }

                    mp.SetColor(customColor);
                    mp.Size = fontSize;
                    AddPiece(mp);
                }
                else
                {
                    var s = msg.IndexOf(">", StringComparison.Ordinal);
                    var e = msg.IndexOf(CTag, StringComparison.InvariantCultureIgnoreCase);
                    var pc = new MessagePiece(
                        msg.Substring(s + 1, e - s - 1).Replace("<a href=\"asfunction:chatLinkAction\">", "")
                            .Replace("</a>", ""), MessagePieceType.Simple, Channel, fontSize, true, customColor);
                    AddPiece(pc);
                }

                //cut message
                return msg.Substring(msg.IndexOf(CTag, StringComparison.InvariantCultureIgnoreCase) + CTag.Length);
            }
        }
        protected string[] ParseLinkedParameters(string a)
        {
            var parStart = a.IndexOf("#####", StringComparison.Ordinal) + 5;
            var parEnd = a.IndexOf('"', parStart);
            var parString = a.Substring(parStart, parEnd - parStart);

            return parString.Split('@');
        }
        protected MessagePiece ParseItemLink(string a)
        {
            var linkData = a.Substring(a.IndexOf("#####", StringComparison.Ordinal) - 1);
            linkData = linkData.Substring(0, linkData.IndexOf(">", StringComparison.Ordinal) - 1);
            var pars = ParseLinkedParameters(a);
            var id = uint.Parse(pars[0]);
            var uid = long.Parse(pars[1]);
            var owner = "";
            try { owner = pars[2]; }
            catch
            {
                // ignored
            }

            var textStart = a.IndexOf('>') + 1;
            var textEnd = a.IndexOf('<', textStart);

            var text = a.Substring(textStart, textEnd - textStart);

            var result = new MessagePiece(ReplaceEscapes(text))
            {
                ItemId = id,
                ItemUid = uid,
                OwnerName = owner,
                Type = MessagePieceType.Item
            };
            result.RawLink = linkData;
            return result;
        }
        protected MessagePiece ParseQuestLink(string a)
        {
            var linkData = a.Substring(a.IndexOf("#####", StringComparison.Ordinal) - 1);
            linkData = linkData.Substring(0, linkData.IndexOf(">", StringComparison.Ordinal) - 1);

            //parsing only name
            var textStart = a.IndexOf('>', a.IndexOf("#####", StringComparison.Ordinal)) + 1;
            var textEnd = a.IndexOf('<', textStart);

            var text = a.Substring(textStart, textEnd - textStart);
            text = ReplaceEscapes(text);

            var result = new MessagePiece(text)
            {
                Type = MessagePieceType.Quest
            };
            result.RawLink = linkData;

            return result;
        }
        protected MessagePiece ParseLocationLink(string a)
        {
            var linkData = a.Substring(a.IndexOf("#####", StringComparison.Ordinal) - 1);
            linkData = linkData.Substring(0, linkData.IndexOf(">", StringComparison.Ordinal) - 1);

            var pars = ParseLinkedParameters(a);
            var locTree = pars[0].Split('_');
            var worldId = uint.Parse(locTree[0]);
            var guardId = uint.Parse(locTree[1]);
            var sectionId = uint.Parse(locTree[2]);
            if (worldId == 1 && guardId == 2 && sectionId == 9) sectionId = 7;
            //var continent = uint.Parse(pars[1]);
            //continent = continent == 0 && worldId == 1 && guardId == 24 && sectionId == 183001 ? 7031 : continent;
            var coords = pars[2].Split(',');
            var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
            var y = double.Parse(coords[1], CultureInfo.InvariantCulture);
            //var z = double.Parse(coords[2], CultureInfo.InvariantCulture);

            //var textStart = a.IndexOf('>', a.IndexOf("#####", StringComparison.Ordinal)) + 1;
            //var textEnd = a.IndexOf('<', textStart);
            //var text = a.Substring(textStart, textEnd - textStart); //get actual map name from database
            //text = ReplaceEscapes(text);

            var world = SessionManager.MapDatabase.Worlds[worldId];
            var guard = world.Guards[guardId];
            var section = guard.Sections[sectionId];
            var sb = new StringBuilder();

            var guardName = guard.NameId != 0 ? SessionManager.MapDatabase.Names[guard.NameId] : "";
            var sectionName = SessionManager.MapDatabase.Names[section.NameId];
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
                Type = MessagePieceType.PointOfInterest,
                Location = new Location(worldId, guardId, sectionId, x, y),
                RawLink = linkData
            };
            // String.Format("{0}_{1}_{2}@{3}@{4},{5},{6}", worldId, guardId, sectionId, continent == 0 && worldId==1 && guardId ==24 && sectionId==183001? 7031 : continent, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture), z.ToString(CultureInfo.InvariantCulture));
            return result;
        }

        private void CheckMention(string text)
        {
            //check if player is mentioned
            try
            {
                foreach (var item in InfoWindowViewModel.Instance.Characters)
                {
                    if (text.IndexOf(item.Name, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        ContainsPlayerName = true;
                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }

        }
        private void CheckRedirect(string text)
        {
            //redirect trading message if it's in global
            if (text.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
            if (text.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;
            if (text.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;

        }
        private string GetCustomColor(string msg)
        {
            var hasSpace = false;
            var colorIndex = msg.IndexOf("COLOR=", StringComparison.InvariantCultureIgnoreCase);
            if (colorIndex == -1)
            {
                colorIndex = msg.IndexOf("COLOR =", StringComparison.InvariantCultureIgnoreCase);
                if (colorIndex != -1) hasSpace = true;
            }
            var offset = hasSpace ? 10 : 8;
            var colorEnd = msg.IndexOf("\"", colorIndex+offset+1, StringComparison.Ordinal);
            if(colorEnd == -1) colorEnd = msg.IndexOf("\'", colorIndex + offset + 1, StringComparison.Ordinal);
            if(colorIndex == -1) return "";
            var col = msg.Substring(colorIndex + offset, colorEnd - colorIndex - offset);
            while (col.Length < 6)
            {
                col = "0" + col;
            }
            return col;

        }
        private string GetPieceContent(string text)
        {
            var textToSplit = text.Replace(" ", " [[");
            var split = textToSplit.Split(new[] { "[[" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var content = new StringBuilder("");
            foreach (var token in split)
            {
                var rgxUrl = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
                if (rgxUrl.IsMatch(token) || token.StartsWith("discord.gg"))
                {
                    //add it as url
                    if (content.ToString() != "")
                    {
                        AddPiece(new MessagePiece(ReplaceEscapes(content.ToString()), MessagePieceType.Simple, Channel, Settings.Settings.FontSize, false));
                        content = new StringBuilder("");
                    }
                    AddPiece(new MessagePiece(ReplaceEscapes(token), MessagePieceType.Url, Channel, Settings.Settings.FontSize, false, "7289da"));
                }
                else
                {
                    //add it as string
                    content.Append(token);
                }
            }
            return content.ToString();
        }
        //private int GetPieceSize(string msg)
        //{
        //    //var sizeIndex = msg.IndexOf("SIZE=", StringComparison.InvariantCultureIgnoreCase) > -1 ?
        //    //    msg.IndexOf("SIZE=", StringComparison.InvariantCultureIgnoreCase) + 6 : -1;
        //    //var sizeEnd = sizeIndex > -1 ? msg.IndexOf("\"", sizeIndex, StringComparison.Ordinal) : 0;
        //    //sizeEnd = sizeEnd > -1 ? sizeEnd : msg.IndexOf('\'', sizeIndex);
        //    return Settings.FontSize;//sizeIndex > -1 ? int.Parse(msg.Substring(sizeIndex, sizeEnd - sizeIndex)) : 18;

        //}
        private static int GetPieceSize()
        {
            return Settings.Settings.FontSize;
        }
        #endregion

        #region System Methods
        protected static Dictionary<string, string> SplitDirectives(string m)
        {
            var parameters = m.Split('\v');
            if (parameters.Length == 1)
            {
                return null;
            }
            var dict = new Dictionary<string, string>();
            for (var i = 1; i < parameters.Length - 1; i = i + 2)
            {
                dict.Add(parameters[i], parameters[i + 1]);
            }
            return dict;
        }
        protected static Dictionary<string, string> BuildParametersDictionary(string p)
        {
            //@464UserNameChippyAdded12ItemName@item:88176?dbid:273547775?masterpiece
            //@1613ItemAmount5ItemName@item:179072?dbid:254819647
            var itemPars = p.Replace("@", "").Split('?');
            var itemParsDict = new Dictionary<string, string>();
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
                var s = txt.IndexOf("<font", StringComparison.Ordinal);
                string x;
                if (s == 0)
                {
                    //piece begins with opening tag
                    var e = txt.IndexOf("</font>", s, StringComparison.Ordinal);
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
            var result = "";
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
        #endregion

        #region Builders
        public static ChatMessage BuildEnchantSystemMessage(string systemMessage)
        {
            var msg = new ChatMessage();
            var mw = "";
            var e = "";
            //if (systemMessage.Contains("Added12"))
            //{
            //    msg.Channel = ChatChannel.Enchant12;
            //}
            //else if (systemMessage.Contains("Added15"))
            //{
            //    msg.Channel = ChatChannel.Enchant15;
            //    mw = " Awakened ";
            //    e = "+15";
            //}
            if (systemMessage.Contains("enchantCount:"))
            {
                var s = systemMessage.IndexOf("enchantCount:", StringComparison.InvariantCultureIgnoreCase);
                var ench = systemMessage.Substring(s+"enchantCount:".Length, 1);
                msg.Channel = ChatChannel.Enchant;
                mw = "";
                e = $"+{ench} ";
            }
            //else if (systemMessage.Contains("enchantCount:7"))
            //{
            //    msg.Channel = ChatChannel.Enchant;
            //    mw = "";
            //    e = "+7 ";
            //}
            //else if (systemMessage.Contains("enchantCount:8"))
            //{
            //    msg.Channel = ChatChannel.Enchant;
            //    mw = "";
            //    e = "+8 ";
            //}
            //else if (systemMessage.Contains("enchantCount:9"))
            //{
            //    msg.Channel = ChatChannel.Enchant;
            //    mw = "";
            //    e = "+9 ";
            //}
            var prm = SplitDirectives(systemMessage);

            msg.Author = prm["UserName"];
            var txt = "{ItemName}";
            txt = ReplaceParameters(txt, prm, true);
            txt = txt.Replace("{", "");
            txt = txt.Replace("}", "");
            var mp = new MessagePieceBuilder(msg.Channel).ParseSysMsgItem(BuildParametersDictionary(txt));

            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append(e);
            sb.Append(mw);
            sb.Append(mp.Text.Substring(1));
            mp.Text = sb.ToString();
            msg.AddPiece(new MessagePiece("Successfully enchanted ", MessagePieceType.Simple, msg.Channel, Settings.Settings.FontSize, false, "cccccc"));
            msg.AddPiece(mp);

            return msg;
        }

        #endregion
    }
}