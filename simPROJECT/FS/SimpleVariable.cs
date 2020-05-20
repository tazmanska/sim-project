using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class SimpleVariable : FSVariable
    {
        public SimpleVariable(int offset)
        {
            Offset = offset;
        }

        public SimpleVariable(int offset, bool forWriting, byte valueSize)
        {
            Offset = offset;
            ForWriting = forWriting;
            ValueSize = valueSize;
        }

        public override string GetStringValue()
        {
            switch (ValueSize)
            {
                case 1:
                    return Value1.ToString();

                case 2:
                    return Value2.ToString();

                case 4:
                    return Value4.ToString();

                case 8:
                    return Value8.ToString();

                default:
                    return "";
            }
        }
    }
}
