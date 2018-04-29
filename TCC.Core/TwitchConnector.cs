using TCC.ViewModels;
using TwitchLib;
using TwitchLib.Models.Client;

namespace TCC
{
    public class TwitchConnector
    {
        private static TwitchConnector _instance;
        public static TwitchConnector Instance => _instance ?? (_instance = new TwitchConnector());

        private TwitchClient client;

        private ConnectionCredentials cred;


        private void Client_OnMessageReceived(object sender, TwitchLib.Events.Client.OnMessageReceivedArgs e)
        {
            var msg = new TCC.Data.ChatMessage(ChatChannel.Twitch, e.ChatMessage.Username, "<FONT>" + e.ChatMessage.Message + "</FONT>");
            ChatWindowManager.Instance.AddChatMessage(msg);
        }

        public void Init()
        {
            if (string.IsNullOrEmpty(SettingsManager.TwitchToken) ||
               string.IsNullOrEmpty(SettingsManager.TwitchName) ||
               string.IsNullOrEmpty(SettingsManager.TwitchChannelName)) return;
            if (SettingsManager.TwitchName.Length < 4) return;
            cred = new ConnectionCredentials(SettingsManager.TwitchName, SettingsManager.TwitchToken);
            client = new TwitchClient(cred, SettingsManager.TwitchChannelName);
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnDisconnected += Client_OnDisconnected;
            client.Connect();
        }

        private void Client_OnDisconnected(object sender, TwitchLib.Events.Client.OnDisconnectedArgs e)
        {
            try
            {
                client.Connect();
            }
            catch 
            {

            }
        }
    }
}
