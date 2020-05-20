using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.PMDG
{
    sealed class PMDGPlane : BasePlane
    {
        public PMDGPlane()
            : base(PlaneType.PMDG737)
        {
            ResetDevice();
            ResetUpdate();
        }

        public override IMCP GetMCP()
        {
            return new PMDG737MCP();
        }
    }
}
