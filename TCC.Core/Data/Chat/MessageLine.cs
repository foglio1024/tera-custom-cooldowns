using FoglioUtils;

namespace TCC.Data.Chat
{
    public class MessageLine : TSPropertyChanged
    {
        public TSObservableCollection<MessagePieceBase> LinePieces { get; protected set; }
        public MessageLine()
        {
            LinePieces = new TSObservableCollection<MessagePieceBase>();
        }
    }
}