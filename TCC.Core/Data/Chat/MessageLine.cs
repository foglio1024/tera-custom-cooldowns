using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Chat;

public class MessageLine : ThreadSafeObservableObject
{
    public ThreadSafeObservableCollection<MessagePieceBase> LinePieces { get; } = new();
}