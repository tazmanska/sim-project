using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Devices
{
    enum simINKeysCommand : byte
    {
        GET_KEYS = 0x01,
        START_SCAN = 0x02,
        STOP_SCAN = 0x03,
        SET_ENCODER = 0x05,
        CLEAR_ENCODERS = 0x06
    }
}
