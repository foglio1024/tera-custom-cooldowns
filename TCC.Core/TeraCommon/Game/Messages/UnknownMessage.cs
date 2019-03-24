using TCC.Parsing;
using TCC.Parsing.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages
{
    // Created when we want a parsed message, but don't know how to handle that OpCode
    public class UnknownMessage : ParsedMessage
    {
        internal UnknownMessage(TeraMessageReader reader) : base(reader)
        {
            // [Foglio] better not do this at every UnknownMessage - it could be a good parser for OpcodeSearcher though
            // added as a setting (needed only for KR)
            if (!Settings.SettingsHolder.CheckGuildBamWithoutOpcode || !MessageFactory.NoGuildBamOpcode) return;
            if (reader.Message.Direction != MessageDirection.ServerToClient || reader.Message.Payload.Count != 54) return;
            try // by HQ 20181228
            {
                //Log.F("GuildQuestUrgent.log", $"\n[{nameof(UnknownMessage)}] opcode : {reader.Message.OpCode}");
                reader.BaseStream.Position = 14;
                var unk3 = reader.ReadTeraString();
                if (!unk3.Contains("@GuildQuest")) return;

                var msg = new S_NOTIFY_GUILD_QUEST_URGENT(reader);
                PacketHandler.HandleNotifyGuildQuestUrgent(msg);
                //TimeManager.Instance.ExecuteGuildBamWebhook();
                Log.F("GuildQuestUrgent.log", $"\n[{nameof(UnknownMessage)}] S_NOTIFY_GUILD_QUEST_URGENT : {reader.Message.OpCode}, reader.Message.Payload.Count : {reader.Message.Payload.Count}");
            }
            catch
            {

            }
        }
    }
}