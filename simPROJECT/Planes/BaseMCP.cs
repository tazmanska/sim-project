using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    class BaseMCP : IMCP
    {
        protected bool _LEDMA = false;
        protected bool _LEDAT = false;
        protected bool _LEDN1 = false;
        protected bool _LEDSPEED = false;
        protected bool _LEDLVLCHG = false;
        protected bool _LEDVNAV = false;
        protected bool _LEDHDGSEL = false;
        protected bool _LEDLNAV = false;
        protected bool _LEDVORLOC = false;
        protected bool _LEDAPP = false;
        protected bool _LEDALTHLD = false;
        protected bool _LEDVS = false;
        protected bool _LEDCMDA = false;
        protected bool _LEDCMDB = false;
        protected bool _LEDCWSA = false;
        protected bool _LEDCWSB = false;

        protected string _CRS = "";
        protected string _IAS = "";
        protected string _HDG = "";
        protected string _ALT = "";
        protected string _VS = "";

        #region IMCP Members

        public virtual void IncCRS(bool fast)
        {
        }

        public virtual void DecCRS(bool fast)
        {
        }

        public virtual void IncIAS(bool fast)
        {
        }

        public virtual void DecIAS(bool fast)
        {
        }

        public virtual void IncALT(bool fast)
        {
        }

        public virtual void DecALT(bool fast)
        {
        }

        public virtual void IncHDG(bool fast)
        {
        }

        public virtual void DecHDG(bool fast)
        {
        }

        public virtual void IncVS(bool fast)
        {
        }

        public virtual void DecVS(bool fast)
        {
        }

        public virtual void PressCO()
        {
        }

        public virtual void PressSpdINTV()
        {
        }

        public virtual void PressVNAV()
        {
        }

        public virtual void PressAltINTV()
        {
        }

        public virtual void PressLNAV()
        {
        }

        public virtual void PressVORLOC()
        {
        }

        public virtual void PressHdgSEL()
        {
        }

        public virtual void PressHdgButton()
        {
        }

        public virtual void PressLvlCHG()
        {
        }

        public virtual void PressSPEED()
        {
        }

        public virtual void PressN1()
        {
        }

        public virtual void PressVS()
        {
        }

        public virtual void PressAltHLD()
        {
        }

        public virtual void PressAPP()
        {
        }

        public virtual void PressDisengage()
        {
        }

        public virtual void PressCwsA()
        {
        }

        public virtual void PressCwsB()
        {
        }

        public virtual void PressCmdA()
        {
        }

        public virtual void PressCmdB()
        {
        }

        public virtual string GetCRS()
        {
            return _CRS;
        }

        public virtual string GetIAS()
        {
            return _IAS;
        }

        public virtual string GetHDG()
        {
            return _HDG;
        }

        public virtual string GetALT()
        {
            return _ALT;
        }

        public virtual string GetVS()
        {
            return _VS;
        }

        #endregion

        #region IPlaneDevice Members

        public virtual void ResetDevice()
        {
        }

        #endregion

        #region IMCP Members

        public virtual void UpdateOutputs()
        {
        }

        public virtual void SetSwitchAT(bool state)
        {
        }

        public virtual void SetSwitchFD(bool state)
        {
        }

        public virtual bool GetLEDMA()
        {
            return _LEDMA;
        }

        public virtual bool GetLEDAT()
        {
            return _LEDAT;
        }

        public virtual bool GetLEDN1()
        {
            return _LEDN1;
        }

        public virtual bool GetLEDSPEED()
        {
            return _LEDSPEED;
        }

        public virtual bool GetLEDLVLCHG()
        {
            return _LEDLVLCHG;
        }

        public virtual bool GetLEDVNAV()
        {
            return _LEDVNAV;
        }

        public virtual bool GetLEDHDGSEL()
        {
            return _LEDHDGSEL;
        }

        public virtual bool GetLEDLNAV()
        {
            return _LEDLNAV;
        }

        public virtual bool GetLEDVORLOC()
        {
            return _LEDVORLOC;
        }

        public virtual bool GetLEDAPP()
        {
            return _LEDAPP;
        }

        public virtual bool GetLEDALTHLD()
        {
            return _LEDALTHLD;
        }

        public virtual bool GetLEDVS()
        {
            return _LEDVS;
        }

        public virtual bool GetLEDCMDA()
        {
            return _LEDCMDA;
        }

        public virtual bool GetLEDCMDB()
        {
            return _LEDCMDB;
        }

        public virtual bool GetLEDCWSA()
        {
            return _LEDCWSA;
        }

        public virtual bool GetLEDCWSB()
        {
            return _LEDCWSB;
        }

        #endregion

        #region IPlaneDevice Members

        public virtual void ResetUpdate()
        {
            NeedUpdate = false;
        }

        public virtual bool NeedUpdate
        {
            get;
            protected set;
        }

        public virtual bool IsTestMode
        {
            get;
            protected set;
        }

        public virtual void Startup()
        {
            _LEDMA = false;
            _LEDAT = false;
            _LEDN1 = false;
            _LEDSPEED = false;
            _LEDLVLCHG = false;
            _LEDVNAV = false;
            _LEDHDGSEL = false;
            _LEDLNAV = false;
            _LEDVORLOC = false;
            _LEDAPP = false;
            _LEDALTHLD = false;
            _LEDVS = false;
            _LEDCMDA = false;
            _LEDCMDB = false;
            _LEDCWSA = false;
            _LEDCWSB = false;

            _CRS = "";
            _IAS = "";
            _HDG = "";
            _ALT = "";
            _VS = "";
        }

        public virtual void Close()
        { }

        public bool HasData
        {
            get;
            protected set;
        }

        public string Error
        {
            get;
            protected set;
        }

        public simPROJECT.Configuration.DeviceConfiguration DeviceConfiguration
        {
            get;
            set;
        }

        #endregion
    }
}
