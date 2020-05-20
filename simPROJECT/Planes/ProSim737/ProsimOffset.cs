using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Threading;

namespace simPROJECT.Planes.ProSim737
{
    class ProsimOffset
    {
        public static ProsimOffset Load(XmlNode node)
        {
            try
            {
                ProsimOffset result = null;
                node = node.SelectSingleNode("fsuipc");
                if (node != null)
                {
                    string serial = node.Attributes["serial"].Value;
                    int port = int.Parse(node.Attributes["port"].Value);                    
                    byte type = (byte)(port >> 16 & 3);
                    if (type == 2)
                    {
                        int off = port & 0xffff;
                        int val = port >> 18;
                        FS.SimpleVariable offset = new simPROJECT.FS.SimpleVariable(off);
                        FS.SimpleVariable offsetReset = new simPROJECT.FS.SimpleVariable(off);
                        offset.ForWriting = true;
                        offsetReset.ForWriting = true;
                        switch (serial)
                        {
                            case "8 bit U":
                                offset.ValueSize = 1;
                                offsetReset.ValueSize = 1;
                                offset.Value1 = (byte)val;
                                break;

                            case "16 bit U":
                            case "16 bit S":
                                offset.ValueSize = 2;
                                offsetReset.ValueSize = 2;
                                offset.Value2 = (short)val;
                                break;

                            case "32 bit U":
                            case "32 bit S":
                                offset.ValueSize = 4;
                                offsetReset.ValueSize = 4;
                                offset.Value4 = val;
                                break;

                            default:
                                return null;
                        }
                        result = new ProsimOffset();
                        result._offset = offset;
                        result._offsetReset = offsetReset;                        
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        private FS.SimpleVariable _offset = null;
        private FS.SimpleVariable _offsetReset = null;

        private void Reset()
        {
            try
            {
                Thread.Sleep(150);
                FS.FSUIPC.FS.Process(_offsetReset);
            }
            catch { }
        }

        public void Set()
        {
            Set(false);
        }

        public void Set(bool reset)
        {
            if (_offset != null)
            {
                FS.FSUIPC.FS.Process(_offset);

                if (reset)
                {
                    new Thread(Reset).Start();
                }
            }
        }
    }
}
