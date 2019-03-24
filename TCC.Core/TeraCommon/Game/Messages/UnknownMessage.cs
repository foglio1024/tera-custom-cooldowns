using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages
{
    // Created when we want a parsed message, but don't know how to handle that OpCode
    public class UnknownMessage : ParsedMessage
    {
        internal UnknownMessage(TeraMessageReader reader)
            : base(reader)
        {
            // [Foglio] better not do this at every UnknownMessage - it could be a good parser for OpcodeSearcher though
            if (Settings.SettingsHolder.CheckGuildBamWithoutOpcode && Parsing.MessageFactory.NoGuildBamOpcode) //by HQ 20190324
            {
                try // by HQ 20181228
                {
                    //if ((reader.Message.Payload.Count >= 46) && (reader.Message.Payload.Count <= 62) && (reader.Message.Direction == MessageDirection.ServerToClient)) // by HQ 20190214
                    if (reader.Message.Payload.Count == 54 && (reader.Message.Direction == MessageDirection.ServerToClient))  // by HQ 20190321
                    {
                        Log.F("GuildQuestUrgent.log", $"\n[{nameof(UnknownMessage)}] opcode : {reader.Message.OpCode}");
                        reader.BaseStream.Position = 14;
                        var unk3 = reader.ReadTeraString();
                        if (!unk3.Contains("@GuildQuest"))
                        {
                            return;
                        }
                        else
                        {
                            TimeManager.Instance.ExecuteGuildBamWebhook();
                            Log.F("GuildQuestUrgent.log", $"\n[{nameof(UnknownMessage)}] S_NOTIFY_GUILD_QUEST_URGENT : {reader.Message.OpCode}, reader.Message.Payload.Count : {reader.Message.Payload.Count}");
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}