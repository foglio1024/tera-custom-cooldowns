using FoglioUtils;

namespace TCC.Data.Chat
{
    public class MessageLine : TSPropertyChanged
    {
        public TSObservableCollection<MessagePiece> LinePieces { get; protected set; }
        public MessageLine()
        {
            LinePieces = new TSObservableCollection<MessagePiece>();
        }
    }
}