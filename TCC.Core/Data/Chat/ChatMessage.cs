using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Annotations;
using FoglioUtils.Extensions;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;

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
                        AddPiece(new MessagePiece(content, MessagePieceType.Simple, App.Settings.FontSize, false, customColor));
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
                var mp = new MessagePiece(source, MessagePieceType.Icon, App.Settings.FontSize, false);
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
                        if (Game.DB.AbnormalityDatabase.Abnormalities.TryGetValue(
                            uint.Parse(inPiece.Split(':')[1]), out var ab)) abName = ab.Name;
                        mp = new MessagePiece(abName, MessagePieceType.Simple, App.Settings.FontSize, false);
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
                    else if (inPiece.StartsWith("@rgn"))
                    {

                        mp = MessagePieceBuilder.BuildSysMsgRegion(inPiece);

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
                        mp = new MessagePiece(inPiece.UnescapeHtml(), MessagePieceType.Simple, App.Settings.FontSize, false, col);
                    }
                    AddPiece(mp);
                }
            }
        }

    }
    public class ChatMessage : TSPropertyChanged, IDisposable
    {
        #region Properties

        private bool _animate = true;
        private bool _isVisible;

        public ChatChannel Channel { get; protected set; }

        public string Timestamp { get; protected set; }

        public string RawMessage { get; protected set; }

        public string Author { get; set; }

        public bool ContainsPlayerName { get; set; }
        public bool Animate
        {
            get => _animate && App.Settings.AnimateChatMessages;
            set => _animate = value;
        }
        public bool ShowTimestamp => App.Settings.ShowTimestamp;
        public bool ShowChannel => App.Settings.ShowChannel;
        public TSObservableCollection<MessageLine> Lines { get; protected set; }
        public TSObservableCollection<MessagePiece> Pieces { get; }

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
        public int Size => App.Settings.FontSize;
        #endregion

        public ChatMessage()
        {
            Dispatcher = ChatWindowManager.Instance.GetDispatcher();
            Pieces = new TSObservableCollection<MessagePiece>(Dispatcher);
            Lines = new TSObservableCollection<MessageLine>(Dispatcher);
            Timestamp = App.Settings.ChatTimestampSeconds ? DateTime.Now.ToLongTimeString() : DateTime.Now.ToShortTimeString();
            RawMessage = "";
        }
        public ChatMessage(ChatChannel ch) :this()
        {
            Channel = ch;
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
                if (Channel == ChatChannel.Raid && WindowManager.ViewModels.GroupVM.IsLeader(Author)) Channel = ChatChannel.RaidLeader;
                switch (ch)
                {
                    case ChatChannel.Greet:
                    case ChatChannel.Angler:
                        ParseDirectMessage(RawMessage.UnescapeHtml());
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

        internal void AddPiece(MessagePiece mp)
        {
            mp.Container = this;
            //Dispatcher.InvokeAsyncIfRequired(() =>
            //{
            //}, DispatcherPriority.DataBind);
            Pieces.Add(mp);
        }
        protected void InsertPiece(MessagePiece mp, int index)
        {
            mp.Container = this;
            //Dispatcher.InvokeAsyncIfRequired(() =>
            //{
            Pieces.Insert(index, mp);
            //}, DispatcherPriority.DataBind);
        }
        protected void RemovePiece(MessagePiece mp)
        {
            //Dispatcher.InvokeAsyncIfRequired(() =>
            //{
                Pieces.Remove(mp);
            //}, DispatcherPriority.DataBind);
        }
        //TODO: refactor
        public void SplitSimplePieces()
        {
            var simplePieces = Pieces.ToSyncList().Where(item => item.Type == MessagePieceType.Simple
                                                              || item.Type == MessagePieceType.Item).ToList();

            foreach (var simplePiece in simplePieces)
            {
                simplePiece.Text = simplePiece.Text.Replace(" ", " [[");
                var tokens = simplePiece.Text.Split(new[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);
                var index = Pieces.IndexOf(simplePiece);
                foreach (var token in tokens)
                {
                    var endsWithK = token.ToLower().EndsWith("k ", StringComparison.InvariantCultureIgnoreCase) ||
                                    token.ToLower().EndsWith("k", StringComparison.InvariantCultureIgnoreCase);
                    var endsWithG = token.ToLower().EndsWith("g ", StringComparison.InvariantCultureIgnoreCase) ||
                                    token.ToLower().EndsWith("g", StringComparison.InvariantCultureIgnoreCase);
                    var isNumber = int.TryParse(token.ToLower().Replace("k ", "").Replace("k", "").Replace("g ", "").Replace("g", ""), out var money);


                    var mp = (endsWithK || endsWithG) && isNumber && (Channel == ChatChannel.Trade ||
                                                                      Channel == ChatChannel.TradeRedirect ||
                                                                      Channel == ChatChannel.Megaphone ||
                                                                      Channel == ChatChannel.Global) ?
                        new MessagePiece(new Money(endsWithK ? money * 1000 : money, 0, 0))
                        //: 
                        //isEmoji?
                        //new MessagePiece(split[j]) { Type = MessagePieceType.Emoji} 
                        :
                        new MessagePiece(token)
                        {
                            Color = simplePiece.Color,
                            Type = simplePiece.Type,
                            ItemId = simplePiece.ItemId,
                            ItemUid = simplePiece.ItemUid,
                            BoundType = simplePiece.BoundType,
                            OwnerName = simplePiece.OwnerName,
                            RawLink = simplePiece.RawLink,
                            Size = simplePiece.Size
                        };
                    InsertPiece(mp, index);
                    index = Pieces.IndexOf(mp) + 1;
                }
                RemovePiece(simplePiece);
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
                Lines.ToSyncList().Last().LinePieces.Add(item);
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
            AddPiece(new MessagePiece(msg, MessagePieceType.Simple, App.Settings.FontSize, false));
        }
        private void ParseEmoteMessage(string msg)
        {
            const string header = "@social:";
            var start = msg.IndexOf(header, StringComparison.Ordinal);
            if (start == -1)
            {
                AddPiece(new MessagePiece(Author + " " + msg, MessagePieceType.Simple, App.Settings.FontSize, false));
                return;
            }
            start += header.Length;
            var id = uint.Parse(msg.Substring(start));
            var text = Game.DB.SocialDatabase.Social[id].Replace("{Name}", Author);
            AddPiece(new MessagePiece(text, MessagePieceType.Simple, App.Settings.FontSize, false));
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
                    if (!App.Loading) CheckMention(text);
                    CheckRedirect(text);
                    var content = GetPieceContent(text);
                    if (content != "")
                    {
                        AddPiece(
                            new MessagePiece(
                                content
                                .Replace("<a href=\"asfunction:chatLinkAction\">", "")
                                .Replace("</a>", "")
                                .UnescapeHtml(),
                            MessagePieceType.Simple, App.Settings.FontSize, false, customColor
                            )
                        );
                    }
                }
            }
            else
            {
                //parse normal non formatted piece
                var text = piece.InnerText;
                if (!App.Loading) CheckMention(text);
                CheckRedirect(text);
                var content = GetPieceContent(text);
                if (content != "")
                {
                    AddPiece(
                        new MessagePiece(
                            content
                            .Replace("<a href=\"asfunction:chatLinkAction\">", "")
                            .Replace("</a>", "")
                            .UnescapeHtml(),
                        MessagePieceType.Simple, App.Settings.FontSize, false
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
                foreach (var item in Game.Account.Characters.Where(c => !c.Hidden))
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
                        AddPiece(new MessagePiece(content.ToString().UnescapeHtml(), MessagePieceType.Simple, App.Settings.FontSize, false));
                        content = new StringBuilder("");
                    }
                    AddPiece(new MessagePiece(token.UnescapeHtml(), MessagePieceType.Url, App.Settings.FontSize, false, "7289da"));
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
        public void Dispose()
        {
            SettingsWindowViewModel.ChatShowChannelChanged -= ShowChannelNPC;
            SettingsWindowViewModel.ChatShowTimestampChanged -= ShowTimestampNPC;
            SettingsWindowViewModel.FontSizeChanged -= FontSizeNPC;

            foreach (var messagePiece in Pieces)
            {
                messagePiece?.Dispose();
            }
            Pieces.Clear();
        }
    }
}