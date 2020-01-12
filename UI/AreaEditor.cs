using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI
{
    public partial class AreaEditor : Form
    {
        private int currentArea = -1;
        private int selectedRoomRect = -1;
        private int biggestX = 0;
        private int biggestY = 0;
        private byte unknown1 = 0;
        private byte unknown2 = 0;
        private byte flagOffset = 0;
        private bool loading = false;

        private Dictionary<int, Rectangle> roomRects = new Dictionary<int, Rectangle>();

        public AreaEditor()
        {
            InitializeComponent();
            mapX.KeyDown += EnterUnfocus;
            mapY.KeyDown += EnterUnfocus;
            areaSongId.KeyDown += EnterUnfocus;
        }

        public void LoadArea(int areaId)
        {
            loading = true;
            areaLabel.Text = "Area: " + areaId.Hex();
            mapX.Text = "FFF";
            mapY.Text = "FFF";
            roomLabel.Text = "Selected Room: -";
            mapX.Enabled = false;
            mapY.Enabled = false;

            canFlute.Enabled = true;
            dungeonMap.Enabled = true;
            moleCave.Enabled = true;
            redName.Enabled = true;
            keysShown.Enabled = true;

            areaSongId.Enabled = true;

            roomRects.Clear();

            currentArea = areaId;
            var maps = MapManager.Instance.MapAreas;
            MapManager.Area area = maps.Single(a => a.Index == currentArea);
            var spot = maps.IndexOf(area);

            var data = new byte[4];

            var areaPath = Project.Instance.projectPath + "/Areas/Area " + StringUtil.AsStringHex2(areaId);
            string areaInfoPath = areaPath + "/" + DataType.areaInfo + "Dat.bin";

            if (!area.areaInfo.SequenceEqual(new byte[] { 0, 0, 0, 0 }))//already a value loaded in 
            {
                data = area.areaInfo;
            }
            if (File.Exists(areaInfoPath))
            {
                data = File.ReadAllBytes(areaInfoPath);
                area.areaInfo = data;
                maps[spot] = area;
            }
            else
            {
                var reader = ROM.Instance.reader;
                var dataloc = ROM.Instance.headers.areaInformationTableLoc + areaId * 4;
                reader.SetPosition(dataloc);
                data = reader.ReadBytes(4);

                area.areaInfo = data;
                maps[spot] = area;

                Console.WriteLine(dataloc.Hex());
                Console.WriteLine(data[0]);
            }

            canFlute.Checked = (data[0] % 2 == 1);//bit 1

            data[0] = (byte)(data[0] >> 1);
            keysShown.Checked = (data[0] % 2 == 1);//bit 2

            data[0] = (byte)(data[0] >> 1);
            redName.Checked = (data[0] % 2 == 1);//bit 4

            data[0] = (byte)(data[0] >> 1);
            dungeonMap.Checked = (data[0] % 2 == 1);//bit 8

            data[0] = (byte)(data[0] >> 1);
            unknown1 = (byte)(data[0] % 2);//bit 10 //currently unknown use

            data[0] = (byte)(data[0] >> 1);
            moleCave.Checked = (data[0] % 2 == 1);//bit 20

            data[0] = (byte)(data[0] >> 1);
            unknown2 = (byte)(data[0] % 2);//bit 40 //unknown

            data[0] = (byte)(data[0] >> 1);
            canFlute.Checked = (data[0] % 2 == 1 || canFlute.Checked);//bit 80 //unused in eur, seems to be same as bit 1?

            areaNameId.Text = data[1].Hex();

            flagOffset = data[2];

            areaSongId.Text = data[3].Hex();

            biggestX = 0;
            biggestY = 0;

            foreach (var room in area.Rooms)
            {
                var rect = room.GetMapRect(areaId);
                roomRects.Add(room.Index, rect);

                if (rect.Bottom > biggestY)
                {
                    biggestY = rect.Bottom;
                }

                if (rect.Right > biggestX)
                {
                    biggestX = rect.Right;
                }
            }
            DrawRects();
            //modify text

            loading = false;
        }

        public void DrawRects()
        {
            var bitmap = new Bitmap(biggestX, biggestY);

            using (var gr = Graphics.FromImage(bitmap))
            {
                var i = 0;
                foreach (var rect in roomRects.Values)
                {
                    var g = i % 8 * 32 + 31;
                    var r = i % 32 / 8 * 32;
                    var b = i / 32 * 32;

                    var color = Color.FromArgb(r, g, b);

                    using (var brush = new SolidBrush(color))
                    {
                        gr.FillRectangle(brush, rect);
                    }

                    using (var font = new Font("Arial", 6))
                    {
                        var point = new Point(rect.X, rect.Y);
                        var invertedColor = Color.FromArgb(255 - r, 255 - g, 255 - b);

                        using (var invertedBrush = new SolidBrush(invertedColor))
                        {

                            gr.DrawString(roomRects.ElementAt(i).Key.Hex(), font, invertedBrush, point);
                        }
                    }


                    i += 1;
                }
            }

            areaLayout.Image = bitmap;
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if (currentArea == -1) //nothing loaded
                return;

            mapX.Enabled = true;
            mapY.Enabled = true;

            Rectangle clickedRect = Rectangle.Empty;
            int index = 0;
            foreach (var rect in roomRects.Values)
            {
                if (rect.Contains(e.X, e.Y))
                {
                    clickedRect = rect;
                    break;
                }
                index += 1;
            }

            if (clickedRect == Rectangle.Empty)
                return;

            selectedRoomRect = roomRects.ElementAt(index).Key;

            roomLabel.Text = "Selected Room: " + selectedRoomRect.Hex();

            mapX.Text = clickedRect.Left.Hex();
            mapY.Text = clickedRect.Top.Hex();
        }

        private void EnterUnfocus(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HiddenLabel.Focus();
            }
        }

        private void mapBox_LostFocus(object sender, EventArgs e)
        {
            var x = Convert.ToInt16(mapX.Text, 16);
            var y = Convert.ToInt16(mapY.Text, 16);
            var roomRect = roomRects[selectedRoomRect];

            if (x != roomRect.Left || y != roomRect.Top)
            {
                roomRect.X = x;
                roomRect.Y = y;
                if (biggestX < roomRect.Right)
                {
                    biggestX = roomRect.Right;
                }

                if (biggestY < roomRect.Bottom)
                {
                    biggestY = roomRect.Bottom;
                }

                roomRects[selectedRoomRect] = roomRect;
                DrawRects();

                var room = MapManager.Instance.FindRoom(currentArea, selectedRoomRect);
                room.SetMapPosition(x, y);

                Project.Instance.AddPendingChange(new RoomMetadataChange(currentArea, selectedRoomRect));//TODO

            }
        }

        private void AreaChanged(object sender, EventArgs e)
        {
            if (loading)
                return;

            var byte1Data =
                (canFlute.Checked ? 0x1 : 0) +
                (keysShown.Checked ? 0x2 : 0) +
                (redName.Checked ? 0x4 : 0) +
                (dungeonMap.Checked ? 0x8 : 0) +
                (unknown1 * 0x10) +
                (moleCave.Checked ? 0x20 : 0) +
                (unknown2 * 0x40) +
                (canFlute.Checked && ROM.Instance.version != RegionVersion.EU ? 0x80 : 0);

            var byte2Data = Convert.ToByte(areaNameId.Text, 16);

            //reserved for spoopy scary byte3 flag offset

            var byte4Data = Convert.ToByte(areaSongId.Text, 16);
            var rom = ROM.Instance;
            var dataloc = ROM.Instance.headers.areaInformationTableLoc + currentArea * 4;
            var data = new byte[4] { (byte)byte1Data, byte2Data, flagOffset, byte4Data };

            var maps = MapManager.Instance.MapAreas;
            MapManager.Area ar = maps.Single(a => a.Index == currentArea);
            var spot = maps.IndexOf(ar);
            ar.areaInfo = data;
            maps[spot] = ar;
            Project.Instance.AddPendingChange(new AreaInfoChange(currentArea));
        }
    }
}
