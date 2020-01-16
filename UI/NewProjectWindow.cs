using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using MinishMaker.Core;

namespace MinishMaker.UI
{
    public partial class NewProjectWindow : Form
    {
        public Project project = null;
        public NewProjectWindow()
        {
            InitializeComponent();
        }

        private void baseROMButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "GBA ROMs|*.gba|All Files|*.*",
                Title = "Select Base TMC ROM"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            baseROMTextBox.Text = ofd.FileName;
        }

        private void projectButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {

                IsFolderPicker = true,
                Title = "Select project root folder"
            };

            if (ofd.ShowDialog(Handle) != CommonFileDialogResult.Ok)
            {
                return;
            }
            projectTextBox.Text = ofd.FileName;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(projectNameTextBox.Text))
            {
                MessageBox.Show("Please name your project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(baseROMTextBox.Text))
            {
                MessageBox.Show("No ROM selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!Directory.Exists(projectTextBox.Text))
            {
                MessageBox.Show("No project folder selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            project = new Project(projectNameTextBox.Text, baseROMTextBox.Text, projectTextBox.Text);

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
