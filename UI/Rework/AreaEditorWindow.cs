using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI.Rework
{
    public partial class AreaEditorWindow : SubWindow
    {
        private int selectedRoomRect = -1;
        private int currentAreaId = -1;
        private int biggestX = 0;
        private int biggestY = 0;
        private bool loading = false;

        private Dictionary<int, Rectangle> roomRects = new Dictionary<int, Rectangle>();

        public AreaEditorWindow()
        {
            InitializeComponent();
            mapX.KeyDown += EnterUnfocus;
            mapY.KeyDown += EnterUnfocus;
            areaSongId.KeyDown += EnterUnfocus;
            areaNameId.KeyDown += EnterUnfocus;
            //flagOffset.key
        }

        public override void Setup()
        {
            MainWindow mw = MainWindow.instance;

            loading = true;

            biggestX = 0;
            biggestY = 0;

            if (mw.currentRoom == null)
            {
                areaLabel.Text = "Area: -";
                mapX.Text = "FFF";
                mapY.Text = "FFF";
                roomLabel.Text = "Selected Room: -";

                currentAreaId = -1;

                mapX.Enabled = false;
                mapY.Enabled = false;

                canFlute.Enabled = false;
                dungeonMap.Enabled = false;
                moleCave.Enabled = false;
                redName.Enabled = false;
                keysShown.Enabled = false;

                areaSongId.Enabled = false;

                roomRects.Clear();
                DrawRects();
                loading = false;
                return;
            }

            var area = mw.currentRoom.Parent;

            if (area.Id == currentAreaId)
            {
                return;
            }

            areaLabel.Text = "Area: " + area.Id.Hex();
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

            area.LoadAreaInfo();

            canFlute.Checked = area.canFlute;
            keysShown.Checked = area.hasKeyCounter;
            redName.Checked = area.hasRedName;
            dungeonMap.Checked = area.usesDungeonMap;
            moleCave.Checked = area.isMoleCave;

            unknown1.Checked = area.usesUnknown1;
            unknown2.Checked = area.usesUnknown2;

            areaNameId.Text = area.nameId.Hex();
            areaSongId.Text = area.songId.Hex();

            flagOffsetBox.Text = area.flagOffset.Hex();

            foreach (var room in area.GetAllRooms())
            {
                try
                {
                    var rect = room.MetaData.GetMapRect();
                    roomRects.Add(room.Id, rect);

                    if (rect.Bottom > biggestY)
                    {
                        biggestY = rect.Bottom;
                    }

                    if (rect.Right > biggestX)
                    {
                        biggestX = rect.Right;
                    }
                }
                catch (RoomException ex) 
                {
                    if(room.Bg1Exists || room.Bg2Exists) {
                        throw (ex);
                    } //otherwise its just an invalid room and should be ignored
                }
            }
            DrawRects();

            loading = false;
        }

        public override void Cleanup()
        {
            
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
            if (currentAreaId == -1) //nothing loaded
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

                var room = MapManager.Instance.FindRoom(currentAreaId, selectedRoomRect);
                room.SetMapPosition(x, y);

                Project.Instance.AddPendingChange(new RoomMetadataChange(currentAreaId, selectedRoomRect));//TODO

            }
        }

        private void AreaChanged(object sender, EventArgs e)
        {
            if (loading)
                return;

            if (currentAreaId == -1) //nothing loaded
                return;

            var byte1Data =
                (canFlute.Checked   ? 0x1 : 0) +
                (keysShown.Checked  ? 0x2 : 0) +
                (redName.Checked    ? 0x4 : 0) +
                (dungeonMap.Checked ? 0x8 : 0) +
                (unknown1.Checked   ? 0x10 : 0) +
                (moleCave.Checked   ? 0x20 : 0) +
                (unknown2.Checked   ? 0x40 : 0) +
                (canFlute.Checked && ROM.Instance.version != RegionVersion.EU ? 0x80 : 0);

            var byte2Data = Convert.ToByte(areaNameId.Text, 16);

            var byte3Data = Convert.ToByte(areaNameId.Text, 16);
            //reserved for spoopy scary byte3 flag offset

            var byte4Data = Convert.ToByte(areaSongId.Text, 16);
            var rom = ROM.Instance;
            var dataloc = ROM.Instance.headers.areaInformationTableLoc + currentAreaId * 4;
            var data = new byte[4] { (byte)byte1Data, byte2Data, byte3Data, byte4Data };

            var area = Utilities.Rework.MapManager.Get().GetArea(currentAreaId);
            area.SetInfo(data);

            Project.Instance.AddPendingChange(new AreaInfoChange(currentAreaId));
        }
    }
}
