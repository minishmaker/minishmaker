using System;
using System.Diagnostics;

namespace MinishMaker.Core
{
    public class Room
    {
        public int Index { get; private set; }

        public Room(int index)
        {
            Index = index;
        }
    }
}
