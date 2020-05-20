using System;
using System.Collections.Generic;
using System.Text;
using FTD2XX_NET;
using simPROJECT.Planes;

namespace simPROJECT.Devices
{
    class Fake737McpDevice : Device
    {
        public Fake737McpDevice()
            : base(new FTDI.FT_DEVICE_INFO_NODE() { SerialNumber = "FAKE" }, "Fake 737 MCP", DeviceType.MCP737)
        {
            
        }

        private IMCP _mcp = null;
        private volatile bool _planeChanged = false;

        internal override void SetPlane(simPROJECT.Planes.IPlane plane)
        {
            if (_mcp != null)
            {
                _mcp.Close();
            }
            _mcp = plane.GetMCP();
            _mcp.DeviceConfiguration = Configuration;
            _mcp.Startup();
            _planeChanged = true;
        }

        private Fake737McpForm _form = null;

        protected override void WorkingThread()
        {
            _form = new Fake737McpForm();

            _form.Show();

            while (!NeedStop)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        protected override void Disable()
        {
            if (_form != null)
            {
                _form._Close();
                _form = null;
            }
        }

        public override void StartIdentify()
        {
        }

        public override void StopIdentify()
        {
        }
    }
}
