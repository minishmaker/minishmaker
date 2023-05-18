using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public class Area
    {
        public int Id { get; private set; }
        public string NiceName { get; private set; }

        public AreaInfo areaInfo;
        private bool infoLoaded = false;

        private string path = "";

        public static PaletteSetManager psm = PaletteSetManager.Get();

        public string Path {
            get 
            {
                if (path.Length == 0)
                {
                    path = Project.Instance.ProjectPath + "/Areas/Area " + StringUtil.AsStringHex2(Id);
                }
                return path;
            }
        }

        private int tilesetCount = 0; //temp value to pass to the validator
        private List<List<AddrData>> tilesetAddrs = new List<List<AddrData>>();
        public List<Tileset> Tilesets { get; private set; }

        private Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        public Nullable<AddrData> Bg1MetaTilesetAddr  { get; private set; }
        public Nullable<AddrData> Bg2MetaTilesetAddr  { get; private set; }
        public Nullable<AddrData> Bg1MetaTileTypeAddr { get; private set; }
        public Nullable<AddrData> Bg2MetaTileTypeAddr { get; private set; }

        public MetaTileset Bg1MetaTileset { get; private set; }
        public MetaTileset Bg2MetaTileset { get; private set; }

        private int PaletteSetId = 0;
        public Area(int areaId, string niceName)
        {
            this.Id = areaId;
            this.NiceName = niceName;
            Tilesets = new List<Tileset>();
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;

            //metatiles
            int tilesetLoc = r.ReadAddr(header.globalTilesetTableLoc + (areaId << 2));

            r.SetPosition(tilesetLoc);

            ParseData(r, ValidateAddr);


            if(Bg1MetaTilesetAddr != null && Bg1MetaTilesetAddr != null)
            {
                Bg1MetaTileset = new MetaTileset(Bg1MetaTilesetAddr.Value, Bg1MetaTileTypeAddr.Value, true, this.Path);
            }

            if (Bg2MetaTilesetAddr != null && Bg2MetaTilesetAddr != null)
            {
                Bg2MetaTileset = new MetaTileset(Bg2MetaTilesetAddr.Value, Bg2MetaTileTypeAddr.Value, false, this.Path);
            }

            int tilesetArea = r.ReadAddr(header.metaTilesetRoot + (areaId << 2));
            uint tilesetEntry = r.ReadUInt32(tilesetArea);
            while (tilesetEntry >> 24 == 8)
            {
                r.SetPosition(tilesetEntry & 0xFFFFFF);
                var count = tilesetAddrs.Count;
                this.tilesetAddrs.Add(new List<AddrData>());
                ParseData(r, ValidateTileset);
                Tilesets.Add(new Tileset(tilesetAddrs[count], PaletteSetId, $"{this.Path}/{DataType.tileset}{tilesetCount}"));
                tilesetCount++;
                tilesetEntry = r.ReadUInt32(tilesetArea + (tilesetCount << 2));
            }


            /*bool cont = true;
            while (cont)
            {
                uint data = r.ReadUInt32();
                uint data2 = r.ReadUInt32();
                uint data3 = r.ReadUInt32();

                if (data2 == 0)
                { //palette but should never be set here as its a per room thing

                }
                else
                {
                    int source = (int)((data & 0x7FFFFFFF) + header.gfxSourceBase); //08324AE4 is tile gfx base
                    int dest = (int)(data2 & 0x7FFFFFFF);
                    bool compressed = (data3 & 0x80000000) != 0; //high bit of size determines LZ or DMA
                    int size = (int)(data3 & 0x7FFFFFFF);
                    AddrData addrdata = new AddrData(source, dest, size, compressed);
                    switch (addrdata.dest)
                    {
                        case 0x0202CEB4:
                            this.bg2MetaTilesAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " meta2 " + areaId.Hex());
                            break;
                        case 0x02012654:
                            this.bg1MetaTilesAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " meta1 " + areaId.Hex());
                            break;
                        case 0x0202AEB4:
                            this.bg2MetaTileTypeAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " type2 " + areaId.Hex());
                            break;
                        case 0x02010654:
                            this.bg1MetaTileTypeAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " type1 " + areaId.Hex());
                            break;
                        default:
                            Debug.Write("Unhandled metatile addr: ");
                            Debug.Write(addrdata.src.Hex() + "->" + addrdata.dest.Hex());
                            Debug.WriteLine(addrdata.compressed ? " (compressed)" : "");
                            break;
                    }
                }
                if (cont == true)
                {
                    cont = (data & 0x80000000) != 0; //high bit determines if more to load
                }
            }*/
        }

        public Room GetRoom(int roomId)
        {
            if (!rooms.ContainsKey(roomId))
            {
                throw new AreaException($"Room id {roomId.Hex()} does not exist within area {Id.Hex()}");
            }

            return rooms[roomId];
        }

        public List<Room> GetAllRooms()
        {
            List<Room> roomList = new List<Room>();
            foreach (var key in rooms.Keys)
            {
                roomList.Add(rooms[key]);
            }
            return roomList;
        }

        public bool RoomExists(int roomId)
        {
            return rooms.ContainsKey(roomId);
        }

        public void AddRoom(Room room)
        {
            rooms.Add(room.Id, room);
        }

        public bool HasRooms()
        {
            return rooms.Count > 0;
        }

        public void LoadAreaInfo()
        {
            if (infoLoaded)
            {
                return;
            }

            //byte[] data;

            //string areaInfoPath = this.Path + "/" + DataType.areaInfo + "Dat.bin";
            string areaInfoPath = this.path + "/" + DataType.areaInfo + ".json";
            if (File.Exists(areaInfoPath))
            {
                //data = File.ReadAllBytes(areaInfoPath);
                areaInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AreaInfo>(File.ReadAllText(areaInfoPath));
            }
            else
            {
                var reader = ROM.Instance.reader;
                var dataloc = ROM.Instance.headers.areaInformationTableLoc + Id * 4;
                reader.SetPosition(dataloc);
                SetInfo(reader.ReadBytes(4));
                //data = reader.ReadBytes(4);
            }

            infoLoaded = true;
        }

        public void SetInfo(byte[] data)
        {
            areaInfo = new AreaInfo();
            areaInfo.canFlute = (data[0] % 2 == 1);//bit 1

            data[0] = (byte)(data[0] >> 1);
            areaInfo.hasKeyCounter = (data[0] % 2 == 1);//bit 2

            data[0] = (byte)(data[0] >> 1);
            areaInfo.hasRedName = (data[0] % 2 == 1);//bit 4

            data[0] = (byte)(data[0] >> 1);
            areaInfo.usesDungeonMap = (data[0] % 2 == 1);//bit 8

            data[0] = (byte)(data[0] >> 1);
            areaInfo.usesUnknown1 = (data[0] % 2 == 1);//bit 10 //currently unknown use

            data[0] = (byte)(data[0] >> 1);
            areaInfo.isMoleCave = (data[0] % 2 == 1);//bit 20

            data[0] = (byte)(data[0] >> 1);
            areaInfo.usesUnknown2 = (data[0] % 2 == 1);//bit 40 //unknown

            data[0] = (byte)(data[0] >> 1);
            areaInfo.canFlute = (data[0] % 2 == 1 || areaInfo.canFlute);//bit 80 //unused in eur, seems to be same as bit 1?

            areaInfo.nameId = data[1];

            areaInfo.flagOffset = data[2];

            areaInfo.songId = data[3];
        }

        public byte[] GetInfoBytes()
        {
            var data = new byte[4];
            data[0]= (byte)((areaInfo.canFlute ? 0x81 : 0) + (areaInfo.hasKeyCounter ? 2 : 0) + (areaInfo.hasRedName ? 4 : 0) + (areaInfo.usesDungeonMap ? 8 : 0) + (areaInfo.usesUnknown1 ? 0x10 : 0) + (areaInfo.isMoleCave ? 0x20 : 0) + (areaInfo.usesUnknown2 ? 0x40 : 0));
            data[1] = areaInfo.nameId;
            data[2] = areaInfo.flagOffset;
            data[3] = areaInfo.songId;

            return data;
        }

        public string GetInfoJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(areaInfo);
        }

        private void ParseData(Reader r, Func<AddrData, bool> postFunc)
        {
            var header = ROM.Instance.headers;
            bool cont = true;
            while (cont)
            {
                uint data = r.ReadUInt32();
                uint data2 = r.ReadUInt32();
                uint data3 = r.ReadUInt32();



                if (data2 == 0)
                { //palette only in room?
                    this.PaletteSetId = (int)(data & 0x7FFFFFFF); //mask off high bit
                    //Console.WriteLine($"area {Id.Hex(2)} : {PaletteSetId.Hex(2)}");
                    psm.ReportUsage(Id, PaletteSetId);
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

        private bool ValidateTileset(AddrData data)
        {
            if ((data.dest & 0xF000000) != 0x6000000)
            { //not valid tile data addr
                Console.WriteLine("Unhandled tile data destination address: " + data.dest.Hex() + " Source:" + data.src.Hex() + " Compressed:" + data.compressed + " Size:" + data.size.Hex());
                return false;
            }

            data.dest = data.dest & 0xFFFFFF;
            this.tilesetAddrs[tilesetCount].Add(data);
            return true;
        }

        private bool ValidateAddr(AddrData addrdata)
        {
            switch (addrdata.dest)
            {
                case 0x0202CEB4:
                    this.Bg2MetaTilesetAddr = addrdata;
                    //Debug.WriteLine(addrdata.src.Hex() + " tileset2 " + this.Id.Hex(2));
                    break;
                case 0x02012654:
                    this.Bg1MetaTilesetAddr = addrdata;
                    //Debug.WriteLine(addrdata.src.Hex() + " tileset1 " + this.Id.Hex(2));
                    break;
                case 0x0202AEB4:
                    this.Bg2MetaTileTypeAddr = addrdata;
                    //Debug.WriteLine(addrdata.src.Hex() + " type2 " + this.Id.Hex(2));
                    break;
                case 0x02010654:
                    this.Bg1MetaTileTypeAddr = addrdata;
                    //Debug.WriteLine(addrdata.src.Hex() + " type1 " + this.Id.Hex(2));
                    break;
                default:
                    //Debug.Write("Unhandled addr: ");
                    //Debug.Write(addrdata.src.Hex() + "->" + addrdata.dest.Hex());
                    //Debug.WriteLine(addrdata.compressed ? " (compressed)" : "");
                    break;
            }
            return true;
        }

        public byte[] GetMetaTileData(ref byte[] tileType, int tileNum, int layer)
        {
            
            switch (layer)
            {
                case 1:
                    tileType = Bg1MetaTileset.GetMetaTileTypeData(tileNum);
                    return Bg1MetaTileset.GetMetaTileData(tileNum);
                case 2:
                    tileType = Bg2MetaTileset.GetMetaTileTypeData(tileNum);
                    return Bg2MetaTileset.GetMetaTileData(tileNum);
                default:
                    return null;
            }
        }

        public void SetMetaTileData(byte[] data, int tileNum, int layer)
        {
            switch (layer)
            {
                case 1:
                    Bg1MetaTileset.SetMetaTileData(data, tileNum);
                    break;
                case 2:
                    Bg2MetaTileset.SetMetaTileData(data, tileNum);
                    break;
            }
        }

        public void SetMetaTileTypeInfo(byte[] typeData, int tileNum, int layer)
        {
            switch (layer)
            {
                case 1:
                    Bg1MetaTileset.SetMetaTileTypeData(typeData, tileNum);
                    break;
                case 2:
                    Bg2MetaTileset.SetMetaTileTypeData(typeData, tileNum);
                    break;
            }
        }

        public long GetCompressedMetaTiles(ref byte[] data, int layer)
        {
            var metaTileset = layer == 1 ? Bg1MetaTileset : Bg2MetaTileset;
            data = new byte[0];
            var size = metaTileset.CompressTileset(ref data);
            return size;
        }

        public long GetCompressedMetaTileTypes(ref byte[] data, int layer)
        {
            var metaTileset = layer == 1 ? Bg1MetaTileset : Bg2MetaTileset;
            data = new byte[0];
            var size = metaTileset.CompressTileTypes(ref data);
            return size;
        }

        public int GetParsedPointerLoc(AreaChangeBase change)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int tilesetsAddrLoc = r.ReadAddr(header.globalTilesetTableLoc + (Id << 2));
            switch (change.changeType)
            {
                case DataType.metaTileType:
                    
                    r.SetPosition(tilesetsAddrLoc);

                    if (change.identifier == 1)
                    {
                        ParseData(r, MetaTileType1Check);
                    }
                    else if (change.identifier == 2)
                    {
                        ParseData(r, MetaTileType2Check);
                    }
                    return (int)r.Position - 12; //step back 12 bytes as the bg was found after reading

                case DataType.metaTileset: //actually metatileset, still needs to be converted in upgrade method
                    r.SetPosition(tilesetsAddrLoc);

                    if (change.identifier == 1)
                    {
                        ParseData(r, MetaTileset1Check);
                    }
                    else if (change.identifier == 2)
                    {
                        ParseData(r, MetaTileset2Check);
                    }
                    return (int)r.Position - 12; //step back 12 bytes as the bg was found after reading

                case DataType.tileset: //actually tileset
                    //get addr of TPA data

                    int metaTilesetArea = r.ReadAddr(header.metaTilesetRoot + (Id << 2));
                    int tsetId = change.identifier / 10;
                    int tsetPart = change.identifier % 10;
                    int metaTilesetEntry = r.ReadAddr(metaTilesetArea + (tsetId << 2));
                    r.SetPosition(metaTilesetEntry);

                    if (tsetPart == (int)Tileset.TilesetDataType.BG1)
                    {
                        ParseData(r, Bg1TilesetCheck);
                    }
                    else if (tsetPart == (int)Tileset.TilesetDataType.BG2)
                    {
                        ParseData(r, Bg2TilesetCheck);
                    }
                    else if (tsetPart == (int)Tileset.TilesetDataType.COMMON)
                    {
                        ParseData(r, CommonTilesetCheck);
                    }
                    return (int)r.Position - 12;

                default:
                    throw new ArgumentOutOfRangeException("The change is not correct: " + change);
            }
        }
        private bool MetaTileType1Check(AddrData data)
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

        private bool MetaTileType2Check(AddrData data)
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

        private bool MetaTileset1Check(Core.AddrData data)
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

        private bool MetaTileset2Check(AddrData data)
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

        private bool Bg1TilesetCheck(AddrData data)
        {
            return (data.dest & 0xFFFF) != 0;
        }

        private bool Bg2TilesetCheck(AddrData data)
        {
            return (data.dest & 0xFFFF) != 0x8000;
        }

        private bool CommonTilesetCheck(AddrData data)
        {
            return (data.dest & 0xFFFF) != 0x4000;
        }
    }

    public struct AreaInfo {
        public byte nameId;
        public byte songId;

        public bool hasRedName;
        public bool canFlute;
        public bool isMoleCave;
        public bool hasKeyCounter;
        public bool usesDungeonMap;

        public bool usesUnknown1;
        public bool usesUnknown2;

        public byte flagOffset;
    }

    public class AreaException : Exception
    {
        public AreaException() { }
        public AreaException(string message) : base(message) { }
        public AreaException(string message, Exception inner) : base(message, inner) { }
    }
}
