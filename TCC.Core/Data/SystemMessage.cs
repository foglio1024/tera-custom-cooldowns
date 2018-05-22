namespace TCC.Data
{
    public struct SystemMessage
    {
        public readonly string Message;
        public readonly int ChatChannel;

        public SystemMessage(string s, int ch)
        {
            Message = s;
            ChatChannel = ch;
        }

    }
}
