using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace simPROJECT.UI
{
    public partial class EmptyConfigurationDialog : PanelConfigurationDialogBase
    {
        public EmptyConfigurationDialog() : this(null)
        { }

        public EmptyConfigurationDialog(Device device) : base(device)
        {
            InitializeComponent();
        }
    }
}
