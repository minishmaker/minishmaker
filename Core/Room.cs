using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using MinishMaker.Utilities;
using static MinishMaker.Core.RoomMetaData;

namespace MinishMaker.Core
{
    public class Room
    {
        public int Index { get; private set; }
        public bool Loaded { get; private set; }

        private RoomMetaData metadata;
        private TileSet tset;
        public TileSet tileSet
        {
            get { return tset; }
        }

        public Point roomSize
        {
            get { return new Point(metadata.TileWidth, metadata.TileHeight); }
        }
        private PaletteSet pset;
        public Color[] palettes
        {
            get { return pset.Palettes; }
        }

        public string paletteString
        {
            get
            {
                return pset.ToPaletteString();
            }
        }

        public bool Bg1Exists
        {
            get { return bg1Exists; }
        }

        public bool Bg2Exists
        {
            get { return bg2Exists; }
        }

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

        public void LoadRoom(int areaIndex)
        {
            metadata = new RoomMetaData(areaIndex, this.Index);

            //build tileset using tile addrs from room metadata
            tset = metadata.GetTileSet();

            //palettes
            pset = metadata.GetPaletteSet();

            //room data
            bg2RoomData = new byte[0x2000]; //this seems to be the maximum size
            bg1RoomData = new byte[0x2000]; //2000 isnt too much memory, so this is just easier

            bg2Exists = metadata.GetBG2Data(ref bg2RoomData, ref bg2MetaTiles);//loads in the data and tiles
            bg1Exists = metadata.GetBG1Data(ref bg1RoomData, ref bg1MetaTiles);//loads in the data, tiles and sets bg1Use20344B0

            if (!bg1Exists && !bg2Exists)
            {
                throw new RoomException("No layers exist for this room, the room is highly likely invalid.");
            }

            Loaded = true; //do not load data a 2nd time for this room
        }

        public Bitmap[] DrawRoom(int areaIndex, bool showBg1, bool showBg2)
        {
            if (Loaded == false)
            {
                LoadRoom(areaIndex);
            }

            //draw using screens
            Bitmap bg1 = new Bitmap(metadata.PixelWidth, metadata.PixelHeight, PixelFormat.Format32bppArgb);
            Bitmap bg2 = new Bitmap(metadata.PixelWidth, metadata.PixelHeight, PixelFormat.Format32bppArgb);

            if (showBg2 && bg2Exists)
                DrawLayer(ref bg2, areaIndex, bg2MetaTiles, bg2RoomData, false);
            if (showBg1 && bg1Exists)
            {
                if (metadata.Bg1Use20344B0)
                {
                    //hacky way to draw with tilemap, instead of using metatiles
                    //this should be handled by the Screen class probably
                    DrawSpecialLayer(ref bg1, bg1RoomData, false);
                }
                else
                {
                    DrawLayer(ref bg1, areaIndex, bg1MetaTiles, bg1RoomData, false);
                }
            }
            return new Bitmap[] { bg1, bg2 };
        }

        public void DrawTile(ref Bitmap b, Point p, int areaIndex, int layer, int tileNum)
        {
            if (layer == 1)
            {
                bg1MetaTiles.DrawMetaTile(ref b, p, tset, pset, tileNum, true);
            }
            if (layer == 2)
            {
                bg2MetaTiles.DrawMetaTile(ref b, p, tset, pset, tileNum, true);
            }
        }

        private void DrawLayer(ref Bitmap b, int areaIndex, MetaTileSet metaTiles, byte[] roomData, bool overwrite)
        {
            int pos = 0; //position in roomData
            ushort[] chunks = new ushort[3];
            ushort[] oldchunks = new ushort[3];
            chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };
            int badTiles = 0;

