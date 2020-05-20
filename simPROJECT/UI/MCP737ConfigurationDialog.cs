using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using simPROJECT.Configuration;
using System.IO;

namespace simPROJECT.UI
{
    public partial class MCP737ConfigurationDialog : PanelConfigurationDialogBase
    {
        public MCP737ConfigurationDialog(Device device) : base(device)
        {
            InitializeComponent();

            if (!DesignMode)
            {
                Text = string.Format("MCP737 '{0}' configuration", device.Name);
            }
        }

        private MCP737Configuration Config
        {
            get { return (MCP737Configuration)Device.Configuration; }
        }

        public override void LoadSettings()
        {
            base.LoadSettings();

            textBox4.Text = Config.PMDGKeysFile;
            textBox5.Text = Config.ProsimConfigFile;
        }

        protected override void Save()
        {
            base.Save();

            // zapisanie ustawień
            Config.PMDGKeysFile = textBox4.Text;
            Config.ProsimConfigFile = textBox5.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = textBox4.Text;
            ofd.Filter = "Keyboard commands file (*.ini)|*.ini|Any file (*.*)|*.*";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                textBox4.Text = ofd.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = textBox5.Text;
            ofd.Filter = "Confi file (*.xml)|*.xml|Any file (*.*)|*.*";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                textBox5.Text = ofd.FileName;
            }
        }
    }
}
