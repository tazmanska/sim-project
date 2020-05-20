using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    interface IPlane : IPlaneDevice
    {
        string Name
        {
            get;
        }

        PlaneType Type
        {
            get;
        }

        IRadio GetRadio();

        IMCP GetMCP();
    }
}
