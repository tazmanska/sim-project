using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.PMDG737NGX
{
    sealed class Plane : BasePlane
    {
        public Plane()
            : base(PlaneType.PMDG737NGX)
        {
            ResetDevice();
            ResetUpdate();
        }

        public override IMCP GetMCP()
        {
            return new MCP();
        }
    }
}
