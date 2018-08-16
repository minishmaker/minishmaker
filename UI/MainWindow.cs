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

        private void OpenButtonClick(object sender, EventArgs e)
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
            LoadRom(ofd.FileName);
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            Close();
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

        private void LoadRom(string fileName)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileName));
            gba_ = new GBFile(br.ReadBytes((int)br.BaseStream.Length));

            if (!IsValidRom())
            {
                MessageBox.Show("Invalid TMC ROM. Please Open a valid ROM.", "Incorrect ROM",MessageBoxButtons.OK);
            }
            fileName_ = fileName;
            
        }
    }
}
