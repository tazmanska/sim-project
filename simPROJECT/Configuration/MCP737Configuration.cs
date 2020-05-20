using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace simPROJECT.Configuration
{
    [XmlRoot(ElementName = "MCP737")]
    public class MCP737Configuration : DeviceConfiguration
    {
        public MCP737Configuration()
        {
            DisplayBrightness = 5;
            LEDBrightness = 5;
            BacklightBrightness = 3;
            WelcomeAnimation = true;
        }

        public byte DisplayBrightness
        {
            get;
            set;
        }

        public byte LEDBrightness
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

        public string PMDGKeysFile
        {
            get;
            set;
        }

        public string ProsimConfigFile
        {
            get;
            set;
        }
    }
}
