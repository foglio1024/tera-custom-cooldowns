using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_ASK_BIDDING_RARE_ITEM : ParsedMessage
    {

        public S_ASK_BIDDING_RARE_ITEM(TeraMessageReader reader) : base(reader)
        {
            //needed only to setup the rolling phase, so not actually parsed
        }
    }
}
