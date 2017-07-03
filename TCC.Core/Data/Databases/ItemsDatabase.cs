using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data.Databases
{
    public static class ItemsDatabase
    {
        public static Dictionary<uint, Item> Items;
        public static void Load()
        {
            var f = File.OpenText(Environment.CurrentDirectory + "/resources/data/items.tsv");
            Items = new Dictionary<uint, Item>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var id = Convert.ToUInt32(s[0]);
                var grad = Convert.ToUInt32(s[1]);
                var bound = s[2];
                var name = s[3];

                //lazy raw overrides because BHS can't do stuff right .-.

                if (bound == "EquipToItem") bound = "Equip";
                if (bound == "none") bound = "None";
                if (bound == "NONE") bound = "None";
                if (bound == "EQUIP") bound = "Equip";

                var item = new Item(id, name, bound, grad);
                Items.Add(id, item);
            }
            Debug.WriteLine("Loaded {0} items.", Items.Count);
        }
    }

    public class Item
    {
        public uint Id { get; private set; }
        public string Name { get; private set; }
        public BoundType BoundType { get; private set; }
        public RareGrade RareGrade { get; private set; }
        public Item(uint id, string name, string b, uint g)
        {
            Id = id;
            Name = name;
            BoundType = (BoundType)Enum.Parse(typeof(BoundType), b);
            RareGrade = (RareGrade)g;
        }
    }

}
