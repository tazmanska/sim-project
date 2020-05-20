using System;
using System.Collections.Generic;
using System.Text;
using simPROJECT.Devices;

namespace simPROJECT.Configuration
{
    public class DeviceConfiguration
    {
        internal static DeviceConfiguration CreateDefaultConfigurationForDeviceType(string serialNumber, DeviceType deviceType)
        {
            DeviceConfiguration result = null;
            switch (deviceType)
            {
                case DeviceType.MultiradioV1:
                    result = new MultiradioV1Configuration();
                    result.SerialNumber = serialNumber;
                    return result;

                case DeviceType.MCP737:
                    result = new MCP737Configuration();
                    result.SerialNumber = serialNumber;
                    return result;
            }
            return new DeviceConfiguration(serialNumber);
        }

        public event EventHandler ConfigurationChangeEvent;

        public DeviceConfiguration()
        {
            Enable = true;
        }

        public DeviceConfiguration(string serialNumber) : this()
        {
            SerialNumber = serialNumber;
        }

        public string SerialNumber
        {
            get;
            set;
        }

        public bool Enable
        {
            get;
            set;
        }

        public void OnConfigurationChanged()
        {
            if (ConfigurationChangeEvent != null)
            {
                ConfigurationChangeEvent(this, EventArgs.Empty);
            }
        }
    }
}
