using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using MinishMaker.Core;

namespace MinishMaker.Utilities
{
    public class DrawingUtil
    {

        #region various draw functions
        public static Bitmap[] DrawRoom(Core.Rework.Room room)
        {
            var images = new Bitmap[2] { new Bitmap(room.MetaData.PixelWidth, room.MetaData.PixelHeight), new Bitmap(room.MetaData.PixelWidth, room.MetaData.PixelHeight) };

            if (room.Bg2Exists)
            {
                images[1] = DrawLayer(room, 2, false);
            }

            if (room.Bg1Exists)
            {
                if (room.MetaData.Bg1Use20344B0)
                {
                    images[0] = DrawSpecialLayer(room, false);
                }
                else
                {
                    images[0] = DrawLayer(room, 1, false);
                }
            }

            return images;
        }

        private static Bitmap DrawLayer(Core.Rework.Room room, int layer, bool overwrite)
        {
            var layerImage = new Bitmap(room.MetaData.PixelWidth, room.MetaData.PixelHeight);
            int pos = 0; //position in roomData
            ushort[] chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };
            ushort[] oldchunks;
            var metaTiles = layer == 1 ? room.Bg1MetaTiles : room.Bg2MetaTiles;


            var areaIndex = room.Parent.Id;
            for (int j = 0; j < room.MetaData.TileHeight; j++)
            {
                for (int i = 0; i < room.MetaData.TileWidth; i++)
                {
                    //hardcoded because there is no easy way to determine which areas use tileswapping
                    if (room.Id == 00 && areaIndex == 01 || areaIndex == 02 || areaIndex == 0x15)
                    {
                        oldchunks = chunks;
                        chunks = GetChunks(room.Parent.Id, (ushort)(i * 16), (ushort)(j * 16));

                        SwapTiles(room, oldchunks, chunks);
                    }
                    //which metatile to draw
                    int mt = room.GetTileData(layer, pos);

                    pos++; //2 bytes per tile
                    if (metaTiles.GetTileImageInfo(mt) == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (mt != 0xFFFF) //nonexistant room data does this, eg. area 0D room 10
                            DrawTileId(ref layerImage, 1, metaTiles, room.MetaData.TileSet, new Point(i * 16, j * 16), room.MetaData.PaletteSet, mt, overwrite);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error drawing metatile. i:" + i.ToString() + ", j:" + j.ToString()
                                            + "\n" + e.Message, e);
                    }
                }
            }
            return layerImage;
        }

        //used for 0D-10, etc.
        private static Bitmap DrawSpecialLayer(Core.Rework.Room room, bool overwrite)
        {
            Bitmap layer = new Bitmap(room.MetaData.PixelWidth, room.MetaData.PixelHeight);
            int pos = 0; //position in tileMap
            for (int j = 0; j < 0x17; j++)
            {
                for (int i = 0; i < 32; i++)
                {
                    ushort data = (ushort)room.GetTileData(1, pos);
                    pos ++;
                    int tnum = data & 0x3FF;
                    tnum += 0x200; //because it's bg1 and base is 6004000 not 6000000
                    int pnum = data >> 12;
                    bool hflip = (data & 0x400) != 0;
                    bool vflip = (data & 0x800) != 0;

                    DrawQuarterTile(ref layer, 1, new Point(i * 8, j * 8), tnum, room.MetaData.TileSet, room.MetaData.PaletteSet.Colors, pnum, hflip, vflip, overwrite);
                }
            }
            return layer;
        }

        public static Bitmap[] DrawTilesetImages(Core.Rework.Room room, int pnum)
        {
            var images = new Bitmap[2];
            images[0] = new Bitmap(256, 0x100); //0x200 * 8 /16 = 0x200 * 0.5
            images[1] = new Bitmap(256, 0x100);

            for (int tnum = 0; tnum < 0x400; tnum++)
            {
                var xpos = tnum % 0x20; //tiles
                var ypos = (tnum - xpos) / 0x20; //tiles
                ypos *= 8; //pixels
                xpos *= 8; //pixels
                var point = new Point(xpos, ypos);
                DrawQuarterTile(ref images[0], 1, point, tnum + 0x200, room, pnum, false, false, true);
                DrawQuarterTile(ref images[1], 1, point, tnum        , room, pnum, false, false, true);
            }

            return images;
        }

        public static Bitmap[] DrawMetatileImages(Core.Rework.Room room, int tilesPerRow)
        {
            var tset = room.MetaData.TileSet;
            var pset = room.MetaData.PaletteSet;
            var totalValues = 0x100;// amount different low values (00-FF)
            var tileSize = 16;// 0x10 pixels
            var rowsRequiredPerHigh = (int)Math.Ceiling((double)((totalValues / tilesPerRow)));//how many rows are needed
            var highValuesUsed = 8; //minish woods stops at 07 FE, so does hyrule town
            int rowsRequired = (int)((highValuesUsed + 0.5) * rowsRequiredPerHigh); //some leeway as the scrollbar doesnt do precision or the bitmap isnt large enough

            var twidth = tileSize * tilesPerRow;
            var theight = tileSize * rowsRequired;

            Bitmap bg1 = new Bitmap(twidth, theight, PixelFormat.Format32bppArgb);
            Bitmap bg2 = new Bitmap(twidth, theight, PixelFormat.Format32bppArgb);

            var pos = 0;
            bool bg1ended = false;
            bool bg2ended = false;

            for (int j = 0; j < rowsRequired; j++)
            {
                if (bg1ended && bg2ended)
                    break;
                for (int i = 0; i < tilesPerRow; i++)
                {
                    if (pos != 0xFFFF)
                    {
                        try
                        {
                            if (room.Bg1Exists && !bg1ended)
                                DrawTileId(ref bg1, 1, room.Bg1MetaTiles, tset, new Point(i * 16, j * 16), pset, pos, true);
                        }
                        catch (ArgumentException)
                        {
                            Debug.WriteLine("end of bg1 metatile file: " + (pos % 256).Hex() + "|" + (pos / 256).Hex());
                            Debug.WriteLine("");
                            bg1ended = true;
                        }

                        try
                        {
                            if (room.Bg2Exists && !bg2ended)
                                DrawTileId(ref bg2, 1, room.Bg2MetaTiles, tset, new Point(i * 16, j * 16), pset, pos, true);
                        }
                        catch (ArgumentException)
                        {
                            Debug.WriteLine("end of bg2 metatile file: " + (pos % 256).Hex() + "|" + (pos / 256).Hex());
                            Debug.WriteLine("");
                            bg2ended = true;
                        }
                    }
                    pos++;
                }
            }

            return new Bitmap[] { bg1, bg2 };
        }

        public static void DrawTileId(ref Bitmap image, int scale, Core.Rework.MetaTileSet mtset, TileSet tset, Point p, Core.Rework.PaletteSet pset, int tileId, bool overwrite)
        {
            try
            {
                DrawTileData(ref image, scale, mtset.GetTileImageInfo(tileId), tset, p, pset.Colors, mtset.IsBg1, overwrite);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException("Attempt to draw empty metatile. Num: 0x" + tileId.ToString("X"), e);
            }
        }

        public static void DrawTileData(ref Bitmap image, int scale, byte[] tileData, TileSet tset, Point p, Color[] palettes, bool isBg1, bool overwrite)
        {
            if (tileData == null || tileData.Length == 0)
                throw new ArgumentNullException("metaTileData", "Cannot draw empty metatile.");

            int i = 0;
            for (int y = 0; y < 2; y += 1)
            {
                for (int x = 0; x < 2; x += 1)
                {
                    ushort data = (ushort)(tileData[i] | (tileData[i + 1] << 8));
                    i += 2;

                    int tnum = data & 0x3FF; //bits 1-10

                    if (isBg1)
                        tnum += 0x200;

                    bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set
                    bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set
                    int pnum = data >> 12;//last 4 bits

                    DrawQuarterTile(ref image, scale, new Point(p.X + (x * 8), p.Y + (y * 8)), tnum, tset, palettes, pnum, hflip, vflip, overwrite);
                }
            }
        }

        //draws a quarter tile
        public static void DrawQuarterTile(ref Bitmap image, int scale, Point p, int tnum, TileSet tset, Color[] pal, int pnum, bool hflip, bool vflip, bool overwrite)
        {
            byte[] data = tset.GetTile(tnum);
            BitmapData bd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);

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
                            SetScaledPixel(posX, posY, color1, scale, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetScaledPixel(posX, posY, Color.Transparent, scale, ref bd);
                           
                        }
                        if (color2.A > 0)//if see through dont draw white
                        {
                            SetScaledPixel(posX + 1, posY, color2, scale, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetScaledPixel(posX + 1, posY, Color.Transparent, scale, ref bd);
                        }
                        dataPos++;
                    }
                }
            }
            image.UnlockBits(bd);
        }

        public static void RescaleImage(ref Bitmap image, int originalScale, int newScale) //redraw the base image into a scaled image
        {
            var scaledImage = new Bitmap(image.Width / originalScale * newScale, image.Height / originalScale * newScale);
            BitmapData wbd = scaledImage.LockBits(new Rectangle(0, 0, scaledImage.Width, scaledImage.Height), ImageLockMode.WriteOnly, scaledImage.PixelFormat);
            BitmapData rbd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);

            unsafe
            {
                byte* readPixels = (byte*)rbd.Scan0.ToPointer();
                for (int y = 0; y < image.Height; y += originalScale)
                {
                    for (int x = 0; x < image.Width; x+= originalScale)
                    {
                        var b = readPixels[(x * 4) + (y * rbd.Stride) + 0]; //B
                        var g = readPixels[(x * 4) + (y * rbd.Stride) + 1]; //G
                        var r = readPixels[(x * 4) + (y * rbd.Stride) + 2]; //R
                        var a = readPixels[(x * 4) + (y * rbd.Stride) + 3]; //A
                        Color c = Color.FromArgb(a, r, g, b);

                        SetScaledPixel(x, y, c, newScale, ref wbd);
                    }
                }
            }

            image.UnlockBits(rbd);
            scaledImage.UnlockBits(wbd);

            image = scaledImage;
        }

        public static Bitmap MagnifyImageArea(Bitmap b, Rectangle rect, int scale)
        {
            Bitmap ret = new Bitmap(rect.Width * scale, rect.Height * scale);
            BitmapData bd = ret.LockBits(new Rectangle(0, 0, ret.Width, ret.Height), ImageLockMode.WriteOnly, ret.PixelFormat);
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Width; j++)
                {
                    var color = b.GetPixel(rect.X + i, rect.Y + j);

                    SetScaledPixel(i, j, color, scale, ref bd);
                }
            }
            return ret;
        }

        public static unsafe void SetPixel(int x, int y, Color c, ref BitmapData bd)
            {
                byte* pixels = (byte*)bd.Scan0.ToPointer();
                pixels[(x * 4) + (y * bd.Stride) + 0] = c.B; //B
                pixels[(x * 4) + (y * bd.Stride) + 1] = c.G; //G
                pixels[(x * 4) + (y * bd.Stride) + 2] = c.R; //R
                pixels[(x * 4) + (y * bd.Stride) + 3] = c.A; //A
            }

        public static void SetScaledPixel(int x, int y, Color c, int scale, ref BitmapData bd)
        {
            if(scale == 1)
            {
                SetPixel(x, y, c, ref bd);
                return;
            }

            for (int sy = 0; sy < scale; sy++)
            {
                var newY = y * scale + sy;
                for (int sx = 0; sx < scale; sx++)
                {
                    var newX = x * scale + sx;
                    SetPixel(newX, newY, c, ref bd);
                }
            }
        }

        public static Bitmap OverlayImage(Bitmap baseImage, Bitmap overlay)
        {
            Bitmap finalImage = new Bitmap(baseImage.Width, baseImage.Height);

            using (Graphics g = Graphics.FromImage(finalImage))
            {
                //set background color
                g.Clear(Color.Black);

                g.DrawImage(baseImage, new Rectangle(0, 0, baseImage.Width, baseImage.Height));
                g.DrawImage(overlay, new Rectangle(0, 0, baseImage.Width, baseImage.Height));
            }
            //Draw the final image in the gridBox
            return finalImage;
        }
        #endregion

        //TODO: FIGURE OUT HOW THESE WORK
        #region tileswap magic

        private static ushort[] GetChunks(int areaIndex, ushort x, ushort y)
        {
            ushort[] ret = new ushort[3];
            var header = ROM.Instance.headers;

            //chunk 00
            int addr;
            switch (areaIndex)
            {
                case 0x01:
                    addr = header.area1Chunk0TableLoc;
                    break;
                case 0x15:
                    addr = header.chunk0TableLoc + 0x042; //area 0x15 (21) uses a different hardcoded ptr tbl
                    break;
                default:
                    addr = header.chunk0TableLoc; //area 1 uses different hardcoded ptr tbl
                    break;
            }

            ret[0] = TestChunk(x, y, addr);


            //chunk 01
            if (areaIndex == 0x02)
            { //no chunk 01 for area 15, only area 02
                ret[1] = TestChunk(x, y, header.chunk1TableLoc);
            }
            else
            {
                ret[1] = 0x00FF;
            }

            //chunk 02
            if (areaIndex == 0x02 || areaIndex == 0x15)
            {
                addr = areaIndex != 0x15 ? header.chunk2TableLoc : header.chunk2TableLoc + 0x02E; //area 0x15 (21) uses different hardcoded ptr tbl
                ret[2] = TestChunk(x, y, addr);
            }
            else
            {
                ret[2] = 0x00FF;
            }

            return ret;
        }

        private static ushort TestChunk(ushort x, ushort y, long addr)
        {
            var r = ROM.Instance.reader;
            r.SetPosition(addr);
            ushort chnk; //note: this do block is essentially check_swap_inner in IDA
            do
            {
                chnk = r.ReadUInt16();
                if (chnk == 0x00FF)
                    break; //no change
                ushort r0 = r.ReadUInt16();
                ushort r1 = r.ReadUInt16();
                ushort r2 = r.ReadUInt16();
                ushort r3 = r.ReadUInt16();

                ushort test_x, test_y;
                unchecked
                {
                    test_x = (ushort)(x - r0); //from check_coords routine
                    test_y = (ushort)(y - r1);
                }

                if (test_x < r2 && test_y < r3)
                    break; //chnk found, so return
            } while (true);
            return chnk;
        }

        private static void SwapTiles(Core.Rework.Room room, ushort[] oldchunks, ushort[] newchunks)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int updatepal = -1;
            var areaIndex = room.Parent.Id;
            var tset = room.MetaData.TileSet;
            var pset = room.MetaData.PaletteSet;
            for (int i = 0; i < 3; i++)
            {
                if (newchunks[i] == oldchunks[i])
                {
                    continue;
                }

                if (newchunks[i] == 0x00FF)
                {
                    continue;
                }

                int baseaddr, src, src2, dest, dest2;
                byte[] newtiles;

                switch (areaIndex)
                {
                    case 0x02:
                    case 0x15:
                        baseaddr = areaIndex != 0x15 ? header.swapBase : header.swapBase + 0x060;
                        r.SetPosition(baseaddr + (newchunks[i] << 4));
                        src = (int)(header.tileOffset + r.ReadUInt32());    //source addrs are stored as offsets from 85A2E80
                        dest = (int)r.ReadUInt32() & 0xFFFFFF;              //mask off the 0x6000000 part
                        src2 = (int)(header.tileOffset + r.ReadUInt32());   //there are 2 sets of tiles for each chunk
                        dest2 = (int)r.ReadUInt32() & 0xFFFFFF;             //one for each bg

                        newtiles = r.ReadBytes(0x1000, src);
                        tset.SetChunk(newtiles, dest);

                        newtiles = r.ReadBytes(0x1000, src2);
                        tset.SetChunk(newtiles, dest2);

                        break;
                    case 0x01: //area 01 works differently, 8 chunks and palette update
                        for (int j = 0; j < 8; j++)
                        {
                            if (j == 0)
                            {//update palette
                                byte pnum = r.ReadByte(header.paletteChangeBase + newchunks[i]);
                                updatepal = (int)pnum;
                            }
                            baseaddr = header.area1SwapBase;
                            r.SetPosition(baseaddr + (newchunks[i] << 6) + (j << 3));

                            src = (header.tileOffset + (int)r.ReadUInt32());
                            dest = (int)r.ReadUInt32() & 0xFFFFFF;

                            newtiles = r.ReadBytes(0x1000, src);
                            tset.SetChunk(newtiles, dest);
                        }
                        break;
                }
            }
            if (updatepal > 0) //if the palette number changed due to swapping, create new paletteset
            {
                pset = new Core.Rework.PaletteSet(updatepal);
            }
        }

        #endregion

        //TODO: CHECK TIME DIFFERENCE
        #region in methods
        public static void DrawTileId(ref Bitmap image, int scale, in Core.Rework.MetaTileSet mtset, in TileSet tset, in Point p, in Core.Rework.PaletteSet pset, int tileId, bool overwrite)
        {
            try
            {
                DrawTileData(ref image, scale, mtset.GetTileImageInfo(tileId), tset, p, pset.Colors, mtset.IsBg1, overwrite);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException("Attempt to draw empty metatile. Num: 0x" + tileId.ToString("X"), e);
            }
        }


        public static void DrawTileData(ref Bitmap image, int scale, in Core.Rework.Room room, in byte[] tileData, in Point p, bool isBg1, bool overwrite)
        {
            if (tileData.Length == 0)
                throw new ArgumentNullException("metaTileData", "Cannot draw empty metatile.");

            int i = 0;
            for (int y = 0; y < 2; y += 1)
            {
                for (int x = 0; x < 2; x += 1)
                {
                    ushort data = (ushort)(tileData[i] | (tileData[i + 1] << 8));
                    i += 2;

                    int tnum = data & 0x3FF; //bits 1-10

                    if (isBg1)
                        tnum += 0x200;

                    bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set
                    bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set
                    int pnum = data >> 12;//last 4 bits

                    DrawQuarterTile(ref image, scale, new Point(p.X + (x * 8), p.Y + (y * 8)), tnum, in room, pnum, hflip, vflip, overwrite);
                }
            }
        }

        public static void DrawQuarterTile(ref Bitmap image, int scale, Point p, int tnum, in Core.Rework.Room room, int pnum, bool hflip, bool vflip, bool overwrite)
        {
            var tset = room.MetaData.TileSet;
            var pal = room.MetaData.PaletteSet.Colors;
            byte[] data = tset.GetTile(tnum);
            BitmapData bd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);

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
                            SetScaledPixel(posX, posY, color1, scale, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetScaledPixel(posX, posY, Color.Transparent, scale, ref bd);

                        }
                        if (color2.A > 0)//if see through dont draw white
                        {
                            SetScaledPixel(posX + 1, posY, color2, scale, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetScaledPixel(posX + 1, posY, Color.Transparent, scale, ref bd);
                        }
                        dataPos++;
                    }
                }
            }
            image.UnlockBits(bd);
        }
        #endregion
    }



    public class DrawingException : Exception
    {
        public DrawingException() { }
        public DrawingException(string message) : base(message) { }
        public DrawingException(string message, Exception inner) : base(message, inner) { }
    }
}
