using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    class S_LOGIN : ParsedMessage
    {
        public short nameOffset, detailsOffset, detailsCount, details2Offset, details2Count;
        public ulong entityId;
        uint model;
        
        public Class CharacterClass { get
            {
                int classId = (int)(model - 10101) % 100;
                return (Class)classId;
            }
        }

        public S_LOGIN(TeraMessageReader reader) : base(reader)
        {
            nameOffset = reader.ReadInt16();
            detailsOffset = reader.ReadInt16();
            detailsCount = reader.ReadInt16();
            details2Offset = reader.ReadInt16();
            details2Count = reader.ReadInt16();
            model = reader.ReadUInt32();
            entityId = reader.ReadUInt64();
            //Console.WriteLine(reader.ReadInt64());
            //Console.WriteLine(reader.ReadInt32());//1
            //Console.WriteLine(reader.ReadByte());//2
            //Console.WriteLine(reader.ReadInt32());//3
            //Console.WriteLine(reader.ReadInt32());//4
            //Console.WriteLine(reader.ReadInt32());//5
            //Console.WriteLine(reader.ReadInt64());
            //Console.WriteLine(reader.ReadInt16());//6
            //Console.WriteLine(reader.ReadInt16());//level
            //Console.WriteLine(reader.ReadInt16());
            //Console.WriteLine(reader.ReadInt16());
            //Console.WriteLine(reader.ReadInt16());
            //Console.WriteLine(reader.ReadInt16());
            //Console.WriteLine(reader.ReadInt32());//7
            //Console.WriteLine(reader.ReadInt32());//8
            //Console.WriteLine(reader.ReadInt16());//9
            //Console.WriteLine(reader.ReadInt64());
            //Console.WriteLine(reader.ReadInt64());
            //Console.WriteLine(reader.ReadInt64());
            //Console.WriteLine(reader.ReadInt32());//10
            //Console.WriteLine(reader.ReadInt32());//10b
            //Console.WriteLine(reader.ReadInt32());//10c
            //Console.WriteLine(reader.ReadInt32());//10d
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadSingle());//11
            //Console.WriteLine(reader.ReadInt32());//12
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());//13
            //Console.WriteLine(reader.ReadInt32());//14
            //Console.WriteLine(reader.ReadByte());//15
            //Console.WriteLine(reader.ReadInt32());//16
            //Console.WriteLine(reader.ReadInt32());//17
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());//19
            //Console.WriteLine(reader.ReadInt32());//20
            //Console.WriteLine(reader.ReadInt32());//21
            //Console.WriteLine(reader.ReadInt32());//22
            //Console.WriteLine(reader.ReadInt32());//23
            //Console.WriteLine(reader.ReadInt32());//24
            //Console.WriteLine(reader.ReadInt32());//25
            //Console.WriteLine(reader.ReadInt32());//26
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());//27
            //Console.WriteLine(reader.ReadByte());//28
            //Console.WriteLine(reader.ReadByte());//29
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            //Console.WriteLine(reader.ReadInt32());
            ////Console.WriteLine(reader.ReadInt32());//30
            ////Console.WriteLine(reader.ReadInt32());//31
            ////Console.WriteLine(reader.ReadInt32());//32
            ////Console.WriteLine(reader.ReadInt32());//33
            //Console.WriteLine(reader.ReadByte());//34
            //Console.WriteLine(reader.ReadInt32());//35
            //Console.WriteLine(reader.ReadInt32());//36
            //Console.WriteLine(reader.ReadInt32());//37
            //Console.WriteLine(reader.ReadSingle());//38
            //Console.WriteLine(reader.ReadInt32());//39
            //Console.WriteLine(reader.ReadByte());//40
            //Console.WriteLine(reader.ReadInt32());//41
            //Console.WriteLine(reader.ReadTeraString());
            //for (int i = 0; i < detailsCount; i++)
            //{
            //    Console.Write(reader.ReadByte()+" ");
            //}
            //for (int i = 0; i < details2Count; i++)
            //{
            //    Console.Write(reader.ReadByte()+" ");
            //}


        }
    }
}
