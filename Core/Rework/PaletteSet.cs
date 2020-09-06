using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinishMaker.Utilities;

namespace MinishMaker.Core.Rework
{
    public class PaletteSet
    {
        public Color[] Colors { get; private set; }

        public PaletteSet(int pnum)
        {
            this.Colors = new Color[16 * 16];

            var r = ROM.Instance.reader;

            var header = ROM.Instance.headers;
            int paletteSetTableLoc = header.paletteSetTableLoc;
            int tileOffset = header.tileOffset;
            int addr = r.ReadAddr(paletteSetTableLoc + (pnum * 4)); //4 byte entries
            int palAddr = (int)(tileOffset + (r.ReadUInt16(addr) << 5));
            byte pstart = r.ReadByte();
            byte pamount = r.ReadByte();

            byte[] pdata = r.ReadBytes(pamount * 0x20, palAddr);

            int pos = 0; //position in pdata array
                         //manual 0th entry as I dont know where it gets it from yet

            Colors[0] = Color.Transparent;
            Colors[1] = Color.FromArgb(14 * 8, 3 * 8, 2 * 8);
            Colors[2] = Color.FromArgb(20 * 8, 5 * 8, 3 * 8);
            Colors[3] = Color.FromArgb(26 * 8, 8 * 8, 4 * 8);
            Colors[4] = Color.FromArgb(31 * 8, 10 * 8, 2 * 8);
            Colors[5] = Color.FromArgb(31 * 8, 23 * 8, 9 * 8);
            Colors[6] = Color.FromArgb(31 * 8, 17 * 8, 2 * 8);
            Colors[7] = Color.FromArgb(31 * 8, 23 * 8, 5 * 8);
            Colors[8] = Color.FromArgb(31 * 8, 28 * 8, 7 * 8);
            Colors[9] = Color.FromArgb(31 * 8, 31 * 8, 12 * 8);
            Colors[10] = Color.FromArgb(14 * 8, 12 * 8, 12 * 8);
            Colors[11] = Color.FromArgb(19 * 8, 17 * 8, 17 * 8);
            Colors[12] = Color.FromArgb(24 * 8, 22 * 8, 22 * 8);
            Colors[13] = Color.FromArgb(29 * 8, 27 * 8, 27 * 8);
            Colors[14] = Color.FromArgb(31 * 8, 31 * 8, 31 * 8);
            Colors[15] = Color.FromArgb(0, 0, 0);

            if (pstart >= 2)
            {
                Colors[16] = Color.Transparent;
                Colors[17] = Color.FromArgb(3 * 8, 6 * 8, 18 * 8);
                Colors[18] = Color.FromArgb(4 * 8, 13 * 8, 25 * 8);
                Colors[19] = Color.FromArgb(5 * 8, 20 * 8, 30 * 8);
                Colors[20] = Color.FromArgb(9 * 8, 26 * 8, 31 * 8);
                Colors[21] = Color.FromArgb(21 * 8, 29 * 8, 31 * 8);
                Colors[22] = Color.FromArgb(13 * 8, 9 * 8, 5 * 8);
                Colors[23] = Color.FromArgb(21 * 8, 12 * 8, 4 * 8);
                Colors[24] = Color.FromArgb(28 * 8, 18 * 8, 5 * 8);
                Colors[25] = Color.FromArgb(31 * 8, 26 * 8, 4 * 8);
                Colors[26] = Color.FromArgb(10 * 8, 12 * 8, 13 * 8);
                Colors[27] = Color.FromArgb(14 * 8, 17 * 8, 18 * 8);
                Colors[28] = Color.FromArgb(20 * 8, 22 * 8, 24 * 8);
                Colors[29] = Color.FromArgb(26 * 8, 27 * 8, 29 * 8);
                Colors[30] = Color.FromArgb(31 * 8, 31 * 8, 31 * 8);
                Colors[31] = Color.FromArgb(0, 0, 0);
            }

            for (int p = pstart; p < (pstart + pamount); p++)
            {
                if (p >= 16)
                {
                    throw new PaletteException("The room attempted to load more than 16 palettes, the room is highly likely invalid.");
                }
                for (int i = 0; i < 0x10; i++)
                {
                    ushort pd = (ushort)(pdata[pos] | (pdata[pos + 1] << 8));
                    pos += 2;
                    int red = (pd & 0x1F) << 3;
                    int green = ((pd >> 5) & 0x1F) << 3;
                    int blue = ((pd >> 10) & 0x1F) << 3;
                    Colors[p * 16 + i] = Color.FromArgb(red, green, blue);
                }
            }
        }

        public string ToPaletteString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CLRX 8 256");
            var i = 0;
            foreach (var color in this.Colors)
            {
                sb.Append("0x00" + color.B.Hex().PadLeft(2, '0') + color.G.Hex().PadLeft(2, '0') + color.R.Hex().PadLeft(2, '0') + " ");
                i++;
                if (i == 4)
                {
                    sb.AppendLine();
                    i = 0;
                }
            }
            return sb.ToString();
        }

    }

    public class PaletteException : Exception
    {
        public PaletteException() { }
        public PaletteException(string message) : base(message) { }
        public PaletteException(string message, Exception inner) : base(message, inner) { }
    }
}
