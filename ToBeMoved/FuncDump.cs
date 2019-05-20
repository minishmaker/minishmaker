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
using static MinishMaker.ToBeMoved.FuncDump.RoomMetaData;

namespace MinishMaker.ToBeMoved
{
	public class FuncDump
	{
		//EVERYTHING IN HERE IS UNTESTED USE AT OWN RISK
		//should now contain everything needed to get a room to render
		

		public class MetaTileSet
		{
			byte[] metaTileSetData;
			bool isBg1;

			public MetaTileSet( byte[] data, bool isBg1 )
			{
				metaTileSetData = data;
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

		public class RoomExtra
		{
			private int roomIndex, areaIndex;

			private RoomMetaData metadata;
			private TileSet tset;
			private PaletteSet pset;
			private MetaTileSet bg2MetaTiles;
			private MetaTileSet bg1MetaTiles;
			private byte[] bg2RoomData;
			private byte[] bg1RoomData;
			private bool bg2Exists = false;
			private bool bg1Exists = false;

			public Bitmap[] MakeRoomImage( bool showBg1, bool showBg2 )
			{
				//draw using screens
				Bitmap bg1 = new Bitmap( metadata.PixelWidth, metadata.PixelHeight, PixelFormat.Format32bppArgb );
				Bitmap bg2 = new Bitmap( metadata.PixelWidth, metadata.PixelHeight, PixelFormat.Format32bppArgb );

				if( showBg2 && bg2Exists )
					DrawLayer( ref bg2, bg2MetaTiles, bg2RoomData, false );
				if( showBg1 && bg1Exists )
				{
					if( metadata.Bg1Use20344B0 )
					{
						//hacky way to draw with tilemap, instead of using metatiles
						//this should be handled by the Screen class probably
						DrawSpecialLayer( ref bg1, bg1RoomData, false );
					}
					else
					{
						DrawLayer( ref bg1, bg1MetaTiles, bg1RoomData, false );
					}
				}
				return new Bitmap[] { bg1, bg2 };
			}

			private void DrawLayer( ref Bitmap b, MetaTileSet metaTiles, byte[] roomData, bool overwrite )
			{
				int pos = 0; //position in roomData
				ushort[] chunks = new ushort[3];
				ushort[] oldchunks = new ushort[3];
				chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };

				for( int j = 0; j < metadata.TileHeight; j++ )
				{
					for( int i = 0; i < metadata.TileWidth; i++ )
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

						pos += 2; //2 bytes per tile

						try
						{
							if( mt != 0xFFFF ) //nonexistant room data does this, eg. area 0D room 10
								metaTiles.DrawMetaTile( ref b, new Point( i * 16, j * 16 ), tset, pset, mt, overwrite );
						}
						catch( Exception e )
						{
							throw new Exception( "Error drawing metatile. i:" + i.ToString() + ", j:" + j.ToString()
												+ "\n" + e.Message, e );
						}
					}
				}
			}

			//used for 0D-10, etc.
			private void DrawSpecialLayer( ref Bitmap b, byte[] tileMap, bool overwrite )
			{
				int pos = 0; //position in tileMap
				for( int j = 0; j < 0x17; j++ )
				{ //not exactly sure what j should go to
					for( int i = 0; i < 32; i++ )
					{
						ushort data = (ushort)(tileMap[pos] | (tileMap[pos + 1] << 8));
						pos += 2;
						int tnum = (int)(data & 0x3FF);
						tnum += 0x200; //because it's bg1 and base is 6004000 not 6000000
						int pnum = (int)(data >> 12);
						bool hflip = (data & 0x400) != 0;
						bool vflip = (data & 0x800) != 0;

						tset.DrawQuarterTile( ref b, new Point( i * 8, j * 8 ), tnum, pset.Palettes[pnum], hflip, vflip, overwrite );
					}
				}
				//}
			}

