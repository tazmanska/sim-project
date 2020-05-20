using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.FSDefault
{
    class FSDefaultPlane : BasePlane
    {
        public FSDefaultPlane()
            : base(PlaneType.FSDefault)
        {
            ResetDevice();
            ResetUpdate();
        }

        public override IMCP GetMCP()
        {
            return new FSDefaultMCP();
        }
    }
}
