using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core
{
    public class SpaceManager
    {
        public class SpaceData
        {
            public int start;
            public int size;

            public SpaceData(int start, int size)
            {
                this.start = start;
                this.size = size;
            }

            public void ReserveBytes(int size)
            {
                this.start += size;
                this.size -= size;
            }

            public void Extend(int start, int size)
            {
                int end = Math.Max(start + size, this.start + this.size);
                this.start = Math.Min(start, this.start);
                this.size = end - start;
            }
        }

        public static SpaceManager Instance;
        public List<SpaceData> spaceData;

        public SpaceManager()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            spaceData = new List<SpaceData>();
        }

        public int ReserveSpace(int size)
        {

            foreach (SpaceData space in spaceData)
            {
                if (space.size < size)
                    continue;
                int location = space.start;
                space.ReserveBytes(size);
                return location;
            }
            return 0;
        }

        public SpaceData FreeSpace(int start, int size)
        {
            // Connect to any overlapping space if possible
            foreach (SpaceData space in spaceData)
            {
                if (space.start <= start + size || start <= space.start + space.size)
                {
                    space.Extend(start, size);
                    return space;
                }
            }

            SpaceData newSpace = new SpaceData(start, size);
            spaceData.Add(newSpace);
            return newSpace;
        }

        public void LoadDefaultSpaces()
        {
            spaceData.Add(new SpaceData(0x607970, 0x7B60));
            spaceData.Add(new SpaceData(0x72CD90, 0x65C0));
            spaceData.Add(new SpaceData(0x73FA10, 0x5D40));
            spaceData.Add(new SpaceData(0x7AAAF0, 0x8FC0));
            spaceData.Add(new SpaceData(0x7B9AF0, 0x67C0));
            spaceData.Add(new SpaceData(0x7C62F0, 0x67C0));
            spaceData.Add(new SpaceData(0x7DF2F0, 0x67C0));
            spaceData.Add(new SpaceData(0x7EBAF0, 0x67C0));
            spaceData.Add(new SpaceData(0x7F82F0, 0x27C0));
            spaceData.Add(new SpaceData(0x800AF0, 0x27C0));
            spaceData.Add(new SpaceData(0x8092F0, 0x27C0));
            spaceData.Add(new SpaceData(0x811AF0, 0x27C0));
            spaceData.Add(new SpaceData(0x81A2F0, 0x27C0));
            spaceData.Add(new SpaceData(0x822AF0, 0x27C0));
            spaceData.Add(new SpaceData(0x82B2F0, 0x27C0));
            spaceData.Add(new SpaceData(0x833AF0, 0x27C0));
            spaceData.Add(new SpaceData(0x83D2F0, 0x17C0));
            spaceData.Add(new SpaceData(0xEF333C, 0x10CCC0));
        }
    }
}
