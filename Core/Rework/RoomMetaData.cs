using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.Core.Rework
{
    //TODO: remove tile and metatile related things
    public class RoomMetaData
    {
        public Room Parent { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public int TileWidth { get { return PixelWidth / 0x10; } }
        public int TileHeight { get { return PixelHeight / 0x10; } }


        private TileSet tileSet;
        public TileSet TileSet {
            get
            {
                if (this.tileSet == null)
                {
                    this.tileSet = new TileSet(tileSetAddrs, Parent.Path);
                }
                return this.tileSet;
            }
        }

        private Core.AddrData? bg2RoomDataAddr;

        private Core.AddrData? bg1RoomDataAddr;

        public bool Bg1Use20344B0 { get; private set; }

        public int PaletteSetID { get; private set; }
        private PaletteSet paletteSet;
        public PaletteSet PaletteSet {
            get
            {
                if (this.paletteSet == null)
                {
                    this.paletteSet = new PaletteSet(PaletteSetID);
                }
                return this.paletteSet;
            }
        }

        private List<ChestData> chestInformation = new List<ChestData>();
        private List<WarpData> warpInformation = new List<WarpData>();
        private Dictionary<int, List<List<byte>>> listInformation = new Dictionary<int, List<List<byte>>>();
        private List<Core.AddrData> tileSetAddrs = new List<Core.AddrData>();

        public int MapPosX { get; private set; }
        public int MapPosY { get; private set; }

        public int tileSetOffset;

        public RoomMetaData(Room parent)
        {
            this.Parent = parent;
            LoadBase();
        }

        int G_listLoopVar; //used in listbinding as there is a variable amount of lists per room
        public void LoadBase()
        {
            var areaId = Parent.Parent.Id;
            var roomId = Parent.Id;

            var path = Parent.Path;

            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;

            int areaRMDTableLoc = r.ReadAddr(header.MapHeaderBase + (areaId << 2));
            int roomMetaDataTableLoc = areaRMDTableLoc + (roomId * 0x0A);

            if (File.Exists(path + "/" + DataType.roomMetaData + "Dat.bin"))
            {
                var data = File.ReadAllBytes(path + "/" + DataType.roomMetaData + "Dat.bin");
                this.MapPosX = (data[0] + (data[1] << 8)) >> 4;
                this.MapPosY = (data[2] + (data[3] << 8)) >> 4;

                if (data.Length == 4) //backwards compatibility because WHY DIDNT I SAVE IT ALL AT FIRST
                {
                    this.PixelWidth = r.ReadUInt16();
                    this.PixelHeight = r.ReadUInt16();
                    tileSetOffset = r.ReadUInt16();
                }
                else
                {
                    this.PixelWidth = (data[4] + (data[5] << 8));
                    this.PixelHeight = (data[6] + (data[7] << 8));
                    tileSetOffset = (data[8] + (data[9] << 8));
                }

                r.SetPosition(roomMetaDataTableLoc + 8);
            }
            else
            {
                this.MapPosX = r.ReadUInt16(roomMetaDataTableLoc) >> 4;
                this.MapPosY = r.ReadUInt16() >> 4;
                this.PixelWidth = r.ReadUInt16();
                this.PixelHeight = r.ReadUInt16();
                tileSetOffset = r.ReadUInt16();
            }

            //get addr of TPA data

            int areaTileSetTableLoc = r.ReadAddr(header.globalTileSetTableLoc + (areaId << 2));
            int roomTileSetLoc = r.ReadAddr(areaTileSetTableLoc + (tileSetOffset << 2));

            r.SetPosition(roomTileSetLoc);

            ParseData(r, ValidateTileSet);

            //get addr of room data 
            int areaTileDataTableLoc = r.ReadAddr(header.globalTileDataTableLoc + (areaId << 2));
            int tileDataLoc = r.ReadAddr(areaTileDataTableLoc + (roomId << 2));
            r.SetPosition(tileDataLoc);

            ParseData(r, ValidateRoomData);

            //attempt at obtaining chest data (+various)
            int areaEntityTableAddrLoc = header.AreaMetadataBase + (areaId << 2);
            int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

            int roomEntityTableAddrLoc = areaEntityTableAddr + (roomId << 2);
            int roomEntityTableAddr = r.ReadAddr(roomEntityTableAddrLoc);

            int areaWarpTableAddrLoc = header.warpInformationTableLoc + (areaId << 2);
            int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

            int roomWarpTableAddrLoc = areaWarpTableAddr + (roomId << 2);

            G_listLoopVar = 0;
            bool hasMoreLists = true;
            while (hasMoreLists)
            {
                if (G_listLoopVar == 3)//skip 3 as thats for chests specifically, might get put in at a later time
                {
                    G_listLoopVar += 1;
                }

                hasMoreLists = LoadGroup(r, roomEntityTableAddr + 4 * G_listLoopVar, 0xFF, Constants._listData + (G_listLoopVar + 1), 0x10, ListBinding); //listData1-X
                G_listLoopVar += 1;
            }

            LoadGroup(r, roomEntityTableAddr + 12, 0x0, DataType.chestData.ToString(), 0x08, ChestBinding);
            //technically not a group but should work with the function
            LoadGroup(r, roomWarpTableAddrLoc, 0xFF, DataType.warpData.ToString(), 20, WarpBinding);
        }

        public bool LoadBGData(ref byte[] bgRoomData, ref MetaTileSet bgMetaTiles, bool isBg1)
        {
            DataType bgDataType;
            Core.AddrData? bgRoomDataAddr;
            Core.AddrData bgMetaTilesAddr;
            Core.AddrData MetaTileTypeAddr;
            Area area = Parent.Parent;

            if (isBg1)
            {
                bgRoomDataAddr = bg1RoomDataAddr;
                bgMetaTilesAddr = area.bg1MetaTilesAddr;
                MetaTileTypeAddr = area.bg1MetaTileTypeAddr;
                bgDataType = DataType.bg1Data;
            }
            else
            {
                bgRoomDataAddr = bg2RoomDataAddr;
                bgMetaTilesAddr = area.bg2MetaTilesAddr;
                MetaTileTypeAddr = area.bg2MetaTileTypeAddr;
                bgDataType = DataType.bg2Data;
            }


            if (bgRoomDataAddr == null) {
                return false;
            }

            string bgPath = Parent.Path + "/" + bgDataType + "Dat.bin";

            byte[] data = DataHelper.GetSavedData(bgPath, true);
            if (data == null) //no saved data, get from original source
            {
                data = DataHelper.GetData(bgRoomDataAddr.Value);
            }

            if (!(isBg1 && Bg1Use20344B0))
            {
                bgMetaTiles = new MetaTileSet(bgMetaTilesAddr, MetaTileTypeAddr, isBg1, area.Path);
            }

            data.CopyTo(bgRoomData, 0);
            return !(isBg1 && Bg1Use20344B0);
        }

        private bool LoadGroup(Reader r, int addr, byte terminator, string dataType, int size, Action<byte[], int> bindingFunc)
        {
            string dataPath = Parent.Path + "/" + dataType + "Dat.bin";
            if (File.Exists(dataPath))
            {
                byte[] data = File.ReadAllBytes(dataPath);
                int index = 0;

                while (index < data.Length && data[index] != terminator)
                {
                    bindingFunc(data, index);
                    index += size;
                }

                return true;
            }
            else
            {
                int tableAddr = r.ReadAddr(addr);

                if (tableAddr == 0)
                {
                    return true;
                }

                if (tableAddr == 0xFF)
                {
                    return false;
                }

                var data = r.ReadBytes(size, tableAddr);

                while (data[0] != terminator)
                {
                    bindingFunc(data, 0);
                    data = r.ReadBytes(size);
                }

                return true;
            }
        }

        private void ParseData(Reader r, Func<Core.AddrData, bool> postFunc)
        {
            var header = ROM.Instance.headers;
            bool cont = true;
            while (cont)
            {
                uint data = r.ReadUInt32();
                uint data2 = r.ReadUInt32();
                uint data3 = r.ReadUInt32();



                if (data2 == 0)
                { //palette
                    this.PaletteSetID = (int)(data & 0x7FFFFFFF); //mask off high bit
                }
                else
                {
                    int source = (int)((data & 0x7FFFFFFF) + header.gfxSourceBase); //08324AE4 is tile gfx base
                    int dest = (int)(data2 & 0x7FFFFFFF);
                    bool compressed = (data3 & 0x80000000) != 0; //high bit of size determines LZ or DMA
                    int size = (int)(data3 & 0x7FFFFFFF);

                    cont = postFunc(new Core.AddrData(source, dest, size, compressed));
                }
                if (cont == true)
                {
                    cont = (data & 0x80000000) != 0; //high bit determines if more to load
                }
            }
        }

        public void SetMapPosition(int x, int y)
        {
            if (x > 0 && y > 0 && x < 0x10000 && y < 0x10000)
            {
                MapPosX = x;
                MapPosY = y;
            }

            Project.Instance.AddPendingChange(new RoomMetadataChange(this.Parent.Parent.Id, this.Parent.Id));
        }

        public Rectangle GetMapRect()
        {
            return new Rectangle(new Point(MapPosX, MapPosY), new Size(TileWidth, TileHeight));
        }

        #region list add/remove/get sets
        public void AddNewChestInformation(int position)
        {
            if (position > chestInformation.Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }
            //TODO: chestdatalarger
            chestInformation.Insert(position, new ChestData());
        }

        public void RemoveChestInformation(int position)
        {
            if (position > chestInformation.Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            chestInformation.RemoveAt(position);
        }

        public ChestData GetChestInformationEntry(int position)
        {
            if (chestInformation.Count <= position || position < 0)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            return chestInformation[position];
        }


        public void AddNewWarpInformation(int position)
        {
            if (position > warpInformation.Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            warpInformation.Insert(position + 1, new WarpData());
        }

        public int GetWarpInformationSize()
        {
            return warpInformation.Count();
        }
        
        public void RemoveWarpInformation(int position)
        {
            if (position > warpInformation.Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            warpInformation.RemoveAt(position);
        }

        public WarpData GetWarpInformationEntry(int position)
        {
            if (warpInformation.Count <= position || position < 0)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            return warpInformation[position];
        }


        public void AddNewListInformation(int listId, int position)
        {
            if(!listInformation.ContainsKey(listId))
            {
                if (position != 0)
                {
                    throw new RoomMetaDataException("Adding to a non-existant list requires a position of 0");
                }
                listInformation.Add(listId, new List<List<byte>>());
                listInformation[listId].Add(new List<byte>(new byte[16]));
                return;
            }

            if (position > listInformation[listId].Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            listInformation[listId].Insert(position + 1, new List<byte>(new byte[16]));
        }

        public void RemoveListInformation(int listId, int position)
        {
            if (!listInformation.ContainsKey(listId))
            {
                throw new RoomMetaDataException($"List {listId} does not exist");
            }

            if (position > listInformation[listId].Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            listInformation[listId].RemoveAt(position);
        }

        public List<byte> GetListInformationEntry(int listId, int position)
        {
            if (!listInformation.ContainsKey(listId))
            {
                throw new RoomMetaDataException($"List {listId} does not exist");
            }

            if (position > listInformation[listId].Count)
            {
                throw new RoomMetaDataException("Position is outside of the list");
            }

            return listInformation[listId][position];
        }

        public int GetListEntryAmount(int listId)
        {
            return listInformation[listId].Count();
        }

        public int[] GetListInformationKeys()
        {
            return listInformation.Keys.ToArray();
        }

        #endregion

        #region data binding methods

        private void ChestBinding(byte[] data, int startIndex)
        {
            chestInformation.Add(new ChestData(data, startIndex));
        }

        private void WarpBinding(byte[] data, int startIndex)
        {
            warpInformation.Add(new WarpData(data, startIndex));
        }

        private void ListBinding(byte[] data, int startIndex)
        {
            List<List<byte>> list;

            if (listInformation.ContainsKey(G_listLoopVar))
            {
                list = listInformation[G_listLoopVar];
            }
            else
            {
                list = new List<List<byte>>();
            }

            var dat = new List<byte>(data.Skip(startIndex).Take(0x10).ToArray());
            list.Add(dat);
        }

        #endregion

        #region ParseData Func's
        //dont have any good names for these 3
        private bool ValidateTileSet(Core.AddrData data)
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

        private bool ValidateRoomData(Core.AddrData data)
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
                    this.Bg1Use20344B0 = true;
                    break;
                default:
                    Debug.Write("Unhandled room data addr: ");
                    Debug.Write(data.src.Hex() + "->" + data.dest.Hex());
                    Debug.WriteLine(data.compressed ? " (compressed)" : "");
                    break;
            }
            return true;
        }

        #endregion

        #region bytify methods

        //sneak in the list id within the byte[]
        public long BytifyListInformation(ref byte[] data) 
        {
            var list = listInformation[data[1]];
            var outdata = new byte[list.Count * 16 + 1];

            for (int i = 0; i < list.Count; i++)
            {
                list[i].CopyTo(outdata, i * 0x10);
            }

            outdata[outdata.Length - 1] = 0xFF;

            data = outdata;
            return outdata.Length;
        }

        public long BytifyWarpInformation(ref byte[] data)
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

        #endregion

        public void SetRoomSize(int xdim, int ydim)
        {
            this.PixelHeight = xdim << 4;
            this.PixelWidth = ydim << 4;
        }

        //To be changed as actual data gets added, changed and tested
        public int GetPointerLoc(DataType type)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            var roomId = Parent.Id;
            var areaId = Parent.Parent.Id;

            int retAddr = 0;
            int areaRMDTableLoc = r.ReadAddr(header.MapHeaderBase + (areaId << 2));
            int roomMetaDataTableLoc = areaRMDTableLoc + (roomId * 0x0A);

            switch (type)
            {
                case DataType.roomMetaData:
                    retAddr = roomMetaDataTableLoc;
                    break;

                case DataType.bg1TileSet:
                case DataType.bg2TileSet:
                case DataType.commonTileSet:
                    //get addr of TPA data

                    int areaTileSetTableLoc = r.ReadAddr(header.globalTileSetTableLoc + (areaId << 2));
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
                    int metaTileSetsAddrLoc = r.ReadAddr(header.globalMetaTileSetTableLoc + (areaId << 2));
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
                    int areaTileDataTableLoc = r.ReadAddr(header.globalTileDataTableLoc + (areaId << 2));
                    int tileDataLoc = r.ReadAddr(areaTileDataTableLoc + (roomId << 2));
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
                    int areaEntityTableAddrLoc = header.AreaMetadataBase + (areaId << 2);
                    int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

                    int roomEntityTableAddrLoc = areaEntityTableAddr + (roomId << 2);
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
                    int areaWarpTableAddrLoc = header.warpInformationTableLoc + (areaId << 2);
                    int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

                    int roomWarpTableAddrLoc = areaWarpTableAddr + (roomId << 2);
                    retAddr = roomWarpTableAddrLoc;
                    break;

                default:
                    break;
            }

            return retAddr;
        }

        #region check methods for GetPointerLoc
        private bool Bg1Check(Core.AddrData data)
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

        private bool Bg2Check(Core.AddrData data)
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

        private bool Meta1Check(Core.AddrData data)
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

        private bool Meta2Check(Core.AddrData data)
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

        private bool Type1Check(Core.AddrData data)
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

        private bool Type2Check(Core.AddrData data)
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

        private bool Tile1Check(Core.AddrData data)
        {
            return data.dest != 0;
        }

        private bool Tile2Check(Core.AddrData data)
        {
            return data.dest != 0x8000;
        }

        private bool TileCommonCheck(Core.AddrData data)
        {
            return data.dest != 0x4000;
        }
        #endregion

        public class RoomMetaDataException : Exception
        {
            public RoomMetaDataException() { }
            public RoomMetaDataException(string message) : base(message) { }
            public RoomMetaDataException(string message, Exception inner) : base(message, inner) { }
        }
    }

    #region datatypes
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

    public class ChestData
    {
        public byte type;
        public byte chestId;
        public byte itemId;
        public byte itemSubNumber;
        public ushort chestLocation;
        public ushort unknown;

        public ChestData(byte[] data, int pos)
        {
            this.type = data[pos];
            this.chestId = data[pos + 1];
            this.itemId = data[pos + 2];
            this.itemSubNumber = data[pos + 3];
            this.chestLocation = (ushort)(data[pos + 4] | (data[pos + 5] << 8));
            this.unknown = (ushort)(data[pos + 6] | (data[pos + 7] << 8));
        }
        public ChestData()
        {
            this.type = 2;
            this.chestId = 0;
            this.itemId = 0;
            this.itemSubNumber = 0;
            this.chestLocation = 0;
            this.unknown = 0;
        }
    }

    public class WarpData
    {
        public ushort warpType = 0;     //2
        public ushort warpXPixel = 0;   //4
        public ushort warpYPixel = 0;   //6
        public ushort destXPixel = 0;   //8
        public ushort destYPixel = 0;   //10 A
        public byte warpVar = 0;        //11 B
        public byte destArea = 0;       //12 C
        public byte destRoom = 0;       //13 D
        public byte exitHeight = 0;     //14 E
        public byte transitionType = 0; //15 F
        public byte facing = 0;         //16 10
        public ushort soundId = 0;      //18 12
                                        //20 14  2 byte padding
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
        public WarpData()
        {
        }
    }
    #endregion
}
