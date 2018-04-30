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
        public Dictionary<uint, Item> Items;
        public Dictionary<uint, Dictionary<int, int>> ExpData;
        public ItemsDatabase(string lang = "EU-EN")
        {
            var f = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/resources/data/items/items-" + lang + ".tsv");
            Items = new Dictionary<uint, Item>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var id = Convert.ToUInt32(s[0]);
                var grad = Convert.ToUInt32(s[1]);
                var name = s[2];
                var expId = Convert.ToUInt32(s[3]);
                var cd = Convert.ToUInt32(s[4]);
                var icon = s[5];

                var item = new Item(id, name, grad, expId, cd, icon);
                Items.Add(id, item);
            }
            var xpFile = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + $"/resources/data/equip_exp/equip_exp-{lang}.xml");
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
        }
        public int GetMaxExp(uint id, int enchant)
        {
            if (!Items.ContainsKey(id)) return 0;
            if (Items[id].ExpId == 0) return 0;
            return ExpData[Items[id].ExpId][enchant];
        }
        public string GetItemName(uint id)
        {
            return Items.ContainsKey(id) ? Items[id].Name : "Unknown";
        }
        public bool TryGetItemSkill(uint itemId, out Skill sk)
        {
            var result = false;
            sk = new Skill(0, Class.None, string.Empty, string.Empty);

            if (Items.ContainsKey(itemId))
            {
                var item = Items[itemId];
                result = true;
                sk = new Skill(itemId, Class.Common, item.Name, "");
                sk.SetSkillIcon(item.Icon);
            }
            return result;

        }

    }

    public class Item
    {
        public uint Id { get; private set; }
        public uint ExpId { get; private set; }
        public string Name { get; private set; }
        public RareGrade RareGrade { get; private set; }
        public uint Cooldown { get; private set; }
        public string Icon { get; private set; }
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
