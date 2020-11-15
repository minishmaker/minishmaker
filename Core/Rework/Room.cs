using System;
using System.Drawing;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.Core.Rework
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

        private MetaTileSet bg2MetaTiles ;
        public MetaTileSet Bg2MetaTiles { get { return bg2MetaTiles; } }

        private MetaTileSet bg1MetaTiles;
        public MetaTileSet Bg1MetaTiles { get { return bg1MetaTiles; } }

        private byte[] bg2RoomData;
        private byte[] bg1RoomData;

        public Room(int roomId, string niceName, Area parent)
        {
            this.Id = roomId;
            this.NiceName = niceName;
            this.Parent = parent;
            this.isLoaded = false;
        }

        public void LoadRoom()
        {
            if (isLoaded) return; //no longer any checks for it needed above this

            metadata = new RoomMetaData(this);

            //room data
            bg2RoomData = new byte[0x2000]; //this seems to be the maximum size
            bg1RoomData = new byte[0x2000]; //2000 isnt too much memory, so this is just easier

            Bg2Exists = MetaData.LoadBGData(ref bg2RoomData, ref bg2MetaTiles, false);//loads in the data and tiles
            Bg1Exists = MetaData.LoadBGData(ref bg1RoomData, ref bg1MetaTiles, true);

            if (!Bg1Exists && !Bg2Exists)
            {
                throw new RoomException("No layers exist for this room, the room is highly likely invalid.");
            }

            isLoaded = true;
        }

        public int GetTileData(int layer, int position)
        {
            if (layer == 1)//bg1
            {
                return bg1RoomData[position * 2] | bg1RoomData[position * 2 + 1] << 8;
            }
            if (layer == 2)//bg2
            {
                return bg2RoomData[position * 2] | bg2RoomData[position * 2 + 1] << 8;
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

        public byte[] GetMetaTileData(ref byte[] tileType, int tileNum, int layer)
        {
            switch (layer)
            {
                case 1:
                    if (!this.Bg1Exists)
                        return null;
                    tileType = bg1MetaTiles.GetTileTypeInfo(tileNum);

                    return bg1MetaTiles.GetTileImageInfo(tileNum);
                case 2:
                    if (!this.Bg2Exists)
                        return null;
                    tileType = bg2MetaTiles.GetTileTypeInfo(tileNum);

                    return bg2MetaTiles.GetTileImageInfo(tileNum);
                default:
                    return null;
            }
        }

        public void SetMetaTileImageInfo(byte[] data, int tileNum, int layer)
        {
            switch (layer)
            {
                case 1:
                    bg1MetaTiles.SetTileImageInfo(data, tileNum);
                    break;
                case 2:
                    bg2MetaTiles.SetTileImageInfo(data, tileNum);
                    break;
            }
        }

        public void SetMetaTileTypeInfo(byte[] typeData, int tileNum, int layer)
        {
            switch (layer)
            {
                case 1:
                    bg1MetaTiles.SetTileTypeInfo(typeData, tileNum);
                    break;
                case 2:
                    bg2MetaTiles.SetTileTypeInfo(typeData, tileNum);
                    break;
            }
        }

        public long GetSaveData(ref byte[] data, DataType type)
        {
            switch (type)
            {
                case DataType.bg1Data:
                    return DataHelper.CompressData(ref data, bg1RoomData);

                case DataType.bg2Data:
                    return DataHelper.CompressData(ref data, bg2RoomData);

                case DataType.chestData:
                    data[0] = 4;
                    return MetaData.BytifyListInformation(ref data);

                case DataType.bg1MetaTileSet:
                    return bg1MetaTiles.CompressMetaTileSet(ref data);
                case DataType.bg2MetaTileSet:
                    return bg2MetaTiles.CompressMetaTileSet(ref data);

                case DataType.listData:
                    return MetaData.BytifyListInformation(ref data);

                case DataType.warpData:
                    return MetaData.BytifyWarpInformation(ref data);

                case DataType.bg1MetaTileType:
                    return bg1MetaTiles.CompressMetaTileTypes(ref data);
                case DataType.bg2MetaTileType:
                    return bg2MetaTiles.CompressMetaTileTypes(ref data);
                
                    case DataType.roomMetaData:
                    return GetRoomSpecifics(ref data);

                case DataType.bg1TileSet:
                    return MetaData.TileSet.GetCompressedTileSetData(ref data, TileSet.TileSetDataType.BG1);
                case DataType.bg2TileSet:
                    return MetaData.TileSet.GetCompressedTileSetData(ref data, TileSet.TileSetDataType.BG2);
                case DataType.commonTileSet:
                    return MetaData.TileSet.GetCompressedTileSetData(ref data, TileSet.TileSetDataType.COMMON);
                default:
                    return 0;
            }
        }

        public int GetPointerLoc(DataType type)
        {
            if (MetaData == null)
                LoadRoom();

            return MetaData.GetPointerLoc(type);
        }

        public long GetRoomSpecifics(ref byte[] data)
        {
            var mapX = MetaData.MapPosX * 16;
            var mapY = MetaData.MapPosY * 16;

            data = new byte[] {
                (byte)(mapX&0xff),
                (byte)(mapX>>8),
                (byte)(mapY&0xff),
                (byte)(mapY>>8),
                (byte)(MetaData.PixelWidth&0xff),
                (byte)(MetaData.PixelWidth>>8),
                (byte)(MetaData.PixelHeight&0xff),
                (byte)(MetaData.PixelHeight>>8),
                (byte)(MetaData.tileSetOffset&0xff),
                (byte)(MetaData.tileSetOffset>>8)
            };

            return 10;
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
                Project.Instance.AddPendingChange(new ChangeTypes.Bg1DataChange(this.Parent.Id, this.Id));
            }

            if (Bg2Exists)
            {
                bg2RoomData = bg2New;
                Project.Instance.AddPendingChange(new ChangeTypes.Bg2DataChange(this.Parent.Id, this.Id));
            }

            MetaData.SetRoomSize(xdim, ydim);
            Project.Instance.AddPendingChange(new ChangeTypes.RoomMetadataChange(this.Parent.Id, this.Id));
        }

    }
}
