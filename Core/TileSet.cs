using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using static MinishMaker.Core.RoomMetaData;


namespace MinishMaker.Core
{
    public class TileSet
    {
        byte[] tilesetData;

        public int Size
        {
            get { return tilesetData.Length / 0x20; }
        }

        public TileSet(List<AddrData> tileSetAddrs, string roomPath)
        {

            string[] files = { roomPath + DataType.bg1TileSet + "Dat.bin", roomPath + DataType.commonTileSet + "Dat.bin", roomPath + DataType.bg2TileSet + "Dat.bin" };
            byte[] tilesetData = new byte[0x10000];
            using (MemoryStream ms = new MemoryStream(tilesetData))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < tileSetAddrs.Count; i++)
                    {
                        var counter = tileSetAddrs[i].dest / 0x4000;
                        byte[] data;
                        if (files.Length > counter && File.Exists(files[counter]))
                        {
                            data = File.ReadAllBytes(files[counter]);
                        }
                        else
                        {
                            ms.Seek(tileSetAddrs[i].dest, SeekOrigin.Begin);
                            data = DataHelper.GetData(tileSetAddrs[i]);
                        }
                        bw.Write(data);
                    }
                    this.tilesetData = tilesetData;
                }
            }
        }

        public TileSet(byte[] data)
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

        public unsafe void SetPixel(int x, int y, Color c, ref BitmapData bd)
        {
            byte* pixels = (byte*)bd.Scan0.ToPointer();
            pixels[(x * 4) + (y * bd.Stride) + 0] = c.B; //B
            pixels[(x * 4) + (y * bd.Stride) + 1] = c.G; //G
            pixels[(x * 4) + (y * bd.Stride) + 2] = c.R; //R
            pixels[(x * 4) + (y * bd.Stride) + 3] = c.A; //A
        }

        //draws a quarter tile
        public void DrawQuarterTile(ref Bitmap tileImg, Point p, int tnum, Color[] pal, int pnum, bool hflip, bool vflip, bool overwrite)
        {
            byte[] data = new byte[0x20];
            Array.Copy(tilesetData, tnum * 0x20, data, 0, 0x20);
            BitmapData bd = tileImg.LockBits(new Rectangle(0, 0, tileImg.Width, tileImg.Height), ImageLockMode.WriteOnly, tileImg.PixelFormat);
            unsafe
            {
                int dataPos = 0;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x += 2) //2 pixels at a time
                    {
                        //itteration						0	1	2	3	4	5	6	7	-	0	1	2	3	4	5	6	7
                        int posX = hflip ? 6 - x : x; //	6+7	4+5	2+3	0+1	x	x	x	x	or	0+1	2+3	4+5	6+7	x	x	x	x
                        int posY = vflip ? 7 - y : y; //	7	6	5	4	3	2	1	0	or	0	1	2	3	4	5	6	7 
                        posX += p.X;
                        posY += p.Y;

                        int colorData = data[dataPos];
                        int data1 = hflip ? colorData >> 4 : colorData & 0x0F; // /16 for last 4 bits or & 15 for the first 4 bits
                        int data2 = hflip ? colorData & 0x0F : colorData >> 4;
                        Color color1 = pal[data1 + pnum * 16];
                        Color color2 = pal[data2 + pnum * 16];

                        if (data1 == 0)
                            color1 = Color.Transparent;
                        if (data2 == 0)
                            color2 = Color.Transparent;

                        if (color1.A > 0)//if see through dont draw white
                        {
                            SetPixel(posX, posY, color1, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetPixel(posX, posY, Color.Transparent, ref bd);
                        }
                        if (color2.A > 0)//if see through dont draw white
                        {
                            SetPixel(posX + 1, posY, color2, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetPixel(posX + 1, posY, Color.Transparent, ref bd);
                        }
                        dataPos++;
                    }

                }
            }
            tileImg.UnlockBits(bd);

        }

        //0 bg1, 1 common, 2 bg2
        public enum TileSetDataType
        {
            BG1 = 0,
            COMMON = 1,
            BG2 = 2,
        }

        public void SetTileSetData(TileSetDataType tsetType, byte[] data)
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

        public byte[] GetTileSetData(TileSetDataType tsetType)
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

        public long GetCompressedTileSetData(ref byte[] data, TileSetDataType tsetType)
        {
            var uncompData = GetTileSetData(tsetType);
            return DataHelper.CompressData(ref data, uncompData);
        }
    }
}