			private void LoadRoom()
			{
				metadata = new RoomMetaData( this.areaIndex, this.roomIndex );

				//build tileset using tile addrs from room metadata
				tset = metadata.GetTileSet();

				//palettes
				pset = metadata.GetPaletteSet();

				//room data
				bg2RoomData = new byte[0x2000]; //this seems to be the maximum size
				bg1RoomData = new byte[0x2000]; //2000 isnt too much memory, so this is just easier

				bg2Exists = metadata.GetBG2Data( ref bg2RoomData, ref bg2MetaTiles );//loads in the data and tiles
				bg1Exists = metadata.GetBG1Data( ref bg1RoomData, ref bg1MetaTiles );//loads in the data, tiles and sets bg1Use20344B0
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
					addr = areaIndex != 0x15 ? header.chunk2TableLoc : header.chunk2TableLoc + 0x02E; //area 0x15 (21) uses different hardcoded ptr tbl
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
							src = (int)(header.tileOffset + r.ReadUInt32());    //source addrs are stored as offsets from 85A2E80
							dest = (int)r.ReadUInt32() & 0xFFFFFF;              //mask off the 0x6000000 part
							src2 = (int)(header.tileOffset + r.ReadUInt32());   //there are 2 sets of tiles for each chunk
							dest2 = (int)r.ReadUInt32() & 0xFFFFFF;             //one for each bg

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
				if( updatepal > 0 ) //if the palette number changed due to swapping, create new paletteset
				{
					pset = new PaletteSet( updatepal );
				}
			}

		}

		public class RoomMetaData
		{
			private int width, height;
			public int PixelWidth
			{
				get{return width * 16; }
			}

			public int PixelHeight
			{
				get{return height * 16;}
			}

			public int TileWidth
			{
				get{return width;}
			}

			public int TileHeight
			{
				get{return height;}
			}

			private int paletteSetID;
			private List<AddrData> tileSetAddrs = new List<AddrData>();

			private AddrData? bg2RoomDataAddr;
			private AddrData bg2MetaTilesAddr;

			private AddrData? bg1RoomDataAddr;
			private AddrData bg1MetaTilesAddr;

			private bool bg1Use20344B0 = false;
			public bool Bg1Use20344B0
			{
				get{return bg1Use20344B0;}
			}

			public struct AddrData
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

			public RoomMetaData( int areaIndex, int roomIndex )
			{
				LoadMetaData( areaIndex, roomIndex );
			}

			private void LoadMetaData( int areaIndex, int roomIndex )
			{
				var r = ROM.Instance.reader;
				var header = ROM.Instance.headers;

				int areaRMDTableLoc = r.ReadAddr( header.MapHeaderBase + (areaIndex << 2) );
				int roomMetaDataTableLoc = areaRMDTableLoc + (roomIndex * 0x0A);

				this.width = r.ReadUInt16( roomMetaDataTableLoc + 4 ) >> 4; //bytes 5+6 pixels/16 = tiles
				this.height = r.ReadUInt16() >> 4;                          //bytes 7+8 pixels/16 = tiles

				//get addr of TPA data
				int tileSetOffset = r.ReadUInt16() << 2;                    //bytes 9+10

				int areaTileSetTableLoc = r.ReadAddr( header.globalTileSetTableLoc + (areaIndex << 2) );
				int roomTileSetLoc = r.ReadAddr( areaTileSetTableLoc + tileSetOffset );

				r.SetPosition( roomTileSetLoc );

				ParseData( r, Set1 );

				//metatiles
				int metaTileSetsLoc = r.ReadAddr( header.globalMetaTileSetTableLoc + (areaIndex << 2) );

				r.SetPosition( metaTileSetsLoc );

				ParseData( r, Set2 );

				//get addr of room data 
				int areaTileDataTableLoc = r.ReadAddr( header.globalTileDataTableLoc + (areaIndex << 2) );
				int tileDataLoc = r.ReadAddr( areaTileDataTableLoc + (roomIndex << 2) );
				r.SetPosition( tileDataLoc );

				ParseData( r, Set3 );
			}

			public TileSet GetTileSet()
			{
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
						return new TileSet( tilesetData );
					}
				}
			}

			public PaletteSet GetPaletteSet()
			{
				return new PaletteSet( paletteSetID );
			}

			public bool GetBG2Data( ref byte[] bg2RoomData, ref MetaTileSet bg2MetaTiles )
			{
				if( bg2RoomDataAddr != null )
				{
					var tileData = GetData( bg2MetaTilesAddr );
					bg2MetaTiles = new MetaTileSet( tileData, false );

					var data = GetData( (AddrData)bg2RoomDataAddr );
					data.CopyTo( bg2RoomData, 0 );

					return true;
				}
				return false;
			}

