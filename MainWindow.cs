using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MinishMaker
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "GBA ROM|*.gba|All Files|*.*";
            ofd.Title = "Select TMC EU ROM";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                statusText.Text = "ROM loaded: " + ofd.FileName;
            }
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
