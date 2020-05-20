using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT
{
    static class DeviceFactory
    {
        public static Device CreateDevice(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo)
        {
            if (deviceInfo.SerialNumber.Length < 2)
            {
                return null;
            }
            string snPrefix = deviceInfo.SerialNumber.Substring(0, 2);
            switch (snPrefix)
            {
                case Devices.MultiradioV1.SN_PREFIX:
                    if (Device.CheckDevice(deviceInfo, Devices.MultiradioV1.TYPE))
                    {
                        return new Devices.MultiradioV1(deviceInfo);
                    }
                    break;

                case Devices.MultiradioV1WithEncoder.SN_PREFIX:
                    if (Device.CheckDevice(deviceInfo, Devices.MultiradioV1WithEncoder.TYPE))
                    {
                        return new Devices.MultiradioV1WithEncoder(deviceInfo);
                    }
                    break;

                case Devices.MCP737.SN_PREFIX:
                    if (Device.CheckDevice(deviceInfo, Devices.MCP737.TYPE, true))
                    {
                        return new Devices.MCP737(deviceInfo);
                    }
                    break;
            }
            return null;
        }
    }
}
