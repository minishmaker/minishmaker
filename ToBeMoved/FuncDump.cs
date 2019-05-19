using MinishMaker.Core;
using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.ToBeMoved
{
	public class FuncDump
	{
		//EVERYTHING IN HERE IS UNTESTED USE AT OWN RISK
		public class TileSet
		{
			byte[] tilesetData;

			public int Size
			{
				get
				{
					return tilesetData.Length / 0x20;
				}
			}

			public TileSet( byte[] data )
			{
				tilesetData = data;
			}

			public void SetChunk( byte[] newdata, int dest )
			{
				newdata.CopyTo( this.tilesetData, dest );
			}

			//draws a quarter tile
			public void DrawQuarterTile( ref Bitmap tileImg, Point p, int tnum, Color[] pal, bool hflip, bool vflip )
			{
				byte[] data = new byte[0x20];
				Array.Copy( tilesetData, tnum * 0x20, data, 0, 0x20 );

				int dataPos = 0;
				for( int y = 0; y < 8; y++ )
				{
					for( int x = 0; x < 8; x += 2 ) //2 pixels at a time
					{
						//itteration						0	1	2	3	4	5	6	7	-	0	1	2	3	4	5	6	7
						int posX = hflip ? 6 - x : x; //	6+7	4+5	2+3	0+1	x	x	x	x	or	0+1	2+3	4+5	6+7	x	x	x	x
						int posY = vflip ? 7 - y : y; //	7	6	5	4	3	2	1	0	or	0	1	2	3	4	5	6	7 
						posX += p.X;
						posY += p.Y;

						int colorData = data[dataPos];
						int data1 = hflip ? colorData >> 4 : colorData & 0x0F; // /16 for last 4 bits or & 15 for the first 4 bits
						int data2 = hflip ? colorData & 0x0F : colorData >> 4;
						Color color1 = pal[data1];
						Color color2 = pal[data2];

						if( color1.A > 0 )
						{
							tileImg.SetPixel( posX, posY, color1 );
						}
						if( color2.A > 0 )
						{
							tileImg.SetPixel( posX + 1, posY, color2 );
						}
						dataPos++;
					}
				}
			}

		}

		public class MetaTileSet
		{

			byte[] metaTileSetData;
			bool isBg1;
			public MetaTileSet( byte[] data, bool isBg1 )
			{
				metaTileSetData = data;
				this.isBg1 = isBg1;
			}
			public void DrawMetaTile( ref Bitmap b, Point p, TileSet tset, PaletteSet pset, int tileId )
			{

				byte[] tileData = new byte[8];
				//write 8 bytes from tileId *8 to 
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

							int tnum = data & 0x3FF;
							if( isBg1 )
								tnum += 0x200;
							bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set?
							bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set?
							int pnum = data >> 12;//last 4 bits

							tset.DrawQuarterTile( ref b, new Point( p.X + (x * 8), p.Y + (y * 8) ), tnum, pset.Palettes[pnum], hflip, vflip );
						}
					}
				}
				catch( ArgumentNullException e )
				{
					throw new ArgumentNullException( "Attempt to draw empty metatile. Num: 0x" + tileId.ToString( "X" ), e );
				}

			}
		}

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

		//looking at the size of this, would benefit from putting back that metadata class
		public class RoomExtra
		{
			private int width, height, roomIndex, areaIndex;
			private TileSet tset;
			private PaletteSet pset;
			private int paletteSetID;
			private List<AddrData> tileSetAddrs = new List<AddrData>();

			private MetaTileSet bg2MetaTiles;
			private MetaTileSet bg1MetaTiles;

			private AddrData? bg2RoomDataAddr;
			private AddrData bg2MetaTilesAddr;
			private AddrData? bg1RoomDataAddr;
			private AddrData bg1MetaTilesAddr;
			private bool bg1Use20344B0 = false;

			private byte[] bg2RoomData;
			private byte[] bg1RoomData;
			private bool bg2Exists = false;
			private bool bg1Exists = false;

			struct AddrData
			{
				public int src;
				public int dest;
				public int size; // in words (2x bytes)
				public bool compressed;

				public AddrData( int src, int dest, int size, bool compressed )
				{
					this.src = src;
					this.dest = dest;
					this.size = size;
					this.compressed = compressed;
				}
			}

			public void LoadRoom()
			{
				var r = ROM.Instance.reader;
				var header = ROM.Instance.headers;

				int areaRMDTableLoc = r.ReadAddr( header.MapHeaderBase + (this.areaIndex << 2) );
				int roomMetaDataTableLoc = areaRMDTableLoc + (this.roomIndex * 0x0A);

				this.width = r.ReadUInt16( roomMetaDataTableLoc + 4 ) >> 4; //bytes 5+6 pixels/16 = tiles
				this.height = r.ReadUInt16() >> 4;  //bytes 7+8 pixels/16 = tiles

				//get addr of TPA data
				int tileSetOffset = r.ReadUInt16() << 2;//bytes 9+10

				int areaTileSetTableLoc = r.ReadAddr( header.globalTileSetTableLoc + (this.areaIndex << 2) );
				int roomTileSetLoc = r.ReadAddr( areaTileSetTableLoc + tileSetOffset );

				r.SetPosition( roomTileSetLoc );

				ParseData( r, Set1 );

				//metatiles
				int metaTileSetsLoc = r.ReadAddr( header.globalMetaTileSetTableLoc + (this.areaIndex << 2) );

				r.SetPosition( metaTileSetsLoc );

				ParseData( r, Set2 );

				//get addr of room data 
				int areaTileDataTableLoc = r.ReadAddr( header.globalTileDataTableLoc + (this.areaIndex << 2) );
				int tileDataLoc = r.ReadAddr( areaTileDataTableLoc + (this.roomIndex << 2) );
				r.SetPosition( tileDataLoc );

				ParseData( r, Set3 );


				//build tileset using tile addrs from room metadata
				byte[] tilesetData = new byte[0x10000];
				using( MemoryStream ms = new MemoryStream( tilesetData ) )
				{
					using( BinaryWriter bw = new BinaryWriter( ms ) )
					{
						for( int i = 0; i < tileSetAddrs.Count; i++ )
						{
							ms.Seek( tileSetAddrs[i].dest, SeekOrigin.Begin );
							byte[] data = GetData( tileSetAddrs[i] );
							bw.Write( data );
						}
						tset = new TileSet( tilesetData );
						//Debug.WriteLine(tset.Size+"COUNTS");
					}
				}

				//palettes
				pset = new PaletteSet( paletteSetID );

				//room data
				//int size = (metadata.Width * metadata.Height) << 1;
				//this size adjustment accounts for modifyRoomData
				//bg2RoomData = new byte[size + ((metadata.Height-1) * 0x80) + ((metadata.Width<<1)-2)];
				//bg1RoomData = new byte[size + ((metadata.Height-1) * 0x80) + ((metadata.Width<<1)-2)];
				bg2RoomData = new byte[0x2000]; //this seems to be the maximum size
				bg1RoomData = new byte[0x2000]; //2000 isnt too much memory, so this is just easier

				if( bg2RoomDataAddr != null )
				{
					var tileData = GetData( bg2MetaTilesAddr );
					bg2MetaTiles = new MetaTileSet( tileData, false );
					//Debug.WriteLine("BG2 Room Src: " + metadata.Bg2RoomDataAddr.Src.ToString("X"));
					//Debug.WriteLine("Compressed: " + metadata.Bg2RoomDataAddr.Compressed.ToString());
					var data = GetData( (AddrData)bg2RoomDataAddr );
					data.CopyTo( bg2RoomData, 0 );
					//modifyRoomData( ref bg2RoomData );

					bg2Exists = true;
				}

				if( bg1RoomDataAddr != null )
				{
					if( bg1Use20344B0 )
					{
						GetData( (AddrData)bg1RoomDataAddr ).CopyTo( bg1RoomData, 0 );
						//bg1RoomData = modifyBg1RoomData20344B0(bg1RoomData);
						bg1Use20344B0 = true;
					}
					else
					{
						bg1MetaTiles = new MetaTileSet( GetData( bg1MetaTilesAddr ), true );
						GetData( (AddrData)bg1RoomDataAddr ).CopyTo( bg1RoomData, 0 );
						//modifyRoomData( ref bg1RoomData );
					}
					bg1Exists = true;
				}


			}

			//commented parts so the program will still build
			private byte[] GetData( AddrData addrData )
			{
				if( addrData.compressed )
				{
					return new byte[1];//Lz77Uncomp( addrData.src );
				}
				else
				{
					return new byte[1];//DMA( addrData.src, addrData.size );
				}
			}

			private void ParseData( Reader r, Func<AddrData, bool> postFunc )
			{
				var header = ROM.Instance.headers;
				bool cont = true;
				while( cont )
				{
					UInt32 data = r.ReadUInt32();
					UInt32 data2 = r.ReadUInt32();
					UInt32 data3 = r.ReadUInt32();

					cont = (data & 0x80000000) != 0; //high bit determines if more to load

					if( data2 == 0 )
					{ //palette
						this.paletteSetID = (int)(data & 0x7FFFFFFF); //mask off high bit
					}
					else
					{
						int source = (int)((data & 0x7FFFFFFF) + header.gfxSourceBase); //8324AE4 is tile gfx base
						int dest = (int)(data2 & 0x7FFFFFFF);
						bool compressed = (data3 & 0x80000000) != 0; //high bit of size determines LZ or DMA
						Console.WriteLine( compressed );
						int size = (int)(data3 & 0x7FFFFFFF);

						cont = postFunc( new AddrData( source, dest, size, compressed ) );
					}
				}
			}

			//dont have any good names for these 3
			private bool Set1( AddrData data )
			{
				if( (data.dest & 0xF000000) != 0x6000000 )
				{ //not valid tile data addr
				  //throw new FormatException("Unhandled tile data destination address: " + dest.Hex() + " Source:" + source.Hex() + " Compressed:" + compressed + " Size:" + size.Hex());
					Console.WriteLine( "Unhandled tile data destination address: " + data.dest.Hex() + " Source:" + data.src.Hex() + " Compressed:" + data.compressed + " Size:" + data.size.Hex() );
					return false;
				}

				data.dest = data.dest & 0xFFFFFF;
				this.tileSetAddrs.Add( data );
				return true;
			}

			private bool Set2( AddrData data )
			{
				//use a switch in case this data is out of order
				switch( data.dest )
				{
					case 0x0202CEB4:
						this.bg2MetaTilesAddr = data;
						Debug.WriteLine( data.src.Hex() + " bg2" );
						break;
					case 0x02012654:
						this.bg1MetaTilesAddr = data;
						Debug.WriteLine( data.src.Hex() + " bg1" );
						break;
					//case 0x0202AEB4:
					//ret[8] = source; //not handled
					//break;
					//case 0x02010654:
					//ret[9] = source; //not handled
					//break;
					default:
						Debug.Write( "Unhandled metatile addr: " );
						Debug.Write( data.src.Hex() + "->" + data.dest.Hex() );
						Debug.WriteLine( data.compressed ? " (compressed)" : "" );
						break;
				}
				return true;
			}

			private bool Set3( AddrData data )
			{
				switch( data.dest )
				{
					case 0x02025EB4:
						this.bg2RoomDataAddr = data;
						break;
					case 0x0200B654:
						this.bg1RoomDataAddr = data;
						break;
					case 0x2002F00:
						this.bg1RoomDataAddr = data;
						this.bg1Use20344B0 = true;
						break;
					default:
						Debug.Write( "Unhandled room data addr: " );
						Debug.Write( data.src.Hex() + "->" + data.dest.Hex() );
						Debug.WriteLine( data.compressed ? " (compressed)" : "" );
						break;
				}
				return true;
			}

			private void DrawRoom( ref Bitmap b, MetaTileSet metaTiles, byte[] roomData )
			{
				int pos = 0; //position in roomData
				ushort[] chunks = new ushort[3];
				ushort[] oldchunks = new ushort[3];
				chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };

				for( int j = 0; j < this.height; j++ )
				{
					pos = j * this.width * 2;
					for( int i = 0; i < this.width; i++ )
					{
						//hardcoded because there is no easy way to determine which areas use tileswapping
						if( this.roomIndex == 00 && this.areaIndex == 01 || this.areaIndex == 02 || this.areaIndex == 0x15 )
						{
							oldchunks = chunks;
							chunks = getChunks( (ushort)(i * 16), (ushort)(j * 16) );

							swapTiles( oldchunks, chunks, (ushort)(i * 16), (ushort)(j * 16) );
						}
						//which metatile to draw
						int mt = roomData[pos] | (roomData[pos + 1] << 8);

						pos += 2;

						try
						{
							if( mt != 0xFFFF ) //nonexistant room data does this, eg. area 0D room 10
								metaTiles.DrawMetaTile( ref b, new Point( i * 16, j * 16 ), tset, pset, mt );
						}
						catch( Exception e )
						{
							throw new Exception( "Error drawing metatile. i:" + i.ToString() + ", j:" + j.ToString()
												+ "\n" + e.Message, e );
						}
					}
				}
			}

			private UInt16[] getChunks( UInt16 x, UInt16 y )
			{
				UInt16[] ret = new ushort[3];
				var header = ROM.Instance.headers;

				//chunk 00
				int addr;
				switch( this.areaIndex )
				{
					case 0x01:
						addr = header.area1Chunk0TableLoc;
						break;
					case 0x15:
						addr = header.chunk0TableLoc + 0x042; //area 0x15 (21) uses a different hardcoded ptr tbl
						break;
					default:
						addr = header.chunk0TableLoc; //area 1 uses different hardcoded ptr tbl
						break;
				}

				ret[0] = TestChunk( x, y, addr );

				//chunk 01
				if( areaIndex == 0x02 )
				{ //no chunk 01 for area 15, only area 02
					ret[1] = TestChunk( x, y, header.chunk1TableLoc );
				}
				else
				{
					ret[1] = 0x00FF;
				}

				//chunk 02
				if( areaIndex == 0x02 || areaIndex == 0x15 )
				{
					addr = areaIndex != 0x15 ? header.chunk2TableLoc : header.chunk2TableLoc + 0x02E; //area 15 uses different hardcoded ptr tbl
					ret[2] = TestChunk( x, y, addr );
				}
				else
				{
					ret[2] = 0x00FF;
				}

				return ret;
			}

			private ushort TestChunk( UInt16 x, UInt16 y, long addr )
			{
				var r = ROM.Instance.reader;
				r.SetPosition( addr );
				UInt16 chnk; //note: this do block is essentially check_swap_inner in IDA
				do
				{
					chnk = r.ReadUInt16();
					if( chnk == 0x00FF )
						break; //no change
					UInt16 r0 = r.ReadUInt16();
					UInt16 r1 = r.ReadUInt16();
					UInt16 r2 = r.ReadUInt16();
					UInt16 r3 = r.ReadUInt16();

					UInt16 test_x, test_y;
					unchecked
					{
						test_x = (UInt16)(x - r0); //from check_coords routine
						test_y = (UInt16)(y - r1);
					}
					//if(y >= 0x230 && y < 0x240)
					//Debug.WriteLine(Convert.ToString(test_y,16) + "<" + Convert.ToString(r3,16));
					if( test_x < r2 && test_y < r3 )
						break; //chnk found, so return
				} while( true );
				return chnk;
			}

			private void swapTiles( ushort[] oldchunks, ushort[] newchunks, ushort x, ushort y )
			{
				var r = ROM.Instance.reader;
				var header = ROM.Instance.headers;
				int updatepal = -1;

				for( int i = 0; i < 3; i++ )
				{
					if( newchunks[i] == oldchunks[i] )
					{
						continue;
					}

					if( newchunks[i] == 0x00FF )
					{
						continue;
					}

					int baseaddr, src, src2, dest, dest2;
					byte[] newtiles = new byte[0x1000];

					switch( areaIndex )
					{
						case 0x02:
						case 0x15:
							baseaddr = areaIndex != 0x15 ? header.swapBase : header.swapBase + 0x060;
							r.SetPosition( baseaddr + (newchunks[i] << 4) );
							src = (int)(header.tileOffset + r.ReadUInt32()); //source addrs are stored as offsets from 85A2E80
							dest = (int)r.ReadUInt32() & 0xFFFFFF; //mask off the 0x6000000 part
							src2 = (int)(header.tileOffset + r.ReadUInt32()); //there are 2 sets of tiles for each chunk
							dest2 = (int)r.ReadUInt32() & 0xFFFFFF; //one for each bg
																	//Debug.WriteLine(Convert.ToString(src,16) + " -> " + Convert.ToString(dest,16));
							newtiles = r.ReadBytes( 0x1000, src );
							tset.SetChunk( newtiles, dest );

							newtiles = r.ReadBytes( 0x1000, src2 );
							tset.SetChunk( newtiles, dest2 );

							break;
						case 0x01: //area 01 works differently, 8 chunks and palette update
							for( int j = 0; j < 8; j++ )
							{
								if( j == 0 )
								{//update palette
									byte pnum = r.ReadByte( header.paletteChangeBase + newchunks[i] );
									updatepal = (int)pnum;
								}
								baseaddr = header.area1SwapBase;
								r.SetPosition( baseaddr + (newchunks[i] << 6) + (j << 3) );

								src = (header.tileOffset + (int)r.ReadUInt32());
								dest = (int)r.ReadUInt32() & 0xFFFFFF;

								newtiles = r.ReadBytes( 0x1000, src );
								tset.SetChunk( newtiles, dest );
							}
							break;
					}
				}
				if( updatepal > 0 )
				{
					pset = new PaletteSet( updatepal );
				}
			}
		}

		private static Bitmap MakeTilesetImage( MetaTileSet mset, PaletteSet pset, TileSet tset )
		{
			var tilesPerRow = 32;//0x20
			var tilesPerValue = 8;
			var totalValues = 0x100;
			var tileSize = 16;// 0x10 pixels

			var divisor = tilesPerRow / tilesPerValue;

			var twidth = tileSize * tilesPerRow;
			var theight = tileSize * (totalValues / divisor);

			Bitmap b = new Bitmap( twidth, theight, PixelFormat.Format32bppArgb );

			var pos = 0;
			bool ended = false;
			for( int j = 0; j < totalValues / divisor; j++ )
			{
				if( ended )
					break;
				for( int i = 0; i < tilesPerRow; i++ )
				{
					try
					{
						if( pos != 0xFFFF )
							mset.DrawMetaTile( ref b, new Point( i * 16, j * 16 ), tset, pset, pos );
					}
					catch( Exception )
					{
						Debug.WriteLine( "end of metatile file: " + (pos % 256).Hex() + "|" + (pos / 256).Hex() );
						Debug.WriteLine( "" );
						ended = true;
						break;
					}
					pos++;
				}
			}

			return b;
		}
	}

	public static class Extensions
	{
		public static string Hex( this uint num )
		{
			return Convert.ToString( num, 16 ).ToUpper();
		}

		public static string Hex( this int num )
		{
			return Convert.ToString( num, 16 ).ToUpper();
		}

		public static string Hex( this long num )
		{
			return Convert.ToString( num, 16 ).ToUpper();
		}

		public static string Hex( this ushort num )
		{
			return Convert.ToString( num, 16 ).ToUpper();
		}

		public static string Hex( this byte num )
		{
			return Convert.ToString( num, 16 ).ToUpper();
		}
	}
}
