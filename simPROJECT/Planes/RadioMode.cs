using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    enum RadioMode
    {
        None = 0x00,
        DME = 0x01,
        COM1 = 0x02,
        COM2 = 0x04,
        NAV1 = 0x08,
        NAV2 = 0x10,
        ADF1 = 0x20,
        ADF2 = 0x40,
        XPDR = 0x80,
        QNH = 0x0100,
    }
}
