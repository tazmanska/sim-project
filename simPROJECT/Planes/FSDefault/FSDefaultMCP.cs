using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.FSDefault
{
    class FSDefaultMCP : BaseMCP
    {
        public FSDefaultMCP()
        {
            _fsVariables = new simPROJECT.FS.FSVariable[] 
            {
                  AP_MASTER_SW
                , AP_NAV1
                , AP_HDG_SEL
                , AP_ALT
                , AP_IAS
                , AP_MACH
                , APD_VS
                , AP_AT
                , AP_APP
                , FS_MACH
                , FS_IAS
                , NAV_GPS
                , AP_FD2
            };
        }

        private FS.FSVariable[] _fsVariables = null;

        private int CONTADOR = 0;
        //&CHECK = TIMER 999,0,25
        private int _CMD = 1;
        private int T_IAS = 140;
        private int T_MACH = 0;
        //private int AP_IAS = 1; jest offset
        //&AP_IAS = DELAY 0,10

        private System.Threading.Timer _timer = null;

        private FS.SimpleVariable AP_MASTER_SW = new simPROJECT.FS.SimpleVariable(0x07BC)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_MASTER_SW2 = new simPROJECT.FS.SimpleVariable(0x07BC)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_NAV1 = new simPROJECT.FS.SimpleVariable(0x07C4)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_HDG_SEL = new simPROJECT.FS.SimpleVariable(0x07C8)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_HDG_SEL2 = new simPROJECT.FS.SimpleVariable(0x07C8)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_ALT = new simPROJECT.FS.SimpleVariable(0x07d0)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_ALT2 = new simPROJECT.FS.SimpleVariable(0x07d0)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_IAS = new simPROJECT.FS.SimpleVariable(0x07DC)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_IAS2 = new simPROJECT.FS.SimpleVariable(0x07DC)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_MACH = new simPROJECT.FS.SimpleVariable(0x07E4)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_MACH2 = new simPROJECT.FS.SimpleVariable(0x07E4)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable APD_MACH = new simPROJECT.FS.SimpleVariable(0x07E8)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable APD_VS = new simPROJECT.FS.SimpleVariable(0x07F2)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable APD_VS2 = new simPROJECT.FS.SimpleVariable(0x07F2)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable AP_AT = new simPROJECT.FS.SimpleVariable(0x0810)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_AT2 = new simPROJECT.FS.SimpleVariable(0x0810)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_APP = new simPROJECT.FS.SimpleVariable(0x0800)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_APP2 = new simPROJECT.FS.SimpleVariable(0x07FC)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_APP22 = new simPROJECT.FS.SimpleVariable(0x0800)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable APD_COURSE1 = new simPROJECT.FS.SimpleVariable(0x0C4E)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable APD_HDG = new simPROJECT.FS.SimpleVariable(0x07CC)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable APD_ALT = new simPROJECT.FS.SimpleVariable(0x07D4)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable FS_MACH = new simPROJECT.FS.SimpleVariable(0x11C6)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable FS_IAS = new simPROJECT.FS.SimpleVariable(0x02BC)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_FD = new simPROJECT.FS.SimpleVariable(0x2EE0)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_FD2 = new simPROJECT.FS.SimpleVariable(0x2EE0)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable AP_NAV = new simPROJECT.FS.SimpleVariable(0x07C4)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable APD_IAS = new simPROJECT.FS.SimpleVariable(0x07E2)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable AP_N1 = new simPROJECT.FS.SimpleVariable(0x080c)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.SimpleVariable NAV_GPS = new simPROJECT.FS.SimpleVariable(0x132c)
        {
            ForWriting = false,
            ValueSize = 4
        };

        private FS.SimpleVariable NAV_GPS2 = new simPROJECT.FS.SimpleVariable(0x132c)
        {
            ForWriting = true,
            ValueSize = 4
        };

        private FS.ControlVariable _fsControl = new simPROJECT.FS.ControlVariable();

        public override void Startup()
        {
            base.Startup();

            CONTADOR = 0;
            //&CHECK = TIMER 999,0,25
            CMD = 1;
            D_COURSE1 = 0;
            D_HDG = 0;
            D_ALT = 10000;
            D_VS = 0;
            T_IAS = 100;
            F_IAS = T_IAS;
            AP_IAS.Value4 = 1;
            //&AP_IAS = DELAY 0,10

            APD_VS.Value2 = 0;

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _timer = new System.Threading.Timer(delegate(object o)
                {
                    CONTADOR++;
                    if (CONTADOR > 10000)
                    {
                        CONTADOR = 1000;
                    }
                    else
                    {
                        if (CONTADOR > 4)
                        {
                            if (APD_VS.Value2 != D_VS)
                            {
                                D_VS = APD_VS.Value2;
                                _VS = D_VS.ToString("0000");
                            }
                        }
                    }
                }, null, 0, 250);
        }

        public override void Close()
        {
            base.Close();

            if (_timer != null)
            {
                _timer.Change(0, System.Threading.Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }
        }

        public override void UpdateOutputs()
        {
            base.UpdateOutputs();

            // odczytanie wszystkich offsetów

            HasData = FS.FSUIPC.FS.Process(_fsVariables);
            if (!HasData)
            {
                Error = "no FS";
            }
            else
            {
                // przetworzenie danych

                _LEDCMDA = false;
                _LEDCMDB = false;
                if (CMD == 1)
                {
                    if (AP_MASTER_SW.Value4 == 1)
                    {
                        _LEDCMDA = true;
                    }
                }
                else
                {
                    if (AP_MASTER_SW.Value4 == 1)
                    {
                        _LEDCMDB = true;
                    }
                }

                if (AP_NAV1.Value4 == 1)
                {
                    _LEDVORLOC = NAV_GPS.Value4 == 0;
                    _LEDLNAV = NAV_GPS.Value4 == 1;
                }
                else
                {
                    _LEDLNAV = false;
                    _LEDVORLOC = false;
                }

                _LEDHDGSEL = AP_HDG_SEL.Value4 == 1;

                _LEDALTHLD = AP_ALT.Value4 == 1;

                if (AP_IAS.Value4 == 1)
                {
                    SPEED = 1;
                    _LEDSPEED = true;
                    F_IAS = T_IAS;
                }
                else
                {
                    if (AP_MACH.Value4 == 0)
                    {
                        _LEDSPEED = false;
                    }
                }

                if (AP_MACH.Value4 == 1)
                {
                    SPEED = 2;
                    _LEDSPEED = true;
                }
                else
                {
                    if (AP_IAS.Value4 == 0)
                    {
                        _LEDSPEED = false;
                    }
                }

                if (AP_AT.Value4 == 1)
                {
                    _LEDAT = true;
                }
                else
                {
                    _LEDAT = false;
                }

                if (AP_APP.Value4 == 1)
                {
                    _LEDAPP = true;
                }
                else
                {
                    _LEDAPP = false;
                }

                _LEDMA = AP_FD2.Value4 != 0;
            }
        }

        private int _D_COURSE1 = 0;

        private int D_COURSE1
        {
            get { return _D_COURSE1; }
            set
            {
                _D_COURSE1 = value;
                _CRS = value.ToString("000");
                APD_COURSE1.Value2 = (short)value;
                FS.FSUIPC.FS.Process(APD_COURSE1);
            }
        }

        private int _D_HDG = 0;

        private int D_HDG
        {
            get { return _D_HDG; }
            set
            {
                _D_HDG = value;
                _HDG = D_HDG.ToString("000");
                APD_HDG.Value2 = (short)(((float)D_HDG * 182.04444f) + 1);
                FS.FSUIPC.FS.Process(APD_HDG);
            }
        }

        private int _D_ALT = 0;

        private int D_ALT
        {
            get { return _D_ALT; }
            set
            {
                _D_ALT = value;
                _ALT = _D_ALT.ToString("00000");
                APD_ALT.Value4 = (int)((double)_D_ALT * 19975.37d);
                FS.FSUIPC.FS.Process(APD_ALT);
            }
        }

        private int _SPEED = 0;

        private int SPEED
        {
            get { return _SPEED; }
            set
            {
                _SPEED = value;
                if (value != 2)
                {
                    F_IAS = T_IAS;
                }
                else
                {
                    F_IAS = T_MACH;
                }
            }
        }

        private int _F_IAS = 0;

        private int F_IAS
        {
            get { return _F_IAS; }
            set
            {
                _F_IAS = value;
                if (SPEED != 2)
                {
                    _IAS = F_IAS.ToString("000");

                    // ustawienie F_IAS na APD_IAS (w FS)
                    APD_IAS.Value2 = (short)F_IAS;
                    FS.FSUIPC.FS.Process(APD_IAS);
                }
                else
                {
                    _IAS = "." + F_IAS.ToString("00");

                    // ustawienie F_IAS * 655.36 na APD_MACH (w FS)
                    APD_MACH.Value4 = (int)((double)F_IAS * 655.36d);
                    FS.FSUIPC.FS.Process(APD_MACH);
                }
            }
        }

        private int _D_VS = 0;

        private int D_VS
        {
            get { return _D_VS; }
            set
            {
                _D_VS = value;
                if (_D_VS < -9000)
                {
                    _D_VS = -9000;
                }
                if (_D_VS > 9000)
                {
                    _D_VS = 9000;
                }
                _VS = _D_VS.ToString("0000");
            }
        }

        private int CMD
        {
            get { return _CMD; }
            set
            {
                _CMD = value;
                if (value == 1)
                {
                    _LEDCMDA = (AP_MASTER_SW.Value4 & 1) > 0;
                }
                else
                {
                    _LEDCMDB = (AP_MASTER_SW.Value4 & 1) > 0;
                }
            }
        }

        public override void IncIAS(bool fast)
        {
            if (SPEED != 2)
            {
                int tmp = T_IAS + (fast ? 10 : 1);
                if (tmp > 500)
                {
                    tmp = 500;
                }
                T_IAS = tmp;
                F_IAS = T_IAS;
            }
            else
            {
                int tmp = T_MACH + 1;
                if (tmp > 99)
                {
                    tmp = 99;
                }
                T_MACH = tmp;
                F_IAS = T_MACH;
            }
        }

        public override void DecIAS(bool fast)
        {
            if (SPEED != 2)
            {
                int tmp = T_IAS - (fast ? 10 : 1);
                if (tmp < 0)
                {
                    tmp = 0;
                }
                T_IAS = tmp;
                F_IAS = T_IAS;
            }
            else
            {
                int tmp = T_MACH - 1;
                if (tmp < 0)
                {
                    tmp = 0;
                }
                T_MACH = tmp;
                F_IAS = T_MACH;
            }
        }

        public override void IncALT(bool fast)
        {
            int tmp = D_ALT + (fast ? 1000 : 100);
            if (tmp > 50000)
            {
                tmp = 50000;
            }
            D_ALT = tmp;
        }

        public override void DecALT(bool fast)
        {
            int tmp = D_ALT - (fast ? 1000 : 100);
            if (tmp < 0)
            {
                tmp = 0;
            }
            D_ALT = tmp;
        }

        public override void IncCRS(bool fast)
        {
            int tmp = D_COURSE1 + (fast ? 10 : 1);
            if (tmp > 359)
            {
                tmp -= 360;
            }
            D_COURSE1 = tmp;
        }

        public override void DecCRS(bool fast)
        {
            int tmp = D_COURSE1 - (fast ? 10 : 1);
            if (tmp < 0)
            {
                tmp += 360;
            }
            D_COURSE1 = tmp;
        }

        public override void IncHDG(bool fast)
        {
            int tmp = D_HDG + (fast ? 10 : 1);
            if (tmp > 359)
            {
                tmp -= 360;
            }
            D_HDG = tmp;
        }

        public override void DecHDG(bool fast)
        {
            int tmp = D_HDG - (fast ? 10 : 1);
            if (tmp < 0)
            {
                tmp += 360;
            }
            D_HDG = tmp;
        }

        public override void IncVS(bool fast)
        {
            int tmp = D_VS + (fast ? 1000 : 100);
            D_VS = tmp;
            CONTADOR = 0;
            APD_VS2.Value2 = (short)D_VS;
            FS.FSUIPC.FS.Process(APD_VS2);
        }

        public override void DecVS(bool fast)
        {
            int tmp = D_VS - (fast ? 1000 : 100);
            D_VS = tmp;
            CONTADOR = 0;
            APD_VS2.Value2 = (short)D_VS;
            FS.FSUIPC.FS.Process(APD_VS2);
        }

        public override void PressCO()
        {
            if (_LEDSPEED)
            {
                if (SPEED != 0)
                {
                    if (SPEED == 1)
                    {
                        FS.FSUIPC.FS.Process(FS_MACH);
                        int tmp = (int)Math.Round(((float)FS_MACH.Value2 / 204.8f));
                        T_MACH = tmp;
                        AP_MACH2.Value4 = 1;
                        FS.FSUIPC.FS.Process(AP_MACH2);
                    }
                    else
                    {
                        FS.FSUIPC.FS.Process(FS_IAS);
                        int tmp = (int)Math.Round(((float)FS_IAS.Value4 / 128f));
                        T_IAS = tmp;
                        AP_IAS2.Value4 = 1;
                        FS.FSUIPC.FS.Process(AP_IAS2);
                    }
                }
            }            
        }

        public override void SetSwitchFD(bool state)
        {
            bool curr = AP_FD2.Value4 != 0;
            if (curr != state)
            {
                AP_FD.Value4 = state ? 1 : 0;
                FS.FSUIPC.FS.Process(AP_FD);
            }
        }

        public override void PressCmdA()
        {            
            AP_MASTER_SW2.ForWriting = false;
            FS.FSUIPC.FS.Process(AP_MASTER_SW2);
            if (AP_MASTER_SW2.Value4 == 1 && CMD == 1)
            {
                AP_MASTER_SW2.Value4 = 0;
            }
            else
            {
                AP_MASTER_SW2.Value4 = 1;
            }
            AP_MASTER_SW2.ForWriting = true;
            FS.FSUIPC.FS.Process(AP_MASTER_SW2);

            CMD = 1;
        }

        public override void PressCmdB()
        {            
            AP_MASTER_SW2.ForWriting = false;
            FS.FSUIPC.FS.Process(AP_MASTER_SW2);
            if (AP_MASTER_SW2.Value4 == 1 && CMD == 2)
            {
                AP_MASTER_SW2.Value4 = 0;
            }
            else
            {
                AP_MASTER_SW2.Value4 = 1;
            }
            AP_MASTER_SW2.ForWriting = true;
            FS.FSUIPC.FS.Process(AP_MASTER_SW2);

            CMD = 2;
        }

        public override void PressDisengage()
        {
            AP_MASTER_SW2.ForWriting = false;
            FS.FSUIPC.FS.Process(AP_MASTER_SW2);
            if (AP_MASTER_SW2.Value4 == 1)
            {
                AP_MASTER_SW2.Value4 = 0;
                AP_MASTER_SW2.ForWriting = true;
                FS.FSUIPC.FS.Process(AP_MASTER_SW2);
            }
        }

        //public override void PressVS()
        //{
        //    AP_ALT2.ForWriting = false;
        //    FS.FSUIPC.FS.Process(AP_ALT2);
        //    if (AP_ALT2.Value4 != 1)
        //    {
        //        AP_ALT2.Value4 = 1;
        //    }
        //    else
        //    {
        //        AP_ALT2.Value4 = 0;
        //    }
        //    AP_ALT2.ForWriting = true;
        //    FS.FSUIPC.FS.Process(AP_ALT2);
        //}

        public override void PressAltHLD()
        {
            AP_ALT2.ForWriting = false;
            FS.FSUIPC.FS.Process(AP_ALT2);
            if (AP_ALT2.Value4 != 1)
            {
                AP_ALT2.Value4 = 1;
            }
            else
            {
                AP_ALT2.Value4 = 0;
            }
            AP_ALT2.ForWriting = true;
            FS.FSUIPC.FS.Process(AP_ALT2);
        }

        public override void PressAPP()
        {
            AP_APP22.ForWriting = false;
            NAV_GPS2.ForWriting = false;
            FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { NAV_GPS2, AP_APP22});
            NAV_GPS2.ForWriting = true;
            AP_APP22.ForWriting = true;
            AP_APP2.ForWriting = true;
            if (AP_APP22.Value4 != 1)
            {
                //AP_APP2.Value4 = 1;
                //FS.FSUIPC.FS.Process(new FS.FSVariable[] { AP_APP2 });

                if (NAV_GPS2.Value4 == 1)
                {
                    NAV_GPS2.Value4 = 0;
                    FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { NAV_GPS2 });
                }

                // włączenie APP
                _fsControl.Value8 = FS.ControlVariable.AP_APR_HOLD_ON;
            }
            else
            {
                //AP_APP22.Value4 = 0;
                //AP_APP2.Value4 = 0;
                //FS.FSUIPC.FS.Process(new FS.FSVariable[] { AP_APP2, AP_APP22 });



                // wyłączenie APP
                _fsControl.Value8 = FS.ControlVariable.AP_APR_HOLD_OFF;
            }
            FS.FSUIPC.FS.Process(_fsControl);
        }

        public override void PressLNAV()
        {
            AP_NAV.ForWriting = false;
            NAV_GPS2.ForWriting = false;
            FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { AP_NAV, NAV_GPS2 });
            NAV_GPS2.ForWriting = true;
            AP_NAV.ForWriting = true;
            if (AP_NAV.Value4 != 1)
            {
                AP_NAV.Value4 = 1;
                NAV_GPS2.Value4 = 1;
                FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { AP_NAV, NAV_GPS2 });
            }
            else
            {
                if (NAV_GPS2.Value4 == 0)
                {
                    NAV_GPS2.Value4 = 1;
                    FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { NAV_GPS2 });
                }
                else
                {
                    AP_NAV.Value4 = 0;
                    FS.FSUIPC.FS.Process(AP_NAV);
                }
            }
        }

        public override void PressVORLOC()
        {
            AP_NAV.ForWriting = false;
            NAV_GPS2.ForWriting = false;
            FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { AP_NAV, NAV_GPS2 });
            NAV_GPS2.ForWriting = true;
            AP_NAV.ForWriting = true;
            if (AP_NAV.Value4 != 1)
            {
                AP_NAV.Value4 = 1;
                NAV_GPS2.Value4 = 0;
                FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { AP_NAV, NAV_GPS2 });
            }
            else
            {
                if (NAV_GPS2.Value4 == 1)
                {
                    AP_NAV.Value4 = 0;
                    FS.FSUIPC.FS.Process(AP_NAV);

                    AP_NAV.Value4 = 1;
                    NAV_GPS2.Value4 = 0;
                    FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { AP_NAV, NAV_GPS2 });

                    //NAV_GPS2.Value4 = 0;
                    //FS.FSUIPC.FS.Process(new FS.SimpleVariable[] { NAV_GPS2 });
                }
                else
                {
                    AP_NAV.Value4 = 0;
                    FS.FSUIPC.FS.Process(AP_NAV);
                }
            }            
        }

        public override void PressHdgSEL()
        {
            AP_HDG_SEL2.ForWriting = false;
            FS.FSUIPC.FS.Process(AP_HDG_SEL2);
            AP_HDG_SEL2.ForWriting = true;
            if (AP_HDG_SEL2.Value4 != 1)
            {
                //NAV_GPS2.Value4 = 0;
                //FS.FSUIPC.FS.Process(NAV_GPS2);

                AP_HDG_SEL2.Value4 = 1;
                FS.FSUIPC.FS.Process(AP_HDG_SEL2);                
            }
            else
            {
                AP_HDG_SEL2.Value4 = 0;
                FS.FSUIPC.FS.Process(AP_HDG_SEL2);
            }            
        }

        public override void PressSPEED()
        {
            if (SPEED == 0)
            {
                AP_IAS2.Value4 = 1;
                FS.FSUIPC.FS.Process(AP_IAS2);
                _LEDN1 = false;
            }
            else
            {
                AP_IAS2.Value4 = 0;
                AP_MACH2.Value4 = 0;
                FS.FSUIPC.FS.Process(new FS.FSVariable[] { AP_IAS2, AP_MACH2 });
                SPEED = 0;
            }    
        }

        public override void SetSwitchAT(bool state)
        {
            bool curr = AP_AT.Value4 != 0;
            if (curr != state)
            {
                AP_AT2.Value4 = state ? 1 : 0;
                FS.FSUIPC.FS.Process(AP_AT2);
            }
        }

        public override void PressN1()
        {
            if (FS.FSUIPC.FS.SimVersion == simPROJECT.FS.FSUIPC.FSVersion.FSX)
            {
                if (!_LEDN1)
                {
                    AP_N1.Value4 = 1;
                    _LEDN1 = true;
                    _LEDSPEED = false;
                }
                else
                {
                    AP_N1.Value4 = 0;
                    _LEDN1 = false;
                }
                FS.FSUIPC.FS.Process(AP_N1);
            }
        }
    }
}
