using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;



namespace TeraPacketParser.Messages
{
    public class S_IMAGE_DATA : ParsedMessage
    {
        private static Dictionary<uint, Bitmap> _database;
        public static Dictionary<uint, Bitmap> Database => _database ?? (_database = new Dictionary<uint, Bitmap>());

        public static void LoadCachedImages()
        {
            if (!Directory.Exists("resources/images/guilds")) return;
            foreach (var file in Directory.EnumerateFiles("resources/images/guilds"))
            {
                Database[Convert.ToUInt32(file.Split('_')[2])] = new Bitmap(file);
            }
        }

        public S_IMAGE_DATA(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            var imageOffset = reader.ReadUInt16();
            var imageLength = reader.ReadUInt16();

            reader.BaseStream.Position = nameOffset - 4;
            var imageName = reader.ReadTeraString();
            if (!imageName.StartsWith("guildlogo")) return;
            reader.BaseStream.Position = imageOffset - 4;
            var imageBytes = reader.ReadBytes(imageLength);
            var width = BitConverter.ToInt32(imageBytes, 8);
            var type = BitConverter.ToInt32(imageBytes, 12);

            var image = new Bitmap(width, width, PixelFormat.Format8bppIndexed);
            var palette = image.Palette;
            int length;
            switch (type)
            {
                case 0:
                case 1:
                    var paletteSize = BitConverter.ToInt32(imageBytes, 16);
                    if (paletteSize >= imageLength - 24) { Debug.WriteLine("palette size too big"); return; }
                    for (var i = 0; i < paletteSize; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(imageBytes[0x14 + i * 3], imageBytes[0x15 + i * 3], imageBytes[0x16 + i * 3]);
                    }
                    length = BitConverter.ToInt32(imageBytes, paletteSize * 3 + 20);
                    break;
                case 2:
                case 3:
                    for (var i = 0; i < 255; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    length = BitConverter.ToInt32(imageBytes, 16);
                    break;
                default:
                    Debug.WriteLine("Unknown guild logo format");
                    return;
            }
            var pixels = image.LockBits(new Rectangle(0, 0, width, width), ImageLockMode.WriteOnly, image.PixelFormat);
            Marshal.Copy(imageBytes, imageLength - length, pixels.Scan0, length);
            image.UnlockBits(pixels);
            image.Palette = palette;
            var id = Convert.ToUInt32(imageName.Split('_')[2]);
            Database[id] = image;
            //if (!Directory.Exists("resources/images/guilds")) Directory.CreateDirectory("resources/images/guilds");
            //image.Save("resources/images/guilds/" + imageName + ".bmp", ImageFormat.Bmp);

        }
    }
}
