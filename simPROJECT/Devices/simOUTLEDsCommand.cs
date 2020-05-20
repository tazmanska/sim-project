using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Devices
{
    enum simOUTLEDsCommand : byte
    {
        LED_ON = 0x01,
        LED_OFF = 0x02,
        ALL_ON = 0x03,
        ALL_OFF = 0x04,
        LEDS = 0x05,
        BRIGHTNESS = 0x06,
        BRIGHTNESS2 = 0x07,
    }
}
