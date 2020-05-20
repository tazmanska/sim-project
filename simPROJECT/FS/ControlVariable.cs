using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.FS
{
    class ControlVariable : FSVariable
    {
        public static long CMD_TFR_COM1 = 66372;
        public static long CMD_TFR_COM2 = 66444;
        public static long CMD_TFR_NAV1 = 66448;
        public static long CMD_TFR_NAV2 = 66452;

        public static long CMD_COM1_WHOLE_INC = 65637;
        public static long CMD_COM1_WHOLE_DEC = 65636;
        public static long CMD_COM1_FRAC_INC = 65639;
        public static long CMD_COM1_FRAC_DEC = 65638;

        public static long CMD_COM2_WHOLE_INC = 66437;
        public static long CMD_COM2_WHOLE_DEC = 66436;
        public static long CMD_COM2_FRAC_INC = 66440;
        public static long CMD_COM2_FRAC_DEC = 66438;

        public static long CMD_NAV1_WHOLE_INC = 65641;
        public static long CMD_NAV1_WHOLE_DEC = 65640;
        public static long CMD_NAV1_FRAC_INC = 65643;
        public static long CMD_NAV1_FRAC_DEC = 65642;

        public static long CMD_NAV2_WHOLE_INC = 65645;
        public static long CMD_NAV2_WHOLE_DEC = 65644;
        public static long CMD_NAV2_FRAC_INC = 65647;
        public static long CMD_NAV2_FRAC_DEC = 65646;

        public static long CMD_ADF1_FRAC_DEC = 66453;
        public static long CMD_ADF1_FRAC_INC = 66454;
        public static long CMD_ADF1_100_DEC = 65666;
        public static long CMD_ADF1_100_INC = 65648;
        public static long CMD_ADF1_10_DEC = 65667;
        public static long CMD_ADF1_10_INC = 65649;
        public static long CMD_ADF1_1_DEC = 65668;
        public static long CMD_ADF1_1_INC = 65650;

        public static long CMD_ADF2_FRAC_DEC = 66551;
        public static long CMD_ADF2_FRAC_INC = 66547;
        public static long CMD_ADF2_100_DEC = 66548;
        public static long CMD_ADF2_100_INC = 66544;
        public static long CMD_ADF2_10_DEC = 66549;
        public static long CMD_ADF2_10_INC = 66545;
        public static long CMD_ADF2_1_DEC = 66550;
        public static long CMD_ADF2_1_INC = 66546;

        public static long CMD_XPDR_1000_DEC = 66455;
        public static long CMD_XPDR_1000_INC = 65651;
        public static long CMD_XPDR_100_DEC = 66456;
        public static long CMD_XPDR_100_INC = 65652;
        public static long CMD_XPDR_10_DEC = 66457;
        public static long CMD_XPDR_10_INC = 65653;
        public static long CMD_XPDR_1_DEC = 66458;
        public static long CMD_XPDR_1_INC = 65654;

        public static long CMD_QNH_DEC = 65884;
        public static long CMD_QNH_INC = 65883;

        public static long AP_APR_HOLD_OFF = 65814;
        public static long AP_APR_HOLD_ON = 65806;

        public ControlVariable()
        {
            Offset = 0x3110;
            ValueSize = 8;
            ForWriting = true;
        }

        public override string GetStringValue()
        {
            throw new NotImplementedException();
        }
    }
}
