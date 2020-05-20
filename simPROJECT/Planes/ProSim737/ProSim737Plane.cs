using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.ProSim737
{
    class ProSim737Plane : BasePlane
    {
        public ProSim737Plane()
            : base(PlaneType.ProSim737)
        {
            ResetDevice();
            ResetUpdate();
        }

        public override IMCP GetMCP()
        {
            return new ProSim737MCP();
        }
    }
}
