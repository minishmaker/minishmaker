using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GBHL;
using MinishMaker.Core;

namespace MinishMaker
{
    public partial class MainWindow : Form
    {
        private ROM ROM_;

        private string fileName_ = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // MenuBar Buttons
        private void OpenButtonClick(object sender, EventArgs e)
        {
            LoadRom();
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        // ToolStrip Buttons
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            LoadRom();
        }

        private void LoadRom()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "GBA ROMs|*.gba|All Files|*.*",
                Title = "Select TMC EU ROM"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                ROM_ = new ROM(ofd.FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (ROM.Instance.version.Equals(RegionVersion.None))
            {
                MessageBox.Show("Invalid TMC ROM. Please Open a valid ROM.", "Incorrect ROM",MessageBoxButtons.OK);
                return;
            }

            fileName_ = ofd.FileName;
        }

    }
}
