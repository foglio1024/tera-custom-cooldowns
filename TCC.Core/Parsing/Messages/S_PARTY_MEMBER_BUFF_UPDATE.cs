using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public struct MemberAbnormal
    {
        public uint Id;
        public uint Duration;
        public int Stacks;
    }

    public struct MemberCondition
    {
        public uint Id;
        public uint Duration;
        public byte Status;
    }
    public class S_PARTY_MEMBER_BUFF_UPDATE : ParsedMessage
    {
        public uint ServerId { get; private set; }
        public uint PlayerId { get; private set; }
        public List<MemberAbnormal> Abnormals { get; private set; }
        public List<MemberCondition> Conditions { get; private set; }

        public S_PARTY_MEMBER_BUFF_UPDATE(TeraMessageReader reader) : base(reader)
        {
            Abnormals = new List<MemberAbnormal>();

            var abnormalsCount = reader.ReadUInt16();
            var abnormalsOffset = reader.ReadUInt16();
            var conditionsCount = reader.ReadUInt16();
            var conditionsOffset = reader.ReadUInt16();

            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();

            for (int i = 0; i < abnormalsCount; i++)
            {
                reader.Skip(4);
                var ab = new MemberAbnormal();
                ab.Id = reader.ReadUInt32();
                ab.Duration = reader.ReadUInt32();
                reader.Skip(4);
                ab.Stacks = reader.ReadInt32();

                Abnormals.Add(ab);
            }
            return;
            for (int i = 0; i < conditionsCount; i++)
            {
                reader.Skip(4);
                var cond = new MemberCondition();
                cond.Id = reader.ReadUInt32();
                cond.Duration = reader.ReadUInt32();
                cond.Status = reader.ReadByte();

                Conditions.Add(cond);
            }

        }
    }
}
