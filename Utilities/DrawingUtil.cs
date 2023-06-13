using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using MinishMaker.Core;

namespace MinishMaker.Utilities
{
    public class DrawingUtil
    {
        public static bool enableTileSwap = true;

        #region various draw functions
        public static Bitmap[] DrawRoom(Room room)
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

        private static Bitmap DrawLayer(Room room, int layer, bool overwrite)
        {
            var layerImage = new Bitmap(room.MetaData.PixelWidth, room.MetaData.PixelHeight);
            int pos = 0; //position in roomData
            ushort[] chunks = new ushort[3] { 0x00FF, 0x00FF, 0x00FF };
            ushort[] oldchunks;
            var metaTiles = layer == 1 ? room.Parent.Bg1MetaTileset : room.Parent.Bg2MetaTileset;


            var areaIndex = room.Parent.Id;
            var tileset = room.Parent.Tilesets[room.MetaData.tilesetId];
            var paletteSet = PaletteSetManager.Get().GetSet(tileset.paletteSetId);
            for (int j = 0; j < room.MetaData.TileHeight; j++)
            {
                for (int i = 0; i < room.MetaData.TileWidth; i++)
                {
                     
                    //hardcoded because there is no easy way to determine which areas use tileswapping yet
                    if (enableTileSwap && (room.Id == 00 && areaIndex == 01 || areaIndex == 02 || areaIndex == 0x15))
                    {
                        oldchunks = chunks;
                        chunks = GetChunks(room.Parent.Id, (ushort)(i * 16), (ushort)(j * 16));

                        var newPal = SwapTiles(room, oldchunks, chunks);
                        if (newPal != -1)
                        {
                            paletteSet = PaletteSetManager.Get().GetSet(newPal);
                        }
                    }
                    //which metatile to draw
                    int mt = room.GetTileData(layer, pos);

                    pos++; //2 bytes per tile
                    if (metaTiles.GetMetaTileData(mt) == null)
                    {
                        continue;
                    }
                    try
                    {
                        if (mt != 0xFFFF) //nonexistant room data does this, eg. area 0D room 10
                            DrawMetaTileId(ref layerImage, metaTiles, tileset, new Point(i * 16, j * 16), paletteSet, mt, overwrite);
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
        private static Bitmap DrawSpecialLayer(Room room, bool overwrite)
        {
            Bitmap layer = new Bitmap(room.MetaData.PixelWidth, room.MetaData.PixelHeight);
            int pos = 0; //position in tileMap
            var tset = room.Parent.Tilesets[room.MetaData.tilesetId];
            var colors = PaletteSetManager.Get().GetSet(tset.paletteSetId).Colors;
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

                    var tileData = tset.GetTile(tnum);
                    DrawTileFromPalette(ref layer, new Point(i * 8, j * 8), tileData, colors, pnum, hflip, vflip, overwrite);
                }
            }
            return layer;
        }

        public static Bitmap[] DrawTilesetImages(Area area, int tsetnum, int pnum)
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
                DrawQuarterTile(ref images[0], point, tnum + 0x200, area, tsetnum, pnum, false, false, true);
                DrawQuarterTile(ref images[1], point, tnum        , area, tsetnum, pnum, false, false, true);
            }

            return images;
        }

