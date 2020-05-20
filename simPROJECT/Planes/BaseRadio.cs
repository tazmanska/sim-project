using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    class BaseRadio : IRadio
    {
        #region IRadio Members

        public virtual void IncLeft()
        {
        }

        public virtual void DecLeft()
        {
        }

        public virtual void IncRight()
        {
        }

        public virtual void DecRight()
        {
        }

        public virtual void IncMode()
        {
        }

        public virtual void DecMode()
        {
        }

        public virtual bool ModeC
        {
            get;
            set;
        }

        public virtual void PressTFR()
        {
        }

        public virtual void PressTEST()
        {
        }

        public virtual void PressModeButton()
        {
        }

        public virtual void PressLeftButton()
        {
        }

        public virtual void PressRightButton()
        {
        }

        public virtual string GetActive()
        {
            return "";
        }

        public virtual string GetStandby()
        {
            return "";
        }

        public virtual RadioMode Mode
        {
            get;
            set;
        }

        #endregion

        #region IPlaneDevice Members

        public virtual void ResetDevice()
        {
            ModeC = false;
            Mode = RadioMode.None;
        }

        #endregion

        #region IPlaneDevice Members


        public virtual void ResetUpdate()
        {
        }

        public virtual bool NeedUpdate
        {
            get { return false; }
        }

        public virtual bool IsTestMode
        {
            get { return false; }
        }

        public virtual void Startup()
        { }

        public virtual void Close()
        { }

        public virtual bool HasData
        {
            get { return true; }
        }

        public virtual string Error
        {
            get { return ""; }
        }

        public simPROJECT.Configuration.DeviceConfiguration DeviceConfiguration
        {
            get;
            set;
        }

        #endregion
    }
}
