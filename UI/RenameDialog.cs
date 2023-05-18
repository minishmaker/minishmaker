using System;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Utilities;

namespace MinishMaker.UI
{
    public partial class RenameDialog : Form
    {
        int areaId = -1;
        int roomId = -1;

        public RenameDialog()
        {
            InitializeComponent();
        }

        public void SetTarget(int area, int room, string areaName, string roomName)
        {
            areaId = area;
            roomId = room;
            areaNameBox.Text = areaName;
            roomNameBox.Text = roomName;

            if (roomName == "")
                roomNameBox.Enabled = false;
            else
                roomNameBox.Enabled = true;
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            var areaName = areaNameBox.Text;
            var roomName = roomNameBox.Text;
            var p = Project.Instance;
            var areaKey = new Tuple<int, int>(areaId, -1);
            var roomKey = new Tuple<int, int>(areaId, roomId);

            if (p.roomNames.ContainsKey(areaKey))
            {
                if (!p.roomNames[areaKey].Equals(areaName))
                {
                    p.customNames[areaKey] = areaName;
                }
            }
            else
            {
                p.customNames.Add(areaKey, areaName);
            }

            if (roomId != -1) 
            {
                if (p.roomNames.ContainsKey(roomKey) )
                {
                    if (!p.roomNames[roomKey].Equals(roomName))
                    {
                        p.customNames[roomKey] = roomName;
                    }
                }
                else
                {
                    p.customNames.Add(roomKey, roomName);
                }
            }
            MainWindow.instance.ChangeNodeName(areaId.Hex(), areaName, roomId.Hex(), roomName);

            this.Hide();
        }
    }
}
