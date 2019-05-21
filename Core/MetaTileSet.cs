using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MinishMaker.Core.RoomMetaData;

namespace MinishMaker.Core
{
	public class MetaTileSet
	{
		byte[] metaTileSetData;
		bool isBg1;

		public MetaTileSet( AddrData addrData, bool isBg1 )
		{
			metaTileSetData = DataHelper.GetData( addrData );
			this.isBg1 = isBg1;
		}

		public void DrawMetaTile( ref Bitmap b, Point p, TileSet tset, PaletteSet pset, int tileId, bool overwrite )
		{
			byte[] tileData = new byte[8];
			//write 8 bytes from tileId *8 to tileData
			Array.Copy( metaTileSetData, tileId << 3, tileData, 0, 8 );
			try
			{
				if( tileData.Length == 0 )
					throw new ArgumentNullException( "metaTileData", "Cannot draw empty metatile." );
				int i = 0;
				for( int y = 0; y < 2; y += 1 )
				{
					for( int x = 0; x < 2; x += 1 )
					{
						UInt16 data = 0;
						data = (ushort)(tileData[i] | (tileData[i + 1] << 8));
						i += 2;

						int tnum = data & 0x3FF; //bits 1-10

						if( isBg1 )
							tnum += 0x200;

						bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set?
						bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set?
						int pnum = data >> 12;//last 4 bits

						tset.DrawQuarterTile( ref b, new Point( p.X + (x * 8), p.Y + (y * 8) ), tnum, pset.Palettes[pnum], hflip, vflip, overwrite );
					}
				}
			}
			catch( ArgumentNullException e )
			{
				throw new ArgumentNullException( "Attempt to draw empty metatile. Num: 0x" + tileId.ToString( "X" ), e );
			}

		}
	}
}
