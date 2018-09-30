using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PLAYER_CHANGE_STAMINA : ParsedMessage
    {
        private int currentStamina, maxStamina, unk1, unk2, unk3;

        public int CurrentST => currentStamina;
        public int MaxST => maxStamina;

        public S_PLAYER_CHANGE_STAMINA(TeraMessageReader reader) : base(reader)
        {
            currentStamina = reader.ReadInt32();
            maxStamina = reader.ReadInt32();
            reader.Skip(12);
            //unk1 = reader.ReadInt32();
            //unk2 = reader.ReadInt32();
            //unk3 = reader.ReadInt32();
        }
    }
}