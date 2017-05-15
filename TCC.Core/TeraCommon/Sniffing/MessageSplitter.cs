using System;
using Tera.PacketLog;

namespace Tera.Sniffing
{
    public class MessageSplitter
    {
        private readonly BlockSplitter _clientSplitter = new BlockSplitter();
        private readonly BlockSplitter _serverSplitter = new BlockSplitter();
        private DateTime _time;

        public MessageSplitter()
        {
            _clientSplitter.BlockFinished += ClientBlockFinished;
            _serverSplitter.BlockFinished += ServerBlockFinished;
            _clientSplitter.Resync += ClientResync;
            _serverSplitter.Resync += ServerResync;
        }

        public event Action<Message> MessageReceived;
        public event Action<MessageDirection, int, int> Resync;

        private void ClientResync(int skipped, int size)
        {
            OnResync(MessageDirection.ClientToServer, skipped, size);
        }

        private void ServerResync(int skipped, int size)
        {
            OnResync(MessageDirection.ServerToClient, skipped, size);
        }

        protected void OnResync(MessageDirection direction, int skipped, int size)
        {
            var handler = Resync;
            handler?.Invoke(direction,skipped,size);
        }

        private void ClientBlockFinished(byte[] block)
        {
            OnMessageReceived(new Message(_time, MessageDirection.ClientToServer, new ArraySegment<byte>(block)));
        }

        private void ServerBlockFinished(byte[] block)
        {
            OnMessageReceived(new Message(_time, MessageDirection.ServerToClient, new ArraySegment<byte>(block)));
        }

        protected void OnMessageReceived(Message message)
        {
            var handler = MessageReceived;
            handler?.Invoke(message);
        }

        public void ClientToServer(DateTime time, byte[] data)
        {
            _time = time;
            _clientSplitter.Data(data);
            _clientSplitter.PopAllBlocks();
        }

        public void ServerToClient(DateTime time, byte[] data)
        {
            _time = time;
            _serverSplitter.Data(data);
            _serverSplitter.PopAllBlocks();
        }
    }
}