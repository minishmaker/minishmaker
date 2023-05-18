using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinishMaker.UI
{
    public class SubWindow : Form
    {
        public SubWindow Instance { get; protected set; }

        public SubWindow()
        {
            this.KeyPreview = true;
            KeyDown += ProxyKeyPress;
        }

        public virtual void Setup()
        {
            throw new NotImplementedException();
        }
        public virtual void Cleanup()
        {
            throw new NotImplementedException();
        }

        public void EnterUnfocus(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessTabKey(true);
            }
        }


        public virtual void ProxyKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                MainWindow.instance.SaveAllChanges();
            } else if (e.Control && e.KeyCode == Keys.B) {
                MainWindow.instance.BuildProject();
            }
        }
    }
}
