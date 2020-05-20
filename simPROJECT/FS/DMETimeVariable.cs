using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class DMETimeVariable : FSVariable
    {
        public DMETimeVariable(int offset)
        {
            Offset = offset;
            ValueSize = 2;
        }

        public override string GetStringValue()
        {
            if (Value2 < 0 || Value2 == 9999)
            {
                return "--- t";
            }

            return ((((double)Value2 / 10d) / 60d).ToString("0000.0")) + " t";
        }
    }
}
