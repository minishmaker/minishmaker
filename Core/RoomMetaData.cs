using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.UI;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    //TODO: remove tile and metatile related things
    public class RoomMetaData
    {
        public Room Parent { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public int TileWidth { get { return PixelWidth / 0x10; } }
        public int TileHeight { get { return PixelHeight / 0x10; } }

        public int MapPosX { get; private set; }
        public int MapPosY { get; private set; }

        public int tilesetId;

        private Core.AddrData? bg2RoomDataAddr;

        private Core.AddrData? bg1RoomDataAddr;

        public bool Bg1Use20344B0 { get; private set; }

        public int PaletteSetID { get; private set; }
        public bool hasPalette = false;
        //private PaletteSet paletteSet;
        /*public PaletteSet PaletteSet {
            get
            {
                var id = Parent.Parent.Tilesets[tilesetId].paletteSetId;
                if(hasPalette)
                {
                    id = PaletteSetID;
                }
                
                return PaletteSetManager.Get().GetSet(id);
            }
        }*/

        //private List<ChestData> chestInformation = new List<ChestData>();
        private List<WarpData> warpInformation = new List<WarpData>();
        private Dictionary<int, List<List<byte>>> listInformation = new Dictionary<int, List<List<byte>>>();
//        private List<AddrData> tilesetAddrs = new List<AddrData>();
        private Dictionary<int, byte> usedAdresses = new Dictionary<int, byte>();
        private Dictionary<byte, int> usedLists = new Dictionary<byte, int>();
        private List<byte> listLinks = new List<byte>();

        private bool isCreated = false;

        public RoomMetaData(Room parent, bool isCreated = false)
        {
            this.isCreated = isCreated;
            this.Parent = parent;
            LoadBase();
        }

        byte G_listLoopVar; //used in listbinding as there is a variable amount of lists per room
        public void LoadBase()
        {
            var areaId = Parent.Parent.Id;
            var roomId = Parent.Id;

            var path = Parent.Path;

            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;

            int mapInfoArea = r.ReadAddr(header.MapInfoRoot + (areaId << 2));
            int mapInfoRoomLoc = mapInfoArea + (roomId * 0x0A);

            if (File.Exists(path + "/" + DataType.roomMetaData + ".json"))
            {
                string metaDataJson = File.ReadAllText(path + "/" + DataType.roomMetaData + ".json");
                RoomMetaDataStruct rmds = (RoomMetaDataStruct)Newtonsoft.Json.JsonConvert.DeserializeObject(metaDataJson);
                this.MapPosX = rmds.mapX;
                this.MapPosY = rmds.mapY;
                this.PixelWidth = rmds.sizeX;
                this.PixelHeight = rmds.sizeY;
                this.tilesetId = rmds.tilesetId;
                /*var data = File.ReadAllBytes(path + "/" + DataType.roomMetaData + "Dat.bin");
                this.MapPosX = (data[0] + (data[1] << 8)) >> 4; //do we want it in tiles? used in not tiles anywhere else?
                this.MapPosY = (data[2] + (data[3] << 8)) >> 4;

                if (data.Length == 4) //backwards compatibility because WHY DIDNT I SAVE IT ALL AT FIRST
                {
                    r.SetPosition(mapInfoRoomLoc + 4);
                    this.PixelWidth = r.ReadUInt16();
                    this.PixelHeight = r.ReadUInt16();
                    this.tilesetOffset = r.ReadUInt16();
                }
                else
                {
                    this.PixelWidth = (data[4] + (data[5] << 8));
                    this.PixelHeight = (data[6] + (data[7] << 8));
                    this.tilesetOffset = (data[8] + (data[9] << 8));
                }*/
            }
            else
            {
                this.MapPosX = r.ReadUInt16(mapInfoRoomLoc);
                this.MapPosY = r.ReadUInt16();
                this.PixelWidth = r.ReadUInt16();
                this.PixelHeight = r.ReadUInt16();
                this.tilesetId = r.ReadUInt16();
            }

            /*//get addr of TPA data
            if (tilesetOffset != 0) {
                int a = 0; //DEBUG
            }
            int tilesetArea = r.ReadAddr(header.TilesetRoot + (areaId << 2));
            int tilesetRoom = r.ReadAddr(tilesetArea + (tilesetOffset << 2));

            r.SetPosition(tilesetRoom);

            ParseData(r, ValidateTileset);*/

            //get addr of room data 
            int tileDataArea = r.ReadAddr(header.TileDataRoot + (areaId << 2));
            int tileDataRoom = r.ReadAddr(tileDataArea + (roomId << 2));
            r.SetPosition(tileDataRoom);

            ParseData(r, ValidateRoomData);

            //attempt at obtaining chest data (+various)
            int listTableArea = r.ReadAddr(header.ListTableRoot + (areaId << 2));
            int listTableRoom = r.ReadAddr(listTableArea + (roomId << 2));

            int warpTableArea = r.ReadAddr(header.WarpTableRoot + (areaId << 2));
            int warpTableRoom = warpTableArea + (roomId << 2);

            G_listLoopVar = 0;
            bool hasMoreLists = true;
            while (hasMoreLists)
            {
                byte terminator = 0xFF;
                if (G_listLoopVar == 3)
                {
                    terminator = 0x0;
                }
                hasMoreLists = LoadGroupWithLinks(r, listTableRoom, terminator, Constants._listData, G_listLoopVar, 0x20, ListBinding); //listData1-X
                //Console.WriteLine("loaded: 0x" + (listTableRoom + 4 * G_listLoopVar).Hex() + " for list id " + G_listLoopVar);
                G_listLoopVar += 1;
            }

            //LoadGroup(r, listTableRoom + 12, 0x0, DataType.chestData.ToString(), 0x08, ChestBinding);
            //technically not a group but should work with the function
            LoadGroup(r, warpTableRoom, 0xFF, DataType.warpData.ToString(), 20, WarpBinding);
        }

        public bool LoadBGData(ref byte[] bgRoomData, bool isBg1)
        {
            AddrData? bgRoomDataAddr;
            //AddrData metaTilesetAddr;
            //AddrData metaTileTypeAddr;
            //Area area = Parent.Parent;

            if (isBg1)
            {
                bgRoomDataAddr = bg1RoomDataAddr;
                //metaTilesetAddr = area.Bg1MetaTilesetAddr.Value;
                //metaTileTypeAddr = area.Bg1MetaTileTypeAddr.Value;
            }
            else
            {
                bgRoomDataAddr = bg2RoomDataAddr;
                //metaTilesetAddr = area.Bg2MetaTilesetAddr.Value;
                //metaTileTypeAddr = area.Bg2MetaTileTypeAddr.Value;
            }


            if (bgRoomDataAddr == null) {
                return false;
            }

            //string bgPath = Parent.Path + "/" + DataType.bgData + (isBg1 ? 1 : 2) + "Dat.bin";
            string bgPath = Parent.Path + "/" + DataType.bgData + (isBg1 ? "01" : "02") + ".json";
            byte[] data = DataHelper.GetByteArrayFromJSON(bgPath, 2);
            //byte[] data = DataHelper.GetFromSavedData(bgPath, true);

            if (data == null) //no saved data, get from original source
            {
                data = DataHelper.GetData(bgRoomDataAddr.Value);
            }

            //not sure what this will break
            /*if (!(isBg1 && Bg1Use20344B0))
            {
                bgMetaTiles = new MetaTileset(metaTilesetAddr, metaTileTypeAddr, isBg1, area.Path);
            }*/

            data.CopyTo(bgRoomData, 0);
            return !(isBg1 && Bg1Use20344B0);
        }

        private bool LoadGroupWithLinks(Reader r, int addr, byte terminator, string dataType, int index, int size, Func<byte[], int, int> bindingFunc)
        {
            var tableAddr = r.ReadAddr(addr + 4 * index);
            if (usedAdresses.ContainsKey(tableAddr))
            {
                if (File.Exists($"{Parent.Path}/{dataType}{index.Hex(2)}.json"))
                {
                    listLinks.Add(0xFF);
                }
                else
                {
                    var listId = usedAdresses[tableAddr];
                    if (File.Exists($"{Parent.Path}/{dataType}{listId.Hex(2)}.json"))
                    {
                        //create change to always link to this list
                        Project.Instance.AddPendingChange(new ListDataChange(Parent.Parent.Id, Parent.Id, index));
                    }

                    listLinks.Add(listId);
                }
            }
            else
            {
                usedAdresses.Add(tableAddr, (byte)index);
                usedLists.Add((byte)index, tableAddr);
                listLinks.Add(0xFF);
            }

            return LoadGroup(r, addr + 4 * index, terminator, dataType + index.Hex(2), size, bindingFunc);
        }

        private bool LoadGroup(Reader r, int addr, byte terminator, string dataType, int size, Func<byte[], int, int> bindingFunc)
        {
            string dataPath = $"{Parent.Path}/{dataType}.json";

            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath);

                if (dataType.Contains(Constants._listData))
                {
                    ListStruct data = Newtonsoft.Json.JsonConvert.DeserializeObject<ListStruct>(json);
                    listInformation[G_listLoopVar] = data.list;
                    if (data.link != 0xff)
                    {
                        listLinks[G_listLoopVar] = (byte)data.link;
                    }
                }
                else if (dataType.Contains(DataType.warpData.ToString()))
                {
                    warpInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WarpData>>(json);
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
                    var readData = bindingFunc(data, 0);
                    if (readData == 0) break; //escape value for table pointers
                    r.SetPosition(r.Position - (size - readData)); //ex, only needed 8 bytes but read 16, jump back 8
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
                    this.hasPalette = true;
                    Console.WriteLine($"Optional paletteset {Parent.Parent.Id.Hex(2)}-{Parent.Id.Hex(2)} : {PaletteSetID.Hex(2)}");
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
            return new Rectangle(new Point(MapPosX >> 4, MapPosY >> 4), new Size(TileWidth, TileHeight));
        }

        #region list add/remove/get sets
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
            if (!listInformation.ContainsKey(listId))
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

        /*private int ChestBinding(byte[] data, int startIndex)
        {
            chestInformation.Add(new ChestData(data, startIndex));
            return 8;
        }*/

        private int WarpBinding(byte[] data, int startIndex)
        {
            warpInformation.Add(new WarpData(data, startIndex));
            return 20;
        }

        private int ListBinding(byte[] data, int startIndex)
        {
            List<byte> dataList = data.ToList();
            int dataSize;
            ObjectDefinitionParser.Filter f = ObjectDefinitionParser.FilterData(out dataSize, dataList, G_listLoopVar);
            List<List<byte>> list;

            if (listInformation.ContainsKey(G_listLoopVar))
            {
                list = listInformation[G_listLoopVar];
            }
            else
            {
                list = new List<List<byte>>();
            }

            if (f.elements.Count() != 0 && list.Count == 0)
            {
                foreach (var el in f.elements)
                {
                    if(el.valueType == ObjectDefinitionParser.ObjectValueType.TABLEPOINTER)
                    {
                        //ugly? yes, duplicate outside func? yes, avoids editing bindings to take another argument? yes
                        var header = ROM.Instance.headers;
                        var r = ROM.Instance.reader;
                        var roomId = Parent.Id;
                        var areaId = Parent.Parent.Id;
                        int listTableArea = r.ReadAddr(header.ListTableRoot + (areaId << 2));
                        int listTableRoom = r.ReadAddr(listTableArea + (roomId << 2));

                        var fullAddr = r.ReadInt(listTableRoom + 4 * G_listLoopVar);
                        var entry = new List<byte>();

                        entry.Add((byte)(fullAddr & 0xFF));
                        entry.Add((byte)((fullAddr >> 8) & 0xFF));
                        entry.Add((byte)((fullAddr >> 16) & 0xFF));
                        entry.Add((byte)((fullAddr >> 24) & 0xFF));
                        list.Add(entry);
                        listInformation[G_listLoopVar] = list;
                        return 0;
                    }
                }
            }

            var dat = new List<byte>(data.Skip(startIndex).Take(dataSize).ToArray());
            list.Add(dat);
            listInformation[G_listLoopVar] = list;
            return dataSize;
        }

        #endregion

        #region ParseData Func's
        //dont have any good names for these 3
        /*private bool ValidateTileset(AddrData data)
        {
            if ((data.dest & 0xF000000) != 0x6000000)
            { //not valid tile data addr
                Console.WriteLine("Unhandled tile data destination address: " + data.dest.Hex() + " Source:" + data.src.Hex() + " Compressed:" + data.compressed + " Size:" + data.size.Hex());
                return false;
            }

            data.dest = data.dest & 0xFFFFFF;
            this.tilesetAddrs.Add(data);
            return true;
        }*/

        private bool ValidateRoomData(AddrData data)
        {
            switch (data.dest)
            {
                case 0x02025EB4:
                    this.bg2RoomDataAddr = data;
                    //Debug.WriteLine("bg2 " + data.src.Hex());
                    break;
                case 0x0200B654:
                    this.bg1RoomDataAddr = data;
                    //Debug.WriteLine("bg1 " + data.src.Hex());
                    break;
                case 0x02002F00:
                    this.bg1RoomDataAddr = data;
                    //Debug.WriteLine("bg1 spec" + data.src.Hex());
                    this.Bg1Use20344B0 = true;
                    break;
                default:
                    //Debug.Write("Unhandled room data addr: ");
                    //Debug.Write(data.src.Hex() + "->" + data.dest.Hex());
                    //Debug.WriteLine(data.compressed ? " (compressed)" : "");
                    break;
            }
            return true;
        }

        #endregion

        #region bytify methods

        public long BytifyListInformation(out byte[] data, out byte linkId, int listId)
        {
            var list = listInformation[listId];
            var outdata = new byte[list.Count * 16 + 2];
            var loc = 0;
            for (int i = 0; i < list.Count; i++)
            {
                //list[i].CopyTo(outdata, i * 0x10);
                list[i].CopyTo(outdata, loc);
                loc += list[i].Count();
            }

            var link = listLinks[listId];

            data = outdata.Take(loc + 1).ToArray();

            if (listId != 3)
            {
                data[data.Length - 1] = 0xFF;
            }

            long addr = -1;
            var depth = 0;
            while (link != 0xFF && !usedLists.ContainsKey(link))
            {
                if (depth >= 10)
                {
                    MainWindow.Notify($"List link likely stuck in a loop for area:0x{Parent.Parent.Id.Hex()} room:0x{Parent.Id.Hex()} on a chain containing list:{link}", "Save failed");
                    break;
                }
                link = listLinks[link]; //in case of links to links
                depth++;
            }

            if (depth < 10 && link != 0xFF)
            {
                addr = (usedLists[link]);
            }
            linkId = link;
            return addr;
        }

        public string SerializeList(Change change)
        {
            var list = listInformation[change.identifier];
            return Newtonsoft.Json.JsonConvert.SerializeObject(new ListStruct(list, GetLinkFor(change.identifier)));
        }
        public string SerializeWarps()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(warpInformation);
        }

        public string SerializeMetaData()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new RoomMetaDataStruct(PixelWidth, PixelHeight, MapPosX, MapPosY, tilesetId));
        }

        public byte[] GetMetadataBytes()
        {
            var data = new byte[] {
                (byte)(MapPosX&0xff),
                (byte)(MapPosX>>8),
                (byte)(MapPosY&0xff),
                (byte)(MapPosY>>8),
                (byte)(PixelWidth&0xff),
                (byte)(PixelWidth>>8),
                (byte)(PixelHeight&0xff),
                (byte)(PixelHeight>>8),
                (byte)(tilesetId&0xff),
                (byte)(tilesetId>>8)
            };

            return data;
        }


        public struct ListStruct
        {
            public List<List<byte>> list;
            public int link;
            public ListStruct(List<List<byte>> list, int link)
            {
                this.list = list;
                this.link = link;
            }
        }

        public struct RoomMetaDataStruct
        {
            public int sizeX;
            public int sizeY;
            public int mapX;
            public int mapY;
            public int tilesetId;

            public RoomMetaDataStruct(int sizeX, int sizeY, int mapX, int mapY, int tilesetId)
            {
                this.sizeX = sizeX;
                this.sizeY = sizeY;
                this.mapX = mapX;
                this.mapY = mapY;
                this.tilesetId = tilesetId;
            }
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
        public int GetBGPointerLoc(BgDataChange change)
        {
            if (isCreated) {
                return -1;
            }

            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            var roomId = Parent.Id;
            var areaId = Parent.Parent.Id;

            int areaTileDataTableLoc = r.ReadAddr(header.TileDataRoot + (areaId << 2));
            int tileDataLoc = r.ReadAddr(areaTileDataTableLoc + (roomId << 2));
            r.SetPosition(tileDataLoc);

            if (change.identifier == 1)
            {
                ParseData(r, Bg1Check);
            }
            else if (change.identifier == 2)
            {
                ParseData(r, Bg2Check);
            }
            return (int)r.Position - 12; //step back 12 bytes as the bg was found after reading
        }

        public List<byte> GetAllLinkedTo(int listId)
        {
            var linkedListIds = new List<byte>();
            //crawl
            for (int i = 0; i < listLinks.Count; i++)
            {
                if (listLinks[i] == listId)
                {
                    linkedListIds.Add((byte)i);
                }
            }
            return linkedListIds;
        }

        public byte GetLinkFor(int listId)
        {
            return listLinks[listId];
        }

        public void SetLinkFor(int listId, byte value)
        {
            listLinks[listId] = value;
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
