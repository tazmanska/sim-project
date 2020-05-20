using System;
using System.Collections.Generic;
using System.Text;
using simPROJECT.Configuration;
using System.IO;
using simPROJECT.Devices;
using simPROJECT.Planes;
using System.Diagnostics;

namespace simPROJECT
{
    public class DeviceChangeEventArgs : EventArgs
    {
        public DeviceChangeEventArgs(Device device, bool remove)
        {
            Device = device;
            Remove = remove;
        }

        public Device Device
        {
            get;
            private set;
        }

        public bool Remove
        {
            get;
            private set;
        }
    }

    public class Globals
    {
        private static readonly Globals __instance = new Globals();

        public static Globals Instance
        {
            get { return __instance; }
        }

        private Globals()
        {
            CurrentPlane = PlaneFactory.CreatePlane(PlaneType.Test);
        }

        public void ChangePlane(PlaneType planeType)
        {
            Debug.WriteLine(string.Format("Zmieniam samolot na: {0}", planeType));
            IPlane plane = PlaneFactory.CreatePlane(planeType);
            plane.Startup();
            Device[] devices = Devices;
            for (int i = 0; i < devices.Length; i++)
            {
                devices[i].SetPlane(plane);
            }
            CurrentPlane.Close();
            CurrentPlane = plane;
        }

        public event EventHandler<DeviceChangeEventArgs> NewDeviceEvent;
        public event EventHandler<DeviceChangeEventArgs> RemoveDeviceEvent;

        public void AddDevice(Device device)
        {
            Debug.WriteLine(string.Format("Dodaję urządzenie: {0}", device.ToString()));
            lock (_devicesLock)
            {
                _devices.Add(device);
            }
            if (NewDeviceEvent != null)
            {
                NewDeviceEvent(this, new DeviceChangeEventArgs(device, false));
            }
        }

        public void RemoveDevice(Device device)
        {
            Debug.WriteLine(string.Format("Usuwam urządzenie: {0}", device.ToString()));
            lock (_devicesLock)
            {
                _devices.Remove(device);
            }
            if (RemoveDeviceEvent != null)
            {
                RemoveDeviceEvent(this, new DeviceChangeEventArgs(device, true));
            }
        }

        public void ClearDevices()
        {

        }

        private object _devicesLock = new object();

        private List<Device> _devices = new List<Device>();

        public Device[] Devices
        {
            get
            {
                lock (_devicesLock)
                {
                    return _devices.ToArray();
                }
            }
        }

        private static string __configurationFilePath = "";

        static Globals()
        {
            string applicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            applicationData = Path.Combine(applicationData, Program.APP_NAME);

            // utworzenie katalogu 
            if (!Directory.Exists(applicationData))
            {
                Directory.CreateDirectory(applicationData);
            }

            __configurationFilePath = Path.Combine(applicationData, Program.APP_NAME + ".xml");
        }

        /// <summary>
        /// Metoda zwraca informację czy wskazane urządzenie jest widziane po raz pierwszy.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsNewDevice(string key)
        {
            return Configuration.Devices.Find(delegate(DeviceConfiguration o)
            {
                return o.SerialNumber == key;
            }) == null;
        }

        public DeviceConfiguration GetConfiguration(string key, DeviceType deviceType)
        {
            // poszukanie na liście konfiguracji urządzenia o wskazanym SN
            DeviceConfiguration result = Configuration.Devices.Find(delegate(DeviceConfiguration o)
            {
                return o.SerialNumber == key;
            });
            if (result == null)
            {
                result = DeviceConfiguration.CreateDefaultConfigurationForDeviceType(key, deviceType);
                Configuration.Devices.Add(result);
            }
            return result;
        }

        private ConfigurationFile _configuration = null;

        private ConfigurationFile Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = ConfigurationFile.Load(__configurationFilePath);
                }
                return _configuration;
            }
        }

        public void SaveConfiguration()
        {
            Configuration.Save(__configurationFilePath);
        }

        internal IPlane CurrentPlane
        {
            get;
            set;
        }
    }
}
