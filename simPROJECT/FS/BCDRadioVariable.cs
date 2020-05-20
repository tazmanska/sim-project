using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class BCDRadioVariable : FSVariable
    {
        public BCDRadioVariable(int offset)
        {
            Offset = offset;
            ValueSize = 2;
        }

        public override string GetStringValue()
        {
            string t = Value2.ToString("X4");
            t = "1" + t.Substring(0, 2) + "." + t.Substring(2, 2);
            return t;
        }

        public void COMFractInc()
        {
            Read();
            Value2 = COMFractInc(Value2);
            Write();
        }

        public void COMFractDec()
        {
            Read();
            Value2 = COMFractDec(Value2);
            Write();
        }

        public void COMWholeInc()
        {
            Read();
            Value2 = COMWholeInc(Value2);
            Write();
        }

        public void COMWholeDec()
        {
            Read();
            Value2 = COMWholeDec(Value2);
            Write();
        }

        public static short COMFractInc(short v)
        {
            byte v1 = (byte)((v & 0x00f0) >> 4);
            byte v0 = (byte)(v & 0x000f);

            int val = 100 * v1 + 10 * v0;
            if ((v0 == 2) || (v0 == 7))
            {
                val += 5;
            }

            val += 25;

            if (val > 975)
            {
                val -= 1000;
            }

            val /= 10;
            v0 = (byte)(val % 10);
            val /= 10;
            v1 = (byte)(val);

            return (short)((v & 0xff00) | (v1 << 4) | v0);
        }

        public static short COMFractDec(short v)
        {
            byte v1 = (byte)((v & 0x00f0) >> 4);
            byte v0 = (byte)(v & 0x000f);

            int val = 100 * v1 + 10 * v0;
            if ((v0 == 2) || (v0 == 7))
            {
                val += 5;
            }

            val -= 25;

            if (val < 0)
            {
                val += 1000;
            }

            val /= 10;
            v0 = (byte)(val % 10);
            val /= 10;
            v1 = (byte)(val);

            return (short)((v & 0xff00) | (v1 << 4) | v0);
        }

        public static short COMWholeInc(short v)
        {
            byte v1 = (byte)((v & 0xf000) >> 12);
            byte v0 = (byte)((v & 0x0f00) >> 8);

            int val = 10 * v1 + v0;

            val++;

            if (val > 36)
            {
                val = 18;
            }

            v0 = (byte)(val % 10);
            v1 = (byte)(val / 10);

            return (short)((v1 << 12) | (v0 << 8) | (v & 0x00ff));
        }

        public static short COMWholeDec(short v)
        {
            byte v1 = (byte)((v & 0xf000) >> 12);
            byte v0 = (byte)((v & 0x0f00) >> 8);

            int val = 10 * v1 + v0;

            val--;

            if (val < 18)
            {
                val = 36;
            }

            v0 = (byte)(val % 10);
            v1 = (byte)(val / 10);

            return (short)((v1 << 12) | (v0 << 8) | (v & 0x00ff));
        }
    }
}
