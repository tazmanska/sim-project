using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace simPROJECT.Planes.ProSim737
{
    class ProSim737MCP : BaseMCP
    {
        public ProSim737MCP()
        {
            _prosimVariables = new simPROJECT.FS.FSVariable[]
            {
                  _prosimIndicators
                , _prosimCRS
                , _prosimALT
                , _prosimVS
                , _prosimHDG
                , _prosimIAS
                , _prosimDisplays
            };
        }

        public override void ResetDevice()
        {
            base.ResetDevice();
            Startup();
        }

        private bool? _fd = null;
        private bool? _at = null;

        private bool _disengage = false;

        private ProsimOffset _spdIntv = null;
        private ProsimOffset _altIntv = null;
        private ProsimOffset _rate10 = null;
        private ProsimOffset _rate15 = null;
        private ProsimOffset _rate20 = null;
        private ProsimOffset _rate25 = null;
        private ProsimOffset _rate30 = null;
        private bool _rateMode = false;
        private int _rate = 0;
        
        public override void Startup()
        {
            base.Startup();
            _prosimControls.ForWriting = false;
            _fd = null;
            _at = null;

            _disengage = false;
            _rateMode = false;
            _rate = 0;

            // wczytanie offsetów dla SPD Intv, ALT Intv i Bank rate limit
            LoadOffsets();
        }

        private void LoadOffsets()
        {
            _spdIntv = null;
            _altIntv = null;
            _rate10 = null;
            _rate15 = null;
            _rate20 = null;
            _rate25 = null;
            _rate30 = null;
            _rateMode = false;
            _rate = 0;
            try
            {
                if (DeviceConfiguration is simPROJECT.Configuration.MCP737Configuration)
                {
                    string configFile = ((simPROJECT.Configuration.MCP737Configuration)DeviceConfiguration).ProsimConfigFile;
                    if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(configFile);
                        _spdIntv = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Speed Int Pushed']"));
                        _altIntv = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Alt Int Pushed']"));
                        _rate10 = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Bank limit 10']"));
                        _rate15 = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Bank limit 15']"));
                        _rate20 = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Bank limit 20']"));
                        _rate25 = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Bank limit 25']"));
                        _rate30 = ProsimOffset.Load(xml.SelectSingleNode("//config/mappings/mapping[@connection='MCP Bank limit 30']"));
                    }
                }
            }
            catch { }
        }

        private FS.SimpleVariable _prosimIndicators = new simPROJECT.FS.SimpleVariable(0x04f0)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimCRS = new simPROJECT.FS.SimpleVariable(0x0c4e)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimCRSWrite = new simPROJECT.FS.SimpleVariable(0x0c4e)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimALT = new simPROJECT.FS.SimpleVariable(0x540a)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimALTWrite = new simPROJECT.FS.SimpleVariable(0x540a)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimVS = new simPROJECT.FS.SimpleVariable(0x540c)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimVSWrite = new simPROJECT.FS.SimpleVariable(0x540c)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimHDG = new simPROJECT.FS.SimpleVariable(0x5408)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimHDGWrite = new simPROJECT.FS.SimpleVariable(0x5408)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimIAS = new simPROJECT.FS.SimpleVariable(0x5406)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimIASWrite = new simPROJECT.FS.SimpleVariable(0x5406)
        {
            ForWriting = true,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimDisplays = new simPROJECT.FS.SimpleVariable(0x051c)
        {
            ForWriting = false,
            ValueSize = 2
        };

        private FS.SimpleVariable _prosimControls = new simPROJECT.FS.SimpleVariable(0x5410)
        {
            ForWriting = false,
            ValueSize = 8
        };

        private FS.FSVariable[] _prosimVariables = null;

        private bool IsMACH
        {
            get { return ((int)_prosimIndicators.Value2 & 32768) > 0; }
        }

        private int _heading = 0;

        public override void UpdateOutputs()
        {
            HasData = FS.FSUIPC.FS.Process(_prosimVariables);
            if (!HasData)
            {
                Error = "no FS";
            }
            else
            {
                if (_rate == 0)
                {
                    // wysłanie rozkazu ustawienia rate 10
                    _rate = 20;
                    UpdateBankLimit();
                }

                _CRS = _prosimCRS.Value2.ToString("000");
                if (IsMACH)
                {
                    // mach
                    int mach = (_prosimIAS.Value2 - 100);
                    if (mach < 0)
                    {
                        mach = 0;
                    }
                    _IAS = "." + mach.ToString("00");
                }
                else
                {
                    _IAS = _prosimIAS.Value2.ToString("000");
                }
                _heading = (int)Math.Round(((((double)(ushort)_prosimHDG.Value2) * 360d) / 65536d));
                _HDG = _heading.ToString("000");
                _ALT = (_prosimALT.Value2 * 100).ToString("00000");
                _VS = (_prosimVS.Value2).ToString("0000");

                if ((_prosimDisplays.Value2 & 1) > 0)
                {
                    // wyczyszczenie wyświetlacza VS
                    _VS = "";
                }

                if ((_prosimDisplays.Value2 & 2) > 0)
                {
                    // wyczyszczenie wyświetlacza IAS
                    _IAS = "";
                }

                // kontrolki
                _LEDCMDA = (_prosimIndicators.Value2 & (1 << 0)) > 0;
                _LEDCMDB = (_prosimIndicators.Value2 & (1 << 1)) > 0;
                _LEDVS = (_prosimIndicators.Value2 & (1 << 2)) > 0;
                _LEDALTHLD = (_prosimIndicators.Value2 & (1 << 3)) > 0;
                _LEDAPP = (_prosimIndicators.Value2 & (1 << 4)) > 0;
                _LEDVORLOC = (_prosimIndicators.Value2 & (1 << 5)) > 0;
                _LEDLNAV = (_prosimIndicators.Value2 & (1 << 6)) > 0;
                _LEDHDGSEL = (_prosimIndicators.Value2 & (1 << 7)) > 0;
                _LEDLVLCHG = (_prosimIndicators.Value2 & (1 << 8)) > 0;
                _LEDSPEED = (_prosimIndicators.Value2 & (1 << 9)) > 0;
                _LEDN1 = (_prosimIndicators.Value2 & (1 << 10)) > 0;
                _LEDAT = (_prosimIndicators.Value2 & (1 << 11)) > 0;
                _LEDMA = (_prosimIndicators.Value2 & (1 << 12)) > 0;
                _LEDVNAV = (_prosimIndicators.Value2 & (1 << 14)) > 0;

                _LEDMA = _fd != null && _fd.Value;

                if (_rateMode)
                {
                    _HDG = "r" + _rate.ToString("00");
                }
            }
        }

        private void SendKey(int bit)
        {
            if (!_prosimControls.ForWriting)
            {
                FS.FSUIPC.FS.Process(_prosimControls);
                _prosimControls.ForWriting = true;
            }

            long one = 1;

            if (((_prosimControls.Value8 >> bit) & 1) > 0)
            {
                // wyłączenie bitu
                _prosimControls.Value8 &= ~(one << bit);
            }
            else
            {
                // włączenie bitu
                _prosimControls.Value8 |= (one << bit);
            }

            FS.FSUIPC.FS.Process(_prosimControls);
        }

        public override void SetSwitchFD(bool state)
        {
            if (_fd == null || _fd.Value != state)
            {
                SendKey(state ? 37 : 38);
                _fd = state;
            }
        }

        public override void SetSwitchAT(bool state)
        {
            if (_at == null || _at.Value != state)
            {
                SendKey(state ? 19 : 20);
                _at = state;
            }
        }

        public override void PressAltHLD()
        {
            SendKey(30);
        }

        public override void PressAltINTV()
        {
            if (_altIntv != null)
            {
                _altIntv.Set(true);
            }
        }

        public override void PressAPP()
        {
            SendKey(29);
        }

        public override void PressCmdA()
        {
            SendKey(32);
        }

        public override void PressCmdB()
        {
            SendKey(36);
        }

        public override void PressCO()
        {
            SendKey(23);
        }

        public override void PressCwsA()
        {
            SendKey(12);
        }

        public override void PressCwsB()
        {
            SendKey(42);
        }

        public override void PressDisengage()
        {
            if (_disengage)
            {
                SendKey(41);
            }
            else
            {
                SendKey(40);
            }
            _disengage = !_disengage;
        }

        public override void PressHdgButton()
        {
            _rateMode = !_rateMode;
        }

        public override void PressHdgSEL()
        {
            SendKey(25);
        }

        public override void PressLNAV()
        {
            SendKey(27);
        }

        public override void PressLvlCHG()
        {
            SendKey(24);
        }

        public override void PressN1()
        {
            SendKey(21);
        }

        public override void PressSpdINTV()
        {
            if (_spdIntv != null)
            {
                _spdIntv.Set(true);
            }
        }

        public override void PressSPEED()
        {
            SendKey(22);
        }

        public override void PressVNAV()
        {
            SendKey(26);
        }

        public override void PressVORLOC()
        {
            SendKey(28);
        }

        public override void PressVS()
        {
            SendKey(31);
        }

        public override void DecALT(bool fast)
        {
            if (_prosimALT.Value2 > 0)
            {
                //SendEncoder(fast && _prosimALT.Value2 >= 10  ? 8 : 6);

                _prosimALTWrite.Value2 = _prosimALT.Value2;
                if (fast && _prosimALTWrite.Value2 >= 10)
                {
                    _prosimALTWrite.Value2 -= 10;
                }
                else
                {
                    _prosimALTWrite.Value2 -= 1;
                }
                FS.FSUIPC.FS.Process(_prosimALTWrite);
            }
        }

        public override void DecCRS(bool fast)
        {
            //SendEncoder(16);

            _prosimCRSWrite.Value2 = _prosimCRS.Value2;
            if (fast)
            {
                _prosimCRSWrite.Value2 -= 10;
            }
            else
            {
                _prosimCRSWrite.Value2 -= 1;
            }
            if (_prosimCRSWrite.Value2 < 0)
            {
                _prosimCRSWrite.Value2 += 360;
            }
            FS.FSUIPC.FS.Process(_prosimCRSWrite);
        }

        private void UpdateBankLimit()
        {
            switch (_rate)
            {
                case 10:
                    if (_rate10 != null)
                    {
                        _rate10.Set();
                    }
                    break;

                case 15:
                    if (_rate15 != null)
                    {
                        _rate15.Set();
                    }
                    break;

                case 20:
                    if (_rate20 != null)
                    {
                        _rate20.Set();
                    }
                    break;

                case 25:
                    if (_rate25 != null)
                    {
                        _rate25.Set();
                    }
                    break;

                case 30:
                    if (_rate30 != null)
                    {
                        _rate30.Set();
                    }
                    break;
            }
        }

        public override void DecHDG(bool fast)
        {
            //SendEncoder(fast ? 4 : 2);
            if (_rateMode)
            {
                if (_rate > 10)
                {
                    _rate -= 5;

                    UpdateBankLimit();
                }                
            }
            else
            {
                double hdg = _heading;
                hdg -= fast ? 10 : 1;
                if (hdg < 0)
                {
                    hdg += 360;
                }
                _prosimHDGWrite.Value2 = (short)(ushort)Math.Round(65536d * (hdg / 360d)); ;
                FS.FSUIPC.FS.Process(_prosimHDGWrite);
            }
        }

        public override void DecIAS(bool fast)
        {
            //SendEncoder(fast ? 12 : 10);

            _prosimIASWrite.Value2 = _prosimIAS.Value2;
            _prosimIASWrite.Value2 -= fast ? (short)10 : (short)1;
            if (IsMACH)
            {
                _prosimIASWrite.Value2 -= 100;

                if (_prosimIASWrite.Value2 < 0)
                {
                    _prosimIASWrite.Value2 = 0;
                }

                _prosimIASWrite.Value2 += 100;
            }
            else
            {
                if (_prosimIASWrite.Value2 < 100)
                {
                    _prosimIASWrite.Value2 = 100;
                }
            }
            FS.FSUIPC.FS.Process(_prosimIASWrite);
        }

        public override void DecVS(bool fast)
        {
            //SendEncoder(14);

            if (_prosimVS.Value2 > -7000)
            {
                _prosimVSWrite.Value2 = _prosimVS.Value2;
                _prosimVSWrite.Value2 -= _prosimVSWrite.Value2 > -1000 && _prosimVSWrite.Value2 < 1000 ? (short)50 : (short)100;
                FS.FSUIPC.FS.Process(_prosimVSWrite);
            }
        }

        public override void IncALT(bool fast)
        {
            if (_prosimALT.Value2 < 500)
            {
                //SendEncoder(fast ? 9 : 7);

                _prosimALTWrite.Value2 = _prosimALT.Value2;
                if (fast)
                {
                    _prosimALTWrite.Value2 += 10;
                }
                else
                {
                    _prosimALTWrite.Value2 += 1;
                }
                FS.FSUIPC.FS.Process(_prosimALTWrite);
            }
        }

        public override void IncCRS(bool fast)
        {
            //SendEncoder(17);

            _prosimCRSWrite.Value2 = _prosimCRS.Value2;
            if (fast)
            {
                _prosimCRSWrite.Value2 += 10;
            }
            else
            {
                _prosimCRSWrite.Value2 += 1;
            }
            if (_prosimCRSWrite.Value2 > 359)
            {
                _prosimCRSWrite.Value2 -= 360;
            }
            FS.FSUIPC.FS.Process(_prosimCRSWrite);
        }

        public override void IncHDG(bool fast)
        {
            //SendEncoder(fast ? 5 : 3);
            if (_rateMode)
            {
                if (_rate < 30)
                {
                    _rate += 5;

                    UpdateBankLimit();
                }
            }
            else
            {
                double hdg = _heading;
                hdg += fast ? 10 : 1;
                if (hdg > 359)
                {
                    hdg -= 360;
                }
                _prosimHDGWrite.Value2 = (short)Math.Round(65536d * (hdg / 360d)); ;
                FS.FSUIPC.FS.Process(_prosimHDGWrite);
            }
        }

        public override void IncIAS(bool fast)
        {
            //SendEncoder(fast ? 13 : 11);
            
            _prosimIASWrite.Value2 = _prosimIAS.Value2;
            _prosimIASWrite.Value2 += fast ? (short)10 : (short)1;
            if (IsMACH)
            {
                _prosimIASWrite.Value2 -= 100;

                if (_prosimIASWrite.Value2 > 80)
                {
                    _prosimIASWrite.Value2 = 80;
                }

                _prosimIASWrite.Value2 += 100;
            }
            else
            {
                if (_prosimIASWrite.Value2 > 400)
                {
                    _prosimIASWrite.Value2 = 400;
                }
            }
            FS.FSUIPC.FS.Process(_prosimIASWrite);
        }

        public override void IncVS(bool fast)
        {
            //SendEncoder(15);
            if (_prosimVS.Value2 < 7000)
            {
                _prosimVSWrite.Value2 = _prosimVS.Value2;
                _prosimVSWrite.Value2 += _prosimVSWrite.Value2 > -1000 && _prosimVSWrite.Value2 < 1000 ? (short)50 : (short)100;
                FS.FSUIPC.FS.Process(_prosimVSWrite);
            }
        }
    }
}
