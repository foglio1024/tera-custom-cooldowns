namespace TCC.Data.Chat
{
    public struct SystemMessage
    {
        public string Message { get; }
        public int ChatChannel { get; }

        public SystemMessage(string s, int ch)
        {
            Message = s;
            ChatChannel = ch;
        }

    }
}
