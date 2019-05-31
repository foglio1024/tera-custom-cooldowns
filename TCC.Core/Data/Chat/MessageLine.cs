namespace TCC.Data.Chat
{
    public class MessageLine : TSPropertyChanged
    {
        public SynchronizedObservableCollection<MessagePiece> LinePieces { get; protected set; }
        public MessageLine()
        {
            LinePieces = new SynchronizedObservableCollection<MessagePiece>();
        }
    }
}