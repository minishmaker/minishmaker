using System;
using System.Windows.Forms;

namespace MinishMaker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string file = null;
            if(args.Length>=1) {
                file = args[0];
            }
            Application.Run(new UI.MainWindow(file));
        }
    }
}
