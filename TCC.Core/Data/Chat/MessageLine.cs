using Nostrum;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Chat
{
    public class MessageLine : ThreadSafePropertyChanged
    {
        public ThreadSafeObservableCollection<MessagePieceBase> LinePieces { get; protected set; }
        public MessageLine()
        {
            LinePieces = new ThreadSafeObservableCollection<MessagePieceBase>();
        }
    }
}