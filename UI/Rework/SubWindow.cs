using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinishMaker.UI.Rework
{
    public class SubWindow : Form
    {
        public SubWindow Instance { get; protected set; }

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
    }
}
