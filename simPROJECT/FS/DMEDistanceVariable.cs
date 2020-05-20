using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class DMEDistanceVariable : FSVariable
    {
        public DMEDistanceVariable(int offset)
        {
            Offset = offset;
            ValueSize = 2;
        }

        public override string GetStringValue()
        {
            if (Value2 <= 0)
            {
                return "--- d";
            }
            return ((double)Value2 / 10d).ToString("00000.0") + " d";
        }
    }
}
