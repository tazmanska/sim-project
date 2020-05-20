using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace simPROJECT
{
    public partial class InformationDialog : Form
    {
        public InformationDialog()
        {
            InitializeComponent();
            ForceClose = false;

            simPROJECT.Planes.PMDG.PMDG737MCP mcp = new simPROJECT.Planes.PMDG.PMDG737MCP();
            mcp.Startup();
            int index = 10;
            while (index-- > 0)
            {
                mcp.UpdateOutputs();
                System.Threading.Thread.Sleep(100);
            }
        }

        public bool ForceClose
        {
            get;
            set;
        }

        private void Information_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ForceClose)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
