using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    abstract class FSVariable
    {
        public int Offset = 0;
        public int Token = 0;

        public byte Value1 = 0;
        public short Value2 = 0;
        public int Value4 = 0;
        public long Value8 = 0;

        public byte OldValue1 = 0;
        public short OldValue2 = 0;
        public int OldValue4 = 0;
        public long OldValue8 = 0;

        public byte ValueSize = 0;

        public bool IsArray = false;
        public bool IsString = false;

        public bool ForWriting = false;

        public abstract string GetStringValue();

        public virtual void Reset()
        {
            Value1 = 0;
            Value2 = 0;
            Value4 = 0;
            Value8 = 0;
        }

        public virtual void Processed()
        {
            OldValue1 = Value1;
            OldValue2 = Value2;
            OldValue4 = Value4;
            OldValue8 = Value8;
        }

        public virtual bool HasChanged()
        {
            switch (ValueSize)
            {
                case 1:
                    return OldValue1 != Value1;

                case 2:
                    return OldValue2 != Value2;

                case 4:
                    return OldValue4 != Value4;

                case 8:
                    return OldValue8 != Value8;

                default:
                    throw new Exception();
            }
        }

        public virtual bool Read()
        {
            return FSUIPC.FS.Read(this);
        }

        public virtual bool Write()
        {
            return FSUIPC.FS.Write(this);
        }
    }
}
