using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class XPDRVariable : FSVariable
    {
        public XPDRVariable()
        {
            Offset = 0x0354;
            ValueSize = 2;
        }

        public override string GetStringValue()
        {
            return " " + Value2.ToString("X4");
        }
    }
}