        public static Bitmap[] DrawMetatileImages(Room room, int tilesPerRow)
        {
            return DrawMetatileImages(room.Parent, room.MetaData.tilesetId, tilesPerRow);
        }
        public static Bitmap[] DrawMetatileImages(Area area, int tsetnum, int tilesPerRow)
        { 
            var tset = area.Tilesets[tsetnum];
            var pset = PaletteSetManager.Get().GetSet(tset.paletteSetId);
            var totalValues = 0x100;// amount different low values (00-FF)
            var tileSize = 16;// 0x10 pixels
            var rowsRequiredPerHigh = (int)Math.Ceiling((double)((totalValues / tilesPerRow)));//how many rows are needed
            var highValuesUsed = 8; //minish woods stops at 07 FE, so does hyrule town
            int rowsRequired = (int)((highValuesUsed + 0.5) * rowsRequiredPerHigh); //some leeway as the scrollbar doesnt do precision or the bitmap isnt large enough

            var twidth = tileSize * tilesPerRow;
            var theight = tileSize * rowsRequired;

            Bitmap bg1 = new Bitmap(twidth, theight, PixelFormat.Format32bppArgb);
            Bitmap bg2 = new Bitmap(twidth, theight, PixelFormat.Format32bppArgb);
            //fill images with white
            using (Graphics g = Graphics.FromImage(bg1))
            {
                g.Clear(Color.White);
            }

            using (Graphics g = Graphics.FromImage(bg2))
            {
                g.Clear(Color.White);
            }

            var tileId = 0;
            bool bg1ended = false;
            bool bg2ended = false;

            var bg1MetaTiles = area.Bg1MetaTileset;
            var bg2MetaTiles = area.Bg2MetaTileset;
            for (int j = 0; j < rowsRequired; j++)
            {
                if (bg1ended && bg2ended)
                {
                    //Debug.WriteLine("");
                    break;
                }
                    
                for (int i = 0; i < tilesPerRow; i++)
                {
                    var point = new Point(i * 16, j * 16);
                    if (tileId != 0xFFFF)
                    {
                        if (bg1MetaTiles != null && !bg1ended)
                        {
                            DrawMetaTileData(ref bg1, bg1MetaTiles.GetMetaTileData(tileId), tset, point, pset.Colors, true, false);
                            if ((tileId + 1) * 8 >= bg1MetaTiles.dataSize)
                            {
                                //Debug.WriteLine("end of bg1 metatile file: " + tileId.Hex(4));
                                
                                bg1ended = true;
                            }
                        }

                        if (bg2MetaTiles != null && !bg2ended)
                        {
                            DrawMetaTileData(ref bg2, bg2MetaTiles.GetMetaTileData(tileId), tset, point, pset.Colors, false, false);
                            if ((tileId + 1) * 8 >= bg2MetaTiles.dataSize)
                            {
                                //Debug.WriteLine("end of bg2 metatile file: " + tileId.Hex(4));
                                //Debug.WriteLine("");
                                bg2ended = true;
                            }
                        }
                    }
                    tileId++;
                }
            }

            return new Bitmap[] { bg1, bg2 };
        }

        public static void DrawMetaTileId(ref Bitmap image, MetaTileset mtset, Tileset tset, Point p, PaletteSet pset, int tileId, bool overwrite)
        {
            DrawMetaTileData(ref image, mtset.GetMetaTileData(tileId), tset, p, pset.Colors, mtset.IsBg1, overwrite);
        }

        public static void DrawMetaTileData(ref Bitmap image, byte[] metaTileData, Tileset tset, Point pixelPos, Color[] palettes, bool isBg1, bool overwrite)
        {
            BitmapData bd = image.LockBits(new Rectangle(pixelPos.X, pixelPos.Y, 16, 16), ImageLockMode.WriteOnly, image.PixelFormat);
            bd = DrawMetaTileData( bd, metaTileData, tset, new Point(0, 0), palettes, isBg1, overwrite);
            image.UnlockBits(bd);
        }

        public static BitmapData DrawMetaTileData(BitmapData bd, byte[] metaTileData, Tileset tset, Point pixelPos, Color[] palettes, bool isBg1, bool overwrite, int scale = 1)
        {
            if (metaTileData == null || metaTileData.Length == 0)
                throw new ArgumentNullException("metaTileData", "Cannot draw empty metatile.");
            //Debug.WriteLine("tilePixels:" + pixelPos.X + ":" + pixelPos.Y);
            int i = 0;
            for (int y = 0; y < 2; y += 1)
            {
                for (int x = 0; x < 2; x += 1)
                {
                    ushort data = (ushort)(metaTileData[i] | (metaTileData[i + 1] << 8));
                    i += 2;

                    int tnum = data & 0x3FF; //bits 1-10

                    if (isBg1)
                        tnum += 0x200;

                    bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set
                    bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set
                    int pnum = data >> 12;//last 4 bits
                    int xPos = pixelPos.X + (8 * x);
                    int yPos = pixelPos.Y + (8 * y);

                    
                    Point shiftedPos = new Point(xPos, yPos);
                    var tileData = tset.GetTile(tnum);
                    DrawTileFromPalette(bd, shiftedPos, tileData, palettes, pnum, hflip, vflip, overwrite, scale);
                }
            }
            return bd;
        }

