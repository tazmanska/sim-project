using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using simPROJECT.UI;

namespace simPROJECT
{
    /// <summary>
    /// Główna klasa aplikacji.
    /// </summary>
    class AppService : IDisposable
    {
        public AppService()
        {
            Running = false;
        }

        public void Run()
        {
            if (Running)
            {
                return;
            }

            _deviceService = new DevicesService();
            Globals.Instance.NewDeviceEvent += new EventHandler<DeviceChangeEventArgs>(_deviceService_NewDeviceEvent);
            Globals.Instance.RemoveDeviceEvent += new EventHandler<DeviceChangeEventArgs>(_deviceService_RemoveDeviceEvent);

            // utworzenie menu podstawowego
            _menu = new ContextMenu();
            _menu.Popup += new EventHandler(_menu_Popup);

            _informationMenuItem = new MenuItem(Properties.Resources.Info, delegate(object sender, EventArgs e)
            {
                // pokazanie okna z informacjami o programie
                ShowInformationDialog();
            });

            _optionsMenuItem = new MenuItem(Properties.Resources.Options, delegate(object sender, EventArgs e)
            {
                // pokazanie okna z opcjami

            });

            _exitMenuItem = new MenuItem(Properties.Resources.CloseApplication, delegate(object sender, EventArgs e)
            {
                Exit();
                Application.Exit();
            });

            _testModeMenuItem = new MenuItem("TEST mode", delegate(object sender, EventArgs e)
                {
                    Globals.Instance.ChangePlane(simPROJECT.Planes.PlaneType.Test);
                });
            _testModeMenuItem.RadioCheck = true;

            _devicesLabelMenuItem = new MenuItem(" *** Devices ***");
            _devicesLabelMenuItem.Enabled = false;

            _planesLabelMenuItem = new MenuItem(" *** Planes ***");
            _planesLabelMenuItem.Enabled = false;

            _tray = new AppTrayIcon(_container, _menu);
            _tray.Visible = true;

            // wystartowanie obsługi urządzeń

            _deviceService.Start();

            Running = true;
        }

        private void _deviceService_NewDeviceEvent(object sender, DeviceChangeEventArgs e)
        {
            // wykrycie nowego urządzenia
            //if (_globals.IsNewDevice(e.Device.SerialNumber))
            {
                // pokazanie informacji o wykryciu nowego urządzenia
                _tray.ShowInfo(string.Format(Properties.Resources.NewDeviceDetected, e.Device.Name));
            }
        }

        private void _deviceService_RemoveDeviceEvent(object sender, DeviceChangeEventArgs e)
        {
            // usunięcie urządzenia

            // pokazanie informacji o usunięciu urządzenia
            _tray.ShowInfo(string.Format("Device removed: {0}", e.Device.Name));
        }

        private MenuItem _informationMenuItem = null;
        private MenuItem _optionsMenuItem = null;
        private MenuItem _exitMenuItem = null;
        private MenuItem _testModeMenuItem = null;
        private MenuItem _devicesLabelMenuItem = null;
        private MenuItem _planesLabelMenuItem = null;

