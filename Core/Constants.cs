using System;
using System.IO;
using System.Reflection;

namespace MinishMaker.Core
{
	public enum RegionVersion
	{
		EU,
		JP,
		US,
		None
	}

	public struct HeaderData
	{
		public int MapHeaderBase;
		// Name on this is gonna have to change sometime
		public int AreaMetadataBase;

		//added for now, change names to whatever you want
		public int tileOffset;
		public int paletteSetTableLoc;
		public int chunk0TableLoc;
		public int area1Chunk0TableLoc;
		public int chunk1TableLoc;
		public int chunk2TableLoc;
		public int swapBase;
		public int paletteChangeBase;
		public int area1SwapBase;
		public int globalTileSetTableLoc;
		public int gfxSourceBase;
		public int globalMetaTileSetTableLoc;
		public int globalTileDataTableLoc;

		public HeaderData( int map, int area, int tileOffset, int paletteSetTableLoc, int c0TableLoc, int a1C0TableLoc, int c1TableLoc, int c2TableLoc, int swapBase, int paletteChangeBase, int area1SwapBase, int globalTileSetTableLoc, int gfxSourceBase, int globalMetaTileSetTableLoc, int globalTileDataTableLoc )
		{
			this.MapHeaderBase = map;
			this.AreaMetadataBase = area;
			this.tileOffset = tileOffset;
			this.paletteSetTableLoc = paletteSetTableLoc;
			this.chunk0TableLoc = c0TableLoc; //looks like each next chunkTable is a 0x16 further (eu and us), not adding because of possible jp
			this.area1Chunk0TableLoc = a1C0TableLoc;
			this.chunk1TableLoc = c1TableLoc; //c0+ 0x16
			this.chunk2TableLoc = c2TableLoc; //c0+ 0x32
			this.swapBase = swapBase;
			this.paletteChangeBase = paletteChangeBase;
			this.area1SwapBase = area1SwapBase; //above -0x140?
			this.globalTileSetTableLoc = globalTileSetTableLoc;
			this.gfxSourceBase = gfxSourceBase;
			this.globalMetaTileSetTableLoc = globalMetaTileSetTableLoc;
			this.globalTileDataTableLoc = globalTileDataTableLoc;
		}
	}

	public class Header
	{
		// Will fill out when relevant, only need EU for now
		private readonly HeaderData[] headerTable = new HeaderData[]
		{   //             MAP     , AREA	,	TILEOFFSET	PALETTESET	CHUNK0		CHUNK0AREA1	CHUNK1		CHUNK2		SWAP		PALETTECHANGE	AREA1SWAP	TILESET		GFXSOURCE	METATILE	TILEDATA
            new HeaderData(0x11D95C, 0x0D4828,  0x5A23D0,   0xFED88,    0x107AEC,   0x1077AC,   0x107B02,   0x107B18,   0x107B5C,   0x107940,       0x107800,   0x101BC8,   0x323FEC,   0x1027F8,   0x1070E4),
			new HeaderData(0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0),
			new HeaderData(0x11E214, 0,0,0,0,0,0,0,0,0,0,0,0,0,0)
		};

		public HeaderData GetHeaderAddresses( RegionVersion region )
		{
			return headerTable[(int)region];
		}
	}



	public class Space
	{
		
		public struct SpaceData
		{
			public uint start;
			public uint size;

			public SpaceData(uint start, uint size)
			{
				this.start=start;
				this.size=size;
			}
		}

		private static SpaceData[] spaceData = null;

		public static SpaceData[]Spaces
		{
			get
			{
				if( spaceData == null )
				{
					spaceData = LoadSpaces();
				}

				return spaceData;
			}
		}

		private static SpaceData[] LoadSpaces()
		{

			string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\voidFile.txt");
			var text = System.IO.File.ReadAllText(path);
			var data = text.Split(',');
			if(data.Length%2!=0)
			{
				throw new System.Exception("Incorrect void file, not an even number of adresses (start,end)");
			}
			var voids = new SpaceData[data.Length/2];
			for(int i = 0; i<data.Length; i+=2)
			{
				var start = Convert.ToUInt32(data[i],16);
				var end = Convert.ToUInt32(data[i+1],16);
				var size = end-start;
				voids[i/2]= new SpaceData(start,size);
			}

			return voids;
		}
	}
}
