using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace simPROJECT.UI
{
    public partial class PanelConfigurationDialogBase : Form
    {
        public PanelConfigurationDialogBase()
        {
            InitializeComponent();
            IsVisible = false;
        }

        protected PanelConfigurationDialogBase(Device device)
        {
            InitializeComponent();
            Device = device;
            IsVisible = false;
        }

        public virtual void LoadSettings()
        {
            if (!DesignMode)
            {                
                Name = string.Format(Properties.Resources.PanelConfiguration, Device.Name);

                checkBox1.Checked = Device.Configuration.Enable;

                textBox3.Text = Device.DeviceType.ToString();
                textBox2.Text = Device.SerialNumber;
                textBox1.Text = Device.Name;
            }
        }

        public bool IsVisible
        {
            get;
            set;
        }

        public Device Device
        {
            get;
            private set;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected virtual bool CheckSettings()
        {
            // sprawdzenie nazwy

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckSettings())
            {
                return;
            }
            Save();
            Device.Configuration.OnConfigurationChanged();
            Close();
        }

        protected virtual void Save()
        {
            Device.Configuration.Enable = checkBox1.Checked;
            Device.ChangeName(textBox1.Text);

            if (EnableDevice)
            {
                Device.TurnOn();
            }
            else
            {
                Device.TurnOff();
            }
        }

        private bool _allowClose = false;

        internal void ForceClose()
        {
            _allowClose = true;
            Close();
        }

        public new void Show()
        {
            IsVisible = true;
            base.Show();
        }

        private void PanelConfigurationDialogBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowClose)
            {
                e.Cancel = true;
                IsVisible = false;
                Hide();
                return;
            }
        }

        public bool EnableDevice
        {
            get { return checkBox1.Checked; }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Device.StartIdentify();
                MessageBox.Show(this, "Device should looks like switched on. Click OK to end identify.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Device.StopIdentify();
            }
        }
    }
}
