using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    abstract class BasePlane : IPlane
    {
        protected BasePlane(PlaneType type) : this(type, PlaneFactory.PlaneTypeToName(type))
        {
        }

        protected BasePlane(PlaneType type, string name)
        {
            Type = type;
            Name = name;
        }

        #region IPlane Members

        public virtual IRadio GetRadio()
        {
            return new DummyPlane.DummyRadio();
        }

        public virtual IMCP GetMCP()
        {
            return new DummyPlane.DummyMCP();
        }

        public string Name
        {
            get;
            private set;
        }

        public PlaneType Type
        {
            get;
            private set;
        }

        public void ResetDevice()
        {
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

        public virtual bool HasData
        {
            get { return true; }
        }

        public virtual string Error
        {
            get { return ""; }
        }

        public virtual void Startup()
        {
        }

        public virtual void Close()
        {
        }

        public simPROJECT.Configuration.DeviceConfiguration DeviceConfiguration
        {
            get;
            set;
        }

        #endregion
    }
}
