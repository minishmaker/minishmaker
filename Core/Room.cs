using System;
using System.Diagnostics;

namespace MinishMaker.Core
{
    public class Room
    {
        public struct RoomData
        {
            public int width;
            public int height;
        }

        public RoomData RoomMetaData { get; private set; }

        public Room(int area, int room)
        {
            RoomData data = new RoomData();
            int baseAddress = ROM.Instance.reader.ReadAddr(ROM.Instance.headers.MapHeaderBase + (area << 2));
            data.width = ROM.Instance.reader.ReadUInt16(baseAddress + (room * 0x0A) +4) >> 4;
            data.height = ROM.Instance.reader.ReadUInt16() >> 4;
            Debug.WriteLine("Width {0} Height {1}", data.width, data.height);
        }
    }
}
