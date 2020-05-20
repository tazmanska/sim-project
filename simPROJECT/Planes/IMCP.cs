using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Planes
{
    interface IMCP : IPlaneDevice
    {
        /// <summary>
        /// aktualizacja stanów wyjść (LED)
        /// </summary> 
        void UpdateOutputs();

        #region Enkodery

        void IncCRS(bool fast);

        void DecCRS(bool fast);

        void IncIAS(bool fast);

        void DecIAS(bool fast);

        void IncALT(bool fast);

        void DecALT(bool fast);

        void IncHDG(bool fast);

        void DecHDG(bool fast);

        void IncVS(bool fast);

        void DecVS(bool fast);

        #endregion

        #region Przełączniki

        void SetSwitchAT(bool state);

        void SetSwitchFD(bool state);

        #endregion

        #region Przyciski

        void PressCO();

        void PressSpdINTV();

        void PressVNAV();

        void PressAltINTV();

        void PressLNAV();

        void PressVORLOC();

        void PressHdgSEL();

        void PressHdgButton();

        void PressLvlCHG();

        void PressSPEED();

        void PressN1();

        void PressVS();

        void PressAltHLD();

        void PressAPP();

        void PressDisengage();

        void PressCwsA();

        void PressCwsB();

        void PressCmdA();

        void PressCmdB();

        #endregion

        #region Wyświetlacze

        string GetCRS();

        string GetIAS();

        string GetHDG();

        string GetALT();

        string GetVS();

        #endregion

        #region Diody LED i diody w przyciskach

        bool GetLEDMA();

        bool GetLEDAT();

        bool GetLEDN1();

        bool GetLEDSPEED();

        bool GetLEDLVLCHG();

        bool GetLEDVNAV();

        bool GetLEDHDGSEL();

        bool GetLEDLNAV();

        bool GetLEDVORLOC();

        bool GetLEDAPP();

        bool GetLEDALTHLD();

        bool GetLEDVS();

        bool GetLEDCMDA();

        bool GetLEDCMDB();

        bool GetLEDCWSA();

        bool GetLEDCWSB();

        #endregion
    }
}
