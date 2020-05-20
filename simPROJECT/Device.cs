using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using simPROJECT.Devices;
using System.Diagnostics;
using simPROJECT.Planes;

namespace simPROJECT
{
    public abstract class Device
    {
        public static bool CheckDevice(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo, DeviceType deviceType)
        {
            return CheckDevice(deviceInfo, deviceType, false);
        }

        public static bool CheckDevice(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo, DeviceType deviceType, bool resetCommunication)
        {
            FTD2XX_NET.FTDI driver = null;
            try
            {
                // połączenie z urządzeniem
                driver = new FTD2XX_NET.FTDI();
                FTD2XX_NET.FTDI.FT_STATUS ret = driver.OpenByLocation(deviceInfo.LocId);
                if (ret == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    ret = driver.ResetDevice();
                    //ret = driver.ResetPort();
                    ret = driver.SetBaudRate(56700);
                    ret = driver.SetDataCharacteristics(8, 2, 0);                    
                    ret = driver.SetFlowControl(FTD2XX_NET.FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0);

                    uint available = 0;
                    uint red = 0;
                    uint written = 0;

                    // resetowanie urządzenia
                    if (resetCommunication)
                    {
                        // wysłanie resetu
                        ret = driver.GetRxBytesAvailable(ref available);
                        red = 0;
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            driver.Close();
                            driver = null;
                            return false;
                        }
                        if (available > 0)
                        {
                            byte[] tmp = new byte[available];
                            ret = driver.Read(tmp, (uint)tmp.Length, ref red);
                            if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                            {
                                driver.Close();
                                driver = null;
                                return false;
                            }
                        }

                        available = 0;
                        int tries = 16;
                        while (available < 3 && tries-- > 0)
                        {
                            ret = driver.Write(new byte[] { 254 }, 1, ref written);
                            if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK || written != 1)
                            {
                                driver.Close();
                                driver = null;
                                return false;
                            }

                            Thread.Sleep(20);

                            ret = driver.GetRxBytesAvailable(ref available);
                            red = 0;
                            if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                            {
                                driver.Close();
                                driver = null;
                                return false;
                            }
                        }

                        if (available < 3)
                        {
                            driver.Close();
                            driver = null;
                            return false;
                        }

                        if ((available % 3) != 0)
                        {
                            Thread.Sleep(20);
                            ret = driver.GetRxBytesAvailable(ref available);
                            red = 0;
                            if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                            {
                                driver.Close();
                                driver = null;
                                return false;
                            }
                        }

                        byte[] tmp3 = new byte[available];
                        ret = driver.Read(tmp3, (uint)tmp3.Length, ref red);
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            driver.Close();
                            driver = null;
                            return false;
                        }
                    }

                    // wysłanie rozkazu ciszy                    
                    byte[] data = new byte[3] { 255, 1, 204 };
                    written = 0;
                    ret = driver.Write(data, data.Length, ref written);
                    if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        return false;
                    }