        private void _menu_Popup(object sender, EventArgs e)
        {
            _menu.MenuItems.Clear();

            _menu.MenuItems.Add(_informationMenuItem);
            _menu.MenuItems.Add("-");

            // lista urządzeń
            Device[] devices = Globals.Instance.Devices;
            if (devices != null && devices.Length > 0)
            {
                // etykieta "Devices"
                _menu.MenuItems.Add(_devicesLabelMenuItem);

                Array.Sort<Device>(devices, delegate(Device left, Device right)
                {
                    return left.Name.CompareTo(right.Name);
                });
                for (int i = 0; i < devices.Length; i++)
                {
                    MenuItem item = new MenuItem(devices[i].Name);
                    item.Tag = devices[i];
                    item.Click += new EventHandler(item_Click);
                    _menu.MenuItems.Add(item);
                }
            }
            else
            {
                MenuItem item = new MenuItem(Properties.Resources.SearchingDevices);
                item.Enabled = false;
                _menu.MenuItems.Add(item);
            }

#if DEBUG
            var fakeMcp = new MenuItem("Fake MCP");
            fakeMcp.Click += new EventHandler((ev, o) =>
            {
                var device = new Devices.Fake737McpDevice();
                Globals.Instance.AddDevice(device);
                device.SetPlane(Globals.Instance.CurrentPlane);
                device.TurnOn();
            });
            _menu.MenuItems.Add(fakeMcp);
#endif

            //_menu.MenuItems.Add("-");
            //_menu.MenuItems.Add(_optionsMenuItem);
            _menu.MenuItems.Add("-");

            // etykieta "Planes"
            _menu.MenuItems.Add(_planesLabelMenuItem);

            // test mode
            _menu.MenuItems.Add(_testModeMenuItem);
            _testModeMenuItem.Checked = Globals.Instance.CurrentPlane.Type == simPROJECT.Planes.PlaneType.Test;

            // lista samolotów
            Planes.PlaneType[] planes = Planes.PlaneFactory.GetAvailablePlaneTypes();
            for (int i = 0; i < planes.Length; i++)
            {
                MenuItem item = new MenuItem(Planes.PlaneFactory.PlaneTypeToName(planes[i]));
                item.Tag = planes[i];
                item.Click +=new EventHandler(plane_Click);
                item.RadioCheck = true;
                item.Checked = Globals.Instance.CurrentPlane.Type == planes[i];
                _menu.MenuItems.Add(item);
            }

            _menu.MenuItems.Add("-");
            _menu.MenuItems.Add(_exitMenuItem);
        }

        private void item_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem)
            {
                if (((MenuItem)sender).Tag is Device)
                {
                    ShowDeviceOptions((Device)((MenuItem)sender).Tag);
                }
            }
        }


        private void plane_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem)
            {
                if (((MenuItem)sender).Tag is Planes.PlaneType && !((MenuItem)sender).Checked)
                {
                    Globals.Instance.ChangePlane((simPROJECT.Planes.PlaneType)((MenuItem)sender).Tag);
                }
            }
        }

        private void ShowDeviceOptions(Device device)
        {
            if (!_deviceOptionDialogs.ContainsKey(device))
            {
                _deviceOptionDialogs.Add(device, device.CreateSettingsDialog());
            }
            PanelConfigurationDialogBase dialog = _deviceOptionDialogs[device];
            if (!dialog.IsVisible)
            {
                dialog.LoadSettings();
            }
            dialog.Show();
            dialog.BringToFront();
        }

        private Dictionary<Device, PanelConfigurationDialogBase> _deviceOptionDialogs = new Dictionary<Device, PanelConfigurationDialogBase>();

        private DevicesService _deviceService = null;

        private InformationDialog _informationDialog = null;

        private void ShowInformationDialog()
        {
            if (_informationDialog == null)
            {
                _informationDialog = new InformationDialog();
            }
            _informationDialog.Show();
            _informationDialog.BringToFront();
        }

        public bool Running
        {
            get;
            private set;
        }

        public void Exit()
        {
            if (!Running)
            {
                return;
            }

            if (_informationDialog != null)
            {
                try
                {
                    _informationDialog.Hide();
                    _informationDialog.ForceClose = true;
                    _informationDialog.Close();
                }
                catch { }
                try
                {
                    _informationDialog.Dispose();
                }
                catch { }
                _informationDialog = null;
            }

            // zamknięcie wszystkich dialogów konfiguracyjnych
            foreach (KeyValuePair<Device, PanelConfigurationDialogBase> kvp in _deviceOptionDialogs)
            {
                kvp.Value.ForceClose();
                kvp.Value.Dispose();
            }

            _deviceOptionDialogs.Clear();

            _tray.Visible = false;

            if (_deviceService != null)
            {
                _deviceService.Stop();
            }

            Running = false;

            //Properties.Settings.Default.Save();

            Globals.Instance.SaveConfiguration();
        }

        private AppTrayIcon _tray = null;
        private IContainer _container = new Container();

        private ContextMenu _menu = null;

        #region IDisposable Members

        public void Dispose()
        {
            Exit();

            if (_deviceService != null)
            {
                _deviceService.Dispose();
                _deviceService = null;
            }
        }

        #endregion
    }
}
