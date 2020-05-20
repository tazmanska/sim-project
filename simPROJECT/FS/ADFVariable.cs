using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class ADFVariable : FSVariable
    {
        private class _ADFVariable : FSVariable
        {
            public _ADFVariable(int offset)
            {
                Offset = offset;
                ValueSize = 2;
            }

            public override string GetStringValue()
            {
                string t = Value2.ToString("X4");
                return t.Substring(1, 1) + t.Substring(3, 1);
            }
        }

        public ADFVariable(int firstOffset, int secondOffset)
        {
            Offset = firstOffset;
            ValueSize = 2;
            _variable = new _ADFVariable(secondOffset);
        }

        private _ADFVariable _variable = null;

        public FSVariable SecondVariable
        {
            get { return _variable; }
        }

        public override string GetStringValue()
        {
            string t = Value2.ToString("X4").Substring(1, 3);
            string t2 = _variable.GetStringValue();
            return t2.Substring(0, 1) + t + "." + t2.Substring(1, 1);
        }
    }
}
