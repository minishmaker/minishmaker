namespace MinishMaker.Core
{
    public class Constants
    {
        public static readonly string _listData = "listData";
    }
 
    public enum RegionVersion
    {
        EU,
        JP,
        US,
        None
    }

    public struct HeaderData
    {
        public int MapInfoRoot;
        // Name on this is gonna have to change sometime
        public int ListTableRoot;

        //added for now, change names to whatever you want
        public int tileOffset;
        public int paletteSetTableLoc;
        public int chunk0TableLoc;
        public int area1Chunk0TableLoc;
        public int chunk1TableLoc;
        public int chunk2TableLoc;
        public int swapBase;
        public int paletteBase;
        public int area1SwapBase;
        public int metaTilesetRoot;
        public int gfxSourceBase;
        public int globalTilesetTableLoc;
        public int TileDataRoot;
        public int areaInformationTableLoc;
        public int WarpTableRoot;
        public int languageTableLoc;

        public HeaderData(int map, int area, int tileOffset, int paletteSetTableLoc, int c0TableLoc, int a1C0TableLoc, int c1TableLoc, int c2TableLoc, int swapBase, int paletteBase, int area1SwapBase, int globalMetaTilesetTableLoc, int gfxSourceBase, int globalTilesetTableLoc, int globalTileDataTableLoc, int areaInformationTableLoc, int warpInformationTableLoc, int languageTableLoc)
        {
            this.MapInfoRoot = map;
            this.ListTableRoot = area;
            this.tileOffset = tileOffset;
            this.paletteSetTableLoc = paletteSetTableLoc;
            this.chunk0TableLoc = c0TableLoc; //looks like each next chunkTable is a 0x16 further (eu and us), not adding because of possible jp
            this.area1Chunk0TableLoc = a1C0TableLoc;
            this.chunk1TableLoc = c1TableLoc; //c0+ 0x16
            this.chunk2TableLoc = c2TableLoc; //c0+ 0x32
            this.swapBase = swapBase;
            this.paletteBase = paletteBase;
            this.area1SwapBase = area1SwapBase; //above -0x140?
            this.metaTilesetRoot = globalMetaTilesetTableLoc;
            this.gfxSourceBase = gfxSourceBase;
            this.globalTilesetTableLoc = globalTilesetTableLoc;
            this.TileDataRoot = globalTileDataTableLoc;
            this.areaInformationTableLoc = areaInformationTableLoc;
            this.WarpTableRoot = warpInformationTableLoc;
            this.languageTableLoc = languageTableLoc;
        }
    }

    public class Header
    {
        // Will fill out when relevant, only need EU for now
        private readonly HeaderData[] headerTable =
        {   //             MAP     , ENTITY?,	TILEOFFSET	PALETTESET	CHUNK0		CHUNK0AREA1	CHUNK1		CHUNK2		SWAP		PALETTECHANGE	AREA1SWAP	TILESET 	GFXSOURCE	METATILE 	TILEDATA	AREADATA	WARP		LANGUAGE
  /*EU*/    new HeaderData(0x11D95C, 0x0D4828,  0x5A23D0,   0x0FED88,   0x107AEC,   0x1077AC,   0x107B02,   0x107B18,   0x107B5C,   0x107940,       0x107800,   0x101BC8,   0x323FEC,   0x1027F8,   0x1070E4,   0x127468,   0x139EDC,   0x108968),
  /*JP*/    new HeaderData(0x11DED8, 0x0D4E9C,  0x5A2B20,   0x0FF500,   0x10805C,   0x107D18,   0x108072,   0x108088,   0x1080CC,   0x107EAC,       0x107D6C,   0x102134,   0x324710,   0x102D64,   0x107650,   0x1279F4,   0x13A41C,   0x108ED8),//0x109214
  /*US*/    new HeaderData(0x11E214, 0x0D50FC,  0x5A2E80,   0x0FF850,   0x108398,   0x108050,   0x1083AE,   0x1083C4,   0x108408,   0x1081E4,       0x1080A4,   0x10246C,   0x324AE4,   0x10309C,   0x107988,   0x127D30,   0x13A7F0,   0x109214)//0x108ED8
        };

        public HeaderData GetHeaderAddresses(RegionVersion region)
        {
            return headerTable[(int)region];
        }
    }
}
