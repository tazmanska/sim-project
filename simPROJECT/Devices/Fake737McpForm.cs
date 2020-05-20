using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace simPROJECT.Devices
{
    public partial class Fake737McpForm : Form
    {
        public Fake737McpForm()
        {
            InitializeComponent();
        }

        internal void _Close()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(Close));
                return;
            }
            Close();
        }
    }
}
