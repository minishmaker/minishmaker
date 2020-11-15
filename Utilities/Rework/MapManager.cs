using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinishMaker.Core;
using MinishMaker.Core.Rework;
using Room = MinishMaker.Core.Rework.Room;

namespace MinishMaker.Utilities.Rework
{
    public class MapManager
    {
        private static MapManager instance;
        private Dictionary<int, Area> areas;

        private MapManager() { }
        public static MapManager Get()
        {
            if (instance == null)
            {
                instance = new MapManager();
                instance.RegenerateAreas();
            }

            return instance;
        }

        private void RegenerateAreas()
        {
            areas = new Dictionary<int, Area>();

            // Loop through known map address table.
            for (int areaNum = 0; areaNum < 0x90; areaNum++)
            {
                var areaKey = new Tuple<int, int>(areaNum, -1);
                var areaName = "";

                if (Core.Rework.Project.Instance.roomNames.ContainsKey(areaKey))
                {
                    areaName = Core.Rework.Project.Instance.roomNames[areaKey];
                }

                Area area = new Area(areaNum, areaName);

                for (int roomNum = 0; roomNum < 0x40; roomNum++)
                {
                    if (IsValidRoom(areaNum, roomNum))
                    {
                        if (IsStableRoom(areaNum, roomNum))
                        {
                            var roomName = "";
                            var roomKey = new Tuple<int, int>(areaNum, roomNum);

                            if (Core.Rework.Project.Instance.roomNames.ContainsKey(roomKey))
                            {
                                roomName = Core.Rework.Project.Instance.roomNames[roomKey];
                            }

                            area.AddRoom(new Room(roomNum, roomName, area));
                        }
                    }

                    else break;
                }

                // At least one room in area, so add to list.
                if (area.HasRooms())
                {
                    areas.Add(areaNum, area);
                }
            }
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

        public Room GetRoom(int areaId, int roomId)
        {
            return GetArea(areaId).GetRoom(roomId);
        }

        public Area GetArea(int areaId)
        {
            if (!areas.ContainsKey(areaId))
            {
                throw new AreaException($"Area with id {areaId.Hex()} does not exist.");
            }

            return areas[areaId];
        }

        public List<Area> GetAllAreas()
        {
            List<Area> areaList = new List<Area>();
            foreach (var key in areas.Keys) 
            {
                areaList.Add(areas[key]);
            }
            return areaList;
        }

        public bool RoomExists(int areaId, int roomId)
        {
            return areas.ContainsKey(areaId) && areas[areaId].RoomExists(roomId);
        }
    }
}
