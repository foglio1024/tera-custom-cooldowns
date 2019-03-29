using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.Annotations;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Data.Chat
{
    public class ChatMessage : TSPropertyChanged, IDisposable
    {
        #region Properties

        private bool _animate = true;
        private bool _isVisible;

        public ChatChannel Channel { get; protected set; }

        [UsedImplicitly] public string Timestamp { get; protected set; }

        public string RawMessage { get; private set; }

        public string Author { get; set; }

        public bool ContainsPlayerName { get; set; }
        public bool Animate
        {
            get => _animate && SettingsHolder.AnimateChatMessages;
            set => _animate = value;
        }
        [UsedImplicitly] public bool ShowTimestamp => SettingsHolder.ShowTimestamp;
        [UsedImplicitly] public bool ShowChannel => SettingsHolder.ShowChannel;
        public SynchronizedObservableCollection<MessageLine> Lines { get; protected set; }
        public SynchronizedObservableCollection<MessagePiece> Pieces { get; }

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
                N();
            }
        }
        public int Size => SettingsHolder.FontSize;
        #endregion


        public ChatMessage()
        {
            Dispatcher = ChatWindowManager.Instance.GetDispatcher();
            Pieces = new SynchronizedObservableCollection<MessagePiece>(Dispatcher);
            Lines = new SynchronizedObservableCollection<MessageLine>(Dispatcher);
            Timestamp = SettingsHolder.ChatTimestampSeconds ? DateTime.Now.ToLongTimeString() : DateTime.Now.ToShortTimeString();
            RawMessage = "";
        }
        public ChatMessage(ChatChannel ch, string auth, string msg) : this()
        {
            Channel = ch;
            RawMessage = msg;
            var authHtml = new HtmlDocument();
            authHtml.LoadHtml(auth);
            Author = authHtml.DocumentNode.InnerText;

            try
            {
                if (Channel == ChatChannel.Raid && WindowManager.GroupWindow.VM.IsLeader(Author)) Channel = ChatChannel.RaidLeader;
                switch (ch)
                {
                    case ChatChannel.Greet:
                    case ChatChannel.Angler:
                        ParseDirectMessage(RawMessage.ReplaceHtmlEscapes());
                        break;
                    case ChatChannel.Emote:
                        ParseEmoteMessage(msg);
                        break;
                    default:
                        //ParseFormattedMessage(msg);
                        ParseHtmlMessage(msg);
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
#if DEBUG
            Log.F("sysmsg.log", $"{systemMessage} -- {m.Message}");
#endif
            Channel = ch;
            RawMessage = systemMessage;
            Author = "System";
            try
            {
                var prm = ChatUtils.SplitDirectives(systemMessage);
                var txt = m.Message.ReplaceHtmlEscapes().Replace("<BR>", "\r\n");
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
                        AddPiece(new MessagePiece(content, MessagePieceType.Simple, SettingsHolder.FontSize, false, customColor));
                    }
                }
                else
                {
                    //more parameters

                    foreach (var htmlPiece in htmlPieces)
                    {
                        ParseSysHtmlPiece(htmlPiece);
                    }

                    void ParseSysHtmlPiece(HtmlNode piece)
                    {
                        if (piece.Name == "img")
                        {
                            var source = piece.GetAttributeValue("src", "")
                                              .Replace("img://__", "")
                                              .ToLower();
                            var mp = new MessagePiece(source, MessagePieceType.Icon, SettingsHolder.FontSize, false);
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

                                MessagePiece mp;
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
                                    if (SessionManager.DB.AbnormalityDatabase.Abnormalities.TryGetValue(
                                        uint.Parse(inPiece.Split(':')[1]), out var ab)) abName = ab.Name;
                                    mp = new MessagePiece(abName, MessagePieceType.Simple, SettingsHolder.FontSize, false);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@achievement"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgAchi(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@GuildQuest"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgGuildQuest(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@dungeon"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgDungeon(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@accountBenefit"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgAccBenefit(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@AchievementGradeInfo"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgAchiGrade(inPiece);
                                }
                                else if (inPiece.StartsWith("@quest"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgQuest(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@creature"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgCreature(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.StartsWith("@zoneName"))
                                {
                                    mp = MessagePieceBuilder.BuildSysMsgZone(inPiece);
                                    mp.SetColor(col);
                                }
                                else if (inPiece.Contains("@money"))
                                {
                                    var t = inPiece.Replace("@money", "");
                                    mp = new MessagePiece(new Money(t));
                                    Channel = ChatChannel.Money;
                                }
                                else
                                {
                                    mp = new MessagePiece(inPiece.ReplaceHtmlEscapes(), MessagePieceType.Simple, SettingsHolder.FontSize, false, col);
                                }
                                AddPiece(mp);
                            }
                        }
                    }
                }

            }
            catch
            {
                Log.F($"Failed to parse system message: {systemMessage} -- {m.Message}");
                // ignored
            }
        }

        private void AddPiece(MessagePiece mp)
        {
            mp.Container = this;
            Pieces.Add(mp);
        }
        private void InsertPiece(MessagePiece mp, int index)
        {
            Dispatcher.Invoke(() =>
            {
                mp.Container = this;
                Pieces.Insert(index, mp);
            });
        }
        private void RemovePiece(MessagePiece mp)
        {
            Dispatcher.Invoke(() => Pieces.Remove(mp));
        }
        //TODO: refactor
        public void SplitSimplePieces()
        {
            var simplePieces = new List<MessagePiece>();
            foreach (var item in Pieces)
            {
                if (item.Type == MessagePieceType.Simple || item.Type == MessagePieceType.Item) simplePieces.Add(item);
            }

            for (var i = 0; i < simplePieces.Count; i++)
            {
                simplePieces[i].Text = simplePieces[i].Text.Replace(" ", " [[");
                var split = simplePieces[i].Text.Split(new[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);
                var index = Pieces.IndexOf(simplePieces[i]);
                for (var j = 0; j < split.Length; j++)
                {

                    var endsWithK = split[j].ToLower().EndsWith("k ", StringComparison.InvariantCultureIgnoreCase) ||
                                    split[j].ToLower().EndsWith("k", StringComparison.InvariantCultureIgnoreCase);
                    var endsWithG = split[j].ToLower().EndsWith("g ", StringComparison.InvariantCultureIgnoreCase) ||
                                    split[j].ToLower().EndsWith("g", StringComparison.InvariantCultureIgnoreCase);
                    var isNumber = int.TryParse(split[j].ToLower().Replace("k ", "").Replace("k", "").Replace("g ", "").Replace("g", ""), out var money);


                    var mp = (endsWithK || endsWithG) && isNumber && (Channel == ChatChannel.Trade ||
                                                                      Channel == ChatChannel.TradeRedirect ||
                                                                      Channel == ChatChannel.Megaphone ||
                                                                      Channel == ChatChannel.Global) ?
                        new MessagePiece(new Money(endsWithK ? money * 1000 : money, 0, 0))
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
                    InsertPiece(mp, index);
                    index = Pieces.IndexOf(mp) + 1;
                }
                RemovePiece(simplePieces[i]);
            }

            // split lines
            Lines.Add(new MessageLine());
            foreach (var item in Pieces)
            {
                if (item.Text.Contains("\r\n") || item.Text.Contains("\n\t") || item.Text.Contains("\n"))
                {
                    item.Text = item.Text.Replace("\r\n", "").Replace("\n\t", "").Replace("\n", "");
                    Lines.Add(new MessageLine());
                }
                Lines.Last().LinePieces.Add(item);
            }
        }
        private void ShowChannelNPC()
        {
            N(nameof(ShowChannel));
        }
        private void ShowTimestampNPC()
        {
            N(nameof(ShowTimestamp));
        }
        private void FontSizeNPC()
        {
            N(nameof(Size));
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            Dispatcher.Invoke(() =>
            {
                foreach (var item in Pieces.ToSyncList())
                {
                    sb.Append(item.Text);
                }
            });
            return sb.ToString();
        }


        private void ParseDirectMessage(string msg)
        {
            AddPiece(new MessagePiece(msg, MessagePieceType.Simple, SettingsHolder.FontSize, false));
        }
        private void ParseEmoteMessage(string msg)
        {
            const string header = "@social:";
            var start = msg.IndexOf(header, StringComparison.Ordinal);
            if (start == -1)
            {
                AddPiece(new MessagePiece(Author + " " + msg, MessagePieceType.Simple, SettingsHolder.FontSize, false));
                return;
            }
            start += header.Length;
            var id = uint.Parse(msg.Substring(start));
            var text = SessionManager.DB.SocialDatabase.Social[id].Replace("{Name}", Author);
            AddPiece(new MessagePiece(text, MessagePieceType.Simple, SettingsHolder.FontSize, false));
        }
        private void ParseHtmlMessage(string msg)
        {
            var html = new HtmlDocument(); html.LoadHtml(msg);
            var htmlPieces = html.DocumentNode.ChildNodes;

            foreach (var htmlPiece in htmlPieces)
            {
                ParseHtmlPiece(htmlPiece);
            }
        }

        private void ParseHtmlPiece(HtmlNode piece)
        {
            if (piece.HasAttributes)
            {
                var customColor = ChatUtils.GetCustomColor(piece);
                if (customColor == "")
                {

                }
                if (piece.HasChildNodes && piece.ChildNodes.Count == 1 && piece.ChildNodes[0].Name != "#text")
                {
                    //parse ChatLinkAction
                    var chatLinkAction = piece.ChildNodes.FirstOrDefault(x =>
                        x.Name.IndexOf("ChatLinkAction", StringComparison.InvariantCultureIgnoreCase) != -1);
                    if (chatLinkAction != null)
                    {
                        var mp = MessagePieceBuilder.ParseChatLinkAction(chatLinkAction);
                        mp.SetColor(customColor);
                        AddPiece(mp);
                    }
                    else
                    {
                        piece.ChildNodes.ToList().ForEach(ParseHtmlPiece);
                    }
                }
                else
                {
                    //parse normal formatted piece
                    var text = piece.InnerText;
                    CheckMention(text);
                    CheckRedirect(text);
                    var content = GetPieceContent(text);
                    if (content != "")
                    {
                        AddPiece(
                            new MessagePiece(
                                content
                                .Replace("<a href=\"asfunction:chatLinkAction\">", "")
                                .Replace("</a>", "")
                                .ReplaceHtmlEscapes(),
                            MessagePieceType.Simple, SettingsHolder.FontSize, false, customColor
                            )
                        );
                    }
                }
            }
            else
            {
                //parse normal non formatted piece
                var text = piece.InnerText;
                CheckMention(text);
                CheckRedirect(text);
                var content = GetPieceContent(text);
                if (content != "")
                {
                    AddPiece(
                        new MessagePiece(
                            content
                            .Replace("<a href=\"asfunction:chatLinkAction\">", "")
                            .Replace("</a>", "")
                            .ReplaceHtmlEscapes(),
                        MessagePieceType.Simple, SettingsHolder.FontSize, false
                        )
                    );
                }
            }
        }


        private void CheckMention(string text)
        {
            //check if player is mentioned
            try
            {
                foreach (var item in WindowManager.Dashboard.VM.Characters.Where(c => !c.Hidden))
                {
                    if (text.IndexOf(item.Name, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
                    ContainsPlayerName = true;
                    break;
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
            if ((text.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                 text.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                 text.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                 Channel == ChatChannel.Global) Channel = ChatChannel.TradeRedirect;

        }
        private string GetPieceContent(string text)
        {
            var textToSplit = text.Replace(" ", " [[");
            var split = textToSplit.Split(new[] { "[[" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var content = new StringBuilder("");
            foreach (var token in split)
            {
                var rgxUrl = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
                if (rgxUrl.IsMatch(token)
                    || token.StartsWith("discord.gg")
                    || token.StartsWith("twitch.tv", StringComparison.OrdinalIgnoreCase))
                {
                    //add it as url
                    if (content.ToString() != "")
                    {
                        AddPiece(new MessagePiece(content.ToString().ReplaceHtmlEscapes(), MessagePieceType.Simple, SettingsHolder.FontSize, false));
                        content = new StringBuilder("");
                    }
                    AddPiece(new MessagePiece(token.ReplaceHtmlEscapes(), MessagePieceType.Url, SettingsHolder.FontSize, false, "7289da"));
                }
                else
                {
                    //add it as string
                    content.Append(token);
                }
            }
            return content.ToString();
        }

        // -- Builders ----------------------------------------------------------------
        public static ChatMessage BuildEnchantSystemMessage(string systemMessage)
        {
            var msg = new ChatMessage();
            var mw = "";
            var e = "";

            if (systemMessage.Contains("enchantCount:"))
            {
                var s = systemMessage.IndexOf("enchantCount:", StringComparison.InvariantCultureIgnoreCase);
                var ench = systemMessage.Substring(s + "enchantCount:".Length, 1);
                msg.Channel = ChatChannel.Enchant;
                mw = "";
                e = $"+{ench} ";
            }

            var prm = ChatUtils.SplitDirectives(systemMessage);

            msg.Author = prm["UserName"];
            var txt = "{ItemName}";
            txt = ChatUtils.ReplaceParameters(txt, prm, true);
            txt = txt.Replace("{", "");
            txt = txt.Replace("}", "");
            var mp = MessagePieceBuilder.BuildSysMsgItem(txt);
            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append(e);
            sb.Append(mw);
            sb.Append(mp.Text.Substring(1));
            mp.Text = sb.ToString();
            msg.AddPiece(new MessagePiece("Successfully enchanted ", MessagePieceType.Simple, SettingsHolder.FontSize, false, "cccccc"));
            msg.AddPiece(mp);

            return msg;
        }

        public void Dispose()
        {
            foreach (var messagePiece in Pieces)
            {
                if (messagePiece == null) continue;
                messagePiece.Dispose();
            }
            Pieces.Clear();
        }
    }
    public class LfgMessage : ChatMessage
    {
        private int _tries = 10;
        private Listing _linkedListing;

        public Listing LinkedListing
        {
            get => _linkedListing;
            set
            {
                if (_linkedListing == value) return;
                _linkedListing = value;
                N();
            }
        }

        public uint AuthorId { get; }
        private DispatcherTimer _timer;
        public LfgMessage(uint authorId, string author, string msg) : base(ChatChannel.LFG, author, msg)
        {
            AuthorId = authorId;
        }

        private void GetListing(object sender, EventArgs e)
        {
            if (_tries == 0)
            {
                _timer.Stop();
                return;
            }
            LinkedListing = FindListing();
            if (LinkedListing == null)
            {
                Log.CW("Linked listing is still null!");
                _tries--;
                return;
            }
            _timer.Stop();
            WindowManager.LfgListWindow.VM.EnqueueRequest(LinkedListing.LeaderId);

        }

        private Listing FindListing()
        {
            return WindowManager.LfgListWindow.VM.Listings.FirstOrDefault(x =>
                x.Players.Any(p => p.Name == Author) ||
                x.LeaderName == Author ||
                x.Message == RawMessage);
        }

        public void LinkLfg()
        {
            LinkedListing = FindListing();
            if (LinkedListing != null) return;
            Log.CW("Linked listing is null! Requesting list.");
            WindowManager.LfgListWindow.VM.EnqueueListRequest();
            _timer = new DispatcherTimer(TimeSpan.FromSeconds(1.5), DispatcherPriority.Background, GetListing, Dispatcher);
            _timer.Start();
        }
    }
}
//protected string[] ParseLinkedParameters(string a)
//{
//    var parStart = a.IndexOf("#####", StringComparison.Ordinal) + 5;
//    var parEnd = a.IndexOf('"', parStart);
//    var parString = a.Substring(parStart, parEnd - parStart);

//    return parString.Split('@');
//}
//private string GetCustomColor(string msg)
//{
//var hasSpace = false;
//var colorIndex = msg.IndexOf("COLOR=", StringComparison.InvariantCultureIgnoreCase);
//    if (colorIndex == -1)
//{
//    colorIndex = msg.IndexOf("COLOR =", StringComparison.InvariantCultureIgnoreCase);
//    if (colorIndex != -1) hasSpace = true;
//}
//var offset = hasSpace ? 10 : 8;
//var colorEnd = msg.IndexOf("\"", colorIndex + offset + 1, StringComparison.Ordinal);
//    if (colorEnd == -1) colorEnd = msg.IndexOf("\'", colorIndex + offset + 1, StringComparison.Ordinal);
//    if (colorIndex == -1) return "";
//var col = msg.Substring(colorIndex + offset, colorEnd - colorIndex - offset);
//    while (col.Length < 6)
//{
//    col = "0" + col;
//}
//return col;

//}

//protected void ParseFormattedMessage(string msg)
//{
//    //add missing font tags
//    var pieces = SplitByFontTags(msg);
//    var sb = new StringBuilder();
//    for (var i = 0; i < pieces.Length; i++)
//    {
//        if (!pieces[i].StartsWith("<font", StringComparison.InvariantCultureIgnoreCase))
//        {
//            pieces[i] = $"<font>{pieces[i]}</font>";
//        }

//        sb.Append(pieces[i]);
//    }

//    msg = sb.ToString();

//    var piecesCount = Regex.Matches(msg, CTag, RegexOptions.IgnoreCase).Count;


//    for (var i = 0; i < piecesCount; i++)
//    {
//        try
//        {
//            msg = ParsePiece(msg); //adds piece to list and cuts msg
//        }
//        catch
//        {
//            // ignored
//        }
//    }
//}
//protected string ParsePiece(string msg)
//{
//    var start = msg.IndexOf(OTag, StringComparison.InvariantCultureIgnoreCase) + OTag.Length;
//    if (msg[start] == '>')
//    {
//        //it's not formatted: just take the value and add it to pieces
//        start++;
//        var end = msg.IndexOf(CTag, start, StringComparison.InvariantCultureIgnoreCase);
//        //get the message text
//        var text = msg.Substring(start, end - start);

//        CheckMention(text);
//        CheckRedirect(text);
//        var content = GetPieceContent(text);
//        if (content != "")
//        {
//            AddPiece(new MessagePiece(StringUtils.ReplaceHtmlEscapes(content.Replace("<a href=\"asfunction:chatLinkAction\">", "").Replace("</a>", "")),
//                                        MessagePieceType.Simple, Channel, Settings.Settings.FontSize, false));
//        }

//        //cut message
//        return msg.Substring(end + CTag.Length);
//    }
//    else
//    {
//        //it's formatted: parse then add
//        var customColor = GetCustomColor(msg);
//        var fontSize = GetPieceSize(); //msg);
//        //get link type
//        var linkIndex = msg.IndexOf("#####", StringComparison.Ordinal);
//        if (linkIndex > -1)
//        {
//            var t = msg.Substring(linkIndex - 1, 1);
//            var type = int.Parse(t);

//            var aStart = msg.IndexOf("<ChatLinkAction", StringComparison.Ordinal);
//            var aEnd = msg.IndexOf("</ChatLinkAction>", StringComparison.Ordinal);

//            var a = msg.Substring(aStart, aEnd - aStart + 1);

//            MessagePiece mp;

//            switch (type)
//            {
//                case 1:
//                    mp = ParseItemLink(a);
//                    break;
//                case 2:
//                    mp = ParseQuestLink(a);
//                    break;
//                case 3:
//                    mp = ParseLocationLink(a);
//                    break;
//                default:
//                    throw new Exception();
//            }

//            mp.SetColor(customColor);
//            mp.Size = fontSize;
//            AddPiece(mp);
//        }
//        else
//        {
//            var s = msg.IndexOf(">", StringComparison.Ordinal);
//            var e = msg.IndexOf(CTag, StringComparison.InvariantCultureIgnoreCase);
//            var pc = new MessagePiece(
//                msg.Substring(s + 1, e - s - 1).Replace("<a href=\"asfunction:chatLinkAction\">", "")
//                    .Replace("</a>", ""), MessagePieceType.Simple, Channel, fontSize, true, customColor);
//            AddPiece(pc);
//        }

//        //cut message
//        return msg.Substring(msg.IndexOf(CTag, StringComparison.InvariantCultureIgnoreCase) + CTag.Length);
//    }
//}
//protected MessagePiece ParseItemLink(string a)
//{
//    var linkData = a.Substring(a.IndexOf("#####", StringComparison.Ordinal) - 1);
//    linkData = linkData.Substring(0, linkData.IndexOf(">", StringComparison.Ordinal) - 1);
//    var pars = ParseLinkedParameters(a);
//    var id = uint.Parse(pars[0]);
//    var uid = long.Parse(pars[1]);
//    var owner = "";
//    try { owner = pars[2]; }
//    catch
//    {
//        // ignored
//    }

//    var textStart = a.IndexOf('>') + 1;
//    var textEnd = a.IndexOf('<', textStart);

//    var text = a.Substring(textStart, textEnd - textStart);

//    var result = new MessagePiece(StringUtils.ReplaceHtmlEscapes(text))
//    {
//        ItemId = id,
//        ItemUid = uid,
//        OwnerName = owner,
//        Type = MessagePieceType.Item
//    };
//    result.RawLink = linkData;
//    return result;
//}
//protected MessagePiece ParseQuestLink(string a)
//{
//    var linkData = a.Substring(a.IndexOf("#####", StringComparison.Ordinal) - 1);
//    linkData = linkData.Substring(0, linkData.IndexOf(">", StringComparison.Ordinal) - 1);

//    //parsing only name
//    var textStart = a.IndexOf('>', a.IndexOf("#####", StringComparison.Ordinal)) + 1;
//    var textEnd = a.IndexOf('<', textStart);

//    var text = a.Substring(textStart, textEnd - textStart);
//    text = StringUtils.ReplaceHtmlEscapes(text);

//    var result = new MessagePiece(text)
//    {
//        Type = MessagePieceType.Quest
//    };
//    result.RawLink = linkData;

//    return result;
//}
//protected MessagePiece ParseLocationLink(string a)
//{
//    var linkData = a.Substring(a.IndexOf("#####", StringComparison.Ordinal) - 1);
//    linkData = linkData.Substring(0, linkData.IndexOf(">", StringComparison.Ordinal) - 1);

//    var pars = ParseLinkedParameters(a);
//    var locTree = pars[0].Split('_');
//    var worldId = uint.Parse(locTree[0]);
//    var guardId = uint.Parse(locTree[1]);
//    var sectionId = uint.Parse(locTree[2]);
//    if (worldId == 1 && guardId == 2 && sectionId == 9) sectionId = 7;
//    //var continent = uint.Parse(pars[1]);
//    //continent = continent == 0 && worldId == 1 && guardId == 24 && sectionId == 183001 ? 7031 : continent;
//    var coords = pars[2].Split(',');
//    var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
//    var y = double.Parse(coords[1], CultureInfo.InvariantCulture);
//    //var z = double.Parse(coords[2], CultureInfo.InvariantCulture);

//    //var textStart = a.IndexOf('>', a.IndexOf("#####", StringComparison.Ordinal)) + 1;
//    //var textEnd = a.IndexOf('<', textStart);
//    //var text = a.Substring(textStart, textEnd - textStart); //get actual map name from database
//    //text = ReplaceHtmlEscapes(text);

//    var world = SessionManager.DB.MapDatabase.Worlds[worldId];
//    var guard = world.Guards[guardId];
//    var section = guard.Sections[sectionId];
//    var sb = new StringBuilder();

//    var guardName = guard.NameId != 0 ? SessionManager.DB.MapDatabase.Names[guard.NameId] : "";
//    var sectionName = SessionManager.DB.MapDatabase.Names[section.NameId];
//    //sb.Append(MapDatabase.Names[world.NameId]);
//    sb.Append("<");

//    sb.Append(guardName);
//    if (guardName != sectionName)
//    {
//        if (guardName != "") sb.Append(" - ");
//        sb.Append(sectionName);
//    }
//    sb.Append(">");


//    var result = new MessagePiece(sb.ToString())
//    {
//        Type = MessagePieceType.PointOfInterest,
//        Location = new Location(worldId, guardId, sectionId, x, y),
//        RawLink = linkData
//    };
//    // String.Format("{0}_{1}_{2}@{3}@{4},{5},{6}", worldId, guardId, sectionId, continent == 0 && worldId==1 && guardId ==24 && sectionId==183001? 7031 : continent, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture), z.ToString(CultureInfo.InvariantCulture));
//    return result;
//}
//protected string[] SplitByFontTags(string txt)
//{
////formatted text
//var result = new List<string>();
//    while (true)
//{
//    var s = txt.IndexOf("<font", StringComparison.Ordinal);
//    string x;
//    if (s == 0)
//    {
//        //piece begins with opening tag
//        var e = txt.IndexOf("</font>", s, StringComparison.Ordinal);
//        x = txt.Substring(s, e - s + 7);
//    }
//    else if (s == -1)
//    {
//        //piece doesen't contain opening tag (end of string)
//        x = txt.Substring(0);
//    }
//    else
//    {
//        //opening tag is not at the beginning
//        x = txt.Substring(0, s);
//    }
//    result.Add(x);
//    var regex = new Regex(Regex.Escape(x));
//    txt = regex.Replace(txt, "", 1);

//    if (txt.Length == 0) break;
//}

//return result.ToArray();
//}
