using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GBHL;
using MinishMaker.TMC;

namespace MinishMaker
{
    public partial class MainWindow : Form
    {
        private GBFile gba_;

        private string fileName_ = "";
        private RegionVersion version = RegionVersion.EU;

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

        private bool IsValidRom()
        {
            // Determine Game region and if valid ROM
            byte[] regionBytes = gba_.ReadBytes(0xAC, 4);
            string region = System.Text.Encoding.UTF8.GetString(regionBytes);

            if (region == "BZMP")
            {
                version = RegionVersion.EU;
                return true;
            }

            if (region == "BZMJ")
            {
                version = RegionVersion.JP;
                return true;
            }

            if (region == "BZME")
            {
                version = RegionVersion.US;
                return true;
            }
            return false;
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

            BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));
            gba_ = new GBFile(br.ReadBytes((int)br.BaseStream.Length));

            if (!IsValidRom())
            {
                MessageBox.Show("Invalid TMC ROM. Please Open a valid ROM.", "Incorrect ROM",MessageBoxButtons.OK);
                return;
            }
            fileName_ = ofd.FileName;
            
        }

    }
}
