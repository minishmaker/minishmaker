using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace MinishMaker.Core
{
    public class MapManager
    {
        // Area wrapper for room list
        public struct Area
        {
            public int Index { get; private set; }
            public List<Room> areaRooms { get; private set; }

            public Area(int index)
            {
                Index = index;
                areaRooms = new List<Room>();
            }

            public void Add(Room room)
            {
                areaRooms.Add(room);
            }

            public void Clear()
            {
                areaRooms.Clear();
            }

            public int Count()
            {
                return areaRooms.Count;
            }

            public List<Room> Rooms()
            {
                return areaRooms;
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
            for (int areaNum = 0; areaNum < 0x90; areaNum++)
            {
                Area area = new Area(areaNum);
                int roomNum = 0;
                do
                {
                    if (IsValidRoom(areaNum, roomNum))
                    {
                        if (roomNum == 0)
                        {
                            area.Clear();
                        }

                        area.Add(new Room());
                        roomNum++;
                    }

                    // Obviously temp, remove once IsValidRoom complete. arbitrary number for testing.
                    if (roomNum == 20) break;

                    else break;
                } while (true);

                // Rooms exist in this area, so add to list.
                if (area.Count() > 0)
                {
                    areas.Add(area);
                }
            }
            MapAreas = areas;
        }

        private bool IsValidRoom(int area, int room)
        {
            // TODO Actually check the rooms
            return true;
        }
    }
}