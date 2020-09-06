using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinishMaker.Utilities;

namespace MinishMaker.Core.Rework
{
    public class Area
    {
        public int Id { get; private set; }
        public string NiceName { get; private set; }
        public int nameId;
        public int songId;
        
        public bool hasRedName;
        public bool canFlute;
        public bool isMoleCave;
        public bool hasKeyCounter;
        public bool usesDungeonMap;

        public bool usesUnknown1;
        public bool usesUnknown2;

        public byte flagOffset;

        private bool infoLoaded = false;

        private string path;
        public string Path {
            get 
            {
                if (path.Length == 0)
                {
                    path = Project.Instance.projectPath + "/Areas/Area " + StringUtil.AsStringHex2(Id);
                }
                return path;
            }
        }
        public TileSet TileSet { get; private set; }
        public MetaTileSet MetaTileSet { get; private set; }

        private Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        public Core.AddrData bg1MetaTilesAddr { get; private set; }
        public Core.AddrData bg2MetaTilesAddr { get; private set; }
        public Core.AddrData bg1MetaTileTypeAddr { get; private set; }
        public Core.AddrData bg2MetaTileTypeAddr { get; private set; }

        public Area(int areaId, string niceName)
        {
            this.Id = areaId;
            this.NiceName = $"({this.Id}) {niceName}";

            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;

            //metatiles
            int metaTileSetsLoc = r.ReadAddr(header.globalMetaTileSetTableLoc + (areaId << 2));

            r.SetPosition(metaTileSetsLoc);

            bool cont = true;
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
                    Core.AddrData addrdata = new Core.AddrData(source, dest, size, compressed);
                    switch (addrdata.dest)
                    {
                        case 0x0202CEB4:
                            this.bg2MetaTilesAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " bg2");
                            break;
                        case 0x02012654:
                            this.bg1MetaTilesAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " bg1");
                            break;
                        case 0x0202AEB4:
                            this.bg2MetaTileTypeAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " type2");
                            break;
                        case 0x02010654:
                            this.bg1MetaTileTypeAddr = addrdata;
                            Debug.WriteLine(addrdata.src.Hex() + " type1");
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
            }
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

            byte[] data;

            string areaInfoPath = path + "/" + DataType.areaInfo + "Dat.bin";

            if (File.Exists(areaInfoPath))
            {
                data = File.ReadAllBytes(areaInfoPath);
            }
            else
            {
                var reader = ROM.Instance.reader;
                var dataloc = ROM.Instance.headers.areaInformationTableLoc + Id * 4;
                reader.SetPosition(dataloc);
                data = reader.ReadBytes(4);
            }

            SetInfo(data);

            infoLoaded = true;
        }

        public void SetInfo(byte[] data)
        {
            canFlute = (data[0] % 2 == 1);//bit 1

            data[0] = (byte)(data[0] >> 1);
            hasKeyCounter = (data[0] % 2 == 1);//bit 2

            data[0] = (byte)(data[0] >> 1);
            hasRedName = (data[0] % 2 == 1);//bit 4

            data[0] = (byte)(data[0] >> 1);
            usesDungeonMap = (data[0] % 2 == 1);//bit 8

            data[0] = (byte)(data[0] >> 1);
            usesUnknown1 = (data[0] % 2 == 1);//bit 10 //currently unknown use

            data[0] = (byte)(data[0] >> 1);
            isMoleCave = (data[0] % 2 == 1);//bit 20

            data[0] = (byte)(data[0] >> 1);
            usesUnknown2 = (data[0] % 2 == 1);//bit 40 //unknown

            data[0] = (byte)(data[0] >> 1);
            canFlute = (data[0] % 2 == 1 || canFlute);//bit 80 //unused in eur, seems to be same as bit 1?

            nameId = data[1];

            flagOffset = data[2];

            songId = data[3];
        }
    }

    public class AreaException : Exception
    {
        public AreaException() { }
        public AreaException(string message) : base(message) { }
        public AreaException(string message, Exception inner) : base(message, inner) { }
    }
}
