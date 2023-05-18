using System;
using System.Drawing;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.UI
{
    public partial class PaletteEditorWindow : SubWindow
    {
        private PaletteSet currentSet;
        private int paletteSetId = -1;
        private int selectedX = -1;
        private int selectedY = -1;
        private bool shouldTrigger = true;
        
        public PaletteEditorWindow()
        {
            InitializeComponent();

            RInput.KeyDown += EnterUnfocus;
            GInput.KeyDown += EnterUnfocus;
            BInput.KeyDown += EnterUnfocus;

            RInput.LostFocus += new EventHandler((object o, EventArgs e)=>{ ChangeColor(InputColor.RED); });
            GInput.LostFocus += new EventHandler((object o, EventArgs e) => { ChangeColor(InputColor.GREEN); });
            BInput.LostFocus += new EventHandler((object o, EventArgs e) => { ChangeColor(InputColor.BLUE); });

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    var posX = x;
                    var posY = y;
                    var element = new PictureBox();
                    element.BackColor = Color.Black;
                    element.Size = new Size(15, 15);
                    element.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
                    element.BorderStyle = BorderStyle.FixedSingle;
                    element.Click += new EventHandler((object o, EventArgs e) => { SetColorData(posX, posY); });
                    tableLayoutPanel1.Controls.Add(element, x + 1, y);
                }
            }
        }


        public override void Setup()
        {
            RInput.Enabled = false;
            BInput.Enabled = false;
            GInput.Enabled = false;
            if(Project.Instance == null || MainWindow.instance.currentArea == null)
            {
                return;
            }

            var room = MainWindow.instance.currentRoom;
            var area = room.Parent;
            paletteSetId = area.Tilesets[room.MetaData.tilesetId].paletteSetId;//.PaletteSetId;
            SetIdValue.Text = $"0x{paletteSetId.Hex(2)}";
            var r = ROM.Instance.reader;
            var h = ROM.Instance.headers;
            int paletteSetTableLoc = h.paletteSetTableLoc;
            int addr = r.ReadAddr(paletteSetTableLoc + (paletteSetId * 4)); //4 byte entries
            int palAddr = r.ReadUInt16(addr);
            PaletteOffsetValue.Text = $"0x{palAddr.Hex(4)}";
            StartValue.Text = $"0x{r.ReadByte().Hex(2)}";
            AmountValue.Text = $"0x{r.ReadByte().Hex(2)}";
            var psm = PaletteSetManager.Get();
            var currentIdUsage = psm.GetIdUsage(paletteSetId);
            IdUsedValue.Text = $"{currentIdUsage.Count}";
            var idUsedTooltipText = "";
            foreach (var areaId in currentIdUsage)
            {
                idUsedTooltipText += "0x" + areaId.Hex(2) + " ";
            }
            toolTip1.SetToolTip(IdUsedValue, idUsedTooltipText);

            var set = psm.GetSet(paletteSetId);
            currentSet = set;
            for (var row = 0; row < 16; row++)
            {
                var useColor = row >= set.start && row <= set.start + set.amount;
                for (var column = 0; column < 16; column++)
                {
                    var image = tableLayoutPanel1.GetControlFromPosition(column + 1, row);
                    var color = useColor ? set.Colors[row * 16 + column] : Color.Black;
                    image.BackColor = color;
                }
            }
        }

        public override void Cleanup()
        {
            
        }

        private void SetColorData(int x, int y)
        {
            if (MainWindow.instance.currentRoom == null)
            {
                return;
            }
            if(y < currentSet.start || y > currentSet.start + currentSet.amount)
            {
                return;
            }

            selectedX = x;
            selectedY = y;
            var pb = (PictureBox)tableLayoutPanel1.GetControlFromPosition(x + 1, y);
            var color = pb.BackColor;
            shouldTrigger = false;
            RInput.Text = (color.R / 8).ToString();
            GInput.Text = (color.G / 8).ToString();
            BInput.Text = (color.B / 8).ToString();
            RInput.Enabled = true;
            BInput.Enabled = true;
            GInput.Enabled = true;
            shouldTrigger = true;
        }

        private void ChangeColor(InputColor inputColor)
        {
            if (!shouldTrigger)
            {
                return;
            }

            var element = tableLayoutPanel1.GetControlFromPosition(selectedX + 1, selectedY);
            int parseValue;
            switch (inputColor)
            {
                case InputColor.RED:
                    if (!int.TryParse(RInput.Text, out parseValue) || parseValue > 31 || parseValue < 0) {
                        shouldTrigger = false;
                        RInput.Text = (element.BackColor.R / 8).ToString();
                        shouldTrigger = true;
                        return;
                    }

                    element.BackColor = Color.FromArgb(255, parseValue * 8, element.BackColor.G, element.BackColor.B);
                    
                    break;
                case InputColor.GREEN:
                    if (!int.TryParse(GInput.Text, out parseValue) || parseValue > 31 || parseValue < 0)
                    {
                        shouldTrigger = false;
                        GInput.Text = (element.BackColor.G / 8).ToString();
                        shouldTrigger = true;
                        return;
                    }

                    element.BackColor = Color.FromArgb(255, element.BackColor.R, parseValue * 8, element.BackColor.B);
                    break;
                case InputColor.BLUE:
                    if (!int.TryParse(BInput.Text, out parseValue) || parseValue > 31 || parseValue < 0)
                    {
                        shouldTrigger = false;
                        BInput.Text = (element.BackColor.B / 8).ToString();
                        shouldTrigger = true;
                        return;
                    }

                    element.BackColor = Color.FromArgb(255, element.BackColor.R, element.BackColor.G, parseValue * 8);
                    break;
            }
            //refresh room
            MainWindow.instance.RedrawRoom();

            //ADD PALETTE CHANGE
            var index = selectedX + (selectedY * 16);
            currentSet.Colors[index] = element.BackColor;
            Project.Instance.AddPendingChange(new PaletteChange(paletteSetId));
        }

        private enum InputColor
        {
            RED,
            GREEN,
            BLUE
        }
    }
}
