using System;
using System.Drawing;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public class Room
    {
        public Area Parent { get; private set; }
        public int Id { get; private set; }
        private bool isLoaded = false;
        private string path = "";
        public string Path {
            get
            {
                if (path.Length == 0)
                {
                    path = Parent.Path+ "/Room " + StringUtil.AsStringHex2(Id);
                }
                return path;
            }
        }
        private RoomMetaData metadata;
        public RoomMetaData MetaData {
            get
            {
                if (metadata == null)
                {
                    LoadRoom();
                }

                return metadata;
            }
        }

        public Point RoomSize { get { return new Point(MetaData.TileWidth, MetaData.TileHeight); } }
        public string NiceName { get; private set; }

        public bool Bg1Exists { get; private set; }
        public bool Bg2Exists { get; private set; }
        public bool IsCreated { get; private set; }

        //private MetaTileset bg2MetaTiles ;
        //public MetaTileset Bg2MetaTiles { get { return bg2MetaTiles; } }

        //private MetaTileset bg1MetaTiles;
        //public MetaTileset Bg1MetaTiles { get { return bg1MetaTiles; } }


        private byte[] bg2RoomData;
        private byte[] bg1RoomData;

        public Room(int roomId, string niceName, Area parent, bool isCreated = false)
        {
            this.Id = roomId;
            this.NiceName = niceName;
            this.Parent = parent;
            this.isLoaded = false;
            this.IsCreated = isCreated;
        }

        public void LoadRoom()
        {
            if (isLoaded) return; //no longer any checks for it needed above this

            metadata = new RoomMetaData(this, IsCreated);

            //room data
            bg2RoomData = new byte[0x2000]; //this seems to be the maximum size
            bg1RoomData = new byte[0x2000]; //2000 isnt too much memory, so this is just easier

            Bg2Exists = MetaData.LoadBGData(ref bg2RoomData, false);//loads in the data and tiles
            Bg1Exists = MetaData.LoadBGData(ref bg1RoomData, true);

            if (!Bg1Exists && !Bg2Exists)
            {
                throw new RoomException("No layers exist for this room, the room is highly likely invalid.");
            }

            isLoaded = true;
        }

        public int GetTileData(int layer, int position)
        {
            try
            {
                switch(layer)
                {
                    case 1: //bg1
                        return bg1RoomData[position * 2] | bg1RoomData[position * 2 + 1] << 8;
                    case 2: //bg2
                        return bg2RoomData[position * 2] | bg2RoomData[position * 2 + 1] << 8;
                    default:
                        return -1;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"{e.Message}\nSelected position {position} on layer {layer} out of range.");
            }

            return -1;
        }

        public void SetTileData(int layer, int position, int data)
        {
            byte high = (byte)(data >> 8);
            byte low = (byte)(data & 0xFF);
            var pos = position * 2;

            if (layer == 1)//bg1
            {
                bg1RoomData[pos] = low;
                bg1RoomData[pos + 1] = high;
            }
            else if (layer == 2)//bg2
            {
                bg2RoomData[pos] = low;
                bg2RoomData[pos + 1] = high;
            }
        }


        public long GetBGData(ref byte[] data, int layer)
        {
            byte[] bgData = layer == 1 ? bg1RoomData : bg2RoomData;
            return DataHelper.CompressData(ref data, bgData); 
        }


        public string GetBGJSON(Change change)
        {
            var bg = change.identifier == 1 ? bg1RoomData : bg2RoomData;
            byte[] truncated = new byte[metadata.TileWidth * MetaData.TileHeight * 2];
            Array.Copy(bg, truncated, truncated.Length);
            var width = metadata.TileWidth;
            return DataHelper.ByteArrayToFormattedJSON(truncated, width, 2);
        }


        public void ResizeRoom(int xdim, int ydim)
        {
            var newSize = xdim * ydim * 2;

            var differenceY = ydim - this.RoomSize.Y;

            var bg1New = new byte[newSize];
            var bg2New = new byte[newSize];

            var lowestY = this.RoomSize.Y < ydim ? this.RoomSize.Y : ydim;
            var lowestX = this.RoomSize.X < xdim ? this.RoomSize.X : xdim;

            for (int i = 0; i < lowestY; i++)
            {
                var src = i * this.RoomSize.X * 2;
                var dest = i * xdim * 2;
                if (Bg1Exists)
                {
                    Array.Copy(bg1RoomData, src, bg1New, dest, lowestX * 2);
                }
                if (Bg2Exists)
                {
                    Array.Copy(bg2RoomData, src, bg2New, dest, lowestX * 2);
                }
            }

            if (differenceY > 0)
            {
                for (int i = 0; i < differenceY; i++)
                {
                    for (int j = 0; j < xdim * 2; j++)
                    {
                        var pos = (this.RoomSize.Y + i) * xdim * 2 + j;
                        if (Bg1Exists)
                        {
                            bg1New.SetValue((byte)0, pos);
                        }
                        if (Bg2Exists)
                        {
                            bg2New.SetValue((byte)0, pos);
                        }
                    }
                }
            }

            if (Bg1Exists)
            {
                bg1RoomData = bg1New;
                Project.Instance.AddPendingChange(new ChangeTypes.BgDataChange(this.Parent.Id, this.Id, 1));
            }

            if (Bg2Exists)
            {
                bg2RoomData = bg2New;
                Project.Instance.AddPendingChange(new ChangeTypes.BgDataChange(this.Parent.Id, this.Id, 2));
            }

            MetaData.SetRoomSize(xdim, ydim);
            Project.Instance.AddPendingChange(new ChangeTypes.RoomMetadataChange(this.Parent.Id, this.Id));
        }

        public int[] GetSameTiles(int layer, int tileData)
        {
            int[] sameTiles = new int[this.RoomSize.X * this.RoomSize.Y];
            var layerTiles = layer == 1 ? bg1RoomData : bg2RoomData;
            var arrayIndex = 0;
            byte high = (byte)(tileData >> 8);
            byte low = (byte)(tileData & 0xFF);
            for (int i = 0; i < sameTiles.Length; i++)
            {
                if(layerTiles[i*2] == low && layerTiles[i*2+1] == high)
                {
                    sameTiles[arrayIndex] = i;
                    arrayIndex++;
                }
            }
            Array.Resize(ref sameTiles, arrayIndex);

            return sameTiles;
        }
    }

    public class RoomException : Exception
    {
        public RoomException() { }
        public RoomException(string message) : base(message) { }
        public RoomException(string message, Exception inner) : base(message, inner) { }
    }
}
