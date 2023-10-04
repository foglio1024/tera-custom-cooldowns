using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Databases;

public class ItemExpDatabase : DatabaseBase
{
    protected override string FolderName => "equip_exp";
    protected override string Extension => "xml";

    public Dictionary<uint, Dictionary<int, int>> ExpData;

    public ItemExpDatabase(string lang) : base(lang)
    {
        ExpData = new Dictionary<uint, Dictionary<int, int>>();

    }

    public override void Load()
    {
        ExpData.Clear();
        var xpFile = XDocument.Load(FullPath);
        foreach (var xElement in xpFile.Descendants().Where(x => x.Name == "EquipmentExp"))
        {
            var id = Convert.ToUInt32(xElement.Attribute("id")?.Value);
            var d = new Dictionary<int, int>();
            foreach (var element in xElement.Descendants().Where(x => x.Name == "Exp"))
            {
                var step = Convert.ToInt32(element.Attribute("enchantStep")?.Value);
                var max = Convert.ToInt32(element.Attribute("maxExp")?.Value);
                d.Add(step, max);
            }

            ExpData.Add(id, d);
        }
    }
}