            for (int j = 0; j < metadata.TileHeight; j++)
            {
                for (int i = 0; i < metadata.TileWidth; i++)
                {
                    //hardcoded because there is no easy way to determine which areas use tileswapping
                    if (Index == 00 && areaIndex == 01 || areaIndex == 02 || areaIndex == 0x15)
                    {
                        oldchunks = chunks;
                        chunks = GetChunks(areaIndex, (ushort)(i * 16), (ushort)(j * 16));

                        SwapTiles(areaIndex, oldchunks, chunks, (ushort)(i * 16), (ushort)(j * 16));
                    }
                    //which metatile to draw
                    int mt = roomData[pos] | (roomData[pos + 1] << 8);

                    pos += 2; //2 bytes per tile
                    if (metaTiles.GetTileInfo(mt) == null)
                    {
                        badTiles++;
                        continue;
                    }
                    try
                    {
                        if (mt != 0xFFFF) //nonexistant room data does this, eg. area 0D room 10
                            metaTiles.DrawMetaTile(ref b, new Point(i * 16, j * 16), tset, pset, mt, overwrite);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error drawing metatile. i:" + i.ToString() + ", j:" + j.ToString()
                                            + "\n" + e.Message, e);
                    }
                }
            }
            if (badTiles > 0)
            {
                throw new RoomException("Found " + badTiles + " bad tiles while trying to draw them, the room may be unused.");
            }
        }

        //used for 0D-10, etc.
        private void DrawSpecialLayer(ref Bitmap b, byte[] tileMap, bool overwrite)
        {
            int pos = 0; //position in tileMap
            for (int j = 0; j < 0x17; j++)
            {
                for (int i = 0; i < 32; i++)
                {
                    ushort data = (ushort)(tileMap[pos] | (tileMap[pos + 1] << 8));
                    pos += 2;
                    int tnum = (int)(data & 0x3FF);
                    tnum += 0x200; //because it's bg1 and base is 6004000 not 6000000
                    int pnum = (int)(data >> 12);
                    bool hflip = (data & 0x400) != 0;
                    bool vflip = (data & 0x800) != 0;

                    tset.DrawQuarterTile(ref b, new Point(i * 8, j * 8), tnum, pset.Palettes, pnum, hflip, vflip, overwrite);
                }
            }
        }

        private void SwapTiles(int areaIndex, ushort[] oldchunks, ushort[] newchunks, ushort x, ushort y)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int updatepal = -1;

            for (int i = 0; i < 3; i++)
            {
                if (newchunks[i] == oldchunks[i])
                {
                    continue;
                }

                if (newchunks[i] == 0x00FF)
                {
                    continue;
                }

                int baseaddr, src, src2, dest, dest2;
                byte[] newtiles = new byte[0x1000];

                switch (areaIndex)
                {
                    case 0x02:
                    case 0x15:
                        baseaddr = areaIndex != 0x15 ? header.swapBase : header.swapBase + 0x060;
                        r.SetPosition(baseaddr + (newchunks[i] << 4));
                        src = (int)(header.tileOffset + r.ReadUInt32());    //source addrs are stored as offsets from 85A2E80
                        dest = (int)r.ReadUInt32() & 0xFFFFFF;              //mask off the 0x6000000 part
                        src2 = (int)(header.tileOffset + r.ReadUInt32());   //there are 2 sets of tiles for each chunk
                        dest2 = (int)r.ReadUInt32() & 0xFFFFFF;             //one for each bg

                        newtiles = r.ReadBytes(0x1000, src);
                        tset.SetChunk(newtiles, dest);

                        newtiles = r.ReadBytes(0x1000, src2);
                        tset.SetChunk(newtiles, dest2);

                        break;
                    case 0x01: //area 01 works differently, 8 chunks and palette update
                        for (int j = 0; j < 8; j++)
                        {
                            if (j == 0)
                            {//update palette
                                byte pnum = r.ReadByte(header.paletteChangeBase + newchunks[i]);
                                updatepal = (int)pnum;
                            }
                            baseaddr = header.area1SwapBase;
                            r.SetPosition(baseaddr + (newchunks[i] << 6) + (j << 3));

                            src = (header.tileOffset + (int)r.ReadUInt32());
                            dest = (int)r.ReadUInt32() & 0xFFFFFF;

                            newtiles = r.ReadBytes(0x1000, src);
                            tset.SetChunk(newtiles, dest);
                        }
                        break;
                }
            }
            if (updatepal > 0) //if the palette number changed due to swapping, create new paletteset
            {
                pset = new PaletteSet(updatepal);
            }
        }

        private UInt16[] GetChunks(int areaIndex, UInt16 x, UInt16 y)
        {
            UInt16[] ret = new ushort[3];
            var header = ROM.Instance.headers;

            //chunk 00
            int addr;
            switch (areaIndex)
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

            ret[0] = TestChunk(x, y, addr);


            //chunk 01
            if (areaIndex == 0x02)
            { //no chunk 01 for area 15, only area 02
                ret[1] = TestChunk(x, y, header.chunk1TableLoc);
            }
            else
            {
                ret[1] = 0x00FF;
            }


            //chunk 02
            if (areaIndex == 0x02 || areaIndex == 0x15)
            {
                addr = areaIndex != 0x15 ? header.chunk2TableLoc : header.chunk2TableLoc + 0x02E; //area 0x15 (21) uses different hardcoded ptr tbl
                ret[2] = TestChunk(x, y, addr);
            }
            else
            {
                ret[2] = 0x00FF;
            }

            return ret;
        }

        private ushort TestChunk(UInt16 x, UInt16 y, long addr)
        {
            var r = ROM.Instance.reader;
            r.SetPosition(addr);
            UInt16 chnk; //note: this do block is essentially check_swap_inner in IDA
            do
            {
                chnk = r.ReadUInt16();
                if (chnk == 0x00FF)
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

                if (test_x < r2 && test_y < r3)
                    break; //chnk found, so return
            } while (true);
            return chnk;
        }

        public int GetTileData(int layer, int position)
        {
            if (layer == 1)//bg1
            {
                return bg1RoomData[position] | bg1RoomData[position + 1] << 8;
            }
            if (layer == 2)//bg2
            {
                return bg2RoomData[position] | bg2RoomData[position + 1] << 8;
            }
            return -1;
        }

        public void SetTileData(int layer, int position, int data)
        {
            byte high = (byte)(data >> 8);
            byte low = (byte)(data & 0xFF);
            if (layer == 1)//bg1
            {
                bg1RoomData[position] = low;
                bg1RoomData[position + 1] = high;
            }
            if (layer == 2)//bg2
            {
                bg2RoomData[position] = low;
                bg2RoomData[position + 1] = high;
            }
        }

        public long GetSaveData(ref byte[] data, DataType type)
        {
            switch (type)
            {
                case DataType.bg1Data:
                    return metadata.CompressBG1(ref data, bg1RoomData);
                case DataType.bg2Data:
                    return metadata.CompressBG2(ref data, bg2RoomData);
                case DataType.chestData:
                    return metadata.GetChestData(ref data);
                case DataType.bg1MetaTileSet:
                    return bg1MetaTiles.GetCompressedMetaTileSet(ref data);
                case DataType.bg2MetaTileSet:
                    return bg2MetaTiles.GetCompressedMetaTileSet(ref data);
                case DataType.list1Data:
                    return metadata.GetList1Data(ref data);
                case DataType.list2Data:
                    return metadata.GetList2Data(ref data);
                case DataType.list3Data:
                    return metadata.GetList3Data(ref data);
                case DataType.warpData:
                    return metadata.GetWarpData(ref data);
                case DataType.bg1MetaTileType:
                    return bg1MetaTiles.GetCompressedMetaTileTypes(ref data);
                case DataType.bg2MetaTileType:
                    return bg2MetaTiles.GetCompressedMetaTileTypes(ref data);
                case DataType.roomMetaData:
                    return GetMetadata(ref data);
                case DataType.bg1TileSet:
                    return tileSet.GetCompressedTileSetData(ref data, TileSet.TileSetDataType.BG1);
                case DataType.bg2TileSet:
                    return tileSet.GetCompressedTileSetData(ref data, TileSet.TileSetDataType.BG2);
                case DataType.commonTileSet:
                    return tileSet.GetCompressedTileSetData(ref data, TileSet.TileSetDataType.COMMON);
                default:
                    return 0;
            }
        }

        public int GetPointerLoc(DataType type, int areaIndex)
        {
            if (metadata == null)
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
            var rowsRequiredPerHigh = (int)Math.Ceiling((double)((totalValues / tilesPerRow)));//how many rows are needed
            var highValuesUsed = 8; //minish woods stops at 07 FE, so does hyrule town
            int rowsRequired = (int)((highValuesUsed + 0.5) * rowsRequiredPerHigh); //some leeway as the scrollbar doesnt do precision or the bitmap isnt large enough

            var twidth = tileSize * tilesPerRow;
            var theight = tileSize * rowsRequired;

            Bitmap bg1 = new Bitmap(twidth, theight, PixelFormat.Format32bppArgb);
            Bitmap bg2 = new Bitmap(twidth, theight, PixelFormat.Format32bppArgb);

            //commented code here is to be re-enabled once fully understood and changed
            //ushort[] chunks = new ushort[3];
            //ushort[] oldchunks = new ushort[3];
            //chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };

            var pos = 0;
            bool ended = false;
            for (int j = 0; j < rowsRequired; j++)
            {
                if (ended)
                    break;
                for (int i = 0; i < tilesPerRow; i++)
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
                        if (pos != 0xFFFF)
                        {
                            if (bg1Exists)
                                bg1MetaTiles.DrawMetaTile(ref bg1, new Point(i * 16, j * 16), tset, pset, pos, true);

                            if (bg2Exists)
                                bg2MetaTiles.DrawMetaTile(ref bg2, new Point(i * 16, j * 16), tset, pset, pos, true);
                        }
                    }
                    catch (ArgumentException)
                    {
                        Debug.WriteLine("end of metatile file: " + (pos % 256).Hex() + "|" + (pos / 256).Hex());
                        Debug.WriteLine("");
                        ended = true;
                        break;
                    }
                    pos++;
                }
            }

            return new Bitmap[] { bg1, bg2 };
        }

        public List<ChestData> GetChestData()
        {
            return metadata.ChestInfo;
        }

        public void AddChestData(ChestData data)
        {
            metadata.AddChestData(data);
        }

        public void RemoveChestData(ChestData data)
        {
            metadata.RemoveChestData(data);
        }

        public List<ObjectData> GetList1Data()
        {
            return metadata.List1Information;
        }

        public List<ObjectData> GetList2Data()
        {
            return metadata.List2Information;
        }

        public List<ObjectData> GetList3Data()
        {
            return metadata.List3Information;
        }

        public List<WarpData> GetWarpData()
        {
            return metadata.WarpInformation;
        }

        public byte[] GetMetaTileData(ref byte[] tileType, int tileNum, int layer)
        {

            switch (layer)
            {
                case 1:
                    if (!this.bg1Exists)
                        return null;
                    tileType = bg1MetaTiles.GetTileTypeInfo(tileNum);

                    return bg1MetaTiles.GetTileInfo(tileNum);
                case 2:
                    if (!this.bg2Exists)
                        return null;
                    tileType = bg2MetaTiles.GetTileTypeInfo(tileNum);

                    return bg2MetaTiles.GetTileInfo(tileNum);
                default:
                    return null;
            }
        }

        public void SetMetaTileData(byte[] data, byte[] typeData, int tileNum, int layer)
        {
            switch (layer)
            {
                case 1:
                    bg1MetaTiles.SetTileInfo(data, typeData, tileNum);
                    break;
                case 2:
                    bg2MetaTiles.SetTileInfo(data, typeData, tileNum);
                    break;
            }
        }

        public Rectangle GetMapRect(int areaIndex)
        {
            if (metadata == null)
            {
                metadata = new RoomMetaData(areaIndex, this.Index);
            }

            return new Rectangle(new Point(metadata.mapPosX, metadata.mapPosY), new Size(metadata.TileWidth, metadata.TileHeight));
        }

        public void SetMapPosition(int x, int y)
        {
            if (x > 0 && y > 0 && x < 0x10000 && y < 0x10000)
            {
                metadata.mapPosX = x;
                metadata.mapPosY = y;
            }
        }

        public void ResizeRoom(int areaIndex, int xdim, int ydim)
        {
            var oldSize = this.roomSize.X * this.roomSize.Y * 2;
            var newSize = xdim * ydim * 2;
            //var differenceX = xdim - this.roomSize.X;
            var differenceY = ydim - this.roomSize.Y;
            var bg1New = new byte[newSize];
            var bg2New = new byte[newSize];
            var lowestY = this.roomSize.Y < ydim ? this.roomSize.Y : ydim;
            var lowestX = this.roomSize.X < xdim ? this.roomSize.X : xdim;

            for (int i = 0; i < lowestY; i++)
            {
                var src = i * this.roomSize.X * 2;
                var dest = i * xdim * 2;
                if (Bg1Exists)
                {
                    Array.Copy(bg1RoomData, src, bg1New, dest, lowestX * 2);
                }
                if (Bg2Exists)
                {
                    Array.Copy(bg2RoomData, src, bg2New, dest, lowestX * 2);
                }
            }

            if (differenceY > 0)
            {
                for (int i = 0; i < differenceY; i++)
                {
                    for (int j = 0; j < xdim * 2; j++)
                    {
                        var pos = (this.roomSize.Y + i) * xdim * 2 + j;
                        if (Bg1Exists)
                        {
                            bg1New.SetValue((byte)0, pos);
                        }
                        if (Bg2Exists)
                        {
                            bg2New.SetValue((byte)0, pos);
                        }
                    }
                }
            }

            if (Bg1Exists)
            {
                bg1RoomData = bg1New;
                Project.Instance.AddPendingChange(new ChangeTypes.Bg1DataChange(areaIndex, this.Index));
            }

            if (Bg2Exists)
            {
                bg2RoomData = bg2New;
                Project.Instance.AddPendingChange(new ChangeTypes.Bg2DataChange(areaIndex, this.Index));
            }

            metadata.SetRoomSize(xdim, ydim);
            Project.Instance.AddPendingChange(new ChangeTypes.RoomMetadataChange(areaIndex, this.Index));
        }

        public long GetMetadata(ref byte[] data)
        {
            var mapX = metadata.mapPosX * 16;
            var mapY = metadata.mapPosY * 16;

            data = new byte[] {
                (byte)(mapX&0xff),
                (byte)(mapX>>8),
                (byte)(mapY&0xff),
                (byte)(mapY>>8),
                (byte)(metadata.PixelWidth&0xff),
                (byte)(metadata.PixelWidth>>8),
                (byte)(metadata.PixelHeight&0xff),
                (byte)(metadata.PixelHeight>>8),
                (byte)(metadata.tileSetOffset&0xff),
                (byte)(metadata.tileSetOffset>>8)
                };

            return 10;
        }
    }

    public class RoomException : Exception
    {
        public RoomException() { }
        public RoomException(string message) : base(message) { }
        public RoomException(string message, Exception inner) : base(message, inner) { }
    }
}
