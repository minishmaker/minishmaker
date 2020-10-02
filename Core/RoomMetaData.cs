using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public struct AddrData
    {
        public int src;
        public int dest;
        public int size; // in words (2x bytes)
        public bool compressed;

        public AddrData(int src, int dest, int size, bool compressed)
        {
            this.src = src;
            this.dest = dest;
            this.size = size;
            this.compressed = compressed;
        }
    }

    public struct ChestData
    {
        public byte type;
        public byte chestId;
        public byte itemId;
        public byte itemSubNumber;
        public ushort chestLocation;
        public ushort unknown;

        public ChestData(byte type, byte chestId, byte itemId, byte itemSubNumber, ushort chestLocation, ushort other)
        {
            this.type = type;
            this.chestId = chestId;
            this.itemId = itemId;
            this.itemSubNumber = itemSubNumber;
            this.chestLocation = chestLocation;
            this.unknown = other;
        }
    }

    public struct ObjectData
    {
        public byte objectType;
        public byte objectSub;
        public byte objectId;
        public byte d1item;
        public byte d2itemSub;
        public byte d3;
        public byte d4;
        public byte d5;
        public ushort pixelX;
        public ushort pixelY;
        public ushort flag1;
        public ushort flag2;

        public ObjectData(byte[] data, int index)
        {
            objectType = data[index + 0];
            objectSub = data[index + 1];
            objectId = data[index + 2];
            d1item = data[index + 3];
            d2itemSub = data[index + 4];
            d3 = data[index + 5];
            d4 = data[index + 6];
            d5 = data[index + 7];
            pixelX = (ushort)(data[index + 8] + (data[index + 9] << 8));
            pixelY = (ushort)(data[index + 10] + (data[index + 11] << 8));
            flag1 = (ushort)(data[index + 12] + (data[index + 13] << 8));
            flag2 = (ushort)(data[index + 14] + (data[index + 15] << 8));
        }
    }

    public struct WarpData
    {
        public ushort warpType; //2
        public ushort warpXPixel;//4
        public ushort warpYPixel;//6
        public ushort destXPixel;//8
        public ushort destYPixel;//10 A
        public byte warpVar;//11 B
        public byte destArea;//12 C
        public byte destRoom;//13 D
        public byte exitHeight;//14 E
        public byte transitionType;//15 F
        public byte facing;//16 10
        public ushort soundId;//18 12
                              //2 byte padding 20 14
        public WarpData(byte[] data, int offset)
        {
            warpType = (ushort)(data[0 + offset] + (data[1 + offset] << 8));
            warpXPixel = (ushort)(data[2 + offset] + (data[3 + offset] << 8));
            warpYPixel = (ushort)(data[4 + offset] + (data[5 + offset] << 8));
            destXPixel = (ushort)(data[6 + offset] + (data[7 + offset] << 8));
            destYPixel = (ushort)(data[8 + offset] + (data[9 + offset] << 8));
            warpVar = data[10 + offset];
            destArea = data[11 + offset];
            destRoom = data[12 + offset];
            exitHeight = data[13 + offset];
            transitionType = data[14 + offset];
            facing = data[15 + offset];
            soundId = (ushort)(data[16 + offset] + (data[17 + offset] << 8));

        }
    }

    public class RoomMetaData
    {
        private int width, height;
        public int mapPosX, mapPosY, tileSetOffset;

        public int PixelWidth
        {
            get
            {
                return width;
            }
        }

        public int PixelHeight
        {
            get
            {
                return height;
            }
        }

        public int TileWidth
        {
            get
            {
                return width / 16;
            }
        }

        public int TileHeight
        {
            get
            {
                return height / 16;
            }
        }

        private string roomPath;
        private string areaPath;

        private int paletteSetID;
        private List<AddrData> tileSetAddrs = new List<AddrData>();

        private List<ChestData> chestInformation = new List<ChestData>();
        public List<ChestData> ChestInfo
        {
            get { return chestInformation; }
        }

        private List<WarpData> warpInformation = new List<WarpData>();
        public List<WarpData> WarpInformation
        {
            get { return warpInformation; }
        }

        private List<ObjectData> list1Information = new List<ObjectData>();
        public List<ObjectData> List1Information
        {
            get { return list1Information; }
        }

        private List<ObjectData> list2Information = new List<ObjectData>();
        public List<ObjectData> List2Information
        {
            get { return list2Information; }
        }

        private List<ObjectData> list3Information = new List<ObjectData>();
        public List<ObjectData> List3Information
        {
            get { return list3Information; }
        }

        private AddrData? bg2RoomDataAddr;
        private AddrData bg2MetaTilesAddr;

        private AddrData? bg1RoomDataAddr;
        private AddrData bg1MetaTilesAddr;

        private AddrData metaTileTypeAddr1;
        private AddrData metaTileTypeAddr2;

        private bool chestDataLarger = false;
        public bool ChestDataLarger
        {
            get { return chestDataLarger; }
        }

        private bool bg1Use20344B0 = false;
        public bool Bg1Use20344B0
        {
            get
            {
                return bg1Use20344B0;
            }
        }

        public RoomMetaData(int areaIndex, int roomIndex)
        {
            LoadMetaData(areaIndex, roomIndex);
        }

        private void LoadMetaData(int areaIndex, int roomIndex)
        {
            areaPath = Project.Instance.projectPath + "/Areas/Area " + StringUtil.AsStringHex2(areaIndex);
            roomPath = areaPath + "/Room " + StringUtil.AsStringHex2(roomIndex);

            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;

            int areaRMDTableLoc = r.ReadAddr(header.MapHeaderBase + (areaIndex << 2));
            int roomMetaDataTableLoc = areaRMDTableLoc + (roomIndex * 0x0A);

            if (File.Exists(roomPath + "/" + DataType.roomMetaData + "Dat.bin"))
            {
                var data = File.ReadAllBytes(roomPath + "/" + DataType.roomMetaData + "Dat.bin");
                this.mapPosX = (data[0] + (data[1] << 8)) >> 4;
                this.mapPosY = (data[2] + (data[3] << 8)) >> 4;

                if (data.Length == 4) //backwards compatibility because WHY DIDNT I SAVE IT ALL AT FIRST
                {
                    this.width = r.ReadUInt16();
                    this.height = r.ReadUInt16();
                    tileSetOffset = r.ReadUInt16();
                }
                else
                {
                    this.width = (data[4] + (data[5] << 8));
                    this.height = (data[6] + (data[7] << 8));
                    tileSetOffset = (data[8] + (data[9] << 8));
                }
                r.SetPosition(roomMetaDataTableLoc + 8);
            }
            else
            {
                this.mapPosX = r.ReadUInt16(roomMetaDataTableLoc) >> 4;
                this.mapPosY = r.ReadUInt16() >> 4;
                this.width = r.ReadUInt16();
                this.height = r.ReadUInt16();
                tileSetOffset = r.ReadUInt16();
            }

            //get addr of TPA data

            int areaTileSetTableLoc = r.ReadAddr(header.globalTileSetTableLoc + (areaIndex << 2));
            int roomTileSetLoc = r.ReadAddr(areaTileSetTableLoc + (tileSetOffset << 2));

            r.SetPosition(roomTileSetLoc);

            ParseData(r, Set1);

            //metatiles
            int metaTileSetsLoc = r.ReadAddr(header.globalMetaTileSetTableLoc + (areaIndex << 2));

            r.SetPosition(metaTileSetsLoc);

            ParseData(r, Set2);

            //get addr of room data 
            int areaTileDataTableLoc = r.ReadAddr(header.globalTileDataTableLoc + (areaIndex << 2));
            int tileDataLoc = r.ReadAddr(areaTileDataTableLoc + (roomIndex << 2));
            r.SetPosition(tileDataLoc);

            ParseData(r, Set3);

            //attempt at obtaining chest data (+various)
            int areaEntityTableAddrLoc = header.AreaMetadataBase + (areaIndex << 2);
            int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

            int roomEntityTableAddrLoc = areaEntityTableAddr + (roomIndex << 2);
            int roomEntityTableAddr = r.ReadAddr(roomEntityTableAddrLoc);

            int areaWarpTableAddrLoc = header.warpInformationTableLoc + (areaIndex << 2);
            int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

            int roomWarpTableAddrLoc = areaWarpTableAddr + (roomIndex << 2);

            LoadGroup(r, roomEntityTableAddr, 0xFF, DataType.list1Data, 0x10, List1Binding);
            LoadGroup(r, roomEntityTableAddr + 4, 0xFF, DataType.list2Data, 0x10, List2Binding);
            LoadGroup(r, roomEntityTableAddr + 8, 0xFF, DataType.list3Data, 0x10, List3Binding);

            LoadGroup(r, roomEntityTableAddr + 12, 0x0, DataType.chestData, 0x08, ChestBinding);
            //technically not a group but should work with the function
            LoadGroup(r, roomWarpTableAddrLoc, 0xFF, DataType.warpData, 20, WarpBinding);
        }

        public void LoadGroup(Reader r, int addr, byte terminator, DataType dataType, int size, Action<byte[], int> bindingFunc)
        {
            string dataPath = roomPath + "/" + dataType + "Dat.bin";
            if (File.Exists(dataPath))
            {
                byte[] data = File.ReadAllBytes(dataPath);
                int index = 0;

                while (index < data.Length && data[index] != terminator)
                {
                    bindingFunc(data, index);
                    index += size;
                }
            }
            else
            {
                int tableAddr = r.ReadAddr(addr);

                if (tableAddr == 0)
                {
                    return;
                }

                var data = r.ReadBytes(size, tableAddr);

                while (data[0] != terminator)
                {
                    bindingFunc(data, 0);
                    data = r.ReadBytes(size);
                }
            }
        }

        public TileSet GetTileSet()
        {
            return new TileSet(tileSetAddrs, roomPath);
        }

        public PaletteSet GetPaletteSet()
        {
            return new PaletteSet(paletteSetID, areaPath);
        }

        public int GetPaletteSetID()
        {
            return paletteSetID;
        }

        public bool GetBG2Data(ref byte[] bg2RoomData, ref MetaTileSet bg2MetaTiles)
        {
            if (bg2RoomDataAddr != null)
            {
                bg2MetaTiles = new MetaTileSet(bg2MetaTilesAddr, metaTileTypeAddr2, false, areaPath, 2);

                byte[] data = null;
                string bg2Path = roomPath + "/" + DataType.bg2Data + "Dat.bin";

                data = Project.Instance.GetSavedData(bg2Path, true);
                if (data == null)
                {
                    data = DataHelper.GetData((AddrData)bg2RoomDataAddr);
                }
                data.CopyTo(bg2RoomData, 0);

                return true;
            }
            return false;
        }

        public bool GetBG1Data(ref byte[] bg1RoomData, ref MetaTileSet bg1MetaTiles)
        {
            if (bg1RoomDataAddr != null)
            {
                byte[] data = null;
                string bg1Path = roomPath + "/" + DataType.bg1Data + "Dat.bin";

                data = Project.Instance.GetSavedData(bg1Path, true);
                if (data == null)
                {
                    data = DataHelper.GetData((AddrData)bg1RoomDataAddr);
                }

                if (!bg1Use20344B0)
                {
                    bg1MetaTiles = new MetaTileSet(bg1MetaTilesAddr, metaTileTypeAddr1, true, areaPath, 1);
                }

                data.CopyTo(bg1RoomData, 0);
                return !bg1Use20344B0;
            }
            return false;
        }

        private void ParseData(Reader r, Func<AddrData, bool> postFunc)
        {
            var header = ROM.Instance.headers;
            bool cont = true;
            while (cont)
            {
                UInt32 data = r.ReadUInt32();
                UInt32 data2 = r.ReadUInt32();
                UInt32 data3 = r.ReadUInt32();



                if (data2 == 0)
                { //palette
                    this.paletteSetID = (int)(data & 0x7FFFFFFF); //mask off high bit
                }
                else
                {
                    int source = (int)((data & 0x7FFFFFFF) + header.gfxSourceBase); //08324AE4 is tile gfx base
                    int dest = (int)(data2 & 0x7FFFFFFF);
                    bool compressed = (data3 & 0x80000000) != 0; //high bit of size determines LZ or DMA
                    int size = (int)(data3 & 0x7FFFFFFF);

                    cont = postFunc(new AddrData(source, dest, size, compressed));
                }
                if (cont == true)
                {
                    cont = (data & 0x80000000) != 0; //high bit determines if more to load
                }
            }
        }

        public long CompressBG1(ref byte[] outdata, byte[] bg1data)
        {
            var compressed = new byte[bg1data.Length];
            long totalSize = 0;
            MemoryStream ous = new MemoryStream(compressed);
            totalSize = DataHelper.Compress(bg1data, ous, false);

            outdata = new byte[totalSize];
            Array.Copy(compressed, outdata, totalSize);
            //var sizeDifference = totalSize - bg1RoomDataAddr.Value.size;

            totalSize |= 0x80000000;

            return totalSize;
        }

        public long CompressBG2(ref byte[] outdata, byte[] bg2data)
        {
            var compressed = new byte[bg2data.Length];
            long totalSize = 0;
            MemoryStream ous = new MemoryStream(compressed);
            totalSize = DataHelper.Compress(bg2data, ous, false);

            outdata = new byte[totalSize];
            Array.Copy(compressed, outdata, totalSize);
            //var sizeDifference = totalSize - bg2RoomDataAddr.Value.size;

            totalSize |= 0x80000000;

            return totalSize;
        }

        public long GetChestData(ref byte[] outdata)
        {
            outdata = new byte[chestInformation.Count * 8 + 8];

            for (int i = 0; i < chestInformation.Count; i++)
            {
                var index = i * 8;
                var data = chestInformation[i];
                outdata[index] = data.type;
                outdata[index + 1] = data.chestId;
                outdata[index + 2] = data.itemId;
                outdata[index + 3] = data.itemSubNumber;
                byte high = (byte)(data.chestLocation >> 8);
                byte low = (byte)(data.chestLocation - (high << 8));
                outdata[index + 4] = low;
                outdata[index + 5] = high;
                high = (byte)(data.unknown >> 8);
                low = (byte)(data.unknown - (high << 8));
                outdata[index + 6] = low;
                outdata[index + 7] = high;
            }

            for (int j = 0; j < 8; j++)
                outdata[outdata.Length - 8 + j] = 0;

            return outdata.Length;
        }

        public void AddChestData(ChestData data)
        {
            chestDataLarger = true; //larger so should be moved
            chestInformation.Add(data);
        }

        public void RemoveChestData(ChestData data)
        {
            chestInformation.Remove(data);
        }

        public long GetList1Data(ref byte[] data)
        {
            return GetListData(ref data, list1Information);
        }

        public long GetList2Data(ref byte[] data)
        {
            return GetListData(ref data, list2Information);
        }

        public long GetList3Data(ref byte[] data)
        {
            return GetListData(ref data, list3Information);
        }

        public long GetListData(ref byte[] data, List<ObjectData> list)
        {
            var outdata = new byte[list.Count * 16 + 1];

            for (int i = 0; i < list.Count; i++)
            {
                var index = i * 16;
                var currentData = list[i];

                outdata[index++] = currentData.objectType;
                outdata[index++] = currentData.objectSub;
                outdata[index++] = currentData.objectId;
                outdata[index++] = currentData.d1item;
                outdata[index++] = currentData.d2itemSub;
                outdata[index++] = currentData.d3;
                outdata[index++] = currentData.d4;
                outdata[index++] = currentData.d5;
                outdata[index++] = (byte)(currentData.pixelX & 0xff);
                outdata[index++] = (byte)(currentData.pixelX >> 8);
                outdata[index++] = (byte)(currentData.pixelY & 0xff);
                outdata[index++] = (byte)(currentData.pixelY >> 8);
                outdata[index++] = (byte)(currentData.flag1 & 0xff);
                outdata[index++] = (byte)(currentData.flag1 >> 8);
                outdata[index++] = (byte)(currentData.flag2 & 0xff);
                outdata[index++] = (byte)(currentData.flag2 >> 8);
            }

            outdata[outdata.Length - 1] = 0xFF;

            data = outdata;
            return outdata.Length;
        }

        public long GetWarpData(ref byte[] data)
        {
            var outdata = new byte[warpInformation.Count * 20 + 20];

            for (int i = 0; i < warpInformation.Count; i++)
            {
                var index = i * 20;
                var currentData = warpInformation[i];
                outdata[index + 0] = (byte)(currentData.warpType & 0xff);
                outdata[index + 1] = (byte)(currentData.warpType >> 8);
                outdata[index + 2] = (byte)(currentData.warpXPixel & 0xff);
                outdata[index + 3] = (byte)(currentData.warpXPixel >> 8);
                outdata[index + 4] = (byte)(currentData.warpYPixel & 0xff);
                outdata[index + 5] = (byte)(currentData.warpYPixel >> 8);
                outdata[index + 6] = (byte)(currentData.destXPixel & 0xff);
                outdata[index + 7] = (byte)(currentData.destXPixel >> 8);
                outdata[index + 8] = (byte)(currentData.destYPixel & 0xff);
                outdata[index + 9] = (byte)(currentData.destYPixel >> 8);
                outdata[index + 10] = currentData.warpVar;
                outdata[index + 11] = currentData.destArea;
                outdata[index + 12] = currentData.destRoom;
                outdata[index + 13] = currentData.exitHeight;
                outdata[index + 14] = currentData.transitionType;
                outdata[index + 15] = currentData.facing;
                outdata[index + 16] = (byte)(currentData.soundId & 0xff);
                outdata[index + 17] = (byte)(currentData.soundId >> 8);
                outdata[index + 18] = 0;
                outdata[index + 19] = 0;//padding
            }

            outdata[outdata.Length - 20] = 0xFF;
            outdata[outdata.Length - 19] = 0xFF;
            for (int j = 2; j < 20; j++)
                outdata[outdata.Length - 20 + j] = 0;

            data = outdata;
            return outdata.Length;
        }


        //To be changed as actual data gets changed and tested
        public int GetPointerLoc(DataType type, int areaIndex, int roomIndex)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int retAddr = 0;
            int areaRMDTableLoc = r.ReadAddr(header.MapHeaderBase + (areaIndex << 2));
            int roomMetaDataTableLoc = areaRMDTableLoc + (roomIndex * 0x0A);

            switch (type)
            {
                case DataType.roomMetaData:
                    retAddr = roomMetaDataTableLoc;
                    break;

                case DataType.bg1TileSet:
                case DataType.bg2TileSet:
                case DataType.commonTileSet:
                    //get addr of TPA data

                    int areaTileSetTableLoc = r.ReadAddr(header.globalTileSetTableLoc + (areaIndex << 2));
                    int roomTileSetLoc = r.ReadAddr(areaTileSetTableLoc + (tileSetOffset << 2));

                    r.SetPosition(roomTileSetLoc);

                    if (type == DataType.bg1TileSet)
                    {
                        ParseData(r, Tile1Check);
                    }
                    if (type == DataType.bg2TileSet)
                    {
                        ParseData(r, Tile2Check);
                    }
                    if (type == DataType.commonTileSet)
                    {
                        ParseData(r, TileCommonCheck);
                    }
                    retAddr = (int)r.Position - 12;
                    break;

                case DataType.bg1MetaTileSet:
                case DataType.bg2MetaTileSet:
                case DataType.bg1MetaTileType:
                case DataType.bg2MetaTileType:
                    int metaTileSetsAddrLoc = r.ReadAddr(header.globalMetaTileSetTableLoc + (areaIndex << 2));
                    //retAddr = metaTileSetsAddrLoc;
                    r.SetPosition(metaTileSetsAddrLoc);
                    if (type == DataType.bg1MetaTileSet)
                    {
                        ParseData(r, Meta1Check);
                    }
                    if (type == DataType.bg2MetaTileSet)
                    {
                        ParseData(r, Meta2Check);
                    }
                    if (type == DataType.bg1MetaTileType)
                    {
                        ParseData(r, Type1Check);
                    }
                    if (type == DataType.bg2MetaTileType)
                    {
                        ParseData(r, Type2Check);
                    }
                    retAddr = (int)r.Position - 12; //step back 12 bytes as the bg was found after reading
                    break;

                case DataType.bg1Data:
                case DataType.bg2Data:
                    int areaTileDataTableLoc = r.ReadAddr(header.globalTileDataTableLoc + (areaIndex << 2));
                    int tileDataLoc = r.ReadAddr(areaTileDataTableLoc + (roomIndex << 2));
                    r.SetPosition(tileDataLoc);

                    if (type == DataType.bg1Data)
                    {
                        ParseData(r, Bg1Check);
                    }
                    else //not bg1 so has to be bg2
                    {
                        ParseData(r, Bg2Check);
                    }
                    retAddr = (int)r.Position - 12; //step back 12 bytes as the bg was found after reading
                    break;

                case DataType.chestData:
                case DataType.list1Data:
                case DataType.list2Data:
                case DataType.list3Data:
                    int areaEntityTableAddrLoc = header.AreaMetadataBase + (areaIndex << 2);
                    int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

                    int roomEntityTableAddrLoc = areaEntityTableAddr + (roomIndex << 2);
                    int roomEntityTableAddr = r.ReadAddr(roomEntityTableAddrLoc);

                    //4 byte chunks, 1-3 are unknown use, 4th seems to be chests
                    var offset = 0;
                    if (type == DataType.list1Data)
                        offset = 0x00;
                    if (type == DataType.list2Data)
                        offset = 0x04;
                    if (type == DataType.list3Data)
                        offset = 0x08;
                    if (type == DataType.chestData)
                        offset = 0x0C;
                    retAddr = roomEntityTableAddr + offset;

                    Console.WriteLine(retAddr);
                    break;

                case DataType.warpData:
                    int areaWarpTableAddrLoc = header.warpInformationTableLoc + (areaIndex << 2);
                    int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

                    int roomWarpTableAddrLoc = areaWarpTableAddr + (roomIndex << 2);
                    retAddr = roomWarpTableAddrLoc;
                    break;
                case DataType.palette:
                    int paletteSetTableLoc = header.paletteSetTableLoc;
                    int addr = r.ReadAddr(paletteSetTableLoc + ( paletteSetID * 4));
                    retAddr = header.tileOffset + (r.ReadUInt16(addr) << 5);
                    break;
                default:
                    break;
            }

            return retAddr;
        }

        //bindings for data to usable structs
        private void ChestBinding(byte[] data, int startIndex)
        {
            var type = data[startIndex];
            var id = data[startIndex + 1];
            var item = data[startIndex + 2];
            var subNum = data[startIndex + 3];
            ushort loc = (ushort)(data[startIndex + 4] | (data[startIndex + 5] << 8));
            ushort other = (ushort)(data[startIndex + 6] | (data[startIndex + 7] << 8));
            chestInformation.Add(new ChestData(type, id, item, subNum, loc, other));
        }

        private void List1Binding(byte[] data, int startIndex)
        {
            list1Information.Add(new ObjectData(data, startIndex));
        }

        private void List2Binding(byte[] data, int startIndex)
        {
            list2Information.Add(new ObjectData(data, startIndex));
        }

        private void List3Binding(byte[] data, int startIndex)
        {
            list3Information.Add(new ObjectData(data, startIndex));
        }

        private void WarpBinding(byte[] data, int startIndex)
        {
            warpInformation.Add(new WarpData(data, startIndex));
        }

        //dont have any good names for these 3
        private bool Set1(AddrData data)
        {
            if ((data.dest & 0xF000000) != 0x6000000)
            { //not valid tile data addr
                Console.WriteLine("Unhandled tile data destination address: " + data.dest.Hex() + " Source:" + data.src.Hex() + " Compressed:" + data.compressed + " Size:" + data.size.Hex());
                return false;
            }

            data.dest = data.dest & 0xFFFFFF;
            this.tileSetAddrs.Add(data);
            return true;
        }

        private bool Set2(AddrData data)
        {
            //use a switch in case this data is out of order
            switch (data.dest)
            {
                case 0x0202CEB4:
                    this.bg2MetaTilesAddr = data;
                    Debug.WriteLine(data.src.Hex() + " bg2");
                    break;
                case 0x02012654:
                    this.bg1MetaTilesAddr = data;
                    Debug.WriteLine(data.src.Hex() + " bg1");
                    break;
                case 0x0202AEB4:
                    this.metaTileTypeAddr2 = data;
                    Debug.WriteLine(data.src.Hex() + " type2");
                    break;
                case 0x02010654:
                    this.metaTileTypeAddr1 = data;
                    Debug.WriteLine(data.src.Hex() + " type1");
                    break;
                default:
                    Debug.Write("Unhandled metatile addr: ");
                    Debug.Write(data.src.Hex() + "->" + data.dest.Hex());
                    Debug.WriteLine(data.compressed ? " (compressed)" : "");
                    break;
            }
            return true;
        }

        private bool Set3(AddrData data)
        {
            switch (data.dest)
            {
                case 0x02025EB4:
                    this.bg2RoomDataAddr = data;
                    break;
                case 0x0200B654:
                    this.bg1RoomDataAddr = data;
                    break;
                case 0x02002F00:
                    this.bg1RoomDataAddr = data;
                    this.bg1Use20344B0 = true;
                    break;
                default:
                    Debug.Write("Unhandled room data addr: ");
                    Debug.Write(data.src.Hex() + "->" + data.dest.Hex());
                    Debug.WriteLine(data.compressed ? " (compressed)" : "");
                    break;
            }
            return true;
        }

        private bool Bg1Check(AddrData data)
        {
            switch (data.dest)
            {
                case 0x0200B654:
                    return false;
                case 0x2002F00:
                    return false;
                default:
                    break;
            }
            return true;
        }

        private bool Bg2Check(AddrData data)
        {
            switch (data.dest)
            {
                case 0x02025EB4:
                    return false;
                default:
                    break;
            }
            return true;
        }

        private bool Meta1Check(AddrData data)
        {
            switch (data.dest)
            {
                case 0x02012654:
                    return false;
                default:
                    break;
            }
            return true;
        }

        private bool Type1Check(AddrData data)
        {
            switch (data.dest)
            {
                case 0x02010654:
                    return false;
                default:
                    break;
            }
            return true;
        }

        private bool Meta2Check(AddrData data)
        {
            switch (data.dest)
            {
                case 0x0202CEB4:
                    return false;
                default:
                    break;
            }
            return true;
        }

        private bool Type2Check(AddrData data)
        {
            switch (data.dest)
            {
                case 0x0202AEB4:
                    return false;
                default:
                    break;
            }
            return true;
        }

        private bool Tile1Check(AddrData data)
        {
            return data.dest != 0;
        }

        private bool Tile2Check(AddrData data)
        {
            return data.dest != 0x8000;
        }

        private bool TileCommonCheck(AddrData data)
        {
            return data.dest != 0x4000;
        }

        public void SetRoomSize(int xdim, int ydim)
        {
            this.width = xdim << 4;
            this.height = ydim << 4;
        }
    }
}
