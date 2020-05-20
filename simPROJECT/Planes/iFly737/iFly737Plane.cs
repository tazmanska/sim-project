using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.iFly737
{
    class iFly737Plane : BasePlane
    {
        public iFly737Plane()
            : base(PlaneType.iFly737)
        {
            ResetDevice();
            ResetUpdate();
        }

        public override IMCP GetMCP()
        {
            return new iFly737MCP();
        }
    }
}
