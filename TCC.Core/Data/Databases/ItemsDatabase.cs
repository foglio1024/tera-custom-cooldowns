using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public class ItemsDatabase
    {
        private static ItemsDatabase _instance;
        public static ItemsDatabase Instance => _instance ?? (_instance = new ItemsDatabase());
        public Dictionary<uint, Item> Items;
        public Dictionary<uint, Dictionary<int, int>> ExpData;
        public ItemsDatabase(string lang = "EU-EN")
        {
            var f = File.OpenText(Environment.CurrentDirectory + "/resources/data/items/items-" + lang + ".tsv");
            Items = new Dictionary<uint, Item>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var id = Convert.ToUInt32(s[0]);
                var grad = Convert.ToUInt32(s[1]);
                //var bound = s[2];
                var name = s[2];
                var expId = Convert.ToUInt32(s[3]);
                var cd = Convert.ToUInt32(s[4]);
                var icon = s[5];
                //lazy raw overrides because BHS can't do stuff right .-.

                //if (bound == "EquipToItem") bound = "Equip";
                //if (bound == "none") bound = "None";
                //if (bound == "NONE") bound = "None";
                //if (bound == "EQUIP") bound = "Equip";

                var item = new Item(id, name, grad, expId, cd, icon);
                Items.Add(id, item);
            }
            var xpFile = XDocument.Load(Environment.CurrentDirectory + "/resources/data/EquipmentExpData.xml");
            ExpData = new Dictionary<uint, Dictionary<int, int>>();
            foreach (var xElement in xpFile.Descendants().Where(x => x.Name == "EquipmentExp"))
            {
                var id = Convert.ToUInt32(xElement.Attribute("id").Value);
                var d = new Dictionary<int, int>();
                foreach (var element in xElement.Descendants().Where(x => x.Name == "Exp"))
                {
                    var step = Convert.ToInt32(element.Attribute("enchantStep").Value);
                    var max = Convert.ToInt32(element.Attribute("maxExp").Value);
                    d.Add(step, max);
                }
                ExpData.Add(id, d);
            }
            Debug.WriteLine("Loaded {0} items.", Items.Count);

        }

        public int GetMaxExp(uint id, int enchant)
        {
            if (Items[id].ExpId == 0) return 0;
            return ExpData[Items[id].ExpId][enchant];
        }
        public string GetItemName(uint id)
        {
            return Items.ContainsKey(id) ? Items[id].Name : "Unknown";
        }

        public static void Reload(string lang)
        {
            _instance = new ItemsDatabase(lang);
        }
    }

    public class Item
    {
        public uint Id { get; private set; }
        public uint ExpId { get; private set; }
        public string Name { get; private set; }
        public RareGrade RareGrade { get; private set; }
        public  uint Cooldown { get; private set; }
        public  string Icon { get; private set; }
        public Item(uint id, string name, uint g, uint expId, uint cd, string icon)
        {
            Id = id;
            Name = name;
            RareGrade = (RareGrade)g;
            ExpId = expId;
            Cooldown = cd;
            Icon = icon;
        }
    }

}
