using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    interface IPlaneDevice
    {
        void Startup();

        void Close();

        void ResetDevice();

        void ResetUpdate();

        bool NeedUpdate
        {
            get;
        }

        bool IsTestMode
        {
            get;
        }

        bool HasData
        {
            get;
        }

        string Error
        {
            get;
        }

        simPROJECT.Configuration.DeviceConfiguration DeviceConfiguration
        {
            get;
            set;
        }
    }
}
