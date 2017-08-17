using Tera.Game;
using Tera.Game.Messages;

namespace TCC
{
    public class S_PLAYER_CHANGE_STAMINA : ParsedMessage
    {
        int currentStamina, maxStamina, unk1, unk2, unk3;

        public int CurrentST { get => currentStamina; }
        public int MaxST { get => maxStamina; }

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