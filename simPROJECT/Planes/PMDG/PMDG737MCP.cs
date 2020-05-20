using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace simPROJECT.Planes.PMDG
{
    class PMDG737MCP : BaseMCP
    {
        public PMDG737MCP()
        {
            _fsVariables = new simPROJECT.FS.SimpleVariable[]
            {
                  _fsPMDGMCPstatus
                , _fsPMDGSPDINTV
                , _fsPMDGCOstatus
                , _fsPMDGIAS
                , _fsPMDGHDG
                , _fsPMDGALT
                , _fsPMDGVS
                , _fsIAS
                , _fsMACH
                , _fsCRS


                , _fsPMDGAT
            };
        }

        private FS.SimpleVariable[] _fsVariables = null;

        private FS.SimpleVariable _fsPMDGMCPstatus = new simPROJECT.FS.SimpleVariable(0x62BC, false, 4);
        private FS.SimpleVariable _fsPMDGSPDINTV = new simPROJECT.FS.SimpleVariable(0x62C0, false, 1);
        private FS.SimpleVariable _fsPMDGCOstatus = new simPROJECT.FS.SimpleVariable(0x621E, false, 2);
        private FS.SimpleVariable _fsPMDGIAS = new simPROJECT.FS.SimpleVariable(0x6220, false, 2);
        private FS.SimpleVariable _fsPMDGHDG = new simPROJECT.FS.SimpleVariable(0x622C, false, 2);
        private FS.SimpleVariable _fsPMDGALT = new simPROJECT.FS.SimpleVariable(0x622E, false, 2);
        private FS.SimpleVariable _fsPMDGVS = new simPROJECT.FS.SimpleVariable(0x6230, false, 2);
        private FS.SimpleVariable _fsIAS = new simPROJECT.FS.SimpleVariable(0x02BC, false, 4);
        private FS.SimpleVariable _fsMACH = new simPROJECT.FS.SimpleVariable(0x11C6, false, 2);
        private FS.SimpleVariable _fsCRS = new simPROJECT.FS.SimpleVariable(0x0C4E, false, 2);

        private FS.SimpleVariable _fsPMDGAT = new simPROJECT.FS.SimpleVariable(0x621c, false, 1);
        private FS.SimpleVariable _fsPMDGATWrite = new simPROJECT.FS.SimpleVariable(0x621c, true, 1);

        private bool _loadedKeys = false;
        private bool _bankLimitMode = false;
        private int _bankLimit = 20;

        public override void Startup()
        {
            base.Startup();
            _loadedKeys = false;
            _FDswitchKey = null;
            _ATswitchKey = null;
            _N1bpKey = null;
            _SPEEDbpKey = null;
            _VNAVbpKey = null;
            _LVLCHGbpKey = null;
            _HDGbpKey = null;
            _LNAVbpKey = null;
            _VORLOCbpKey = null;
            _APPbpKey = null;
            _ALTHOLDbpKey = null;
            _VSbpKey = null;
            _CMDAbpKey = null;
            _CMDBbpKey = null;
            _CWSAbpKey = null;
            _CWSBbpKey = null;
            _APDISCObpKey = null;
            _CObpKey = null;
            _CRSLEFTrotLKey = null;
            _CRSLEFTrotRKey = null;
            _IASrotLKey = null;
            _IASrotRKey = null;
            _HDGrotLKey = null;
            _HDGrotRKey = null;
            _ALTrotLKey = null;
            _ALTrotRKey = null;
            _VSrotLKey = null;
            _VSrotRKey = null;
            _CRSLEFTrotLKeyFast = null;
            _CRSLEFTrotRKeyFast = null;
            _IASrotLKeyFast = null;
            _IASrotRKeyFast = null;
            _HDGrotLKeyFast = null;
            _HDGrotRKeyFast = null;
            _ALTrotLKeyFast = null;
            _ALTrotRKeyFast = null;
            _VSrotLKeyFast = null;
            _VSrotRKeyFast = null;
            _SPDIntvKey = null;
            _ALTIntvKey = null;
            _bankLimitInc = null;
            _bankLimitDec = null;

            MEMALT = 10000;
            MEMHDG = 0;

            _bankLimitMode = false;
            _bankLimit = 20;
        }

        public override void Close()
        {
            base.Close();
        }

        static PMDG737MCP()
        {
            __keysMap.Add(48, Utils.VK.KEY_0);
            __keysMap.Add(49, Utils.VK.KEY_1);
            __keysMap.Add(50, Utils.VK.KEY_2);
            __keysMap.Add(51, Utils.VK.KEY_3);
            __keysMap.Add(52, Utils.VK.KEY_4);
            __keysMap.Add(53, Utils.VK.KEY_5);
            __keysMap.Add(54, Utils.VK.KEY_6);
            __keysMap.Add(55, Utils.VK.KEY_7);
            __keysMap.Add(56, Utils.VK.KEY_8);
            __keysMap.Add(57, Utils.VK.KEY_9);
            __keysMap.Add(65, Utils.VK.KEY_A);
            __keysMap.Add(66, Utils.VK.KEY_B);
            __keysMap.Add(67, Utils.VK.KEY_C);
            __keysMap.Add(68, Utils.VK.KEY_D);
            __keysMap.Add(69, Utils.VK.KEY_E);
            __keysMap.Add(70, Utils.VK.KEY_F);
            __keysMap.Add(71, Utils.VK.KEY_G);
            __keysMap.Add(72, Utils.VK.KEY_H);
            __keysMap.Add(73, Utils.VK.KEY_I);
            __keysMap.Add(74, Utils.VK.KEY_J);
            __keysMap.Add(75, Utils.VK.KEY_K);
            __keysMap.Add(76, Utils.VK.KEY_L);
            __keysMap.Add(77, Utils.VK.KEY_M);
            __keysMap.Add(78, Utils.VK.KEY_N);
            __keysMap.Add(79, Utils.VK.KEY_O);
            __keysMap.Add(80, Utils.VK.KEY_P);
            __keysMap.Add(81, Utils.VK.KEY_Q);
            __keysMap.Add(82, Utils.VK.KEY_R);
            __keysMap.Add(83, Utils.VK.KEY_S);
            __keysMap.Add(84, Utils.VK.KEY_T);
            __keysMap.Add(85, Utils.VK.KEY_U);
            __keysMap.Add(86, Utils.VK.KEY_V);
            __keysMap.Add(87, Utils.VK.KEY_W);
            __keysMap.Add(88, Utils.VK.KEY_X);
            __keysMap.Add(89, Utils.VK.KEY_Y);
            __keysMap.Add(90, Utils.VK.KEY_Z);
            __keysMap.Add(-64, Utils.VK.OEM_3);
            __keysMap.Add(-67, Utils.VK.SUBTRACT);
            __keysMap.Add(-69,  Utils.VK.OEM_NEC_EQUAL);
            __keysMap.Add(-36, Utils.VK.OEM_5);
            __keysMap.Add(-37, Utils.VK.OEM_4);
            __keysMap.Add(-35, Utils.VK.OEM_6);
            __keysMap.Add(-70, Utils.VK.OEM_1);
            __keysMap.Add(-34, Utils.VK.OEM_7);
            __keysMap.Add(-68, Utils.VK.OEM_COMMA);
            __keysMap.Add(-66, Utils.VK.OEM_PERIOD);
            __keysMap.Add(-65, Utils.VK.OEM_2);
            __keysMap.Add(46, Utils.VK.DELETE);
            __keysMap.Add(112, Utils.VK.F1);
            __keysMap.Add(113, Utils.VK.F2);
            __keysMap.Add(114, Utils.VK.F3);
            __keysMap.Add(115, Utils.VK.F4);
            __keysMap.Add(116, Utils.VK.F5);
            __keysMap.Add(117, Utils.VK.F6);
            __keysMap.Add(118, Utils.VK.F7);
            __keysMap.Add(119, Utils.VK.F8);
            __keysMap.Add(120, Utils.VK.F9);
            __keysMap.Add(121, Utils.VK.F10);
            __keysMap.Add(122, Utils.VK.F11);
            __keysMap.Add(123, Utils.VK.F12);
        }

        private static Dictionary<int, Utils.VK> __keysMap = new Dictionary<int, Utils.VK>();

        private static Utils.VK[] ParseKeys(string keys)
        {
            if (string.IsNullOrEmpty(keys))
            {
                return null;
            }
            List<Utils.VK> result = new List<simPROJECT.Utils.VK>();
            string[] t = keys.Split(new char[] { ',' });
            if (t.Length == 3)
            {
                int modifiers = int.Parse(t[0].Trim());
                int key = int.Parse(t[1].Trim());
                if ((modifiers & 0x04) == 0x04)
                {
                    // SHIFT
                    result.Add(simPROJECT.Utils.VK.SHIFT);
                }                
                if ((modifiers & 0x02) == 0x02)
                {
                    // CTRL
                    result.Add(simPROJECT.Utils.VK.CONTROL);
                }
                if ((modifiers & 0x01) == 0x01)
                {
                    // TAB
                    result.Add(simPROJECT.Utils.VK.TAB);
                }
                if (__keysMap.ContainsKey(key))
                {
                    result.Add(__keysMap[key]);
                }
            }
            return result.ToArray();
        }

        private void LoadPMDGKeys()
        {
            try
            {
                if (DeviceConfiguration is simPROJECT.Configuration.MCP737Configuration)
                {
                    string keysFile = ((simPROJECT.Configuration.MCP737Configuration)DeviceConfiguration).PMDGKeysFile;
                    if (!string.IsNullOrEmpty(keysFile) && File.Exists(keysFile))
                    {                        
                        Utils.IniFile iniFile = new simPROJECT.Utils.IniFile(keysFile);
                        _FDswitchKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press FD L", null));
                        _ATswitchKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press AT", null));
                        _N1bpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press N1", null));
                        _SPEEDbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press SPD", null));
                        _VNAVbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press VNAV", null));
                        _LVLCHGbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press LVLCHG", null));
                        _HDGbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press HDGSEL", null));
                        _LNAVbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press LNAV", null));
                        _VORLOCbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press VORLOC", null));
                        _APPbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press APP", null));
                        _ALTHOLDbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press ALTHLD", null));
                        _VSbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press VS", null));
                        _CMDAbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press CMD L", null));
                        _CMDBbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press CMD R", null));
                        _CWSAbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press CWS L", null));
                        _CWSBbpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press CWS R", null));
                        _APDISCObpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press APDISCON", null));
                        _CObpKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press CO", null));
                        _CRSLEFTrotLKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Course", null));
                        _CRSLEFTrotRKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Course", null));
                        _IASrotLKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Speed", null));
                        _IASrotRKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Speed", null));
                        _HDGrotLKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Heading", null));
                        _HDGrotRKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Heading", null));
                        _ALTrotLKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Altitude", null));
                        _ALTrotRKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Altitude", null));
                        _VSrotLKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease VS", null));
                        _VSrotRKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase VS", null));
                        _CRSLEFTrotLKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Course Fast", null));
                        _CRSLEFTrotRKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Course Fast", null));
                        _IASrotLKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Speed Fast", null));
                        _IASrotRKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Speed Fast", null));
                        _HDGrotLKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Hdg Fast", null));
                        _HDGrotRKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Hdg Fast", null));
                        _ALTrotLKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Altitude Fast", null));
                        _ALTrotRKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Altitude Fast", null));
                        _VSrotLKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease VS Fast", null));
                        _VSrotRKeyFast = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase VS Fast", null));
                        _SPDIntvKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press SPD INTV", null));
                        _ALTIntvKey = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Press ALT INTV", null));
                        _bankLimitDec = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Decrease Bank Limiter", null));
                        _bankLimitInc = ParseKeys(iniFile.ReadValue("Keyboard", "MCP Increase Bank Limiter", null));
                    }
                }
                _loadedKeys = true;
            }
            catch { }
        }

        private Utils.VK[] _FDswitchKey = null;
        private Utils.VK[] _ATswitchKey = null;
        private Utils.VK[] _N1bpKey = null;
        private Utils.VK[] _SPEEDbpKey = null;
        private Utils.VK[] _VNAVbpKey = null;
        private Utils.VK[] _LVLCHGbpKey = null;
        private Utils.VK[] _HDGbpKey = null;
        private Utils.VK[] _LNAVbpKey = null;
        private Utils.VK[] _VORLOCbpKey = null;
        private Utils.VK[] _APPbpKey = null;
        private Utils.VK[] _ALTHOLDbpKey = null;
        private Utils.VK[] _VSbpKey = null;
        private Utils.VK[] _CMDAbpKey = null;
        private Utils.VK[] _CMDBbpKey = null;
        private Utils.VK[] _CWSAbpKey = null;
        private Utils.VK[] _CWSBbpKey = null;
        private Utils.VK[] _APDISCObpKey = null;
        private Utils.VK[] _CObpKey = null;
        private Utils.VK[] _CRSLEFTrotLKey = null;
        private Utils.VK[] _CRSLEFTrotRKey = null;
        private Utils.VK[] _IASrotLKey = null;
        private Utils.VK[] _IASrotRKey = null;
        private Utils.VK[] _HDGrotLKey = null;
        private Utils.VK[] _HDGrotRKey = null;
        private Utils.VK[] _ALTrotLKey = null;
        private Utils.VK[] _ALTrotRKey = null;
        private Utils.VK[] _VSrotLKey = null;
        private Utils.VK[] _VSrotRKey = null;
        private Utils.VK[] _CRSLEFTrotLKeyFast = null;
        private Utils.VK[] _CRSLEFTrotRKeyFast = null;
        private Utils.VK[] _IASrotLKeyFast = null;
        private Utils.VK[] _IASrotRKeyFast = null;
        private Utils.VK[] _HDGrotLKeyFast = null;
        private Utils.VK[] _HDGrotRKeyFast = null;
        private Utils.VK[] _ALTrotLKeyFast = null;
        private Utils.VK[] _ALTrotRKeyFast = null;
        private Utils.VK[] _VSrotLKeyFast = null;
        private Utils.VK[] _VSrotRKeyFast = null;
        private Utils.VK[] _SPDIntvKey = null;
        private Utils.VK[] _ALTIntvKey = null;
        private Utils.VK[] _bankLimitInc = null;
        private Utils.VK[] _bankLimitDec = null;

        public override void UpdateOutputs()
        {
            if (!_loadedKeys)
            {
                LoadPMDGKeys();

                _bankLimit = 20;
                if (_bankLimitDec != null && _bankLimitInc != null)
                {
                    // naciśnięcie 4 razy Inc (żeby przeszło na 30)
                    Utils.Keyboard.ShortcutFS(_bankLimitInc);
                    Utils.Keyboard.ShortcutFS(_bankLimitInc);
                    Utils.Keyboard.ShortcutFS(_bankLimitInc);
                    Utils.Keyboard.ShortcutFS(_bankLimitInc);

                    // naciśnięcie 2 razy Dec (żeby przeszło na 20)
                    Utils.Keyboard.ShortcutFS(_bankLimitDec);
                    Utils.Keyboard.ShortcutFS(_bankLimitDec);
                }
            }

            IsTestMode = false;

            HasData = FS.FSUIPC.FS.Process(_fsVariables);

            if (HasData)
            {
                _LEDMA = (_fsPMDGMCPstatus.Value4 & (1 << 12)) > 0;
                _LEDAT = (_fsPMDGMCPstatus.Value4 & (1 << 14)) > 0;
                _LEDN1 = (_fsPMDGMCPstatus.Value4 & (1 << 15)) > 0;
                _LEDSPEED = (_fsPMDGMCPstatus.Value4 & (1 << 16)) > 0;
                _LEDVNAV = (_fsPMDGMCPstatus.Value4 & (1 << 18)) > 0;
                _LEDLVLCHG = (_fsPMDGMCPstatus.Value4 & (1 << 17)) > 0;
                _LEDHDGSEL = (_fsPMDGMCPstatus.Value4 & (1 << 22)) > 0;
                _LEDLNAV = (_fsPMDGMCPstatus.Value4 & (1 << 19)) > 0;
                _LEDVORLOC = (_fsPMDGMCPstatus.Value4 & (1 << 20)) > 0;
                _LEDAPP = (_fsPMDGMCPstatus.Value4 & (1 << 21)) > 0;
                _LEDALTHLD = (_fsPMDGMCPstatus.Value4 & (1 << 23)) > 0;
                _LEDVS = (_fsPMDGMCPstatus.Value4 & (1 << 24)) > 0;
                _LEDCMDA = (_fsPMDGMCPstatus.Value4 & (1 << 8)) > 0;
                //_LEDCMDB = (_fsPMDGMCPstatus.Value2 & (1 << 9)) > 0;
                _LEDCWSA = (_fsPMDGMCPstatus.Value4 & (1 << 25)) > 0;
                //_LEDCWSB = (_fsPMDGMCPstatus.Value2 & (1 << 26)) > 0;
                _CRS = _fsCRS.Value2 == 360 ? "000" : _fsCRS.Value2.ToString("000");
                HDGaff();
                IASaff();
                ALTaff();
                VSaff();
            }
            else
            {
                Error = "no FS";
            }
        }

        public override void ResetDevice()
        {
            base.ResetDevice();
            Startup();
        }

        private int _memHDG = 0;

        private int MEMHDG
        {
            get { return _memHDG; }
            set
            {
                _memHDG = value;
                if (_memHDG > 359)
                {
                    _memHDG = 0;
                }
            }
        }

        private int _memALT = 10000;

        private int MEMALT
        {
            get { return _memALT; }
            set
            {
                _memALT = value;
                if (_memALT < 0)
                {
                    _memALT = 0;
                }
                if (_memALT > 50000)
                {
                    _memALT = 50000;
                }
            }
        }

        private void VSaff()
        {
            if (_LEDVS)
            {
                if ((ushort)_fsPMDGVS.Value2 < 60000)
                {
                    _VS = _fsPMDGVS.Value2.ToString("0000");
                }
                else
                {
                    int vs = (ushort)_fsPMDGVS.Value2 - 65536;
                    _VS = "-" + (vs * -1).ToString("0000");
                }
            }
            else
            {
                _VS = "";
            }
        }

        private void IASaff()
        {
            bool tmp = _fsPMDGSPDINTV.Value1 == 21;
            if (tmp)
            {
                tmp = _LEDVNAV;
            }
            if (tmp)
            {
                _IAS = "";
            }
            else
            {
                if (_fsPMDGCOstatus.Value2 < 4)
                {
                    _IAS = _fsPMDGIAS.Value2.ToString();
                }
                else
                {
                    int m = _fsPMDGIAS.Value2 / _fsIAS.Value4;
                    m *= 12800;
                    m *= _fsMACH.Value2;
                    _IAS = ((double)m / 20480d).ToString("0.00");
                }
            }
        }

        private void HDGaff()
        {
            if (!_LEDLNAV)
            {
                MEMHDG = _fsPMDGHDG.Value2;
            }
            if (_bankLimitMode)
            {
                _HDG = "r" + _bankLimit.ToString("00");
            }
            else
            {
                _HDG = MEMHDG.ToString("000");
            }
        }

        private void ALTaff()
        {
            if (!_LEDVNAV)
            {
                MEMALT = _fsPMDGALT.Value2;
            }
            _ALT = MEMALT.ToString("00000");
        }

        public override void DecALT(bool fast)
        {
            if (fast && _ALTrotLKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_ALTrotLKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_ALTrotLKey);
            }
        }

        public override void DecCRS(bool fast)
        {
            if (fast && _CRSLEFTrotLKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_CRSLEFTrotLKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_CRSLEFTrotLKey);
            }
        }

        public override void DecHDG(bool fast)
        {
            if (_bankLimitMode)
            {
                if (_bankLimit > 10)
                {
                    _bankLimit -= 5;
                }
                Utils.Keyboard.ShortcutFS(_bankLimitDec);
            }
            else
            {
                if (fast && _HDGrotLKeyFast != null)
                {
                    Utils.Keyboard.ShortcutFS(_HDGrotLKeyFast);
                }
                else
                {
                    Utils.Keyboard.ShortcutFS(_HDGrotLKey);
                }
            }
        }

        public override void DecIAS(bool fast)
        {
            if (fast && _IASrotLKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_IASrotLKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_IASrotLKey);
            }
        }

        public override void DecVS(bool fast)
        {
            if (fast && _VSrotLKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_VSrotLKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_VSrotLKey);
            }
        }

        public override void IncALT(bool fast)
        {
            if (fast && _ALTrotRKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_ALTrotRKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_ALTrotRKey);
            }
        }

        public override void IncCRS(bool fast)
        {
            if (fast && _CRSLEFTrotRKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_CRSLEFTrotRKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_CRSLEFTrotRKey);
            }
        }

        public override void IncHDG(bool fast)
        {
            if (_bankLimitMode)
            {
                if (_bankLimit < 30)
                {
                    _bankLimit += 5;
                }
                Utils.Keyboard.ShortcutFS(_bankLimitInc);
            }
            else
            {
                if (fast && _HDGrotRKeyFast != null)
                {
                    Utils.Keyboard.ShortcutFS(_HDGrotRKeyFast);
                }
                else
                {
                    Utils.Keyboard.ShortcutFS(_HDGrotRKey);
                }
            }
        }

        public override void IncIAS(bool fast)
        {
            if (fast && _IASrotRKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_IASrotRKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_IASrotRKey);
            }
        }

        public override void IncVS(bool fast)
        {
            if (fast && _VSrotRKeyFast != null)
            {
                Utils.Keyboard.ShortcutFS(_VSrotRKeyFast);
            }
            else
            {
                Utils.Keyboard.ShortcutFS(_VSrotRKey);
            }
        }

        public override void PressAltHLD()
        {
            Utils.Keyboard.ShortcutFS(_ALTHOLDbpKey);
        }

        public override void PressAltINTV()
        {
            Utils.Keyboard.ShortcutFS(_ALTIntvKey);
        }

        public override void PressAPP()
        {
            Utils.Keyboard.ShortcutFS(_APPbpKey);
        }

        public override void PressCmdA()
        {
            Utils.Keyboard.ShortcutFS(_CMDAbpKey);
        }

        public override void PressCmdB()
        {
            //Utils.Keyboard.Shortcut(_CMDBbpKey);
            _LEDCMDB = !_LEDCMDB;
            if (!_LEDCMDB)
            {
                _LEDCWSB = false;
            }
        }

        public override void PressCO()
        {
            Utils.Keyboard.ShortcutFS(_CObpKey);
        }

        public override void PressCwsA()
        {
            Utils.Keyboard.ShortcutFS(_CWSAbpKey);
        }

        public override void PressCwsB()
        {
            //Utils.Keyboard.Shortcut(_CWSBbpKey);
            _LEDCWSB = !_LEDCWSB;
            if (!_LEDCWSB)
            {
                _LEDCMDB = false;
            }
        }

        public override void PressDisengage()
        {
            Utils.Keyboard.ShortcutFS(_APDISCObpKey);
        }

        public override void PressHdgButton()
        {
            _bankLimitMode = !_bankLimitMode;
        }

        public override void PressHdgSEL()
        {
            Utils.Keyboard.ShortcutFS(_HDGbpKey);
        }

        public override void PressLNAV()
        {
            Utils.Keyboard.ShortcutFS(_LNAVbpKey);
        }

        public override void PressLvlCHG()
        {
            Utils.Keyboard.ShortcutFS(_LVLCHGbpKey);
        }

        public override void PressN1()
        {
            Utils.Keyboard.ShortcutFS(_N1bpKey);
        }

        public override void PressSpdINTV()
        {
            Utils.Keyboard.ShortcutFS(_SPDIntvKey);
        }

        public override void PressSPEED()
        {
            Utils.Keyboard.ShortcutFS(_SPEEDbpKey);
        }

        public override void PressVNAV()
        {
            Utils.Keyboard.ShortcutFS(_VNAVbpKey);
        }

        public override void PressVORLOC()
        {
            Utils.Keyboard.ShortcutFS(_VORLOCbpKey);
        }

        public override void PressVS()
        {
            Utils.Keyboard.ShortcutFS(_VSbpKey);
        }

        public override void SetSwitchAT(bool state)
        {
            if (state != _LEDAT)
            {
                Utils.Keyboard.ShortcutFS(_ATswitchKey, true);
            }
        }

        public override void SetSwitchFD(bool state)
        {
            if (state != _LEDMA)
            {
                Utils.Keyboard.ShortcutFS(_FDswitchKey, true);
            }
        }

        public override bool IsTestMode
        {
            get
            {
                return _LEDCMDB && _LEDCWSB;
            }
            protected set
            {
                
            }
        }
    }
}

