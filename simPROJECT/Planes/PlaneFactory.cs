using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    static class PlaneFactory
    {
        public static IPlane CreatePlane(PlaneType type)
        {
            switch (type)
            {
                case PlaneType.Test:
                    return new TestPlane.TestPlane();

                case PlaneType.FSDefault:
                    return new FSDefault.FSDefaultPlane();

                case PlaneType.PMDG737:
                    return new PMDG.PMDGPlane();

                case PlaneType.iFly737:
                    return new iFly737.iFly737Plane();

                case PlaneType.ProSim737:
                    return new ProSim737.ProSim737Plane();

                case PlaneType.PMDG737NGX:
                    return new PMDG737NGX.Plane();

                default:
                    return new DummyPlane.DummyPlane();
            }
        }

        public static PlaneType[] GetAvailablePlaneTypes()
        {
            return new PlaneType[] { PlaneType.FSDefault, PlaneType.PMDG737, PlaneType.iFly737, PlaneType.ProSim737, PlaneType.PMDG737NGX,  };
        }

        public static string PlaneTypeToName(PlaneType type)
        {
            switch (type)
            {
                case PlaneType.None:
                    return "dummy plane";

                case PlaneType.Test:
                    return "TEST mode";

                case PlaneType.FSDefault:
                    return "FS Default";

                case PlaneType.PMDG737:
                    return "PMDG B737";

                case PlaneType.iFly737:
                    return "iFly B737";

                case PlaneType.ProSim737:
                    return "ProSim737";

                case PlaneType.PMDG737NGX:
                    return "PMDG 737NGX";

                default:
                    return "unknown";
            }
        }
    }
}
