using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public struct MemberAbnormal
    {
        public uint Id;
        public uint Duration;
        public int Stacks;
    }

    /*
        public struct MemberCondition
        {
            public uint Id;
            public uint Duration;
            public byte Status;
        }
    */
    public class S_PARTY_MEMBER_BUFF_UPDATE : ParsedMessage
    {
        public bool Failed { get; }
        public uint ServerId { get; private set; }
        public uint PlayerId { get; private set; }
        public List<MemberAbnormal> Abnormals { get; private set; }
        //public List<MemberCondition> Conditions { get; private set; }

        public S_PARTY_MEMBER_BUFF_UPDATE(TeraMessageReader reader) : base(reader)
        {
            Abnormals = new List<MemberAbnormal>();

            try
            {
                var abnormalsCount = reader.ReadUInt16();
                reader.Skip(2); // var abnormalsOffset = reader.ReadUInt16();
                reader.Skip(2); // var conditionsCount = reader.ReadUInt16();
                reader.Skip(2); // var conditionsOffset = reader.ReadUInt16();

                ServerId = reader.ReadUInt32();
                PlayerId = reader.ReadUInt32();

                for (var i = 0; i < abnormalsCount; i++)
                {
                    reader.Skip(4);
                    var ab = new MemberAbnormal
                    {
                        Id = reader.ReadUInt32(),
                        Duration = reader.ReadUInt32()
                    };
                    reader.Skip(4);
                    ab.Stacks = reader.ReadInt32();

                    Abnormals.Add(ab);
                }
                /*
                            for (var i = 0; i < conditionsCount; i++)
                            {
                                reader.Skip(4);
                                var cond = new MemberCondition();
                                cond.Id = reader.ReadUInt32();
                                cond.Duration = reader.ReadUInt32();
                                cond.Status = reader.ReadByte();

                                Conditions.Add(cond);
                            }
                */


            }
            catch
            {
                Failed = true; // not used for now
            }
        }
    }
}
