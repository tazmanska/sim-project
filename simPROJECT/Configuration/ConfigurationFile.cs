using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace simPROJECT.Configuration
{
    public class ConfigurationFile
    {
        private ConfigurationFile()
        {
            Devices = new List<DeviceConfiguration>();
        }

        [
                XmlArrayItem(Type = typeof(MultiradioV1Configuration))
            ,   XmlArrayItem(Type = typeof(MCP737Configuration))
        ]
        public List<DeviceConfiguration> Devices
        {
            get;
            set;
        }

        internal void Save(string file)
        {
            try
            {
                if (File.Exists(file)) File.Delete(file);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationFile));
                using (StreamWriter writer = new StreamWriter(file))
                {
                    xs.Serialize(writer, this);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch
            { }
        }

        internal static ConfigurationFile Load(string file)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationFile));
                using (StreamReader reader = new StreamReader(file))
                {
                    ConfigurationFile c = (ConfigurationFile)xs.Deserialize(reader);
                    reader.Close();
                    return c;
                }
            }
            catch
            {
                return new ConfigurationFile();
            }
        }
    }
}
