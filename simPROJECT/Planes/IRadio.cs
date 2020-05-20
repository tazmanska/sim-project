using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    interface IRadio : IPlaneDevice
    {
        #region Enkodery

        void IncLeft();

        void DecLeft();

        void IncRight();

        void DecRight();

        void IncMode();

        void DecMode();

        #endregion

        #region Przełączniki

        bool ModeC
        {
            get;
            set;
        }

        #endregion

        #region Przyciski

        void PressTFR();

        void PressTEST();

        void PressModeButton();

        void PressLeftButton();

        void PressRightButton();

        #endregion

        #region Wyświetlacze

        string GetActive();

        string GetStandby();

        #endregion

        #region Dodatki

        RadioMode Mode
        {
            get;
            set;
        }

        #endregion
    }
}
