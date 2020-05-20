using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FTD2XX_NET;
using System.Diagnostics;

namespace simPROJECT
{
    class DevicesService : IDisposable
    {
        public DevicesService()
        {
        }

        private Thread _thread = null;

        public void Start()
        {
            Stop();

            // wystartowanie wątka
            _stop = false;
            _thread = new Thread(WorkingThread);
            _thread.Start();
        }

        private void WorkingThread()
        {
            try
            {
                // opóźnienie startu
                Thread.Sleep(3000);

                // skasowanie listy urządzeń
                Globals.Instance.ClearDevices();

                List<string> serialNumbers = new List<string>();

                FTDI.FT_STATUS result = FTDI.FT_STATUS.FT_OK;
                FTDI ftdi = new FTDI();

                while (!_stop)
                {
                    // łączenie z FSUIPC
                    if (!FS.FSUIPC.FS.IsConnected)
                    {
                        FS.FSUIPC.FS.Connect();
                    }

                    uint devCount = 0;
                    try
                    {
                        result = ftdi.GetNumberOfDevices(ref devCount);
                    }
                    catch { }
                    if (result == FTDI.FT_STATUS.FT_OK)
                    {
                        FTDI.FT_DEVICE_INFO_NODE[] deviceList = new FTDI.FT_DEVICE_INFO_NODE[devCount];
                        if (devCount > 0)
                        {
                            try
                            {
                                result = ftdi.GetDeviceList(deviceList);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            result = FTDI.FT_STATUS.FT_OK;
                        }
                        if (result == FTDI.FT_STATUS.FT_OK)
                        {
                            // usunięcie urządzeń niepodpiętych
                            Device[] _devices = Globals.Instance.Devices;
                            {
                                int index = _devices.Length;
                                while (index-- > 0)
                                {
                                    Device device = _devices[index];
                                    if (device.SerialNumber != "FAKE" && Array.Find<FTDI.FT_DEVICE_INFO_NODE>(deviceList, delegate(FTDI.FT_DEVICE_INFO_NODE o)
                                    {
                                        return o.SerialNumber == device.SerialNumber;
                                    }) == null)
                                    {
                                        
                                        Globals.Instance.RemoveDevice(_devices[index]);
                                        serialNumbers.Remove(device.SerialNumber);

                                        device.TurnOff();

                                        
                                    }
                                }
                            }

                            for (int i = 0; i < deviceList.Length; i++)
                            {
                                if (deviceList[i].Type == FTDI.FT_DEVICE.FT_DEVICE_232R)
                                {
                                    string sn = deviceList[i].SerialNumber;
                                    if (!serialNumbers.Contains(sn))
                                    {
                                        serialNumbers.Add(sn);

                                        // otwarcie połączenia z urządzeniem
                                        FTD2XX_NET.FTDI driver = new FTDI();
                                        FTD2XX_NET.FTDI.FT_STATUS status = driver.OpenBySerialNumber(sn);
                                        if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                        {
                                            driver.Close();
                                            driver = null;
                                            continue;
                                        }

                                        // odczytanie EEPROM
                                        FTD2XX_NET.FTDI.FT232R_EEPROM_STRUCTURE eeprom = new FTD2XX_NET.FTDI.FT232R_EEPROM_STRUCTURE();
                                        status = driver.ReadFT232REEPROM(eeprom);
                                        if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                        {
                                            driver.Close();
                                            driver = null;
                                            continue;
                                        }

                                        // odczytanie producenta "simPROJECT"
                                        if (eeprom.Manufacturer.ToUpperInvariant() != "SIMPROJECT")
                                        {
                                            driver.Close();
                                            driver = null;
                                            continue;
                                        }

                                        driver.Close();
                                        driver = null;

                                        Device device = DeviceFactory.CreateDevice(deviceList[i]);
                                        if (device != null)
                                        {                                            
                                            Globals.Instance.AddDevice(device);
                                            device.SetPlane(Globals.Instance.CurrentPlane);
                                            device.TurnOn();                                            
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Thread.Sleep(3000);
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Błąd w wątku śledzącym urządzenia: {0}", ex));
            }
            finally
            {

            }
        }

        private volatile bool _stop = false;

        public void Stop()
        {
            _stop = true;
            if (_thread != null)
            {
                try
                {
                    _thread.Join(50);
                }
                catch { }
                try
                {
                    _thread.Abort();
                }
                catch { }
                _thread = null;
            }

            Device[] devices = Globals.Instance.Devices;
            for (int i = 0; i < devices.Length; i++)
            {
                devices[i].TurnOff();
                Globals.Instance.RemoveDevice(devices[i]);
            }

            // zamknięcie połączenia z FS
            FS.FSUIPC.FS.Disconnect();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}