                    // odczytanie wszystkich danych 
                    available = 0;
                    ret = driver.GetRxBytesAvailable(ref available);
                    red = 0;
                    if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        return false;
                    }
                    if (available > 0)
                    {
                        byte[] tmp = new byte[available];
                        ret = driver.Read(tmp, (uint)tmp.Length, ref red);
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            return false;
                        }
                    }

                    // wysłanie zapytania o typ urządzenia
                    data = new byte[3] { 255, 1, 202 };
                    written = 0;
                    ret = driver.Write(data, data.Length, ref written);

                    if (ret == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        int count = 0;
                        while (count++ < 5)
                        {
                            Thread.Sleep(50);
                            ret = driver.GetRxBytesAvailable(ref available);
                            if (ret == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && available > 2)
                            {
                                // odczytanie danych
                                data[0] = data[1] = data[2] = 0;
                                ret = driver.Read(data, (uint)data.Length, ref red);
                                if (ret == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && red == data.Length)
                                {
                                    // sprawdzenie czy jest to raport o typie urządzenia i czy typ zgadza się z przekazanym typem
                                    if (data[0] == 2)
                                    {
                                        byte tmp = (byte)(data[1] & 0x0f);
                                        return tmp == (byte)deviceType;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (driver != null && driver.IsOpen)
                {
                    try
                    {
                        driver.Close();
                    }
                    catch { }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("Typ: {0}, nazwa: {1}, sn: {2}", DeviceType, Name, SerialNumber);
        }

        protected Device(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo, string name, DeviceType deviceType)
        {
            Running = false;
            Name = name;
            SerialNumber = deviceInfo.SerialNumber;
            DeviceType = deviceType;
            Configuration = Globals.Instance.GetConfiguration(SerialNumber, deviceType);
            Configuration.ConfigurationChangeEvent += new EventHandler(Configuration_ConfigurationChangeEvent);
            _enabled = Configuration.Enable;
        }

        public DeviceType DeviceType
        {
            get;
            private set;
        }

        internal abstract void SetPlane(IPlane plane);

        private bool _enabled = false;

        private void Configuration_ConfigurationChangeEvent(object sender, EventArgs e)
        {
            // reagowanie na włączenie/wyłączenie urządzenia
            if (_enabled != Configuration.Enable)
            {
                _enabled = Configuration.Enable;
                if (Configuration.Enable)
                {
                    TurnOn();
                }
                else
                {
                    TurnOff();
                }
            }

            OnConfigurationChange();
        }

        protected virtual void OnConfigurationChange()
        {

        }

        public virtual void ClearDevice()
        {

        }

        public bool Running
        {
            get;
            protected set;
        }

        public string SerialNumber
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        private Thread _workingThread = null;

        protected bool NeedStop
        {
            get;
            private set;
        }

        public void TurnOn()
        {
            if (!Configuration.Enable || Running)
            {
                return;
            }

            TurnOff();

            NeedStop = false;
            _workingThread = new Thread(WorkingThread);
            _workingThread.Start();

            Running = true;
        }

        protected abstract void WorkingThread();

        protected abstract void Disable();

        public void TurnOff()
        {
            NeedStop = true;
            if (_workingThread != null)
            {
                try
                {
                    _workingThread.Join(50);
                }
                catch { }
                try
                {
                    _workingThread.Abort();
                }
                catch { }
                _workingThread = null;
            }
            Running = false;
            Disable();
        }

        public virtual UI.PanelConfigurationDialogBase CreateSettingsDialog()
        {
            return new UI.EmptyConfigurationDialog(this);
        }

        public Configuration.DeviceConfiguration Configuration
        {
            get;
            private set;
        }

        protected FTD2XX_NET.FTDI OpenDevice(bool resetCommunication)
        {
            FTD2XX_NET.FTDI result = new FTD2XX_NET.FTDI();
            if (result.OpenBySerialNumber(SerialNumber) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
            {
                Debug.WriteLine(string.Format("Otwarto połączenie z urządzeniem: {0}", this));

                if (result.ResetDevice() != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    result.Close();
                    result = null;
                    return null;
                }

                // ustawienie parametrów połączenia
                if (result.SetBaudRate(56700) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    result.Close();
                    result = null;
                    return null;
                }

                if (result.SetDataCharacteristics(8, 2, 0) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    result.Close();
                    result = null;
                    return null;
                }

                if (result.SetFlowControl(FTD2XX_NET.FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    result.Close();
                    result = null;
                    return null;
                }

                if (resetCommunication)
                {
                    Debug.WriteLine(string.Format("Resetowanie urządzenia: {0}", this));

                    uint available = 0;
                    uint red = 0;
                    uint written = 0;

                    // wysłanie resetu
                    FTD2XX_NET.FTDI.FT_STATUS ret = result.GetRxBytesAvailable(ref available);
                    red = 0;
                    if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        result.Close();
                        result = null;
                        return null;
                    }
                    if (available > 0)
                    {
                        byte[] tmp = new byte[available];
                        ret = result.Read(tmp, (uint)tmp.Length, ref red);
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            result.Close();
                            result = null;
                            return null;
                        }
                    }

                    available = 0;
                    int tries = 16;
                    while (available < 3 && tries-- > 0)
                    {
                        ret = result.Write(new byte[] { 254 }, 1, ref written);
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK || written != 1)
                        {
                            result.Close();
                            result = null;
                            return null;
                        }

                        Thread.Sleep(20);

                        ret = result.GetRxBytesAvailable(ref available);
                        red = 0;
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            result.Close();
                            result = null;
                            return null;
                        }
                    }

                    if (available < 3)
                    {
                        result.Close();
                        result = null;
                        return null;
                    }

                    if ((available % 3) != 0)
                    {
                        Thread.Sleep(20);
                        ret = result.GetRxBytesAvailable(ref available);
                        red = 0;
                        if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            result.Close();
                            result = null;
                            return null;
                        }
                    }

                    byte[] tmp3 = new byte[available];
                    ret = result.Read(tmp3, (uint)tmp3.Length, ref red);
                    if (ret != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        result.Close();
                        result = null;
                        return null;
                    }
                }

                return result;
            }
            return null;
        }

        protected void WriteToDevice(FTD2XX_NET.FTDI driver, byte[] data)
        {
            uint written = 0;
            if (driver.Write(data, data.Length, ref written) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK || written != data.Length)
            {
                throw new Exception("Błąd wysyłania danych do urządzenia.");
            }            
#if DEBUG
            List<string> tmp = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                tmp.Add("0x" + data[i].ToString("X2"));
            }
            Debug.WriteLine(string.Format("Wysłanie danych do urządzenia ({0}): {1}", this.ToString(), string.Join(" ", tmp.ToArray())));
#endif
        }

        protected bool ReadKeysState(FTD2XX_NET.FTDI driver, ref byte[] keys)
        {
            bool result = false;
            uint available = 0;            
            byte[] data = new byte[3];
            while (driver.GetRxBytesAvailable(ref available) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && available > 2)
            {
                uint red = 0;
                if (driver.Read(data, 3, ref red) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && red == 3)
                {
                    int type = (data[1] >> 4) & 0x0f;
                    int index = data[1] & 0x0f;
                    if (type == 1 && index < keys.Length)
                    {
                        keys[index] = data[2];
                        result = true;
                    }
#if DEBUG
                    Debug.WriteLine(string.Format("Odebrano dane z urządzenia ({0}): 0x{1} 0x{2} 0x{3}", this.ToString(), data[0].ToString("X2"), data[1].ToString("X2"), data[2].ToString("X2")));
#endif
                }
            }
            return result;
        }

        public abstract void StartIdentify();

        public abstract void StopIdentify();
    }
}