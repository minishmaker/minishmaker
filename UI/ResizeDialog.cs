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
    public partial class ResizeDialog : Form
    {
        public ResizeDialog(Point roomSize)
        {
            InitializeComponent();
            this.confirmation.DialogResult = DialogResult.OK;
            this.AcceptButton = confirmation;
            this.xDimBox.Text = roomSize.X + "";
            this.yDimBox.Text = roomSize.Y + "";
        }

        public Point GetDims()
        {
            var x = Convert.ToByte(this.xDimBox.Text);
            var y = Convert.ToByte(this.yDimBox.Text);
            return new Point(x, y);
        }
    }
}
