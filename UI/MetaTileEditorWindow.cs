using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI
{
    public partial class MetaTileEditor : Form
    {
        Bitmap[] tileset = new Bitmap[2];
        Bitmap[] metaTiles = new Bitmap[2];
        byte[] currentTileInfo = new byte[8];
        byte[] currentTileType = new byte[2];
        int pnum = 0;
        int currentLayer = 1;
        public int currentArea = -1;
        bool vFlip = false;
        bool hFlip = false;

        public MetaTileEditor()
        {
            InitializeComponent();
            selectedMetaGridBox.Click += selectedMetaTileBox_Click;
            tLPalette.KeyDown += EnterUnfocus;
            tRPalette.KeyDown += EnterUnfocus;
            bLPalette.KeyDown += EnterUnfocus;
            bRPalette.KeyDown += EnterUnfocus;
            mTType.KeyDown += EnterUnfocus;
        }

        //control functions start here
        private void EnterUnfocus(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HiddenLabel.Focus();
            }
        }

        private void tileSetGridBox_Click(object sender, EventArgs e)
        {
            if (tileSetGridBox.Image == null)
                return;
            //TODO check that the selected index actually gets right number


            sTId.Text = tileSetGridBox.SelectedIndex.Hex();
            selectedTileBox.Image = DrawTile();
        }


        private void metaTileGridBox_Click(object sender, EventArgs e)
        {
            if (metaTileGridBox.Image == null)
                return;

            MouseEventArgs me = (MouseEventArgs)e;

            int xpos = me.X / 0x10;
            int ypos = me.Y / 0x10;


            var enlarged = Magnify(metaTiles[currentLayer - 1], new Rectangle(xpos * 16, ypos * 16, 16, 16), 4);

            var room = MapManager.Instance.MapAreas.Single(a => a.Index == currentArea).Rooms.First();

            currentTileInfo = room.GetMetaTileData(ref currentTileType, metaTileGridBox.SelectedIndex, currentLayer);

            if (currentTileInfo == null)
                return;

            tileChange.Enabled = true;
            mTType.Enabled = true;

            mTId.Text = metaTileGridBox.SelectedIndex.Hex();
            mTType.Text = (currentTileType[1] * 0x100 + currentTileType[0]).Hex();

            tId1.Text = (currentTileInfo[0] + (currentTileInfo[1] << 8) & 0x3ff).Hex(); //first 10 bits of 1st byte
            tLPalette.Text = (currentTileInfo[1] >> 4).Hex();       //last 4 bits of 2nd byte

            tId2.Text = (currentTileInfo[2] + (currentTileInfo[3] << 8) & 0x3ff).Hex(); //first 10 bits of 3th byte
            tRPalette.Text = (currentTileInfo[3] >> 4).Hex();       //last 4 bits of 4th byte

            tId3.Text = (currentTileInfo[4] + (currentTileInfo[5] << 8) & 0x3ff).Hex(); //first 10 bits of 5th byte
            bLPalette.Text = (currentTileInfo[5] >> 4).Hex();       //last 4 bits of 6th byte

            tId4.Text = (currentTileInfo[6] + (currentTileInfo[7] << 8) & 0x3ff).Hex(); //first 10 bits of 7th byte
            bRPalette.Text = (currentTileInfo[7] >> 4).Hex();       //last 4 bits of 8th byte

            selectedMetaGridBox.Image = enlarged;
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            nextButton.Enabled = true;

            pnum -= 1;

            var room = MapManager.Instance.MapAreas.Single(a => a.Index == currentArea).Rooms.First();
            DrawTileset(room.tileSet, room.palettes);

            if (pnum == 0)
            {
                prevButton.Enabled = false;
            }

            tileSetGridBox.Image = tileset[currentLayer - 1];
            PaletteNum.Text = pnum.Hex();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            prevButton.Enabled = true;

            pnum += 1;

            var room = MapManager.Instance.MapAreas.Single(a => a.Index == currentArea).Rooms.First();
            DrawTileset(room.tileSet, room.palettes);

            if (pnum == 15)
            {
                nextButton.Enabled = false;
            }

            tileSetGridBox.Image = tileset[currentLayer - 1];
            PaletteNum.Text = pnum.Hex();
        }

        private void hFlip_CheckedChanged(object sender, EventArgs e)
        {
            if (tileSetGridBox.SelectedIndex == -1)
                return;
            hFlip = hFlipBox.Checked;
            selectedTileBox.Image = DrawTile();
        }

        private void vFlipBox_CheckedChanged(object sender, EventArgs e)
        {
            vFlip = vFlipBox.Checked;
            if (tileSetGridBox.SelectedIndex == -1)
                return;
            selectedTileBox.Image = DrawTile();
        }

        private void layer1Button_Click(object sender, EventArgs e)
        {
            currentLayer = 1;
            layer1Button.Enabled = false;
            layer2Button.Enabled = true;

            selectedTileBox.Image = null;
            tileSetGridBox.SelectedIndex = -1;
            selectedMetaGridBox.Image = null;
            metaTileGridBox.SelectedIndex = -1;

            tileSetGridBox.Image = tileset[currentLayer - 1];
            metaTileGridBox.Image = metaTiles[currentLayer - 1];
        }

        private void layer2Button_Click(object sender, EventArgs e)
        {
            currentLayer = 2;
            layer2Button.Enabled = false;
            layer1Button.Enabled = true;

            selectedTileBox.Image = null;
            tileSetGridBox.SelectedIndex = -1;
            selectedMetaGridBox.Image = null;
            metaTileGridBox.SelectedIndex = -1;

            tileSetGridBox.Image = tileset[currentLayer - 1];
            metaTileGridBox.Image = metaTiles[currentLayer - 1];
        }

        private Bitmap DrawTile()
        {
            var room = MapManager.Instance.MapAreas.Single(a => a.Index == currentArea).Rooms.First();
            var b = new Bitmap(8, 8);
            var tnum = tileSetGridBox.SelectedIndex;

            if (currentLayer == 1)
            {
                tnum += 0x200;
            }

            room.tileSet.DrawQuarterTile(ref b, new Point(0, 0), tnum, room.palettes, pnum, this.hFlip, this.vFlip, true);
            var enlarged = Magnify(b, new Rectangle(0, 0, 8, 8), 4);
            return enlarged;
        }

        private void tLPalette_LostFocus(object sender, EventArgs e)
        {
            PaletteChange(0, tLPalette);
        }

        private void tRPalette_LostFocus(object sender, EventArgs e)
        {
            PaletteChange(2, tRPalette);
        }

        private void bLPalette_LostFocus(object sender, EventArgs e)
        {
            PaletteChange(4, bLPalette);
        }

        private void bRPalette_LostFocus(object sender, EventArgs e)
        {
            PaletteChange(6, bRPalette);
        }

        private void mTType_LostFocus(object sender, EventArgs e)
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

        private void tileChange_Click(object sender, EventArgs e)
        {
            if (metaTileGridBox.SelectedIndex == -1)
                return;

            var room = MapManager.Instance.MapAreas.Single(a => a.Index == currentArea).Rooms.First();
            byte[] metatypes = new byte[2];
            var metadata = room.GetMetaTileData(ref metatypes, metaTileGridBox.SelectedIndex, currentLayer);
            var hasTypeChange = !metatypes.SequenceEqual(currentTileType);
            var hasInfoChange = !metadata.SequenceEqual(currentTileInfo);

            if (hasTypeChange || hasInfoChange)
            {
                room.SetMetaTileData(currentTileInfo, currentTileType, metaTileGridBox.SelectedIndex, currentLayer);
            }

            if (hasInfoChange)
            {
                if (currentLayer == 1)
                    Project.Instance.AddPendingChange(new Bg1MetaTileSetChange(currentArea));
                if (currentLayer == 2)
                    Project.Instance.AddPendingChange(new Bg2MetaTileSetChange(currentArea));
            }
            if (hasTypeChange)
            {
                if (currentLayer == 1)
                    Project.Instance.AddPendingChange(new Bg1MetaTileTypeChange(currentArea));
                if (currentLayer == 2)
                    Project.Instance.AddPendingChange(new Bg2MetaTileTypeChange(currentArea));
            }

            var image = metaTiles[currentLayer - 1];
            int x = metaTileGridBox.SelectedIndex % 16;
            int y = metaTileGridBox.SelectedIndex / 16;
            MetaTileSet.DrawTileData(ref image, currentTileInfo, new Point(x * 16, y * 16), room.tileSet, room.palettes, currentLayer == 1, true);
            metaTileGridBox.Image = image;
        }

        private void selectedMetaTileBox_Click(object sender, EventArgs e)
        {
            if (metaTileGridBox.SelectedIndex == -1 || tileSetGridBox.SelectedIndex == -1)
                return;

            var me = (MouseEventArgs)e;

            int tileX = me.X / 32;
            int tileY = me.Y / 32;

            switch (tileX + (tileY * 2))
            {
                case 0:
                    tId1.Text = tileSetGridBox.SelectedIndex.Hex();
                    break;
                case 1:
                    tId2.Text = tileSetGridBox.SelectedIndex.Hex();
                    break;
                case 2:
                    tId3.Text = tileSetGridBox.SelectedIndex.Hex();
                    break;
                case 3:
                    tId4.Text = tileSetGridBox.SelectedIndex.Hex();
                    break;
            }

            int loc = tileX * 2 + tileY * 4;

            byte lowByte = (byte)(tileSetGridBox.SelectedIndex & 0xff); //only first 8 bits
            byte highByte = (byte)(tileSetGridBox.SelectedIndex >> 8); //trim first 8 bits

            if (hFlip)
                highByte += (1 << 2);

            if (vFlip)
                highByte += (1 << 3);

            currentTileInfo[loc] = lowByte;

            var newByte = currentTileInfo[loc + 1] & 0xf0;//only retain palette (last 4 bits)
            newByte += highByte;
            currentTileInfo[loc + 1] = (byte)newByte;

            selectedMetaGridBox.Image = DrawMetaTile(currentTileInfo);
        }

        //utility functions start here
        public void RedrawTiles(Room room)
        {
            mTType.Enabled = false;
            tileChange.Enabled = false;
            metaTiles = room.DrawTilesetImages(16, 0); //areaindex currently unused because what even is swaptiles
            DrawTileset(room.tileSet, room.palettes);
            metaTileGridBox.Image = metaTiles[currentLayer - 1];
            tileSetGridBox.Image = tileset[currentLayer - 1];

            metaTileGridBox.Selectable = true;
            tileSetGridBox.Selectable = true;
            selectedMetaGridBox.Selectable = true;
        }

        public void DrawTileset(TileSet tset, Color[] palettes)
        {
            tileset[0] = new Bitmap(256, 0x100); //0x200 * 8 /16 = 0x200 * 0.5
            tileset[1] = new Bitmap(256, 0x100);

            for (int tnum = 0; tnum < 0x400; tnum++)
            {
                var xpos = tnum % 0x20; //tiles
                var ypos = (tnum - xpos) / 0x20; //tiles
                ypos *= 8; //pixels
                xpos *= 8; //pixels
                tset.DrawQuarterTile(ref tileset[0], new Point(xpos, ypos), tnum + 0x200, palettes, pnum, false, false, true);
                tset.DrawQuarterTile(ref tileset[1], new Point(xpos, ypos), tnum, palettes, pnum, false, false, true);
            }
        }

        private Bitmap Magnify(Bitmap b, Rectangle rect, int scaleMod)
        {
            Bitmap ret = new Bitmap(rect.Width * scaleMod, rect.Height * scaleMod);
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Width; j++)
                {
                    var xpos = scaleMod * i;
                    var ypos = scaleMod * j;

                    var color = b.GetPixel(rect.X + i, rect.Y + j);
                    for (int k = 0; k < scaleMod; k++)
                    {
                        for (int l = 0; l < scaleMod; l++)
                        {
                            ret.SetPixel(xpos + k, ypos + l, color);
                        }
                    }
                }
            }
            return ret;
        }

        private Bitmap DrawMetaTile(byte[] tiledata)
        {
            var b = new Bitmap(64, 64);
            for (var i = 0; i < 4; i++)
            {
                var tile = DrawTile(new byte[] { tiledata[i * 2], tiledata[i * 2 + 1] });

                b = OverlayImage(b, tile, i * 2);
            }
            return b;
        }

        private Bitmap DrawTile(byte[] tileData)
        {
            UInt16 data = 0;
            data = (ushort)(tileData[0] | (tileData[1] << 8));

            int tnum = data & 0x3FF; //bits 1-10

            bool hflip = ((data >> 10) & 1) == 1;//is bit 11 set
            bool vflip = ((data >> 11) & 1) == 1;//is bit 12 set

            int palnum = data >> 12;//last 4 bits

            var room = MapManager.Instance.MapAreas.Single(a => a.Index == currentArea).Rooms.First();
            var b = new Bitmap(8, 8);

            if (currentLayer == 1)
            {
                tnum += 0x200;
            }

            room.tileSet.DrawQuarterTile(ref b, new Point(0, 0), tnum, room.palettes, palnum, hflip, vflip, true);
            var enlarged = Magnify(b, new Rectangle(0, 0, 8, 8), 4);
            return enlarged;
        }

        private void PaletteChange(int id, TextBox box)
        {
            try
            {
                byte palette = Convert.ToByte(box.Text, 16);
                byte data = currentTileInfo[id + 1];
                byte pCleared = (byte)(data & 0x0f); //only keep first 4 bits
                byte newByte = (byte)(pCleared + (palette << 4));
                currentTileInfo[id + 1] = newByte;
                var newTile = DrawTile(new byte[] { currentTileInfo[id], currentTileInfo[id + 1] });
                var b = OverlayImage((Bitmap)selectedMetaGridBox.Image, newTile, id);
                selectedMetaGridBox.Image = b;
            }
            catch
            {
                box.Text = (currentTileInfo[7] >> 4).Hex();
            }
        }

        public Bitmap OverlayImage(Bitmap baseImage, Bitmap overlay, int id)
        {
            int x = (id % 4) / 2;
            int y = id / 4;
            Bitmap finalImage = new Bitmap(baseImage.Width, baseImage.Height);

            using (Graphics g = Graphics.FromImage(finalImage))
            {
                //set background color
                g.Clear(Color.Transparent);

                g.DrawImage(baseImage, new Rectangle(0, 0, baseImage.Width, baseImage.Height));
                g.DrawImage(overlay, new Point(x * 32, y * 32));
            }
            //Draw the final image in the pictureBox
            return finalImage;
        }
    }
}
