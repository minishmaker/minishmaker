using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace MinishMaker.Core
{
    public class Tileset
    {
        private byte[] tilesetData;
        public int paletteSetId;
        public int Size
        {
            get { return tilesetData.Length / 0x20; }
        }

        private Bitmap[] images;
        public Bitmap[] Images 
        {
            get 
            { 
                if (images == null)
                {
                    images = new Bitmap[2];

                }
                return images;
            }
        }

        public Tileset(List<AddrData> tilesetAddrs, int paletteSetId ,string tilesetBasePath)
        {

            string[] files = {  tilesetBasePath + (int)TilesetDataType.BG1 + ".json",
                                tilesetBasePath + (int)TilesetDataType.COMMON + ".json",
                                tilesetBasePath + (int)TilesetDataType.BG2 + ".json" };
            /*
             string[] files = {  areaPath + "/" + DataType.tileSet + (int)TileSetDataType.BG1 + "Dat.bin",
                                areaPath + "/" + DataType.tileSet + (int)TileSetDataType.COMMON + "Dat.bin",
                                areaPath + "/" + DataType.tileSet + (int)TileSetDataType.BG2 + "Dat.bin" };
            */
            this.paletteSetId = paletteSetId;
            byte[] tilesetData = new byte[0x10000];
            using (MemoryStream ms = new MemoryStream(tilesetData))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < tilesetAddrs.Count; i++)
                    {
                        var counter = tilesetAddrs[i].dest / 0x4000;
                        byte[] data;
                        if (files.Length > counter && File.Exists(files[counter]))
                        {
                            data = DataHelper.GetByteArrayFromJSON2(files[counter], 32);
                        }
                        else
                        {
                            ms.Seek(tilesetAddrs[i].dest, SeekOrigin.Begin);
                            data = DataHelper.GetData(tilesetAddrs[i]);
                        }
                        bw.Write(data);
                    }
                    this.tilesetData = tilesetData;
                }
            }
        }

        public Tileset(byte[] data)
        {
            byte[] tilesetData = new byte[0x10000];
            Array.Copy(tilesetData, data, data.Length);
            this.tilesetData = tilesetData;
        }

        public void SetChunk(byte[] newdata, int dest)
        {
            newdata.CopyTo(this.tilesetData, dest);
        }

        public byte[] GetTile(int tileId) {
            byte[] data = new byte[0x20];
            Array.Copy(tilesetData, tileId * 0x20, data, 0, 0x20);
            return data;
        }

        public byte[] GetTile4(int tileId)
        {
            byte[] data = new byte[0x80];
            Array.Copy(tilesetData, tileId * 0x80, data, 0, 0x80);
            return data;
        }


        //0 bg1, 1 common, 2 bg2
        public enum TilesetDataType
        {
            BG1 = 0,
            COMMON = 1,
            BG2 = 2,
        }

        public void SetTilesetData(TilesetDataType tsetType, byte[] data)
        {
            var dataOffset = (int)tsetType * 0x4000;
            using (MemoryStream ms = new MemoryStream(tilesetData))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    ms.Seek(dataOffset, SeekOrigin.Begin);
                    bw.Write(data);
                }
            }
        }

        public byte[] GetTilesetData(TilesetDataType tsetType)
        {
            var dataOffset = (int)tsetType * 0x4000;
            var data = new byte[0x4000];
            using (MemoryStream ms = new MemoryStream(tilesetData))
            {
                ms.Position = dataOffset;
                ms.Read(data, 0, 0x4000);
            }

            return data;
        }

        public long GetCompressedTilesetData(ref byte[] data, TilesetDataType tsetType)
        {
            var uncompData = GetTilesetData(tsetType);
            var compdata = DataHelper.CompressData(ref data, uncompData);

            return compdata;
        }
    }
}
