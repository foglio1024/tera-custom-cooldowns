using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FoglioUtils;
using FoglioUtils.Extensions;
using TCC.ViewModels;

namespace TCC.Data.Chat
{
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
        public TSObservableCollection<MessagePieceBase> Pieces { get; }

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
            Pieces = new TSObservableCollection<MessagePieceBase>(Dispatcher);
            Lines = new TSObservableCollection<MessageLine>(Dispatcher);
            Timestamp = App.Settings.ChatTimestampSeconds ? DateTime.Now.ToLongTimeString() : DateTime.Now.ToShortTimeString();
            RawMessage = "";
        }
        public ChatMessage(ChatChannel ch) : this()
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

        internal void AddPiece(MessagePieceBase mp)
        {
            mp.Container = this;
            //Dispatcher.InvokeAsyncIfRequired(() =>
            //{
            //}, DispatcherPriority.DataBind);
            Pieces.Add(mp);
        }
        protected void InsertPiece(MessagePieceBase mp, int index)
        {
            mp.Container = this;
            //Dispatcher.InvokeAsyncIfRequired(() =>
            //{
            Pieces.Insert(index, mp);
            //}, DispatcherPriority.DataBind);
        }
        protected void RemovePiece(MessagePieceBase mp)
        {
            //Dispatcher.InvokeAsyncIfRequired(() =>
            //{
            Pieces.Remove(mp);
            //}, DispatcherPriority.DataBind);
        }
        //TODO: refactor
        public void SplitSimplePieces()
        {
            var simplePieces = Pieces.ToSyncList().Where(item => item is SimpleMessagePiece);

            foreach (var simplePiece in simplePieces)
            {
                simplePiece.Text = simplePiece.Text.Replace(" ", " [[");
                var words = simplePiece.Text.Split(new[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);
                var index = Pieces.IndexOf(simplePiece);
                foreach (var word in words)
                {
                    var endsWithK = word.ToLower().EndsWith("k ", StringComparison.InvariantCultureIgnoreCase) ||
                                    word.ToLower().EndsWith("k", StringComparison.InvariantCultureIgnoreCase);
                    var endsWithG = word.ToLower().EndsWith("g ", StringComparison.InvariantCultureIgnoreCase) ||
                                    word.ToLower().EndsWith("g", StringComparison.InvariantCultureIgnoreCase);
                    var isNumber = int.TryParse(word.ToLower().Replace("k ", "").Replace("k", "").Replace("g ", "").Replace("g", ""), out var money);
                    var mp = new MessagePieceBase();
                    if ((endsWithK || endsWithG) && isNumber && (Channel == ChatChannel.Trade ||
                                                                Channel == ChatChannel.TradeRedirect))
                    {
                        mp = new MoneyMessagePiece(new Money(endsWithK ? money * 1000 : money, 0, 0));
                    }
                    else if (simplePiece is ActionMessagePiece amp)
                    {
                        mp = new ActionMessagePiece(word)
                        {
                            ChatLinkAction = amp.ChatLinkAction,
                            Color = amp.Color,
                            Size = amp.Size
                        };
                    }
                    else
                    {
                        mp = new SimpleMessagePiece(word){ Color = simplePiece.Color, Size = simplePiece.Size};
                    }

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
            AddPiece(new SimpleMessagePiece(msg, App.Settings.FontSize, false));
        }
        private void ParseEmoteMessage(string msg)
        {
            const string header = "@social:";
            var start = msg.IndexOf(header, StringComparison.Ordinal);
            if (start == -1)
            {
                AddPiece(new SimpleMessagePiece(Author + " " + msg, App.Settings.FontSize, false));
                return;
            }
            start += header.Length;
            var id = uint.Parse(msg.Substring(start));
            var text = Game.DB.SocialDatabase.Social[id].Replace("{Name}", Author);
            AddPiece(new SimpleMessagePiece(text, App.Settings.FontSize, false));
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

                if (piece.HasChildNodes && piece.ChildNodes.Count == 1 && piece.ChildNodes[0].Name != "#text")
                {
                    //parse ChatLinkAction
                    var chatLinkAction = piece.ChildNodes.FirstOrDefault(x =>
                        x.Name.IndexOf("ChatLinkAction", StringComparison.InvariantCultureIgnoreCase) != -1);
                    if (chatLinkAction != null)
                    {
                        var mp = MessagePieceBuilder.ParseChatLinkAction(chatLinkAction);
                        mp.Color = customColor;
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
                    if (!App.Loading) ContainsPlayerName = ChatUtils.CheckMention(text);
                    CheckRedirect(text);
                    var content = GetPieceContent(text);
                    if (content != "")
                    {
                        AddPiece(
                            new SimpleMessagePiece(
                                content
                                .Replace("<a href=\"asfunction:chatLinkAction\">", "")
                                .Replace("</a>", "")
                                .UnescapeHtml(),App.Settings.FontSize, false, customColor
                            )
                        );
                    }
                }
            }
            else
            {
                //parse normal non formatted piece
                var text = piece.InnerText;
                if (!App.Loading) ContainsPlayerName = ChatUtils.CheckMention(text);
                CheckRedirect(text);
                var content = GetPieceContent(text);
                if (content != "")
                {
                    var p = new SimpleMessagePiece(
                        content
                            .Replace("<a href=\"asfunction:chatLinkAction\">", "")
                            .Replace("</a>", "")
                            .UnescapeHtml(), App.Settings.FontSize, false
                    );
                    if (Channel == ChatChannel.ReceivedWhisper) p.Color = R.Brushes.GoldBrush.Color.ToHex();
                    AddPiece(p);
                }
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
                        AddPiece(new SimpleMessagePiece(content.ToString().UnescapeHtml(), App.Settings.FontSize, false));
                        content = new StringBuilder("");
                    }
                    AddPiece(new UrlMessagePiece(token.UnescapeHtml(), App.Settings.FontSize, false, "7289da"));
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