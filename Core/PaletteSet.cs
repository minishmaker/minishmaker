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
			//manual 0th entry as I dont know where it gets it from yet
			

			palettes[0][0]= Color.Transparent;
			palettes[0][1]= Color.FromArgb(14*8,3*8,2*8);
			palettes[0][2]= Color.FromArgb(20*8,5*8,3*8);
			palettes[0][3]= Color.FromArgb(26*8,8*8,4*8);
			palettes[0][4]= Color.FromArgb(31*8,10*8,2*8);
			palettes[0][5]= Color.FromArgb(31*8,23*8,9*8);
			palettes[0][6]= Color.FromArgb(31*8,17*8,2*8);
			palettes[0][7]= Color.FromArgb(31*8,23*8,5*8);
			palettes[0][8]= Color.FromArgb(31*8,28*8,7*8);
			palettes[0][9]= Color.FromArgb(31*8,31*8,12*8);
			palettes[0][10]= Color.FromArgb(14*8,12*8,12*8);
			palettes[0][11]= Color.FromArgb(19*8,17*8,17*8);
			palettes[0][12]= Color.FromArgb(24*8,22*8,22*8);
			palettes[0][13]= Color.FromArgb(29*8,27*8,27*8);
			palettes[0][14]= Color.FromArgb(31*8,31*8,31*8);
			palettes[0][15]= Color.FromArgb(0,0,0);

			if(pstart>=2)
			{
				
				palettes[1][0]= Color.FromArgb(2*8,16*8,11*8);
				palettes[1][1]= Color.FromArgb(3*8,6*8,18*8);
				palettes[1][2]= Color.FromArgb(4*8,13*8,25*8);
				palettes[1][3]= Color.FromArgb(5*8,20*8,30*8);
				palettes[1][4]= Color.FromArgb(9*8,26*8,31*8);
				palettes[1][5]= Color.FromArgb(21*8,29*8,31*8);
				palettes[1][6]= Color.FromArgb(13*8,9*8,5*8);
				palettes[1][7]= Color.FromArgb(21*8,12*8,4*8);
				palettes[1][8]= Color.FromArgb(28*8,18*8,5*8);
				palettes[1][9]= Color.FromArgb(31*8,26*8,4*8);
				palettes[1][10]= Color.FromArgb(10*8,12*8,13*8);
				palettes[1][11]= Color.FromArgb(14*8,17*8,18*8);
				palettes[1][12]= Color.FromArgb(20*8,22*8,24*8);
				palettes[1][13]= Color.FromArgb(26*8,27*8,29*8);
				palettes[1][14]= Color.FromArgb(31*8,31*8,31*8);
				palettes[1][15]= Color.FromArgb(0,0,0);
			}

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
