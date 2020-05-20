using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes.TestPlane
{
    class TestMCP : BaseMCP
    {
        private bool _cmdA = false;
        private bool _cmdB = false;
        private bool _cwsA = false;
        private bool _cwsB = false;
        private bool _hdgSel = false;
        private bool _altHld = false;
        private bool _app = false;
        private bool _lvlChg = false;
        private bool _at = false;
        private bool _speed = false;
        private bool _ma = false;
        private bool _n1 = false;
        private bool _lnav = false;
        private bool _vnav = false;
        private bool _vs = false;
        private bool _vorloc = false;
        private bool _rangeMode = false;
        
        private int _crs = 0;
        private int _hdg = 0;
        private int _ias = 100;
        private float _mach = 0.6f;
        private bool _isMach = false;
        private int _alt = 0;
        private int _vsFt = 0;
        private int _range = 10;

        public override void UpdateOutputs()
        {
            HasData = true;
        }

        public override void ResetDevice()
        {
            base.ResetDevice();

            _cmdA = false;
            _cmdB = false;
            _cwsA = false;
            _cwsB = false;
            _hdgSel = false;
            _altHld = false;
            _app = false;
            _lvlChg = false;
            _at = false;
            _speed = false;
            _ma = false;
            _n1 = false;
            _lnav = false;
            _vnav = false;
            _vs = false;
            _vorloc = false;
            _rangeMode = false;

            _crs = 0;
            _hdg = 0;
            _ias = 100;
            _mach = 0.6f;
            _isMach = false;
            _alt = 0;
            _vsFt = 0;
            _range = 10;
        }

        public override bool IsTestMode
        {
            get
            {
                return _cmdA && _cmdB && _cwsA && _cwsB;
            }
        }

        #region enkodery

        public override void DecALT(bool fast)
        {
            _alt -= fast ? 1000 : 100;
            if (_alt < 0)
            {
                _alt = 0;
            }
        }

        public override void DecCRS(bool fast)
        {
            _crs -= fast ? 10 : 1;
            if (_crs < 0)
            {
                _crs = 359;
            }
        }

        public override void DecHDG(bool fast)
        {
            if (_rangeMode)
            {
                if (_range > 10)
                {
                    _range -= 5;
                }
            }
            else
            {
                _hdg -= fast ? 10 : 1;
                if (_hdg < 0)
                {
                    _hdg = 359;
                }
            }
        }

        public override void DecIAS(bool fast)
        {
            if (_isMach)
            {
                _mach -= fast ? 0.1f : 0.01f;
                if (_mach < 0.6f)
                {
                    _mach = 0.6f;
                }
            }
            else
            {
                _ias -= fast ? 10 : 1;
                if (_ias < 100)
                {
                    _ias = 100;
                }
            }
        }

        public override void DecVS(bool fast)
        {
            _vsFt -= _vsFt < 1000 ? 50 : 100;
            if (_vsFt < -7900)
            {
                _vsFt = -7900;
            }
        }

        public override void IncALT(bool fast)
        {
            _alt += fast ? 1000 : 100;
            if (_alt > 50000)
            {
                _alt = 50000;
            }
        }

        public override void IncCRS(bool fast)
        {
            _crs += fast ? 10 : 1;
            if (_crs > 359)
            {
                _crs = 0;
            }
        }

        public override void IncHDG(bool fast)
        {
            if (_rangeMode)
            {
                if (_range < 30)
                {
                    _range += 5;
                }
            }
            else
            {
                _hdg += fast ? 10 : 1;
                if (_hdg > 359)
                {
                    _hdg = 0;
                }
            }
        }

        public override void IncIAS(bool fast)
        {
            if (_isMach)
            {
                _mach += fast ? 0.1f : 0.01f;
                if (_mach > 0.82f)
                {
                    _mach = 0.82f;
                }
            }
            else
            {
                _ias += fast ? 10 : 1;
                if (_ias > 340)
                {
                    _ias = 340;
                }
            }
        }

        public override void IncVS(bool fast)
        {
            _vsFt += _vsFt < 1000 ? 50 : 100;
            if (_vsFt > 6000)
            {
                _vsFt = 6000;
            }
        }

        #endregion

        #region przyciski

        public override void SetSwitchAT(bool state)
        {
            _at = state;
        }

        public override void SetSwitchFD(bool state)
        {
            _ma = state;
        }

        public override void PressAltHLD()
        {
            _altHld = !_altHld;
        }

        public override void PressAPP()
        {
            _app = !_app;
        }

        public override void PressAltINTV()
        {
            _alt = 0;
        }

        public override void PressCO()
        {
            _isMach = !_isMach;
        }

        public override void PressDisengage()
        {
            _cmdA = _cmdB = _cwsA = _cwsB = false;
        }

        public override void PressHdgButton()
        {
            _rangeMode = !_rangeMode;
        }

        public override void PressHdgSEL()
        {
            _hdgSel = !_hdgSel;
        }

        public override void PressLNAV()
        {
            _lnav = !_lnav;
        }

        public override void PressLvlCHG()
        {
            _lvlChg = !_lvlChg;
        }

        public override void PressN1()
        {
            _n1 = !_n1;
        }

        public override void PressSpdINTV()
        {
            _ias = 100;
            _mach = 0.6f;
        }

        public override void PressSPEED()
        {
            _speed = !_speed;
        }

        public override void PressVNAV()
        {
            _vnav = !_vnav;
        }

        public override void PressVORLOC()
        {
            _vorloc = !_vorloc;
        }

        public override void PressVS()
        {
            _vs = !_vs;
        }

        public override void PressCmdA()
        {
            _cmdA = !_cmdA;
        }

        public override void PressCmdB()
        {
            _cmdB = !_cmdB;
        }

        public override void PressCwsA()
        {
            _cwsA = !_cwsA;
        }

        public override void PressCwsB()
        {
            _cwsB = !_cwsB;
        }

        #endregion

        #region diody

        public override bool GetLEDALTHLD()
        {
            return _altHld || IsTestMode;
        }

        public override bool GetLEDAPP()
        {
            return _app || IsTestMode;
        }

        public override bool GetLEDAT()
        {
            return _at || IsTestMode;
        }

        public override bool GetLEDHDGSEL()
        {
            return _hdgSel || IsTestMode;
        }

        public override bool GetLEDLNAV()
        {
            return _lnav || IsTestMode;
        }

        public override bool GetLEDLVLCHG()
        {
            return _lvlChg || IsTestMode;
        }

        public override bool GetLEDMA()
        {
            return _ma || IsTestMode;
        }

        public override bool GetLEDN1()
        {
            return _n1 || IsTestMode;
        }

        public override bool GetLEDSPEED()
        {
            return _speed || IsTestMode;
        }

        public override bool GetLEDVNAV()
        {
            return _vnav || IsTestMode;
        }

        public override bool GetLEDVORLOC()
        {
            return _vorloc || IsTestMode;
        }

        public override bool GetLEDVS()
        {
            return _vs || IsTestMode;
        }

        public override bool GetLEDCMDA()
        {
            return _cmdA || IsTestMode;
        }

        public override bool GetLEDCMDB()
        {
            return _cmdB || IsTestMode;
        }

        public override bool GetLEDCWSA()
        {
            return _cwsA || IsTestMode;
        }

        public override bool GetLEDCWSB()
        {
            return _cwsB || IsTestMode;
        }

        #endregion

        #region wyświetlacze

        public override string GetALT()
        {
            if (IsTestMode)
            {
                return "8.8.8.8.8.";
            }
            else
            {
                return _alt.ToString("000");
            }
        }

        public override string GetCRS()
        {
            if (IsTestMode)
            {
                return "8.8.8.";
            }
            else
            {
                return _crs.ToString("000");
            }
        }

        public override string GetHDG()
        {
            if (IsTestMode)
            {
                return "8.8.8.";
            }
            else
            {
                if (_rangeMode)
                {
                    return "r" + _range.ToString();
                }
                else
                {
                    return _hdg.ToString("000");
                }
            }
        }

        public override string GetIAS()
        {
            if (IsTestMode)
            {
                return "8.8.8.8.";
            }
            else
            {
                if (_isMach)
                {
                    return _mach.ToString("0.00");
                }
                else
                {
                    return _ias.ToString();
                }
            }
        }

        public override string GetVS()
        {
            if (IsTestMode)
            {
                return "8.8.8.8.8.";
            }
            else
            {
                return _vsFt.ToString();
            }
        }

        #endregion
    }
}