			public bool GetBG1Data( ref byte[] bg1RoomData, ref MetaTileSet bg1MetaTiles )
			{
				if( bg1RoomDataAddr != null )
				{
					if( bg1Use20344B0 )
					{
						GetData( (AddrData)bg1RoomDataAddr ).CopyTo( bg1RoomData, 0 );
						bg1Use20344B0 = true;
					}
					else
					{
						bg1MetaTiles = new MetaTileSet( GetData( bg1MetaTilesAddr ), true );
						GetData( (AddrData)bg1RoomDataAddr ).CopyTo( bg1RoomData, 0 );
					}
					return true;
				}
				return false;
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

			//TODO:commented parts so the program will still build
			private byte[] GetData( AddrData addrData )
			{
				if( addrData.compressed )
				{
					return Lz77Decompress( addrData.src );
				}
				else
				{
					return DMA( addrData.src, addrData.size );
				}
			}

			private byte[] Lz77Decompress( int addr )
			{
				var r = ROM.Instance.reader;
				var sizeBytes = r.ReadBytes( 2, 1 );
				var decompressedSize = sizeBytes[0] + (sizeBytes[1] << 8); //(sizeBytes[0]| (sizeBytes[1] << 8)| (sizeBytes[2] << 16)) >>8;

				//return the decompressed data
				byte[] data = new byte[decompressedSize];
				r.SetPosition( addr ); //pos was changed from getting size

				using( MemoryStream ms = new MemoryStream( data ) )
					Lz10.Lz77Decompress( ms );
				return data;
			}

			private byte[] DMA( int addr, int size )
			{
				var r = ROM.Instance.reader;
				return r.ReadBytes( size << 2, 0 );
			}

			//dont have any good names for these 3
			private bool Set1( AddrData data )
			{
				if( (data.dest & 0xF000000) != 0x6000000 )
				{ //not valid tile data addr
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
		}

		public static Bitmap MakeTilesetImage( MetaTileSet mset, PaletteSet pset, TileSet tset )
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
							mset.DrawMetaTile( ref b, new Point( i * 16, j * 16 ), tset, pset, pos, true );
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

		public class Lz10
		{
			private static int Unwrap( byte[] data )
			{
				return (data[0] | (data[1] << 8) | (data[2] << 16));
			}

			/// <summary>
			/// Decompress a stream that is compressed in the LZ-10 format.
			/// Doesn't check input length (for ROM hacking projects)
			/// </summary>
			/// <param name="instream">The compressed stream.</param>
			/// <param name="inLength">Unused, left in because CompressionFormat requires it</param>
			/// <param name="outstream">The output stream, where the decompressed data is written to.</param>
			public static long Lz77Decompress( Stream outstream )
			{
				#region format definition from GBATEK/NDSTEK
				/*  Data header (32bit)
					  Bit 0-3   Reserved
					  Bit 4-7   Compressed type (must be 1 for LZ77)
					  Bit 8-31  Size of decompressed data
					Repeat below. Each Flag Byte followed by eight Blocks.
					Flag data (8bit)
					  Bit 0-7   Type Flags for next 8 Blocks, MSB first
					Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
					  Bit 0-7   One data byte to be copied to dest
					Block Type 1 - Compressed - Copy N+3 Bytes from Dest-Disp-1 to Dest
					  Bit 0-3   Disp MSBs
					  Bit 4-7   Number of bytes to copy (minus 3)
					  Bit 8-15  Disp LSBs
				 */
				#endregion
				var r = ROM.Instance.reader;
				var magicByte = 0x10;

				long readBytes = 0;
				byte type = r.ReadByte();

				if( type != magicByte )
					throw new InvalidDataException( "The provided stream is not a valid LZ-0x10 "
								+ "compressed stream (invalid type 0x" + type.ToString( "X" ) + ")" );

				byte[] sizeBytes = r.ReadBytes( 3 );
				int decompressedSize = Unwrap( sizeBytes );
				readBytes += 4;

				if( decompressedSize == 0 )
				{
					sizeBytes = r.ReadBytes( 4 );
					decompressedSize = Unwrap( sizeBytes );
					readBytes += 4;
				}

				// the maximum 'DISP-1' is 0xFFF.
				int bufferLength = 0x1000;
				byte[] buffer = new byte[bufferLength];
				int bufferOffset = 0;


				int currentOutSize = 0;
				int flags = 0, mask = 1;
				while( currentOutSize < decompressedSize )
				{
					// (throws when requested new flags byte is not available)
					#region Update the mask. If all flag bits have been read, get a new set.
					// the current mask is the mask used in the previous run. So if it masks the
					// last flag bit, get a new flags byte.
					if( mask == 1 )
					{
						flags = r.ReadByte();
						readBytes++;
						if( flags < 0 )
							throw new Exception( "The end of the stream was reached before the given amout of data was read." );
						mask = 0x80;
					}
					else
					{
						mask >>= 1;
					}
					#endregion

					// bit = 1 <=> compressed.
					if( (flags & mask) > 0 )
					{
						// (throws when < 2 bytes are available)
						#region Get length and displacement('disp') values from next 2 bytes
						// there are < 2 bytes available when the end is at most 1 byte away
						int byte1 = r.ReadByte();
						readBytes++;
						int byte2 = r.ReadByte();
						readBytes++;
						if( byte2 < 0 )
							throw new Exception( "The end of the stream was reached before the given amout of data was read." );

						// the number of bytes to copy
						int length = byte1 >> 4;
						length += 3;

						// from where the bytes should be copied (relatively)
						int disp = ((byte1 & 0x0F) << 8) | byte2;
						disp += 1;

						if( disp > currentOutSize )
							throw new InvalidDataException( "Cannot go back more than already written. "
									+ "DISP = 0x" + disp.ToString( "X" ) + ", #written bytes = 0x" + currentOutSize.ToString( "X" )
									+ " at 0x" + (r.Position - 2).ToString( "X" ) );
						#endregion

						int bufIdx = bufferOffset + bufferLength - disp;
						for( int i = 0; i < length; i++ )
						{
							byte next = buffer[bufIdx % bufferLength];
							bufIdx++;
							outstream.WriteByte( next );
							buffer[bufferOffset] = next;
							bufferOffset = (bufferOffset + 1) % bufferLength;
						}
						currentOutSize += length;
					}
					else
					{
						int next = r.ReadByte();
						readBytes++;
						if( next < 0 )
							throw new Exception( "The end of the stream was reached before the given amout of data was read." );

						currentOutSize++;
						outstream.WriteByte( (byte)next );
						buffer[bufferOffset] = (byte)next;
						bufferOffset = (bufferOffset + 1) % bufferLength;
					}
					outstream.Flush();
				}

				return decompressedSize;
			}

			/// <summary>
			/// Compresses the input using the 'original', unoptimized compression algorithm.
			/// This algorithm should yield files that are the same as those found in the games.
			/// (delegates to the optimized method if LookAhead is set)
			/// </summary>
			public static unsafe int Compress( int inLength, Stream outstream, bool lookAhead )
			{
				var r = ROM.Instance.reader;
				byte magicByte = 0x10;
				// make sure the decompressed size fits in 3 bytes.
				// There should be room for four bytes, however I'm not 100% sure if that can be used
				// in every game, as it may not be a built-in function.
				if( inLength > 0xFFFFFF )
					throw new Exception( "The compression ratio is not high enough to fit the input in a single compressed file." );

				// use the other method if lookahead is enabled
				if( lookAhead )
				{
					return CompressWithLA( inLength, outstream );
				}

				// save the input data in an array to prevent having to go back and forth in a file
				byte[] indata = new byte[inLength];

				try
				{
					indata = r.ReadBytes( (int)inLength );
				}
				catch
				{
					throw new Exception( "The end of the stream was reached before the given amout of data was read." );
				}

				// write the compression header first
				outstream.WriteByte( magicByte );
				outstream.WriteByte( (byte)(inLength & 0xFF) );
				outstream.WriteByte( (byte)((inLength >> 8) & 0xFF) );
				outstream.WriteByte( (byte)((inLength >> 16) & 0xFF) );

				int compressedLength = 4;

				fixed ( byte* instart = &indata[0] )
				{
					// we do need to buffer the output, as the first byte indicates which blocks are compressed.
					// this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
					byte[] outbuffer = new byte[8 * 2 + 1];
					outbuffer[0] = 0;
					int bufferlength = 1, bufferedBlocks = 0;
					int readBytes = 0;
					while( readBytes < inLength )
					{
						#region If 8 blocks are bufferd, write them and reset the buffer
						// we can only buffer 8 blocks at a time.
						if( bufferedBlocks == 8 )
						{
							outstream.Write( outbuffer, 0, bufferlength );
							compressedLength += bufferlength;
							// reset the buffer
							outbuffer[0] = 0;
							bufferlength = 1;
							bufferedBlocks = 0;
						}
						#endregion

						// determine if we're dealing with a compressed or raw block.
						// it is a compressed block when the next 3 or more bytes can be copied from
						// somewhere in the set of already compressed bytes.
						int disp;
						int oldLength = Math.Min( readBytes, 0x1000 );
						int length = GetOccurrenceLength( instart + readBytes, (int)Math.Min( inLength - readBytes, 0x12 ),
															  instart + readBytes - oldLength, oldLength, out disp, 1 );

						// length not 3 or more? next byte is raw data
						if( length < 3 )
						{
							outbuffer[bufferlength++] = *(instart + (readBytes++));
						}
						else
						{
							// 3 or more bytes can be copied? next (length) bytes will be compressed into 2 bytes
							readBytes += length;

							// mark the next block as compressed
							outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

							outbuffer[bufferlength] = (byte)(((length - 3) << 4) & 0xF0);
							outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
							bufferlength++;
							outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
							bufferlength++;
						}
						bufferedBlocks++;
					}

					// copy the remaining blocks to the output
					if( bufferedBlocks > 0 )
					{
						outstream.Write( outbuffer, 0, bufferlength );
						compressedLength += bufferlength;
					}
				}

				return compressedLength;
			}

			/// <summary>
			/// Variation of the original compression method, making use of Dynamic Programming to 'look ahead'
			/// and determine the optimal 'length' values for the compressed blocks. Is not 100% optimal,
			/// as the flag-bytes are not taken into account.
			/// </summary>
			private static unsafe int CompressWithLA( long inLength, Stream outstream )
			{
				var r = ROM.Instance.reader;
				byte magicByte = 0x10;
				// save the input data in an array to prevent having to go back and forth in a file
				byte[] indata = new byte[inLength];
				try
				{
					indata = r.ReadBytes( (int)inLength );
				}
				catch
				{
					throw new Exception( "The end of the stream was reached before the given amout of data was read." );
				}

				// write the compression header first
				outstream.WriteByte( magicByte );
				outstream.WriteByte( (byte)(inLength & 0xFF) );
				outstream.WriteByte( (byte)((inLength >> 8) & 0xFF) );
				outstream.WriteByte( (byte)((inLength >> 16) & 0xFF) );

				int compressedLength = 4;

				fixed ( byte* instart = &indata[0] )
				{
					// we do need to buffer the output, as the first byte indicates which blocks are compressed.
					// this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
					byte[] outbuffer = new byte[8 * 2 + 1];
					outbuffer[0] = 0;
					int bufferlength = 1, bufferedBlocks = 0;
					int readBytes = 0;

					// get the optimal choices for len and disp
					int[] lengths, disps;
					GetOptimalCompressionLengths( instart, indata.Length, out lengths, out disps );

					while( readBytes < inLength )
					{
						// we can only buffer 8 blocks at a time.
						if( bufferedBlocks == 8 )
						{
							outstream.Write( outbuffer, 0, bufferlength );
							compressedLength += bufferlength;
							// reset the buffer
							outbuffer[0] = 0;
							bufferlength = 1;
							bufferedBlocks = 0;
						}


						if( lengths[readBytes] == 1 )
						{
							outbuffer[bufferlength++] = *(instart + (readBytes++));
						}
						else
						{
							// mark the next block as compressed
							outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

							outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 3) << 4) & 0xF0);
							outbuffer[bufferlength] |= (byte)(((disps[readBytes] - 1) >> 8) & 0x0F);
							bufferlength++;
							outbuffer[bufferlength] = (byte)((disps[readBytes] - 1) & 0xFF);
							bufferlength++;

							readBytes += lengths[readBytes];
						}


						bufferedBlocks++;
					}

					// copy the remaining blocks to the output
					if( bufferedBlocks > 0 )
					{
						outstream.Write( outbuffer, 0, bufferlength );
						compressedLength += bufferlength;
					}
				}

				return compressedLength;
			}

