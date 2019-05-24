using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using static MinishMaker.Core.RoomMetaData;
using static MinishMaker.UI.MainWindow;

namespace MinishMaker.Core
{
    public class Room
    {
        public int Index { get; private set; }
		public bool Loaded { get; private set;}

		private RoomMetaData metadata;
		private TileSet tset;
		private PaletteSet pset;
		private MetaTileSet bg2MetaTiles;
		private MetaTileSet bg1MetaTiles;
		private byte[] bg2RoomData;
		private byte[] bg1RoomData;
		private bool bg2Exists = false;
		private bool bg1Exists = false;
		

        public Room(int roomIndex)
        {
            Index = roomIndex;
			Loaded = false;
        }

		private void LoadRoom(int areaIndex)
		{
			metadata = new RoomMetaData( areaIndex, this.Index );

			//build tileset using tile addrs from room metadata
			tset = metadata.GetTileSet();

			//palettes
			pset = metadata.GetPaletteSet();

			//room data
			bg2RoomData = new byte[0x2000]; //this seems to be the maximum size
			bg1RoomData = new byte[0x2000]; //2000 isnt too much memory, so this is just easier

			bg2Exists = metadata.GetBG2Data( ref bg2RoomData, ref bg2MetaTiles );//loads in the data and tiles
			bg1Exists = metadata.GetBG1Data( ref bg1RoomData, ref bg1MetaTiles );//loads in the data, tiles and sets bg1Use20344B0

			Loaded = true; //do not load data a 2nd time for this room
		}

		public Bitmap[] DrawRoom( int areaIndex, bool showBg1, bool showBg2 )
		{
			if(Loaded == false)
			{
				LoadRoom(areaIndex);
			}

			//draw using screens
			Bitmap bg1 = new Bitmap( metadata.PixelWidth, metadata.PixelHeight, PixelFormat.Format32bppArgb );
			Bitmap bg2 = new Bitmap( metadata.PixelWidth, metadata.PixelHeight, PixelFormat.Format32bppArgb );

			if( showBg2 && bg2Exists )
				DrawLayer( ref bg2, areaIndex, bg2MetaTiles, bg2RoomData, false );
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
					DrawLayer( ref bg1, areaIndex, bg1MetaTiles, bg1RoomData, false );
				}
			}
			return new Bitmap[] { bg1, bg2 };
		}

		public void DrawTile( ref Bitmap b, Point p, int areaIndex, int layer, int tileNum)
		{
			if(layer==1)
			{
				bg1MetaTiles.DrawMetaTile(ref b,p,tset,pset,tileNum,true);
			}
			if(layer==2)
			{
				bg2MetaTiles.DrawMetaTile(ref b,p,tset,pset,tileNum,true);
			}
		}

		private void DrawLayer( ref Bitmap b, int areaIndex, MetaTileSet metaTiles, byte[] roomData, bool overwrite )
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
					if( Index == 00 && areaIndex == 01 || areaIndex == 02 || areaIndex == 0x15 )
					{
						oldchunks = chunks;
						chunks = GetChunks( areaIndex, (ushort)(i * 16), (ushort)(j * 16) );

						SwapTiles( areaIndex, oldchunks, chunks, (ushort)(i * 16), (ushort)(j * 16) );
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
			{ 
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
		}

		private void SwapTiles( int areaIndex, ushort[] oldchunks, ushort[] newchunks, ushort x, ushort y )
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

		private UInt16[] GetChunks( int areaIndex, UInt16 x, UInt16 y )
		{
			UInt16[] ret = new ushort[3];
			var header = ROM.Instance.headers;

			//chunk 00
			int addr;
			switch( areaIndex )
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
		
		public int GetTileData(int layer, int position)
		{
			if(layer==1)//bg1
			{
				return bg1RoomData[position]|bg1RoomData[position+1]<<8;
			}
			if(layer==2)//bg2
			{
				return bg2RoomData[position]|bg2RoomData[position+1]<<8;
			}
			return -1;
		}
		
		public void SetTileData(int layer, int position, int data)
		{
			byte high = (byte)(data >> 8);
			byte low = (byte)(data & 0xFF);
			if(layer==1)//bg1
			{
				bg1RoomData[position]=low;
				bg1RoomData[position+1]=high;
			}
			if(layer==2)//bg2
			{
				bg2RoomData[position]=low;
				bg2RoomData[position+1]=high;
			}
		}

		public long CompressRoomData(ref byte[] data, DataType type )
		{
			if(type == DataType.bg1Data)
				return metadata.CompressBG1(ref data, bg1RoomData);

			if(type == DataType.bg2Data)
				return metadata.CompressBG2(ref data, bg2RoomData);

			return 0;
		}

		public int GetPointerLoc( DataType type , int areaIndex)
		{
			if(metadata==null)
				LoadRoom(areaIndex);

			return metadata.GetPointerLoc(type, areaIndex, Index);
		}
		//confusing myself over the bitmap size logic
		/// <summary>
		/// Draw images of all metatiles in the metatilesets
		/// </summary>
		public Bitmap[] DrawTilesetImages(int tilesPerRow, int areaIndex)
		{
			var totalValues = 0x100;// amount different low values (00-FF)
			var tileSize = 16;// 0x10 pixels
			var rowsRequiredPerHigh = (int)Math.Ceiling((double)((totalValues/tilesPerRow)));//how many rows are needed
			var highValuesUsed = 8; //minish woods stops at 07 FE, so does hyrule town
			int rowsRequired = (int)((highValuesUsed+0.5) * rowsRequiredPerHigh); //some leeway as the scrollbar doesnt do precision or the bitmap isnt large enough

			var twidth = tileSize * tilesPerRow;
			var theight = tileSize * rowsRequired;
			
			Bitmap bg1 = new Bitmap( twidth, theight, PixelFormat.Format32bppArgb );
			Bitmap bg2 = new Bitmap( twidth, theight, PixelFormat.Format32bppArgb );

			//commented code here is to be re-enabled once fully understood and changed
			//ushort[] chunks = new ushort[3];
			//ushort[] oldchunks = new ushort[3];
			//chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };

			var pos = 0;
			bool ended = false;
			for( int j = 0; j < rowsRequired; j++ )
			{
				if( ended )
					break;
				for( int i = 0; i < tilesPerRow; i++ )
				{

					//hardcoded because there is no easy way to determine which areas use tileswapping
					//if( Index == 00 && areaIndex == 01 || areaIndex == 02 || areaIndex == 0x15 )
					//{
					//	oldchunks = chunks;
					//	chunks = GetChunks( areaIndex, (ushort)(i * 16), (ushort)(j * 16) );
					//
					//	SwapTiles( areaIndex, oldchunks, chunks, (ushort)(i * 16), (ushort)(j * 16) );
					//}

					try
					{
						if( pos != 0xFFFF )
						{
							bg1MetaTiles.DrawMetaTile( ref bg1, new Point( i * 16, j * 16 ), tset, pset, pos, true );
							bg2MetaTiles.DrawMetaTile( ref bg2, new Point( i * 16, j * 16 ), tset, pset, pos, true );
						}
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

			return new Bitmap[]{ bg1, bg2 };
		}

		public List<ChestData> GetChestData()
		{
			return metadata.ChestInfo;
		}
	}
}
