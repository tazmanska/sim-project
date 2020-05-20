using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.TestPlane
{
    class TestPlane : BasePlane
    {
        public TestPlane()
            : base(PlaneType.Test)
        {
            ResetDevice();
            ResetUpdate();
        }

        public override IRadio GetRadio()
        {
            return new TestRadio();
        }

        public override IMCP GetMCP()
        {
            return new TestMCP();
        }
    }
}
