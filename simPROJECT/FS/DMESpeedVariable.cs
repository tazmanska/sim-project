using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class DMESpeedVariable : FSVariable
    {
        public DMESpeedVariable(int offset)
        {
            Offset = offset;
            ValueSize = 2;
        }

        public override string GetStringValue()
        {
            if (Value2 < 0)
            {
                return "--- S";
            }
            return ((double)Value2 / 10d).ToString("0000") + " S";
        }
    }
}
