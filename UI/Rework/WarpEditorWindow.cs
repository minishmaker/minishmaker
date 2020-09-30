using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI.Rework
{
    public partial class WarpEditorWindow : SubWindow
    {
        private int warpIndex = -1;
        private int warpInformationSize;
        private Core.Rework.WarpData currentWarp;
        private Core.Rework.Room currentRoom;

        private bool shouldTrigger = false;

        public WarpEditorWindow()
        {
            InitializeComponent();
            removeButton.Click += new EventHandler((object o, EventArgs e) => { ChangedHandler(RemoveButton_Click); });
            newButton.Click += new EventHandler((object o, EventArgs e) => { ChangedHandler(NewButton_Click); });
            warpTypeBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(WarpTypeBox_SelectedIndexChanged); });

            warpY.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(WarpY_TextChanged); });
            warpX.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(WarpX_TextChanged); });
            destY.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(DestY_TextChanged); });
            destX.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(DestX_TextChanged); });
            destArea.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(DestArea_TextChanged); });
            destRoom.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(DestRoom_TextChanged); });

            warpShape.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(WarpShape_TextChanged); });
            exitHeight.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(ExitHeight_TextChanged); });
            transitionTypeBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(TransitionTypeBox_SelectedIndexChanged); });
            facingBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(FacingBox_SelectedIndexChanged); });
            soundId.TextChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(SoundId_TextChanged); });

            topLeftCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            topRightCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            bottomRightCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            bottomLeftCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            leftTopCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            leftBottomCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            rightBottomCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });
            rightTopCheck.CheckedChanged += new EventHandler((object o, EventArgs e) => { ChangedHandler(CheckboxChanged); });

            warpTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            warpTypeBox.DataSource = Enum.GetValues(typeof(WarpType));
            transitionTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            transitionTypeBox.DataSource = Enum.GetValues(typeof(TransitionType));
            facingBox.DropDownStyle = ComboBoxStyle.DropDownList;
            facingBox.DataSource = Enum.GetValues(typeof(Facing));

            warpIndex = -1;

            warpTypeBox.Enabled = false;
            transitionTypeBox.Enabled = false;
            facingBox.Enabled = false;

            destArea.Enabled = false;
            destRoom.Enabled = false;
            exitHeight.Enabled = false;

            destX.Enabled = false;
            destY.Enabled = false;

            soundId.Enabled = false;

            prevButton.Enabled = false;
            nextButton.Enabled = false;
            travelButton.Enabled = false;
            newButton.Enabled = false;
            removeButton.Enabled = false;

            ControlBorderPanel(false);
            ControlAreaPanel(false);
        }

        public enum WarpType
        {
            Border = 0x00,
            Area = 0x01
        }

        public enum TransitionType
        {
            Normal = 0x00,
            MinishInstant = 0x01,
            DropIn = 0x02,
            Instant = 0x03,
            StepForward = 0x04,
            //Crash = 0x05,
            MinishDropIn = 0x06,
            StairExit = 0x07,
            //StairExit2 = 0x08,
            //Crash2 = 0x09,
            HopInForward = 0x0A,
            HopIn = 0x0B,
            FlyIn = 0x0C
        }

        public enum Facing
        {
            Keep = 0x00,
            Up = 0x01,
            Left = 0x02,
            Down = 0x03,
            Right = 0x04,
        }

        public override void Setup()
        {
            if (MainWindow.instance.currentRoom == null)
            {
                return;
            }

            warpIndex = 0;
            currentRoom = MainWindow.instance.currentRoom;
            warpInformationSize = currentRoom.MetaData.GetWarpInformationSize();
            shouldTrigger = false;
            newButton.Enabled = true;

            if (warpInformationSize == 0)
            {
                indexLabel.Text = "0";
                SetBlank();
                shouldTrigger = true;
                return;
            }

            indexLabel.Text = warpIndex + "";
            currentWarp = currentRoom.MetaData.GetWarpInformationEntry(warpIndex);

            if (currentWarp.warpType != 0 && currentWarp.warpType != 1)
            {
                SetBlank();
                shouldTrigger = true;
                return;
            }

            SetFilled(currentWarp);
            shouldTrigger = true;
        }

        public override void Cleanup()
        {
            
        }

        private void SetFilled(Core.Rework.WarpData warp)
        {
            warpTypeBox.Enabled = true;
            transitionTypeBox.Enabled = true;
            facingBox.Enabled = true;

            destArea.Enabled = true;
            destRoom.Enabled = true;
            exitHeight.Enabled = true;

            destX.Enabled = true;
            destY.Enabled = true;

            soundId.Enabled = true;

            prevButton.Enabled = true;
            nextButton.Enabled = true;

            newButton.Enabled = true;
            removeButton.Enabled = true;

            SetWarpControls(warp.warpType);

            if (warp.warpType == 1)
            {
                warpX.Text = warp.warpXPixel.Hex();
                warpY.Text = warp.warpYPixel.Hex();
                warpShape.Text = warp.warpVar.Hex();
                ((MainWindow)Application.OpenForms[0]).HighlightWarp(warp.warpXPixel, warp.warpYPixel);
            }
            else
            { 
                topLeftCheck.Checked = ((warp.warpVar & 0x1) == 0x1);
                topRightCheck.Checked = ((warp.warpVar & 0x2) == 0x2);
                rightTopCheck.Checked = ((warp.warpVar & 0x4) == 0x4);
                rightBottomCheck.Checked = ((warp.warpVar & 0x8) == 0x8);
                bottomLeftCheck.Checked = ((warp.warpVar & 0x10) == 0x10);
                bottomRightCheck.Checked = ((warp.warpVar & 0x20) == 0x20);
                leftTopCheck.Checked = ((warp.warpVar & 0x40) == 0x40);
                leftBottomCheck.Checked = ((warp.warpVar & 0x80) == 0x80);

                warpX.Text = "0";
                warpY.Text = "0";
                RedrawBlock();
                ((MainWindow)Application.OpenForms[0]).HighlightWarp(-1, -1);
            }
            warpTypeBox.SelectedItem = (WarpType)warp.warpType;
            transitionTypeBox.SelectedItem = (TransitionType)warp.transitionType;
            facingBox.SelectedItem = (Facing)warp.facing;

            destX.Text = warp.destXPixel.Hex();
            destY.Text = warp.destYPixel.Hex();
            destArea.Text = warp.destArea.Hex();
            destRoom.Text = warp.destRoom.Hex();
            travelButton.Enabled = Utilities.Rework.MapManager.Get().RoomExists(warp.destArea, warp.destRoom);
            exitHeight.Text = warp.exitHeight.Hex();
            soundId.Text = warp.soundId.Hex();

            if (warpIndex >= warpInformationSize - 1)
            {
                nextButton.Enabled = false;
            }
            else
            {
                nextButton.Enabled = true;
            }

            if (warpIndex <= 0)
            {
                prevButton.Enabled = false;
            }
            else
            {
                prevButton.Enabled = true;
            }
        }

        private void SetBlank()
        {
            travelButton.Enabled = false;
            warpTypeBox.Enabled = false;
            transitionTypeBox.Enabled = false;
            facingBox.Enabled = false;

            destArea.Enabled = false;
            destRoom.Enabled = false;
            exitHeight.Enabled = false;

            destX.Enabled = false;
            destY.Enabled = false;

            soundId.Enabled = false;

            prevButton.Enabled = false;
            nextButton.Enabled = false;

            newButton.Enabled = true;
            removeButton.Enabled = false;

            ControlBorderPanel(false);
            ControlAreaPanel(false);

            warpTypeBox.SelectedItem = WarpType.Border;
            transitionTypeBox.SelectedItem = TransitionType.Normal;
            facingBox.SelectedItem = Facing.Keep;

            destX.Text = "0";
            destY.Text = "0";
            destArea.Text = "0";
            destRoom.Text = "0";
            exitHeight.Text = "0";
            soundId.Text = "0";
            //TODO
            ((MainWindow)Application.OpenForms[0]).HighlightWarp(-1, -1);
        }
        
        private void SetWarpControls(int warpType)
        {
            ControlAreaPanel(warpType == 1);
            ControlBorderPanel(warpType != 1);
        }

        public void ControlAreaPanel(bool visible)
        {
            areaPanel.Visible = visible;
            warpX.Visible = visible;
            warpY.Visible = visible;
            warpShape.Visible = visible;
            label2.Visible = visible;
            label3.Visible = visible;
            label7.Visible = visible;
        }

        public void ControlBorderPanel(bool visible)
        {
            borderPanel.Visible = visible;
            topLeftCheck.Visible = visible;
            topRightCheck.Visible = visible;
            leftTopCheck.Visible = visible;
            leftBottomCheck.Visible = visible;
            rightTopCheck.Visible = visible;
            rightBottomCheck.Visible = visible;
            bottomLeftCheck.Visible = visible;
            bottomRightCheck.Visible = visible;
            label13.Visible = visible;
        }

        public void RedrawBlock()
        {
            var graphic = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(graphic))
            {
                var warp = currentWarp;
                var redpen = new Pen(Color.Red, 2);
                var blackPen = new Pen(Color.Black, 2);

                var pen1 = ((warp.warpVar & 0x1) == 0x1) ? redpen : blackPen;
                var pen2 = ((warp.warpVar & 0x2) == 0x2) ? redpen : blackPen;
                var pen3 = ((warp.warpVar & 0x4) == 0x4) ? redpen : blackPen;
                var pen4 = ((warp.warpVar & 0x8) == 0x8) ? redpen : blackPen;
                var pen5 = ((warp.warpVar & 0x10) == 0x10) ? redpen : blackPen;
                var pen6 = ((warp.warpVar & 0x20) == 0x20) ? redpen : blackPen;
                var pen7 = ((warp.warpVar & 0x40) == 0x40) ? redpen : blackPen;
                var pen8 = ((warp.warpVar & 0x80) == 0x80) ? redpen : blackPen;
                g.DrawLine(pen1, 1, 1, 16, 1);//TL
                g.DrawLine(pen2, 16, 1, 31, 1);//TR
                g.DrawLine(pen3, 31, 1, 31, 16);//RT
                g.DrawLine(pen4, 31, 16, 31, 31);//RB
                g.DrawLine(pen5, 1, 31, 16, 31);//BL
                g.DrawLine(pen6, 16, 31, 31, 31);//BR
                g.DrawLine(pen7, 1, 1, 1, 16);//LT
                g.DrawLine(pen8, 1, 16, 1, 31);//LB
            }
            blockImage.Image = graphic;
        }

        public void CheckboxChanged()
        {
            var borderBits  =
                (topLeftCheck.Checked ? 0x1 : 0) +
                (topRightCheck.Checked ? 0x2 : 0) +
                (rightTopCheck.Checked ? 0x4 : 0) +
                (rightBottomCheck.Checked ? 0x8 : 0) +
                (bottomLeftCheck.Checked ? 0x10 : 0) +
                (bottomRightCheck.Checked ? 0x20 : 0) +
                (leftTopCheck.Checked ? 0x40 : 0) +
                (leftBottomCheck.Checked ? 0x80 : 0);

            currentWarp.warpVar = (byte)borderBits;
            RedrawBlock();
        }

        private void WarpTypeBox_SelectedIndexChanged()
        {
            var newVal = (byte)(int)warpTypeBox.SelectedItem;

            currentWarp.warpType = newVal;
            SetWarpControls(newVal);
        }

        private void TransitionTypeBox_SelectedIndexChanged()
        {
            var newVal = (byte)(int)transitionTypeBox.SelectedItem;

            currentWarp.transitionType = newVal;
        }

        private void FacingBox_SelectedIndexChanged()
        {
            var newVal = (byte)(int)facingBox.SelectedItem;

            currentWarp.facing = newVal;
        }

        private void DestX_TextChanged()
        {
            HandleUInt16String(ref destX, ref currentWarp.destXPixel);
        }

        private void DestY_TextChanged()
        {
            HandleUInt16String(ref destY, ref currentWarp.destYPixel);
        }

        private void DestArea_TextChanged()
        {
            HandleByteString(ref destArea, ref currentWarp.destArea);

            travelButton.Enabled = Utilities.Rework.MapManager.Get().RoomExists(currentWarp.destArea, currentWarp.destRoom);
        }

        private void DestRoom_TextChanged()
        {
            HandleByteString(ref destRoom, ref currentWarp.destRoom);

            travelButton.Enabled = Utilities.Rework.MapManager.Get().RoomExists(currentWarp.destArea, currentWarp.destRoom);
        }

        private void ExitHeight_TextChanged()
        {
            HandleByteString(ref exitHeight, ref currentWarp.exitHeight);
        }

        private void SoundId_TextChanged()
        {
            HandleUInt16String(ref soundId, ref currentWarp.soundId);
        }

        private void WarpShape_TextChanged()
        {
            HandleByteString(ref warpShape, ref currentWarp.warpVar);
        }

        private void WarpY_TextChanged()
        {
            HandleUInt16String(ref warpY, ref currentWarp.warpYPixel);
        }

        private void WarpX_TextChanged()
        {
            HandleUInt16String(ref warpX, ref currentWarp.warpXPixel);
        }

        private void NewButton_Click()
        {
            if (currentRoom == null)
            {
                return;
            }

            if (warpInformationSize == 0)
                warpIndex = -1;

            currentRoom.MetaData.AddNewWarpInformation(warpIndex);
            warpInformationSize += 1;
            warpIndex += 1;

            Setup();

        }

        private void RemoveButton_Click()
        {
            if (currentRoom == null)
            {
                return;
            }

            currentRoom.MetaData.RemoveWarpInformation(warpIndex);

            if (warpIndex != 0)
                warpIndex -= 1;

            Setup();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            warpIndex += 1;
            Setup();
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            warpIndex -= 1;
            Setup();
        }

        private void TravelButton_Click(object sender, EventArgs e)
        {
            MainWindow.instance.ChangeRoom(currentWarp.destArea, currentWarp.destRoom);
        }

        private void ChangedHandler(Action changeAction)
        {
            if (!shouldTrigger)
                return;

            changeAction.Invoke();

            Project.Instance.AddPendingChange(new WarpDataChange(currentRoom.Parent.Id, currentRoom.Id));
        }

        //because why type the same 4 times
        private void HandleByteString(ref TextBox textBox, ref byte property)
        {
            try
            {
                var newVal = Convert.ToByte(textBox.Text, 16);

                property = newVal;
            }
            catch
            {
                textBox.Text = property.Hex();
            }
        }

        //or 5 times
        private void HandleUInt16String(ref TextBox textBox, ref ushort property)
        {
            try
            {
                var newVal = Convert.ToUInt16(textBox.Text, 16);

                property = newVal;
            }
            catch
            {
                textBox.Text = property.Hex();
            }
        }
    }
}