        //draws a quarter tile
        public static void DrawTileFromPalette(ref Bitmap image, Point pixelPos, byte[] tileData, Color[] pal, int pnum, bool hflip, bool vflip, bool overwrite)
        {
            BitmapData bd = image.LockBits(new Rectangle(pixelPos.X, pixelPos.Y, 8, 8), ImageLockMode.WriteOnly, image.PixelFormat);
            bd = DrawTileFromPalette(bd, new Point(0, 0), tileData, pal, pnum, hflip, vflip, overwrite);
            image.UnlockBits(bd);
        }

        public static BitmapData DrawTileFromPalette(BitmapData bd, Point pixelPos, byte[] tileData, Color[] pal, int pnum, bool hflip, bool vflip, bool overwrite, int scale = 1)
        {
            unsafe
            {
                int dataPos = 0;
                var startX = pixelPos.X * scale;
                var startY = pixelPos.Y * scale;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x += 2) //2 pixels at a time
                    {
                        //itteration						0	1	2	3	4	5	6	7	-	0	1	2	3	4	5	6	7
                        int posX = hflip ? 6 - x : x; //	6+7	4+5	2+3	0+1	x	x	x	x	or	0+1	2+3	4+5	6+7	x	x	x	x
                        int posY = vflip ? 7 - y : y; //	7	6	5	4	3	2	1	0	or	0	1	2	3	4	5	6	7 

                        int colorData = tileData[dataPos];
                        int data1 = hflip ? colorData >> 4 : colorData & 0x0F; // /16 for last 4 bits or & 15 for the first 4 bits
                        int data2 = hflip ? colorData & 0x0F : colorData >> 4;
                        Color color1 = pal[data1 + pnum * 16];
                        Color color2 = pal[data2 + pnum * 16];

                        if (data1 == 0 || (color1.A == 0 && overwrite))
                            color1 = Color.Transparent;
                        if (data2 == 0 || (color2.A == 0 && overwrite))
                            color2 = Color.Transparent;

                        if (color1.A > 0 || overwrite)//if see through dont draw white
                        {
                            //SetPixel(pixelPos.X + posX, pixelPos.Y + posY, color1, ref bd);
                            SetScaledPixel(startX, startY, posX, posY, color1, scale, ref bd);
                        }

                        if (color2.A > 0 || overwrite)//if see through dont draw white
                        {
                            //SetPixel(pixelPos.X + posX + 1, pixelPos.Y + posY, color2, ref bd);
                            SetScaledPixel(startX, startY, posX + 1, posY, color2, scale, ref bd);
                        }
                        dataPos++;
                    }
                }
            }

