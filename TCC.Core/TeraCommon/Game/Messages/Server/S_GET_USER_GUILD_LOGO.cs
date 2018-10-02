using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_GET_USER_GUILD_LOGO : ParsedMessage
    {
        internal S_GET_USER_GUILD_LOGO(TeraMessageReader reader) : base(reader)
        {
            reader.ReadUInt16();
            var size = reader.ReadUInt16();
            PlayerId = reader.ReadUInt32();
            GuildId = reader.ReadUInt32();
            //Debug.WriteLine("icon size:"+size+";offset:"+offset+";player:"+PlayerId+";GuildId:"+GuildId);

            var logo = reader.ReadBytes(size);

            GuildLogo = new Bitmap(64,64,PixelFormat.Format8bppIndexed);
            var paletteSize = (size - 0x1018)/3;
            if (paletteSize > 0x100 || paletteSize < 1)
            {
                Debug.WriteLine("Missed guild logo format");
                return;
            }
            var palette = GuildLogo.Palette;
            for (var i = 0; i < paletteSize; i++)
            {
                palette.Entries[i] = Color.FromArgb(logo[0x14 + i*3], logo[0x15 + i*3], logo[0x16 + i*3]);
            }
            var pixels = GuildLogo.LockBits(new Rectangle(0, 0, 64, 64), ImageLockMode.WriteOnly, GuildLogo.PixelFormat);
            Marshal.Copy(logo, size-0x1000, pixels.Scan0, 0x1000);
            GuildLogo.UnlockBits(pixels);
            GuildLogo.Palette = palette;
            //GuildLogo.Save($"q:\\{Time.Ticks}.bmp",ImageFormat.Bmp);
            //System.IO.File.WriteAllBytes($"q:\\{Time.Ticks}.bin", logo);
        }
            
        public uint GuildId { get; }
        public uint PlayerId { get; }
        public Bitmap GuildLogo { get; }
    }
}