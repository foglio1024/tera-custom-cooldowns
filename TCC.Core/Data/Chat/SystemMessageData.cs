namespace TCC.Data.Chat
{
    public struct SystemMessageData
    {
        public string Template { get; }
        public int ChatChannel { get; }

        public SystemMessageData(string s, int ch)
        {
            Template = s;
            ChatChannel = ch;
        }

    }
}
