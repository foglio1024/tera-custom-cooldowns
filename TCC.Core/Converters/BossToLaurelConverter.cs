using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    class BossToLaurelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Npc npc)) return null;
            var laurel = "";
            if (npc.TemplateId == 1000 || npc.TemplateId == 1001) laurel = "bronze";
            if (npc.TemplateId == 2000 || npc.TemplateId == 1002) laurel = "silver";
            if (npc.TemplateId == 3000 || npc.TemplateId == 1003) laurel = "gold";
            if (npc.ZoneId == 622 && npc.TemplateId == 1000) laurel = "gold";
            if (npc.ZoneId == 620 && npc.TemplateId == 1000) laurel = "gold";
            if (npc.ZoneId == 620 && npc.TemplateId == 1001) laurel = "bronze";
            if (npc.ZoneId == 620 && npc.TemplateId == 1004) laurel = "bronze";
            if (npc.ZoneId == 620 && npc.TemplateId == 1005) laurel = "bronze";
            if (npc.ZoneId == 621 && npc.TemplateId == 3001) laurel = "gold";
            if (npc.ZoneId == 434)
            {
                if (npc.TemplateId == 1000 || npc.TemplateId == 2000) laurel = "bronze";
                if (npc.TemplateId == 3000 || npc.TemplateId == 4000) laurel = "silver";
                if (npc.TemplateId == 5000 || npc.TemplateId == 6000) laurel = "gold";
                if (npc.TemplateId == 7000 || npc.TemplateId == 8000 || npc.TemplateId == 9000) laurel = "diamond";
                if (npc.TemplateId == 10000) laurel = "champion";
            }



            return "../resources/images/Icon_Laurels/" + laurel + "_kr_big.png";


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