			/// <summary>
			/// Gets the optimal compression lengths for each start of a compressed block using Dynamic Programming.
			/// This takes O(n^2) time.
			/// </summary>
			/// <param name="indata">The data to compress.</param>
			/// <param name="inLength">The length of the data to compress.</param>
			/// <param name="lengths">The optimal 'length' of the compressed blocks. For each byte in the input data,
			/// this value is the optimal 'length' value. If it is 1, the block should not be compressed.</param>
			/// <param name="disps">The 'disp' values of the compressed blocks. May be 0, in which case the
			/// corresponding length will never be anything other than 1.</param>
			private static unsafe void GetOptimalCompressionLengths( byte* indata, int inLength, out int[] lengths, out int[] disps )
			{
				lengths = new int[inLength];
				disps = new int[inLength];
				int[] minLengths = new int[inLength];

				for( int i = inLength - 1; i >= 0; i-- )
				{
					// first get the compression length when the next byte is not compressed
					minLengths[i] = int.MaxValue;
					lengths[i] = 1;
					if( i + 1 >= inLength )
						minLengths[i] = 1;
					else
						minLengths[i] = 1 + minLengths[i + 1];
					// then the optimal compressed length
					int oldLength = Math.Min( 0x1000, i );
					// get the appropriate disp while at it. Takes at most O(n) time if oldLength is considered O(n)
					// be sure to bound the input length with 0x12, as that's the maximum length for LZ-10 compressed blocks.
					int maxLen = GetOccurrenceLength( indata + i, Math.Min( inLength - i, 0x12 ),
													 indata + i - oldLength, oldLength, out disps[i], 1 );
					if( disps[i] > i )
						throw new Exception( "disp is too large" );
					for( int j = 3; j <= maxLen; j++ )
					{
						int newCompLen;
						if( i + j >= inLength )
							newCompLen = 2;
						else
							newCompLen = 2 + minLengths[i + j];
						if( newCompLen < minLengths[i] )
						{
							lengths[i] = j;
							minLengths[i] = newCompLen;
						}
					}
				}

				// we could optimize this further to also optimize it with regard to the flag-bytes, but that would require 8 times
				// more space and time (one for each position in the block) for only a potentially tiny increase in compression ratio.
			}

