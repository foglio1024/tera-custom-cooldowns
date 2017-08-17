using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TCC.Converters
{
    class DungeonIdToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dgId = (uint)value;
            var path = Environment.CurrentDirectory + "/resources/images/dungeons/" + dgId + ".jpg";
            if (File.Exists(path))
            {
                return path;
            }

            if(dgId == 9860) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 9760 + ".jpg";
            if(dgId == 9070) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 7002 + ".jpg";
            if(dgId == 9830) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 8030 + ".jpg";
            if(dgId == 9057) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 9757 + ".jpg";
            if(dgId == 9054) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 9754 + ".jpg";
            if(dgId == 9031) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 8033 + ".jpg";
            if(dgId == 9032) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 8032 + ".jpg";
            if(dgId == 9069) return Environment.CurrentDirectory + "/resources/images/dungeons/" + 8069 + ".jpg";

            if (dgId + 200 > 9999) dgId = dgId - 200;
            else dgId = dgId + 200;
            path = Environment.CurrentDirectory + "/resources/images/dungeons/" + dgId + ".jpg";

            if (File.Exists(path))
            {
                return path;
            }
            return Environment.CurrentDirectory + "/resources/images/dungeons/unk.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
