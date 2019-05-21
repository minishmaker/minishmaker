using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core
{
	public class PaletteSet
	{
		public Color[][] Palettes;

		public PaletteSet( int pnum )
		{
			this.Palettes = LoadPalettes( pnum );
		}

		public Color[][] LoadPalettes( int pnum )
		{
			Color[][] palettes = new Color[16][];

			for( int x = 0; x < 16; x++ )
				palettes[x] = new Color[16];

			var r = ROM.Instance.reader;

			var header = ROM.Instance.headers;
			int paletteSetTableLoc = header.paletteSetTableLoc;
			int tileOffset = header.tileOffset;
			int addr = r.ReadAddr( paletteSetTableLoc + (pnum * 4) ); //4 byte entries
			int palAddr = (int)(tileOffset + (r.ReadUInt16( addr ) << 5));
			byte pstart = r.ReadByte();
			byte pamount = r.ReadByte();

			byte[] pdata = r.ReadBytes( pamount * 0x20, palAddr );

			int pos = 0; //position in pdata array
			for( int p = pstart; p < (pstart + pamount); p++ )
			{
				for( int i = 0; i < 0x10; i++ )
				{
					ushort pd = (ushort)(pdata[pos] | (pdata[pos + 1] << 8));
					pos += 2;
					int red = (pd & 0x1F) << 3;
					int green = ((pd >> 5) & 0x1F) << 3;
					int blue = ((pd >> 10) & 0x1F) << 3;
					palettes[p][i] = Color.FromArgb( red, green, blue );
					if( i == 0 )
						palettes[p][i] = Color.Transparent; //make 0 = transparent
				}
			}
			return palettes;

		}
	}
}
