using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI
{
    public partial class MetaTileEditorWindow : SubWindow
    {
        Bitmap[] tilesets = new Bitmap[2];
        Bitmap[] metaTiles = new Bitmap[2];
        byte[] currentTileInfo = new byte[8];
        byte[] currentTileType = new byte[2];
        int tsetnum = 0;
        int pnum = 0;
        int currentLayer = 1;
        private Room currentRoom;
        bool vFlip = false;
        bool hFlip = false;
        byte[] pre = new byte[0x4000];

        public MetaTileEditorWindow()
        {
            InitializeComponent();
            selectedMetaGridBox.Click += SelectedMetaTileGridBox_Click;
            tLPalette.KeyDown += EnterUnfocus;
            tRPalette.KeyDown += EnterUnfocus;
            bLPalette.KeyDown += EnterUnfocus;
            bRPalette.KeyDown += EnterUnfocus;
            mTType.KeyDown += EnterUnfocus;
            fileToolStripMenuItem.Enabled = false;
        }

        public override void Setup()
        {
            MainWindow mw = MainWindow.instance;
            currentRoom = mw.currentRoom;
            if (mw.currentRoom != null)
            {
                RedrawTiles();
            }
        }

        public override void Cleanup()
        {
            
        }

        #region Click handlers

        #region Gridboxes
        private void TilesetGridBox_Click(object sender, EventArgs e)
        {
            if (tilesetGridBox.Image == null)
                return;
            //TODO check that the selected index actually gets right number


            sTId.Text = tilesetGridBox.SelectedIndex.Hex();
            selectedTileBox.Image = DrawLargeSelectedTile();
        }

        private void MetaTileGridBox_Click(object sender, EventArgs e)
        {
            if (metaTileGridBox.Image == null)
                return;

            MouseEventArgs me = (MouseEventArgs)e;

            int xpos = me.X / 0x10;
            int ypos = me.Y / 0x10;
            //Console.Write(xpos);
            //Console.WriteLine(ypos);
            var layer = metaTiles[currentLayer - 1];
            var small = layer.Clone(new Rectangle(xpos * 16, ypos * 16, 16, 16), layer.PixelFormat);
            var enlarged = DrawingUtil.ResizeBitmap(small, 4);
            //var enlarged = new Bitmap(small, 64, 64);

            currentTileInfo = currentRoom.Parent.GetMetaTileData(ref currentTileType, metaTileGridBox.SelectedIndex, currentLayer);

            if (currentTileInfo == null)
                return;

            tileChange.Enabled = true;
            mTType.Enabled = true;
            tLPalette.Enabled = true;
            tRPalette.Enabled = true;
            bLPalette.Enabled = true;
            bRPalette.Enabled = true;
            tId1.Text = (currentTileInfo[0] + (currentTileInfo[1] << 8) & 0x3ff).Hex(); //first 10 bits of 1st byte
            tLPalette.Text = (currentTileInfo[1] >> 4).Hex();       //last 4 bits of the 2nd byte

            tId2.Text = (currentTileInfo[2] + (currentTileInfo[3] << 8) & 0x3ff).Hex(); //first 10 bits of 3rd byte
            tRPalette.Text = (currentTileInfo[3] >> 4).Hex();       //last 4 bits of 4th byte

            tId3.Text = (currentTileInfo[4] + (currentTileInfo[5] << 8) & 0x3ff).Hex(); //first 10 bits of 5th byte
            bLPalette.Text = (currentTileInfo[5] >> 4).Hex();       //last 4 bits of 6th byte

            tId4.Text = (currentTileInfo[6] + (currentTileInfo[7] << 8) & 0x3ff).Hex(); //first 10 bits of 7th byte
            bRPalette.Text = (currentTileInfo[7] >> 4).Hex();       //last 4 bits of 8th byte

            mTId.Text = metaTileGridBox.SelectedIndex.Hex();
            mTType.Text = (currentTileType[0] + (currentTileType[1] << 8) & 0x3ff).Hex();

            selectedMetaGridBox.Image = enlarged;
        }

        private void SelectedMetaTileGridBox_Click(object sender, EventArgs e)
        {
            if (metaTileGridBox.SelectedIndex == -1 || tilesetGridBox.SelectedIndex == -1)
                return;

            var me = (MouseEventArgs)e;

            int tileX = me.X / 32;
            int tileY = me.Y / 32;

            switch (tileX + (tileY * 2))
            {
                case 0:
                    tId1.Text = tilesetGridBox.SelectedIndex.Hex();
                    break;
                case 1:
                    tId2.Text = tilesetGridBox.SelectedIndex.Hex();
                    break;
                case 2:
                    tId3.Text = tilesetGridBox.SelectedIndex.Hex();
                    break;
                case 3:
                    tId4.Text = tilesetGridBox.SelectedIndex.Hex();
                    break;
            }

            int loc = tileX * 2 + tileY * 4;

            byte lowByte = (byte)(tilesetGridBox.SelectedIndex & 0xff); //only first 8 bits
            byte highByte = (byte)(tilesetGridBox.SelectedIndex >> 8); //trim first 8 bits

            if (hFlip)
                highByte += (1 << 2);

            if (vFlip)
                highByte += (1 << 3);

            currentTileInfo[loc] = lowByte;

            var newByte = currentTileInfo[loc + 1] & 0xf0;//only retain palette (last 4 bits)
            newByte += highByte;
            currentTileInfo[loc + 1] = (byte)newByte;

            selectedMetaGridBox.Image = DrawLargeMetaTile(currentTileInfo);
        }
        #endregion

        #region Buttons
        private void Layer1Button_Click(object sender, EventArgs e)
        {
            currentLayer = 1;
            layer1Button.Enabled = false;
            layer2Button.Enabled = true;

            selectedTileBox.Image = null;
            tilesetGridBox.SelectedIndex = -1;
            selectedMetaGridBox.Image = null;
            metaTileGridBox.SelectedIndex = -1;

            tilesetGridBox.Image = tilesets[currentLayer - 1];
            metaTileGridBox.Image = metaTiles[currentLayer - 1];
        }

        private void Layer2Button_Click(object sender, EventArgs e)
        {
            currentLayer = 2;
            layer2Button.Enabled = false;
            layer1Button.Enabled = true;

            selectedTileBox.Image = null;
            tilesetGridBox.SelectedIndex = -1;
            selectedMetaGridBox.Image = null;
            metaTileGridBox.SelectedIndex = -1;

            tilesetGridBox.Image = tilesets[currentLayer - 1];
            metaTileGridBox.Image = metaTiles[currentLayer - 1];
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            nextButton.Enabled = true;

            pnum -= 1;

            if (pnum == 0)
            {
                prevButton.Enabled = false;
            }

            RedrawTilesets();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            prevButton.Enabled = true;

            pnum += 1;

            if (pnum == 15)
            {
                nextButton.Enabled = false;
            }
            RedrawTilesets();
        }

        private void TileChange_Click(object sender, EventArgs e)
        {
            if (metaTileGridBox.SelectedIndex == -1)
                return;

            byte[] metatypes = new byte[2];
            var metadata = currentRoom.Parent.GetMetaTileData(ref metatypes, metaTileGridBox.SelectedIndex, currentLayer);
            var hasTypeChange = !metatypes.SequenceEqual(currentTileType);
            var hasInfoChange = !metadata.SequenceEqual(currentTileInfo);


            if (hasInfoChange)
            {
                currentRoom.Parent.SetMetaTileData(currentTileInfo, metaTileGridBox.SelectedIndex, currentLayer);
                MainWindow.instance.RedrawRoom();
                Project.Instance.AddPendingChange(new MetaTilesetChange(currentRoom.Parent.Id, currentLayer));
            }
            if (hasTypeChange)
            {
                currentRoom.Parent.SetMetaTileTypeInfo(currentTileType, metaTileGridBox.SelectedIndex, currentLayer);
                Project.Instance.AddPendingChange(new MetaTileTypeChange(currentRoom.Parent.Id, currentLayer));
            }

            var image = metaTiles[currentLayer - 1];
            int x = metaTileGridBox.SelectedIndex % 16;
            int y = metaTileGridBox.SelectedIndex / 16;
            var tileset = currentRoom.Parent.Tilesets[tsetnum];
            var paletteSet = PaletteSetManager.Get().GetSet(tileset.paletteSetId);
            DrawingUtil.DrawMetaTileData(ref image, currentTileInfo, tileset, new Point(x * 16, y * 16), paletteSet.Colors, currentLayer == 1, true);
            metaTileGridBox.Image = image;
        }

        #endregion

        #region Checkboxes
        private void HFlip_CheckedChanged(object sender, EventArgs e)
        {
            if (tilesetGridBox.SelectedIndex == -1)
                return;
            hFlip = hFlipBox.Checked;
            selectedTileBox.Image = DrawLargeSelectedTile();
        }

        private void VFlipBox_CheckedChanged(object sender, EventArgs e)
        {
            vFlip = vFlipBox.Checked;
            if (tilesetGridBox.SelectedIndex == -1)
                return;
            selectedTileBox.Image = DrawLargeSelectedTile();
        }
        #endregion

        #region Toolstrip
        //import
        private void Bg1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFile("bg1", Tileset.TilesetDataType.BG1);
        }

        private void Bg2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFile("bg2", Tileset.TilesetDataType.BG2);
        }

        private void CommonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFile("common", Tileset.TilesetDataType.COMMON);
        }

        private void paletteToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Palette files| *.pal|All Files|*.*";
                ofd.Title = "Select a palette file";

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var data = File.ReadAllBytes(ofd.FileName);

                //16 palettes * 16 colors * 3 parts (r g b)
                if (data.Length != 16 * 16 * 3)
                {
                    throw new IncorrectFileSizeException("Incorrect palette file size. \r expected size " + (16 * 16 * 3) + " bytes. \r Found" + data.Length);
                }
                
                //TODO:
                //Project.Instance.AddPendingChange(new PaletteChange(-1, currentRoom.Parent.PaletteSetId));
            }
        }

        //export
        private void Bg1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportTileset(Tileset.TilesetDataType.BG1, 0x80, 1);
        }

        private void Bg2ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportTileset(Tileset.TilesetDataType.BG2, 0, 0);
        }

        private void CommonToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportTileset(Tileset.TilesetDataType.COMMON, 0, 1);
        }

        private void PaletteToolStripMenuItem1_Click(object sender, EventArgs e) 
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Palette files|*.pal|All Files|*.*";
                sfd.Title = "Save palette file";
                sfd.FileName = "palette_" + currentRoom.Parent.Id.Hex() + ".pal";

                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                var area = currentRoom.Parent;
                var tset = area.Tilesets[currentRoom.MetaData.tilesetId];
                var palette = PaletteSetManager.Get().GetSet(tset.paletteSetId);
                var palString = palette.ToPaletteString();

                File.WriteAllText(sfd.FileName, palString);
            }
        }

        #endregion

        #endregion

        #region Lost focus handlers
        private void TLPalette_LostFocus(object sender, EventArgs e)
        {
            ChangePalette(0, tLPalette);
        }

        private void TRPalette_LostFocus(object sender, EventArgs e)
        {
            ChangePalette(2, tRPalette);
        }

        private void BLPalette_LostFocus(object sender, EventArgs e)
        {
            ChangePalette(4, bLPalette);
        }

        private void BRPalette_LostFocus(object sender, EventArgs e)
        {
            ChangePalette(6, bRPalette);
        }

        private void MTType_LostFocus(object sender, EventArgs e)
        {
            try
            {
                var value = Convert.ToInt16(mTType.Text, 16);
                byte b0 = (byte)(value & 0xff); //low
                byte b1 = (byte)(value >> 8); //high
                currentTileType = new byte[2] { b0, b1 };
            }
            catch
            {
                mTType.Text = (currentTileType[1] * 0x100 + currentTileType[0]).Hex();
            }
        }
        #endregion

        //utility functions start here
        public void RedrawTiles()
        {
            mTType.Enabled = false;
            tileChange.Enabled = false;
            tLPalette.Enabled = false;
            tRPalette.Enabled = false;
            bLPalette.Enabled = false;
            bRPalette.Enabled = false;
            metaTiles = DrawingUtil.DrawMetatileImages(currentRoom, 16); //areaindex currently unused because what even is swaptiles
            tilesets = DrawingUtil.DrawTilesetImages(currentRoom, pnum);
            metaTileGridBox.Image = metaTiles[currentLayer - 1];
            tilesetGridBox.Image = tilesets[currentLayer - 1];

            metaTileGridBox.Selectable = true;
            tilesetGridBox.Selectable = true;
            selectedMetaGridBox.Selectable = true;
            fileToolStripMenuItem.Enabled = true;
        }

        private void RedrawTilesets()
        {
            tilesets = DrawingUtil.DrawTilesetImages(currentRoom, pnum);
            tilesetGridBox.Image = tilesets[currentLayer - 1];
            PaletteNum.Text = pnum.Hex();
        }

        private Bitmap DrawLargeMetaTile(byte[] tiledata)
        {
            var b = new Bitmap(64, 64);
            BitmapData bdata = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.ReadWrite, b.PixelFormat);
            for (var i = 0; i < 4; i++)
            {
                var x = i % 2 * 8;
                int y = i >= 2 ? 8 : 0;
                bdata = DrawLargeQuarterTile(bdata, new byte[] { tiledata[i * 2], tiledata[i * 2 + 1] }, x, y);
            }
            //var tset = currentRoom.Parent.Tilesets[tsetnum];
            //var paletteSet = PaletteSetManager.Get().GetSet(tset.paletteSetId);

            //DrawingUtil.DrawMetaTileData(ref b, tiledata, tset, new Point(0,0), paletteSet.Colors, true, true)
            //b = DrawingUtil.ResizeBitmap(b, 4);
            b.UnlockBits(bdata);
            return b;
        }

        private BitmapData DrawLargeQuarterTile(BitmapData bdata, byte[] metaTileData, int x = 0, int y = 0)
        {
            ushort data = (ushort)(metaTileData[0] | (metaTileData[1] << 8));

            int tnum = data & 0x3FF; //bits 1-10

            bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set
            bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set

            int palnum = data >> 12;//last 4 bits

            if (currentLayer == 1)
            {
                tnum += 0x200;
            }
            var tset = currentRoom.Parent.Tilesets[tsetnum];
            var tileData = tset.GetTile(tnum);
            return DrawingUtil.DrawTileFromPalette(bdata, new Point(x, y), tileData, PaletteSetManager.Get().GetSet(tset.paletteSetId).Colors, palnum, hflip, vflip, true, 4);
        }

        private Bitmap DrawLargeSelectedTile()
        {
            var b = new Bitmap(32, 32);
            var tnum = tilesetGridBox.SelectedIndex;

            if (currentLayer == 1)
            {
                tnum += 0x200;
            }
            var tset = currentRoom.Parent.Tilesets[tsetnum];
            var tileData = tset.GetTile(tnum);
            var colors = PaletteSetManager.Get().GetSet(tset.paletteSetId).Colors;
            BitmapData bdata = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.ReadWrite, b.PixelFormat);
            bdata = DrawingUtil.DrawTileFromPalette(bdata, new Point(0, 0), tileData, colors, pnum, this.hFlip, this.vFlip, true, 4);
            b.UnlockBits(bdata);
            return b;
        }

        private void ChangePalette(int id, TextBox box)
        {
            try
            {
                byte palette = Convert.ToByte(box.Text, 16);
                byte data = currentTileInfo[id + 1];
                byte pCleared = (byte)(data & 0x0f); //only keep first 4 bits
                byte newByte = (byte)(pCleared + (palette << 4));
                currentTileInfo[id + 1] = newByte;
                var bmap = (Bitmap)selectedMetaGridBox.Image;
                var x = id % 4 == 0 ? 0 : 8;
                var y = id < 4 ? 0 : 8;

                BitmapData bdata = bmap.LockBits(new Rectangle(Point.Empty, bmap.Size), ImageLockMode.ReadWrite, bmap.PixelFormat);
                bdata = DrawLargeQuarterTile(bdata, new byte[] { currentTileInfo[id], currentTileInfo[id + 1] }, x, y);
                bmap.UnlockBits(bdata);
                selectedMetaGridBox.Image = bmap;
            }
            catch
            {
                box.Text = (currentTileInfo[id + 1] >> 4).Hex();
            }
        }

        private void ImportFile(string type, Tileset.TilesetDataType tsetType)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG files|*.png|All Files|*.*";
                ofd.Title = "Select a " + type + " tilset file";

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (!ofd.CheckPathExists)
                {
                    throw new FileNotFoundException();
                }

                byte[] data = File.ReadAllBytes(ofd.FileName);
                Bitmap inImage = BitmapHandler.LoadBitmap(data);
                byte[] tsetData = DecodeIndices(inImage);


                currentRoom.Parent.Tilesets[tsetnum].SetTilesetData(tsetType, tsetData);
                RedrawTiles();

                Project.Instance.AddPendingChange(new TilesetChange(currentRoom.Parent.Id, (int)tsetType + (tsetnum*10)));
            }
        }

        private void ExportTileset(Tileset.TilesetDataType tsetType, int offset, int layer)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Bitmap files|*.png|All Files|*.*";
                sfd.Title = "Save " + tsetType + " tileset file";
                sfd.FileName = tsetType + "_" + currentRoom.Parent.Id.Hex() + ".png";

                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var fileData = currentRoom.Parent.Tilesets[tsetnum].GetTilesetData(tsetType);
                var bmap = new Bitmap(0x100, 0x80, PixelFormat.Format8bppIndexed);

                EncodeIndices(ref bmap, fileData);

                bmap.Save(sfd.FileName);
            }
        }

        private byte[] DecodeIndices(Bitmap bmp)
        {
            byte[] tsetData = new byte[0x4000];
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            var length = data.Stride * data.Height;

            byte[] bytes = new byte[length];

            // Copy bitmap to byte[]
            Marshal.Copy(data.Scan0, bytes, 0, length);

            var tsetDataPos = 0;

            for (int tileY = 0; tileY < 0x10; tileY++)
            {
                for (int tileX = 0; tileX < 0x20; tileX++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            var posY = 8 * tileY + y;
                            var posX = 8 * tileX + x;

                            byte pixData = Marshal.ReadByte(data.Scan0, posX + posY * data.Stride);
                            if (x % 2 == 0)
                            {
                                tsetData[tsetDataPos] = pixData;
                            }
                            else
                            {
                                pixData = (byte)(pixData << 4);
                                tsetData[tsetDataPos] += pixData;
                                tsetDataPos++;
                            }
                        }
                    }
                }
            }

            if (!pre.SequenceEqual(tsetData))
            {
                for(int x = 0; x<tsetData.Count(); x++) {
                    if(pre[x] != tsetData[x]) {
                        var a = 0;
                    }
                }
            }

            bmp.UnlockBits(data);
            return tsetData;
        }

        private void EncodeIndices(ref Bitmap bmp, byte[] tsetData)
        {
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            var tsetDataPos = 0;
            //var palette = bmp.Palette.Entries;
            pre = tsetData;
            for (int tileY = 0; tileY < 0x10; tileY++)
            {
                for (int tileX = 0; tileX < 0x20; tileX++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            var posY = 8 * tileY + y;
                            var posX = 8 * tileX + x;

                            byte palData = tsetData[tsetDataPos];//get color
                            if (x % 2 == 0)
                            {
                                palData = (byte)(palData & 0x0F);
                            }
                            else
                            {
                                palData = (byte)(palData >> 4);
                                tsetDataPos++;
                            }
                            Marshal.WriteByte(data.Scan0, posX + posY * data.Stride, palData);
                        }
                    }
                }
            }

            var length = data.Stride * data.Height;

            byte[] bytes = new byte[length];

            // Copy bitmap to byte[]
            Marshal.Copy(data.Scan0, bytes, 0, length);

            bmp.UnlockBits(data);
        }
    }

    public class IncorrectFileSizeException : Exception
    {
        public IncorrectFileSizeException() { }
        public IncorrectFileSizeException(string message) : base(message) { }
        public IncorrectFileSizeException(string message, Exception inner) : base(message, inner) { }
    }
}

