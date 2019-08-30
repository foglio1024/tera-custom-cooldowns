using System.Windows;
using Newtonsoft.Json;

namespace TCC.Data.Map
{
    public class Location
    {
        public uint World { get; }
        public uint Guard { get; }
        public uint Section { get; }

        [JsonIgnore]
        public string SectionName => Game.DB.GetSectionName(Guard, Section);

        [JsonIgnore]
        public Point Position { get; }

        public Location()
        {
            Position = new Point(0,0);

        }
        public Location(uint w, uint g, uint s, double x, double y)
        {
            World = w;
            Guard = g;
            Section = s;
            Position = new Point(x, y);
        }
        public Location(uint w, uint g, uint s)
        {
            World = w;
            Guard = g;
            Section = s;
            Position = new Point(0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">WorldId_GuardId_SectionId</param>
        public Location(string value)
        {
            var split = value.Split('_');
            World = uint.Parse(split[0]);
            Guard = uint.Parse(split[1]);
            Section = uint.Parse(split[2]);
        }
    }
}
