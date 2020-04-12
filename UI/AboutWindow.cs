using System;
using System.Windows.Forms;
using MinishMaker.Properties;

namespace MinishMaker.UI
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();

            // This is lazy, do something nicer later
            label1.Text = $"{ProductName}" +
                $"\n{AssemblyInfo.GetGitTag()}" +
                "\n\nDeveloped by" +
                "\nMikesky" +
                "\nwjg999" +
                "\nBerylliosis" +
                "\n\n Game research by ppltoast and Leonarth";
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/minishmaker/minishmaker");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://minishmaker.com");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void linkLabel3_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://docs.minishmaker.com/minish-maker/minish-maker");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
