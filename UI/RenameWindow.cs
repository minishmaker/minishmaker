using MinishMaker.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinishMaker.UI
{
    public partial class RenameWindow : Form
    {
        int areaId = -1;
        int roomId = -1;

        public RenameWindow()
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
                p.roomNames[areaKey] = areaName;
            }
            else
            {
                p.roomNames.Add(areaKey, areaName);
            }

            if (p.roomNames.ContainsKey(roomKey))
            {
                p.roomNames[roomKey] = roomName;
            }
            else
            {
                p.roomNames.Add(roomKey, roomName);
            }

            ((MainWindow)Application.OpenForms[0]).ChangeNodeName(areaId + "", areaName, roomId + "", roomName);

            this.Close();
        }
    }
}
