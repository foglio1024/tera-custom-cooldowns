using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCC.Data.Skills;

namespace TCC.Data.Databases
{
    public class ItemsDatabase : DatabaseBase
    {
        protected override string FolderName => "items";
        protected override string Extension => "tsv";

        public Dictionary<uint, Item> Items;
        public ItemsDatabase(string lang) : base(lang)
        {
            Items = new Dictionary<uint, Item>();
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
                sk = new Skill(itemId, Class.Common, item.Name, "") {IconName = item.IconName};
            }
            return result;

        }

        public override void Load()
        {
            Items.Clear();
            var f = File.OpenText(FullPath);
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

            // Manual fix for bugged EU HP pot from HH
            var euBuggedReju = new Item(149644, "Harrowhold Rejuvenation Potion", 1, 0, 30, "icon_items.potion1_tex");
            if (!Items.ContainsKey(euBuggedReju.Id)) Items.Add(euBuggedReju.Id, euBuggedReju);

        }

        public IEnumerable<Item> ItemSkills
        {
            get
            {
                //var ret = new List<Item>();
                //foreach (var item in Items.Values)
                //{
                //    var iconName = "unknown";
                //    if (item.IconName.ToString() != "")
                //    {
                //        iconName = item.IconName.ToString();
                //        iconName = iconName.Replace(".", "/");
                //    }
                //    if (File.Exists(Path.GetDirectoryName(typeof(App).Assembly.Location)+ "/resources/images/" + iconName+ ".png")) ret.Add(item);
                //}

                //return ret;
                return Items.Values.Where(x => x.Cooldown > 0);
            }
        }
    }
}