			/// <summary>
			/// Determine the maximum size of a LZ-compressed block starting at newPtr, using the already compressed data
			/// starting at oldPtr. Takes O(inLength * oldLength) = O(n^2) time.
			/// </summary>
			/// <param name="newPtr">The start of the data that needs to be compressed.</param>
			/// <param name="newLength">The number of bytes that still need to be compressed.
			/// (or: the maximum number of bytes that _may_ be compressed into one block)</param>
			/// <param name="oldPtr">The start of the raw file.</param>
			/// <param name="oldLength">The number of bytes already compressed.</param>
			/// <param name="disp">The offset of the start of the longest block to refer to.</param>
			/// <param name="minDisp">The minimum allowed value for 'disp'.</param>
			/// <returns>The length of the longest sequence of bytes that can be copied from the already decompressed data.</returns>
			private static unsafe int GetOccurrenceLength( byte* newPtr, int newLength, byte* oldPtr, int oldLength, out int disp, int minDisp )
			{
				disp = 0;
				if( newLength == 0 )
					return 0;
				int maxLength = 0;
				// try every possible 'disp' value (disp = oldLength - i)
				for( int i = 0; i < oldLength - minDisp; i++ )
				{
					// work from the start of the old data to the end, to mimic the original implementation's behaviour
					// (and going from start to end or from end to start does not influence the compression ratio anyway)
					byte* currentOldStart = oldPtr + i;
					int currentLength = 0;
					// determine the length we can copy if we go back (oldLength - i) bytes
					// always check the next 'newLength' bytes, and not just the available 'old' bytes,
					// as the copied data can also originate from what we're currently trying to compress.
					for( int j = 0; j < newLength; j++ )
					{
						// stop when the bytes are no longer the same
						if( *(currentOldStart + j) != *(newPtr + j) )
							break;
						currentLength++;
					}

					// update the optimal value
					if( currentLength > maxLength )
					{
						maxLength = currentLength;
						disp = oldLength - i;

						// if we cannot do better anyway, stop trying.
						if( maxLength == newLength )
							break;
					}
				}
				return maxLength;
			}
		}
	}

	//Because I just like extension methods
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
