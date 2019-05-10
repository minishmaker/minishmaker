using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

            public int Count()
            {
                return AreaRooms.Count;
            }

            public List<Room> Rooms()
            {
                return AreaRooms;
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

                for (int roomNum = 0; roomNum < 0x40; roomNum++)
                {
                    if (IsValidRoom(areaNum, roomNum))
                    {
                        if (IsStableRoom(areaNum, roomNum))
                        {
                            area.Add(new Room(roomNum));
                        }
                    }
                    else break;
                }

                // At least one room in area, so add to list.
                if (area.Count() > 0)
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

            int roomaddr = addr + room * 0x0A;
            int roomheader = ROM.Instance.reader.ReadUInt16(roomaddr);

            // Debug prints
            Console.WriteLine("Area: {0} Room: {1}", StringUtil.AsStringHex2(area), StringUtil.AsStringHex2(room));
            Console.WriteLine("Area Data Address: {0}\nArea Data Header: {1}", StringUtil.AsStringGBAAddress(searchaddr), StringUtil.AsStringGBAAddress(addr));
            Console.WriteLine("Room header: {0}", StringUtil.AsStringGBAAddress(addr + room * 0x0A));
            Console.WriteLine("Header Value: {0}", StringUtil.AsStringHex4(roomaddr));

            return roomheader != 0xFFFF;
        }

        private bool IsStableRoom(int area, int room)
        {

            int areasearchaddr = ROM.Instance.headers.AreaMetadataBase + (area << 2);
            int areaaddr = ROM.Instance.reader.ReadAddr(areasearchaddr);

            // This used to happen sometimes for some reason, so I left the check in
            if (areaaddr == 0x000000) return false;

            int roomsearchaddr = areaaddr + (room << 2);
            int roomaddr = ROM.Instance.reader.ReadAddr(roomsearchaddr);

            // If the room is considered valid and has metadata, it's probably stable. BUG: Fortress of Winds 05 and 06, as well as Area 67 Room 04 are false positives
            return roomaddr != 0x000000;
        }
    }
}