using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace simPROJECT.Configuration
{
    [XmlRoot(ElementName = "Multiradio")]
    public class MultiradioV1Configuration : DeviceConfiguration
    {
        public MultiradioV1Configuration()
        {
            DisplayBrightness = 5;
            BacklightBrightness = 3;
            WelcomeAnimation = true;
        }

        public byte DisplayBrightness
        {
            get;
            set;
        }

        public byte BacklightBrightness
        {
            get;
            set;
        }

        public bool WelcomeAnimation
        {
            get;
            set;
        }
    }
}
