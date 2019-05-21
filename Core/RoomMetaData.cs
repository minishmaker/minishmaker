using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinishMaker.Utilities;
namespace MinishMaker.Core
{
	public class RoomMetaData
	{
		private int width, height;
		public int PixelWidth
		{
			get
			{
				return width * 16;
			}
		}

		public int PixelHeight
		{
			get
			{
				return height * 16;
			}
		}

		public int TileWidth
		{
			get
			{
				return width;
			}
		}

		public int TileHeight
		{
			get
			{
				return height;
			}
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
			get
			{
				return bg1Use20344B0;
			}
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
			return new TileSet( tileSetAddrs );
		}

		public PaletteSet GetPaletteSet()
		{
			return new PaletteSet( paletteSetID );
		}

		public bool GetBG2Data( ref byte[] bg2RoomData, ref MetaTileSet bg2MetaTiles )
		{
			if( bg2RoomDataAddr != null )
			{
				bg2MetaTiles = new MetaTileSet( bg2MetaTilesAddr, false );

				var data = DataHelper.GetData( (AddrData)bg2RoomDataAddr );
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
					DataHelper.GetData( (AddrData)bg1RoomDataAddr ).CopyTo( bg1RoomData, 0 );
					bg1Use20344B0 = true;
				}
				else
				{
					bg1MetaTiles = new MetaTileSet( bg1MetaTilesAddr , true );
					DataHelper.GetData( (AddrData)bg1RoomDataAddr ).CopyTo( bg1RoomData, 0 );
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

				

				if( data2 == 0 )
				{ //palette
					this.paletteSetID = (int)(data & 0x7FFFFFFF); //mask off high bit
				}
				else
				{
					int source = (int)((data & 0x7FFFFFFF) + header.gfxSourceBase); //8324AE4 is tile gfx base
					int dest = (int)(data2 & 0x7FFFFFFF);
					bool compressed = (data3 & 0x80000000) != 0; //high bit of size determines LZ or DMA
					int size = (int)(data3 & 0x7FFFFFFF);

					cont = postFunc( new AddrData( source, dest, size, compressed ) );
				}
				if(cont == true)
				{
					cont = (data & 0x80000000) != 0; //high bit determines if more to load
				}
			}
		}

		public void SaveBG1(byte[] bg1data)
		{
			var data = new byte[bg1data.Length];
			long totalSize = 0;
			MemoryStream ous = new MemoryStream( data );
			totalSize = DataHelper.Compress(bg1data, ous, false);

			var compressedData = new byte[totalSize];
			Array.Copy(data,compressedData,totalSize);
			//var sizeDifference = totalSize - bg1RoomDataAddr.Value.size;

			using( MemoryStream s = new MemoryStream( ROM.Instance.romData ) )
			{
				var w = new Writer( s );
				w.SetPosition(bg1RoomDataAddr.Value.src);
				w.WriteBytes(compressedData);
			}
		}

		public void SaveBG2(byte[] bg2data)
		{
			var data = new byte[bg2data.Length];
			long totalSize = 0;
			MemoryStream ous = new MemoryStream( data );
			totalSize = DataHelper.Compress(bg2data, ous, false);
			
			var compressedData = new byte[totalSize];
			Array.Copy(data,compressedData,totalSize);
			//var sizeDifference = totalSize - bg2RoomDataAddr.Value.size;

			using( MemoryStream s = new MemoryStream( ROM.Instance.romData ) )
			{
				var w = new Writer( s );
				w.SetPosition( ((AddrData)bg2RoomDataAddr).src);
				w.WriteBytes(compressedData);
			}
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
}