            return bd;
        }


        public static void DrawRaw(ref Bitmap image, Point pixelPos, int tnum, Tileset tset, Color[] pal, int pnum, bool hflip, bool vflip, bool overwrite)
        {
            byte[] data = tset.GetTile4(tnum);
            BitmapData bd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);

            unsafe
            {
                int dataPos = 0;
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x += 2) //2 pixels at a time
                    {
                        int basePos = dataPos * 3;
                        int r = data[basePos];
                        int g = data[basePos + 1];
                        int b = data[basePos + 2];

                        SetPixel(x, y, Color.FromArgb(0, r, g, b), ref bd);
                        dataPos++;
                    }
                }
            }
            image.UnlockBits(bd);
        }


        public static void SetScaledPixel(int startX, int startY, int x, int y, Color c, int scale, ref BitmapData bd)
        {
            if (scale == 1)
            {
                SetPixel(startX + x, startY + y, c, ref bd);
                return;
            }

            for (int sy = 0; sy < scale; sy++)
            {
                var newY = startY + (y * scale + sy);
                for (int sx = 0; sx < scale; sx++)
                {
                    var newX = startX + (x * scale + sx);
                    SetPixel(newX, newY, c, ref bd);
                }
            }
        }

        public static unsafe void SetPixel(int x, int y, Color c, ref BitmapData bd)
        {
            byte* pixels = (byte*)bd.Scan0.ToPointer();
            pixels[(x * 4) + (y * bd.Stride) + 0] = c.B; //B
            pixels[(x * 4) + (y * bd.Stride) + 1] = c.G; //G
            pixels[(x * 4) + (y * bd.Stride) + 2] = c.R; //R
            pixels[(x * 4) + (y * bd.Stride) + 3] = c.A; //A
        }


        public static Bitmap OverlayImage(Bitmap baseImage, Bitmap overlay, bool clear = true)
        {
            Bitmap finalImage = new Bitmap(baseImage.Width, baseImage.Height);

            using (Graphics g = Graphics.FromImage(finalImage))
            {
                //set background color
                if (clear)
                {
                    g.Clear(Color.Black);
                }

                g.DrawImage(baseImage, new Rectangle(0, 0, baseImage.Width, baseImage.Height));
                g.DrawImage(overlay, new Rectangle(0, 0, baseImage.Width, baseImage.Height));
            }
            //Draw the final image in the gridBox
            return finalImage;
        }
        #endregion

        /*public static Bitmap PastePalette(Bitmap dest, Point destPixel, byte[] paletteNums, PaletteSet paletteSet, int setId, int width, bool hflip, bool vflip, int sourceScaleFactor)
        {
            if (paletteNums.Length % (width / 2f) != 0) throw new ArgumentException($"Invalid data size:{paletteNums.Length}, expected to be divisible by half of width: {width}/2");

            int height = (int)(paletteNums.Length / (width / 2f));
            var destLockSizeX = width * sourceScaleFactor;
            var destLockSizeY = height * sourceScaleFactor;

            BitmapData destData = dest.LockBits(new Rectangle(destPixel.X, destPixel.Y, destLockSizeX, destLockSizeY), ImageLockMode.WriteOnly, dest.PixelFormat);

            destData = PastePalette(destData, destPixel, paletteNums, paletteSet, setId, width, height, hflip, vflip, sourceScaleFactor);

            dest.UnlockBits(destData);

            return dest;
        }*/

        /*public static BitmapData PastePalette(BitmapData destData, Point destPixel, byte[] paletteNums, PaletteSet paletteSet, int setId, int width, int height, bool hflip, bool vflip, int sourceScaleFactor)
        {
            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    var posY = vflip ? height - y - 1 : y;
                    for (int x = 0; x < width; x++)
                    {
                        var posX = hflip ? width - x - 1 : x;
                        var dataPos = (y * width + x) / 2;
                        var shift = dataPos % 2 == 0 ? 4 : 0;
                        var colorNum = paletteNums[dataPos] >> shift;
                        Color color;
                        if (colorNum == 0)
                        {
                            color = Color.Transparent;
                        }
                        else 
                        {
                            color = paletteSet.Colors[setId * 16 + colorNum];
                        }

                        SetScaledPixel(destPixel.X, posX, destPixel.Y, posY, color, sourceScaleFactor, ref destData);
                    }
                }
            }

            return destData;
        }*/

        /*public static Bitmap PasteRegion(Bitmap dest, Point destPixel, Bitmap source, Rectangle sourceArea, int sourceScaleFactor = 1)
        {
            if (destPixel.X + (sourceArea.Width * sourceScaleFactor) >= dest.Width) throw new ArgumentException("Combined width is too large to fit");
            if (destPixel.Y + (sourceArea.Height * sourceScaleFactor) >= dest.Height) throw new ArgumentException("Combined height is too large to fit");
            if (source.Width <= sourceArea.X + sourceArea.Width) throw new ArgumentException("Area width is outside the image bounds");
            if (source.Height <= sourceArea.Y + sourceArea.Height) throw new ArgumentException("Area height is outside the image bounds");
            
            var destLockSizeX = sourceArea.Width * sourceScaleFactor;
            var destLockSizeY = sourceArea.Height * sourceScaleFactor;
            
            BitmapData destData = dest.LockBits(new Rectangle(destPixel.X, destPixel.Y, destLockSizeX, destLockSizeY), ImageLockMode.WriteOnly, dest.PixelFormat);
            BitmapData sourceData = source.LockBits(new Rectangle(sourceArea.X, sourceArea.Y, sourceArea.Width, sourceArea.Height), ImageLockMode.ReadOnly, source.PixelFormat);

            destData = PasteRegion(destData, destPixel, sourceData, sourceArea, sourceScaleFactor);

            dest.UnlockBits(destData);
            source.UnlockBits(sourceData);

            return dest;
        }*/

        /*public static BitmapData PasteRegion(BitmapData destData, Point destPixel, BitmapData sourceData, Rectangle sourceArea, int sourceScaleFactor = 1)
        {
            unsafe
            {
                byte* readPixels = (byte*)sourceData.Scan0.ToPointer();
                var stride = sourceData.Stride;
                for (int y = 0; y < sourceArea.Height; y++)
                {
                    for (int x = 0; x < sourceArea.Width; x++)
                    {
                        var b = readPixels[(x * 4) + (y * stride) + 0]; //B
                        var g = readPixels[(x * 4) + (y * stride) + 1]; //G
                        var r = readPixels[(x * 4) + (y * stride) + 2]; //R
                        var a = readPixels[(x * 4) + (y * stride) + 3]; //A
                        Color c = Color.FromArgb(a, r, g, b);

                        SetScaledPixel(destPixel.X, x, destPixel.Y, y, c, sourceScaleFactor, ref destData);
                    }
                }
            }
            return destData;
        }*/

        /*public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }*/

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

        private static int SwapTiles(Room room, ushort[] oldchunks, ushort[] newchunks)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int updatepal = -1;
            var areaIndex = room.Parent.Id;
            var mtset = room.Parent.Tilesets[room.MetaData.tilesetId];
            //var pset = room.MetaData.PaletteSet;
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
                        mtset.SetChunk(newtiles, dest);

                        newtiles = r.ReadBytes(0x1000, src2);
                        mtset.SetChunk(newtiles, dest2);

                        break;
                    case 0x01: //area 01 works differently, 8 chunks and palette update
                        for (int j = 0; j < 8; j++)
                        {
                            if (j == 0)
                            {//update palette
                                byte pnum = r.ReadByte(header.paletteBase + newchunks[i]);
                                updatepal = (int)pnum;
                            }
                            baseaddr = header.area1SwapBase;
                            r.SetPosition(baseaddr + (newchunks[i] << 6) + (j << 3));

                            src = (header.tileOffset + (int)r.ReadUInt32());
                            dest = (int)r.ReadUInt32() & 0xFFFFFF;

                            newtiles = r.ReadBytes(0x1000, src);
                            mtset.SetChunk(newtiles, dest);
                        }
                        break;
                }
            }
            if (updatepal > 0) //if the palette number changed due to swapping, create new paletteset
            {
                return updatepal;
            }
            return -1;
        }

        #endregion

        //TODO: CHECK TIME DIFFERENCE
        #region in methods
        public static void DrawTileId(ref Bitmap image, in MetaTileset mtset, in Tileset tset, in Point p, in PaletteSet pset, int tileId, bool overwrite)
        {
            try
            {
                DrawMetaTileData(ref image, mtset.GetMetaTileData(tileId), tset, p, pset.Colors, mtset.IsBg1, overwrite);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException("Attempt to draw empty metatile. Num: 0x" + tileId.ToString("X"), e);
            }
        }


        public static void DrawTileData(ref Bitmap image, in Room room, in byte[] tileData, in Point p, bool isBg1, bool overwrite)
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

                    DrawQuarterTile(ref image, new Point(p.X + (x * 8), p.Y + (y * 8)), tnum, room.Parent, room.MetaData.tilesetId, pnum, hflip, vflip, overwrite);
                }
            }
        }

        public static void DrawQuarterTile(ref Bitmap image, Point p, int tnum, in Area area, int tsetNum, int pnum, bool hflip, bool vflip, bool overwrite)
        {
            var tset = area.Tilesets[tsetNum];
            var pal = PaletteSetManager.Get().GetSet(tset.paletteSetId).Colors;
            //var pal = room.MetaData.PaletteSet.Colors;
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
                            SetPixel(p.X + posX, p.Y + posY, color1, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetPixel(p.X + posX, p.Y + posY, Color.Transparent, ref bd);

                        }
                        if (color2.A > 0)//if see through dont draw white
                        {
                            SetPixel(p.X + posX + 1, p.Y + posY, color2, ref bd);
                        }
                        else if (overwrite)
                        {
                            SetPixel(p.X + posX + 1, p.Y + posY, Color.Transparent, ref bd);
                        }
                        dataPos++;
                    }
                }
            }
            image.UnlockBits(bd);
        }
        #endregion

        //dont use for large images(512x512+), can take over half a second to finish
        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int scale)
        {
            Bitmap result = new Bitmap(sourceBMP.Width * scale, sourceBMP.Height * scale);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(sourceBMP, 0, 0, result.Width, result.Height);
            }
            return result;
        }
    }



    public class DrawingException : Exception
    {
        public DrawingException() { }
        public DrawingException(string message) : base(message) { }
        public DrawingException(string message, Exception inner) : base(message, inner) { }
    }
}
