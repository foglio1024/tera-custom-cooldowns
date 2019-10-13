using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Chat;

namespace TCC.ViewModels
{
    public class ChatMessageFactory
    {
        private readonly Dispatcher _dispatcher;
        public ChatMessageFactory(Dispatcher d)
        {
            _dispatcher = d;
        }

        public ChatMessage CreateMessage()
        {
            return _dispatcher.InvokeAsync(() => new ChatMessage()).Result;
        }
        public ChatMessage CreateMessage(ChatChannel ch, string author, string msg)
        {
            return _dispatcher.InvokeAsync(() => new ChatMessage(ch, author, msg)).Result;
        }
        public ChatMessage CreateSystemMessage(string template, SystemMessageData msg, ChatChannel ch, string authorOverride = "System")
        {
            return _dispatcher.InvokeAsync(() => new SystemMessage(template, msg, ch) { Author = authorOverride }).Result;
        }
        public ChatMessage CreateLfgMessage(uint authorId, string author, string msg)
        {
            return _dispatcher.InvokeAsync(() => new LfgMessage(authorId, author, msg)).Result;
        }
    }
}
