using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public class MapManager
    {
        // Area wrapper for room list
        public struct Area
        {
            public int Index { get; private set; }
            private readonly List<Room> AreaRooms;

            public Area(int index)
            {
                Index = index;
                AreaRooms = new List<Room>();
            }

            public void Add(Room room)
            {
                AreaRooms.Add(room);
            }

            public int Count
            {
                get { return AreaRooms.Count; }
            }


            public List<Room> Rooms
            {
                get { return AreaRooms; }
            }
        }

        public static MapManager Instance { get; private set; }

        public List<Area> MapAreas { get; private set; }

        public MapManager()
        {
            Instance = this;

            RegenerateMapList();
        }

        private void RegenerateMapList()
        {
            List<Area> areas = new List<Area>();
            // Loop through known map address table.
            for (int areaNum = 0; areaNum < 0x90; areaNum++)
            {
                Area area = new Area(areaNum);
                int roomNum = 0;
                do
                {
                    if (IsValidRoom(areaNum, roomNum))
                    {
                        // Setup new room data
                        area.Add(new Room());
                        roomNum++;
                    }

                    else break;
                } while (true);

                // At least one room in area, so add to list.
                if (area.Count > 0)
                {
                    areas.Add(area);
                    Console.WriteLine("-------------");
                }
            }
            MapAreas = areas;
        }

        private bool IsValidRoom(int area, int room)
        {
            // Area addresses are 4 bytes long
            int searchaddr = ROM.Instance.headers.MapHeaderBase + (area << 2);
            int addr = (ROM.Instance.reader.ReadAddr(searchaddr));

            // Not a valid data address as doesn't point to anywhere
            if (addr == 0) return false;

            
            int roomaddr = ROM.Instance.reader.ReadUInt16(addr + room * 0x0A);
            
            if (roomaddr == 0xFFFF) return false;
            ROM.Instance.reader.ReadUInt32();
            ROM.Instance.reader.ReadUInt16();

            // Debug prints
            Debug.WriteLine("Area: {0} Room: {1}", StringUtil.AsStringHex2(area), StringUtil.AsStringHex2(room));
            Debug.WriteLine("Area Data Address: {0}\nArea Data Header: {1}", StringUtil.AsStringGBAAddress(searchaddr), StringUtil.AsStringGBAAddress(addr));
            Debug.WriteLine("Room header: {0}", StringUtil.AsStringGBAAddress(addr + room * 0x0A));
            Debug.WriteLine("Header Value: {0}", StringUtil.AsStringHex4(roomaddr));

            int finalval = ROM.Instance.reader.ReadUInt16();
            Debug.WriteLine("Final checked Value: {0}", StringUtil.AsStringHex4(finalval));
            Debug.WriteLine("Comparison check: {0}", StringUtil.AsStringHex4(finalval & 0x8000));

            // BUG This final check isn't perfect. Will think some rooms are valid and others not, particularly when the game would usually softlock. See room doc for more details.
            return (finalval & 0x8000) == 0;
        }
    }
}