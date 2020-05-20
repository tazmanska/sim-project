using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using simPROJECT.Configuration;

namespace simPROJECT.Devices
{
    class MultiradioV1 : Device
    {
        public const string SN_PREFIX = "S1";
        public const DeviceType TYPE = DeviceType.MultiradioV1;

        public MultiradioV1(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo)
            : base(deviceInfo, "MultiRadio (Rotary Switch)", TYPE)
        {
            // utworzenie drivera
            _driver = new FTD2XX_NET.FTDI();

            _activeDictionary.Add('.', 1);
            _activeDictionary.Add('0', 238);
            _activeDictionary.Add('1', 132);
            _activeDictionary.Add('2', 186);
            _activeDictionary.Add('3', 182);
            _activeDictionary.Add('4', 212);
            _activeDictionary.Add('5', 118);
            _activeDictionary.Add('6', 126);
            _activeDictionary.Add('7', 164);
            _activeDictionary.Add('8', 254);
            _activeDictionary.Add('9', 246);

            _activeDictionary.Add('H', 220);
            _activeDictionary.Add('E', 122);
            _activeDictionary.Add('L', 74);
            _activeDictionary.Add('O', 238);
            _activeDictionary.Add('r', 24);
            _activeDictionary.Add('o', 30);
            _activeDictionary.Add('-', 16);
            _activeDictionary.Add('t', 90);
            _activeDictionary.Add('S', 118);
            _activeDictionary.Add('d', 158);
            _activeDictionary.Add('_', 2);
            _activeDictionary.Add('I', 132);
            _activeDictionary.Add('n', 28);
            _activeDictionary.Add('m', 28);
            _activeDictionary.Add('c', 26);
            _activeDictionary.Add('b', 94);
            _activeDictionary.Add('Y', 216);
            _activeDictionary.Add('C', 106);
            _activeDictionary.Add('q', 244);
            _activeDictionary.Add('h', 92);
            _activeDictionary.Add('N', 236);

            _standbyDictionary.Add('.', 1);
            _standbyDictionary.Add('0', 246);
            _standbyDictionary.Add('1', 192);
            _standbyDictionary.Add('2', 94);
            _standbyDictionary.Add('3', 218);
            _standbyDictionary.Add('4', 232);
            _standbyDictionary.Add('5', 186);
            _standbyDictionary.Add('6', 190);
            _standbyDictionary.Add('7', 208);
            _standbyDictionary.Add('8', 254);
            _standbyDictionary.Add('9', 250);

            _standbyDictionary.Add('r', 12);
            _standbyDictionary.Add('A', 252);
            _standbyDictionary.Add('d', 206);
            _standbyDictionary.Add('i', 4);
            _standbyDictionary.Add('o', 142);
            _standbyDictionary.Add('F', 60);
            _standbyDictionary.Add('S', 186);
            _standbyDictionary.Add('n', 140);
            _standbyDictionary.Add('-', 8);
            _standbyDictionary.Add('t', 46);
            _standbyDictionary.Add('H', 236);
            _standbyDictionary.Add('E', 62);
            _standbyDictionary.Add('L', 38);
            _standbyDictionary.Add('O', 246);
            _standbyDictionary.Add('_', 2);
            _standbyDictionary.Add('I', 192);
            _standbyDictionary.Add('q', 248);
            _standbyDictionary.Add('h', 172);
            _standbyDictionary.Add('N', 116);
            _standbyDictionary.Add('C', 54);

            _com1Variables = new simPROJECT.FS.FSVariable[2] 
            {
                new FS.BCDRadioVariable(0x034e),
                new FS.BCDRadioVariable(0x311a)
            };

            _com2Variables = new simPROJECT.FS.FSVariable[2] 
            {
                new FS.BCDRadioVariable(0x3118),
                new FS.BCDRadioVariable(0x311c)
            };

            _nav1Variables = new simPROJECT.FS.FSVariable[2] 
            {
                new FS.BCDRadioVariable(0x0350),
                new FS.BCDRadioVariable(0x311e)
            };

            _nav2Variables = new simPROJECT.FS.FSVariable[2] 
            {
                new FS.BCDRadioVariable(0x0352),
                new FS.BCDRadioVariable(0x3120)
            };

            _adf1Variables = new simPROJECT.FS.FSVariable[2]
            {
                new FS.ADFVariable(0x034c, 0x0356),
                null
            };

            _adf1Variables[1] = ((FS.ADFVariable)_adf1Variables[0]).SecondVariable;

            _adf2Variables = new simPROJECT.FS.FSVariable[2]
            {
                new FS.ADFVariable(0x02d4, 0x02d6),
                null
            };

            _adf2Variables[1] = ((FS.ADFVariable)_adf2Variables[0]).SecondVariable;

            _dmeVariables = new simPROJECT.FS.FSVariable[6]
            {
                new FS.DMEDistanceVariable(0x0300),
                new FS.DMESpeedVariable(0x0302),
                new FS.DMETimeVariable(0x0304),
                new FS.DMEDistanceVariable(0x0306),
                new FS.DMESpeedVariable(0x0308),
                new FS.DMETimeVariable(0x030a)
            };

            _xpdrVariables = new simPROJECT.FS.FSVariable[1]
            {
                new FS.XPDRVariable()
            };

            _controlVariables = new simPROJECT.FS.FSVariable[1]
            {
                _controlVariable
            };

            _comTransmitVariables = new simPROJECT.FS.FSVariable[1] { _comTransmitVariable };

            _xpdrModeVariables = new simPROJECT.FS.FSVariable[1] { _xpdrModeVariable };

            _nav1CrsVariables = new simPROJECT.FS.FSVariable[2] 
            {
                new FS.BCDRadioVariable(0x0350),
                _nav1Crs
            };

            _nav2CrsVariables = new simPROJECT.FS.FSVariable[2] 
            {
                new FS.BCDRadioVariable(0x0352),
                _nav2Crs
            };
        }

        private Dictionary<char, byte> _activeDictionary = new Dictionary<char, byte>();
        private Dictionary<char, byte> _standbyDictionary = new Dictionary<char, byte>();

        private FS.SimpleVariable _nav1Crs = new FS.SimpleVariable(0x0c4e, false, 2);
        private FS.SimpleVariable _nav1CrsWrite = new FS.SimpleVariable(0x0c4e, true, 2);

        private FS.SimpleVariable _nav2Crs = new FS.SimpleVariable(0x0c5e, false, 2);
        private FS.SimpleVariable _nav2CrsWrite = new FS.SimpleVariable(0x0c5e, true, 2);

        private FS.BCDRadioVariable _com1StbyWrite = new simPROJECT.FS.BCDRadioVariable(0x311a);
        private FS.BCDRadioVariable _com2StbyWrite = new simPROJECT.FS.BCDRadioVariable(0x311c);

        private FS.FSVariable[] _com1Variables = null;
        private FS.FSVariable[] _com2Variables = null;
        private FS.FSVariable[] _nav1Variables = null;
        private FS.FSVariable[] _nav1CrsVariables = null;
        private FS.FSVariable[] _nav2Variables = null;
        private FS.FSVariable[] _nav2CrsVariables = null;
        private FS.FSVariable[] _adf1Variables = null;
        private FS.FSVariable[] _adf2Variables = null;
        private FS.FSVariable[] _dmeVariables = null;
        private FS.FSVariable[] _xpdrVariables = null;
        private FS.SimpleVariable _qnhVariable = new simPROJECT.FS.SimpleVariable(0x0330)
        {
            ForWriting = false,
            ValueSize = 2
        };
        

        private FTD2XX_NET.FTDI _driver = null;

        private byte _keysDeviceID = 161;
        private byte _activeDisplayID = 21;
        private byte _standbyDisplayID = 22;

        protected enum Mode
        {
            None = 0x00,
            DME = 0x01,
            COM1 = 0x02,
            COM2 = 0x04,
            NAV1 = 0x08,
            NAV2 = 0x10,
            ADF1 = 0x20,
            ADF2 = 0x40,
            XPDR = 0x80,
            QNH = 0x0100,
            CRS1 = 0x0200,
            CRS2 = 0x0400,
        }

        protected enum DMEMode
        {
            Distance = 0x00,
            Speed = 0x01,
            Time = 0x02
        }

        private DMEMode DecDMEMode(DMEMode mode)
        {
            switch (mode)
            {
                case DMEMode.Distance:
                    return DMEMode.Time;

                case DMEMode.Speed:
                    return DMEMode.Distance;

                default:
                    return DMEMode.Speed;
            }
        }

        private DMEMode IncDMEMode(DMEMode mode)
        {
            switch (mode)
            {
                case DMEMode.Distance:
                    return DMEMode.Speed;

                case DMEMode.Speed:
                    return DMEMode.Time;

                default:
                    return DMEMode.Distance;
            }
        }

        private DMEMode _dme1Mode = DMEMode.Distance;
        private DMEMode _dme2Mode = DMEMode.Distance;

        private AutoResetEvent _receivedEvent = new AutoResetEvent(false);
        private AutoResetEvent _waitForData = new AutoResetEvent(false);

        private Thread _receivingThread = null;

        private Mode _currentMode = Mode.COM1;
        private bool _currentModeChanged = false;

        private byte _displayBrightness = 0;
        private byte _backlightBrightness = 0;

        private volatile bool _test = false;

        private volatile int _xpdrDigitEditing = 0;
        private volatile int _adf1DigitEditing = 0;
        private volatile int _adf2DigitEditing = 0;
        private volatile bool _xpdrIDENT = false;
        private volatile bool _xpdrModeC = false;

        protected override void WorkingThread()
        {
            Debug.WriteLine(string.Format("Wystartowanie wątka obsługującego urządzenie: {0}", this.ToString()));

            // wystartowanie współpracy z urządzeniem         
            _currentMode = Mode.COM1;
            byte[] activeDisplayData = new byte[5] { 0, 0, 0, 0, 0 };
            byte[] standbyDisplayData = new byte[5] { 0, 0, 0, 0, 0 };
            byte[] lastActiveDisplayData = new byte[5] { 0, 0, 0, 0, 0 };
            byte[] lastStandbyDisplayData = new byte[5] { 0, 0, 0, 0, 0 };
            _displayBrightness = ((MultiradioV1Configuration)Configuration).DisplayBrightness;
            _backlightBrightness = ((MultiradioV1Configuration)Configuration).BacklightBrightness;
            Array.Clear(_keys, 0, _keys.Length);
            _currentModeChanged = false;
            byte[] dataToActiveDisplay = new byte[5] { _activeDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 0, 0 };
            byte[] dataToStandbyDisplay = new byte[5] { _standbyDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 0, 0 };

            try
            {
                while (!NeedStop)
                {
                    // próba połączenia z urządzeniem
                    while (_driver == null || !_driver.IsOpen)
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            _driver = new FTD2XX_NET.FTDI();
                            if (_driver.OpenBySerialNumber(SerialNumber) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                            {
                                Debug.WriteLine(string.Format("Otwarto połączenie z urządzeniem: {0}", this));

                                if (_driver.ResetDevice() != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                {
                                    _driver.Close();
                                    _driver = null;
                                    continue;
                                }

                                // ustawienie parametrów połączenia
                                if (_driver.SetBaudRate(56700) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                {
                                    _driver.Close();
                                    _driver = null;
                                    continue;
                                }

                                if (_driver.SetDataCharacteristics(8, 2, 0) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                {
                                    _driver.Close();
                                    _driver = null;
                                    continue;
                                }

                                if (_driver.SetFlowControl(FTD2XX_NET.FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                {
                                    _driver.Close();
                                    _driver = null;
                                    continue;
                                }

                                Debug.WriteLine(string.Format("Inicjalizacja urządzenia: {0}", this));

                                // wysłanie rozkazu zatrzymania skanowania wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.STOP_SCAN });

                                // wysłanie rozkazu wyczyszczeniu konfiguracji wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.CLEAR_ENCODERS });

                                // wysłanie rozkazu konfiguracji enkoderów
                                byte[] encoders = GetEncoders();
                                for (int i = 0; i < encoders.Length; i++)
                                {
                                    WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, encoders[i], /* 1/1 - 0, 1/2 - 1, 1/4 - 2 */ 0 });
                                }
                                //WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, 2, 0 });
                                //WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, 3, 0 });

                                // wysłanie rozkazu skanowania wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.START_SCAN });

                                // odczekanie na raporty
                                Thread.Sleep(50);

                                // wysłanie rozkazu zatrzymania skanowania wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.STOP_SCAN });

                                // odczytanie wszystkich danych 
                                uint available = 0;
                                uint red = 0;
                                if (_driver.GetRxBytesAvailable(ref available) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                {
                                    _driver.Close();
                                    _driver = null;
                                    continue;
                                }
                                if (available > 0)
                                {
                                    byte[] tmp = new byte[available];
                                    if (_driver.Read(tmp, (uint)tmp.Length, ref red) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                                    {
                                        _driver.Close();
                                        _driver = null;
                                        continue;
                                    }
                                }

                                // wyczyszczenie wyświetlaczy i wyłączenie diod
                                WriteToDevice(_driver, new byte[3] { _activeDisplayID, 1, (byte)simOUTLEDsCommand.ALL_OFF });
                                WriteToDevice(_driver, new byte[3] { _standbyDisplayID, 1, (byte)simOUTLEDsCommand.ALL_OFF });

                                 // wysłanie rozkazu raportu wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.GET_KEYS });

                                Thread.Sleep(50);

                                // odczytanie raportów wejść
                                ReadKeysState(_driver, ref _keys);

                                ResetStates();

                                // ustawienie trybu multiradia
                                _currentMode = GetInitialMode(ref _keys);// (Mode)_keys[4];
                                _currentModeChanged = false;
                                _test = GetTestState(ref _keys); //false
                                _xpdrModeC = !GetXPDRStandByState(ref _keys);

                                // ustawienie COM transmit
                                SetCOMTransmit();

                                // zapalenie diody wskazującego tryb
                                //if (_backlightBrightness > 0)
                                {
                                    WriteToDevice(_driver, new byte[5] { _activeDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 4, ModeToLED(_currentMode) });
                                }

                                // ustawienie poziomu jasności
                                WriteToDevice(_driver, new byte[4] { _activeDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displayBrightness });
                                WriteToDevice(_driver, new byte[4] { _standbyDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displayBrightness });
                                WriteToDevice(_driver, new byte[4] { _activeDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | (_displayBrightness)) });

                                if (_backlightBrightness > 0)
                                {
                                    // wysłanie 
                                    //WriteToDevice(_driver, new byte[4] { _activeDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | (_backlightBrightness - 1)) });
                                    WriteToDevice(_driver, new byte[4] { _standbyDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | (_backlightBrightness - 1)) });

                                    // włączenie podświetlenia
                                    WriteToDevice(_driver, new byte[5] { _standbyDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 5, 255 });
                                }

                                // wyzerowanie informacji o stanie wyświetlaczy
                                Array.Clear(lastActiveDisplayData, 0, lastActiveDisplayData.Length);
                                Array.Clear(lastStandbyDisplayData, 0, lastStandbyDisplayData.Length);

                                // resetowanie zmiennych
                                _com1Variables[0].Reset();
                                _com1Variables[1].Reset();

                                _com2Variables[0].Reset();
                                _com2Variables[1].Reset();

                                _nav1Variables[0].Reset();
                                _nav1Variables[1].Reset();

                                _nav1CrsVariables[0].Reset();
                                _nav1CrsVariables[1].Reset();

                                _nav2Variables[0].Reset();
                                _nav2Variables[1].Reset();

                                _nav2CrsVariables[0].Reset();
                                _nav2CrsVariables[1].Reset();

                                _adf1Variables[0].Reset();
                                _adf1Variables[1].Reset();

                                _adf2Variables[0].Reset();
                                _adf2Variables[1].Reset();

                                _dmeVariables[0].Reset();
                                _dmeVariables[1].Reset();
                                _dmeVariables[2].Reset();
                                _dmeVariables[3].Reset();
                                _dmeVariables[4].Reset();
                                _dmeVariables[5].Reset();

                                _xpdrVariables[0].Reset();

                                _comTransmitVariables[0].Reset();

                                _dme1Mode = DMEMode.Distance;
                                _dme2Mode = DMEMode.Distance;

                                _xpdrDigitEditing = 0;
                                _adf1DigitEditing = 0;
                                _adf2DigitEditing = 0;

                                _xpdrIDENT = false;

                                // animacja
                                if (((MultiradioV1Configuration)Configuration).WelcomeAnimation)
                                {
                                    string hello = "         HELLO           ";
                                    int max = hello.Length - 10;
                                    for (int i = 0; i < max; i++)
                                    {
                                        CodeText(hello.Substring(0, 5), ref _activeDictionary, ref activeDisplayData);
                                        CodeText(hello.Substring(5, 5), ref _standbyDictionary, ref standbyDisplayData);
                                        WriteToDisplayIfNeeded(0, 0, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                                        WriteToDisplayIfNeeded(1, 1, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                                        WriteToDisplayIfNeeded(2, 2, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                                        WriteToDisplayIfNeeded(3, 3, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                                        WriteToDisplayIfNeeded(4, 5, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);

                                        WriteToDisplayIfNeeded(0, 0, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                                        WriteToDisplayIfNeeded(1, 1, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                                        WriteToDisplayIfNeeded(2, 2, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                                        WriteToDisplayIfNeeded(3, 3, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                                        WriteToDisplayIfNeeded(4, 4, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                                        hello = hello.Substring(1);
                                        Thread.Sleep(200);
                                    }

                                    // wyzerowanie informacji o stanie wyświetlaczy
                                    Array.Clear(lastActiveDisplayData, 0, lastActiveDisplayData.Length);
                                    Array.Clear(lastStandbyDisplayData, 0, lastStandbyDisplayData.Length);
                                }

                                // wysłanie rozkazu skanowania wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.START_SCAN });

                                // wystartowanie wątka oczekującego na zdarzenie odbioru danych
                                StartReceivingThread();

                                FTD2XX_NET.FTDI.FT_STATUS status = _driver.SetEventNotification(FTD2XX_NET.FTDI.FT_EVENTS.FT_EVENT_RXCHAR, _receivedEvent);

                                Debug.WriteLine(string.Format("Zakończono inicjalizację urządzenia: {0}", this));

                                break;
                            }
                        }
                        catch
                        {
                            if (_driver != null)
                            {
                                _driver.Close();
                                _driver = null;
                            }
                        }
                        if (NeedStop)
                        {
                            break;
                        }
                    }

                    if (NeedStop)
                    {
                        break;
                    }

                    // wysłanie informacji o trybie działania
                    if (_currentModeChanged && /*_backlightBrightness > 0 &&*/ _currentMode != Mode.None)
                    {
                        Debug.WriteLine(string.Format("Zmiana trybu urządzenia: {0}, tryb: {1}", this, _currentMode));

                        _currentModeChanged = false;
                        WriteToDevice(_driver, new byte[5] { _activeDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 4, ModeToLED(_currentMode) });

                    }

                    // wysłanie/odczytanie danych z FS
                    //bool ok =  FS.FSUIPC.FS.IsConnected && FS.FSUIPC.FS.Process(_variables);

                    if (_test && !_xpdrIDENT)
                    {
                        activeDisplayData[0] = activeDisplayData[1] = activeDisplayData[2] = activeDisplayData[3] = activeDisplayData[4] = 255;
                        standbyDisplayData[0] = standbyDisplayData[1] = standbyDisplayData[2] = standbyDisplayData[3] = standbyDisplayData[4] = 255;
                    }
                    else
                    {
                        bool ok = false;

                        // ustawienie danych wysyłanych do urządzenia
                        switch (_currentMode)
                        {
                            case Mode.COM1:
                                if (FS.FSUIPC.FS.Process(_com1Variables))
                                {
                                    CodeText(_com1Variables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                    CodeText(_com1Variables[1].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                    ok = true;
                                }
                                break;

                            case Mode.COM2:
                                if (FS.FSUIPC.FS.Process(_com2Variables))
                                {
                                    CodeText(_com2Variables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                    CodeText(_com2Variables[1].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                    ok = true;
                                }
                                break;

                            case Mode.NAV1:
                                if (FS.FSUIPC.FS.Process(_nav1Variables))
                                {
                                    CodeText(_nav1Variables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                    CodeText(_nav1Variables[1].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                    ok = true;
                                }
                                break;

                            case Mode.NAV2:
                                if (FS.FSUIPC.FS.Process(_nav2Variables))
                                {
                                    CodeText(_nav2Variables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                    CodeText(_nav2Variables[1].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                    ok = true;
                                }
                                break;

                            case Mode.CRS1:
                                if (FS.FSUIPC.FS.Process(_nav1CrsVariables))
                                {
                                    CodeText(_nav1CrsVariables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                    CodeText("C-" + _nav1CrsVariables[1].Value2.ToString("000"), ref _standbyDictionary, ref standbyDisplayData);
                                    ok = true;
                                }
                                break;

                            case Mode.CRS2:
                                if (FS.FSUIPC.FS.Process(_nav2CrsVariables))
                                {
                                    CodeText(_nav2CrsVariables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                    CodeText("C-" + _nav2CrsVariables[1].Value2.ToString("000"), ref _standbyDictionary, ref standbyDisplayData);
                                    ok = true;
                                }
                                break;

                            case Mode.ADF1:
                                if (FS.FSUIPC.FS.Process(_adf1Variables))
                                {
                                    string tmp = _adf1Variables[0].GetStringValue();
                                    CodeText(tmp, ref _activeDictionary, ref activeDisplayData);                                     
                                    CodeText(" ___._", ref _standbyDictionary, ref standbyDisplayData);
                                    tmp = tmp.Replace(".", "");
                                    standbyDisplayData[_adf1DigitEditing] = _standbyDictionary[tmp[4 - _adf1DigitEditing]];
                                    standbyDisplayData[1] |= _standbyDictionary['.'];
                                    ok = true;
                                }
                                break;

                            case Mode.ADF2:
                                if (FS.FSUIPC.FS.Process(_adf2Variables))
                                {
                                    string tmp = _adf2Variables[0].GetStringValue();
                                    CodeText(tmp, ref _activeDictionary, ref activeDisplayData);
                                    CodeText(" ___._", ref _standbyDictionary, ref standbyDisplayData);
                                    tmp = tmp.Replace(".", "");
                                    standbyDisplayData[_adf2DigitEditing] = _standbyDictionary[tmp[4 - _adf2DigitEditing]];
                                    standbyDisplayData[1] |= _standbyDictionary['.'];
                                    ok = true;
                                }
                                break;

                            case Mode.XPDR:
                                if (FS.FSUIPC.FS.Process(_xpdrVariables))
                                {
                                    string tmp = _xpdrVariables[0].GetStringValue();
                                    CodeText(tmp, ref _standbyDictionary, ref standbyDisplayData);
                                    standbyDisplayData[_xpdrDigitEditing] |= _standbyDictionary['.'];
                                    if (_xpdrIDENT)
                                    {
                                        CodeText("IdEnt", ref _activeDictionary, ref activeDisplayData);
                                    }
                                    else
                                    {
                                        if (_xpdrModeC)
                                        {
                                            CodeText("    C", ref _activeDictionary, ref activeDisplayData);
                                        }
                                        else
                                        {
                                            CodeText("  Stb", ref _activeDictionary, ref activeDisplayData);
                                        }
                                    }
                                    ok = true;
                                }
                                break;

                            case Mode.DME:
                                if (FS.FSUIPC.FS.Process(_dmeVariables))
                                {
                                    switch (_dme1Mode)
                                    {
                                        case DMEMode.Distance:
                                            CodeText(_dmeVariables[0].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                            break;

                                        case DMEMode.Speed:
                                            CodeText(_dmeVariables[1].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                            break;

                                        case DMEMode.Time:
                                            CodeText(_dmeVariables[2].GetStringValue(), ref _activeDictionary, ref activeDisplayData);
                                            break;
                                    }

                                    switch (_dme2Mode)
                                    {
                                        case DMEMode.Distance:
                                            CodeText(_dmeVariables[3].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                            break;

                                        case DMEMode.Speed:
                                            CodeText(_dmeVariables[4].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                            break;

                                        case DMEMode.Time:
                                            CodeText(_dmeVariables[5].GetStringValue(), ref _standbyDictionary, ref standbyDisplayData);
                                            break;
                                    }
                                    ok = true;
                                }
                                break;

                            case Mode.QNH:
                                if (FS.FSUIPC.FS.Process(_qnhVariable))
                                {
                                    string tmp = ((double)_qnhVariable.Value2 / 16d).ToString("0");
                                    CodeText(tmp, ref _standbyDictionary, ref standbyDisplayData);
                                    CodeText("  qNH", ref _activeDictionary, ref activeDisplayData);
                                }
                                break;

                            //default:
                            //    CodeText("Error", ref _activeDictionary, ref activeDisplayData);
                            //    CodeText("no. 01", ref _standbyDictionary, ref standbyDisplayData);
                            //    ok = true;
                            //    break;
                        }

                        if (!ok && !FS.FSUIPC.FS.IsConnected)
                        {
                            CodeText("Error", ref _activeDictionary, ref activeDisplayData);
                            CodeText("no FS", ref _standbyDictionary, ref standbyDisplayData);
                        }
                    }

                    // wysłanie danych
                    WriteToDisplayIfNeeded(0, 0, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                    WriteToDisplayIfNeeded(1, 1, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                    WriteToDisplayIfNeeded(2, 2, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                    WriteToDisplayIfNeeded(3, 3, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);
                    WriteToDisplayIfNeeded(4, 5, ref lastActiveDisplayData, ref activeDisplayData, ref dataToActiveDisplay);

                    WriteToDisplayIfNeeded(0, 0, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                    WriteToDisplayIfNeeded(1, 1, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                    WriteToDisplayIfNeeded(2, 2, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                    WriteToDisplayIfNeeded(3, 3, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);
                    WriteToDisplayIfNeeded(4, 4, ref lastStandbyDisplayData, ref standbyDisplayData, ref dataToStandbyDisplay);

                    _waitForData.WaitOne(40, false);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Błąd w wątku urządzenia: {0}, {1}", this, ex));
            }
            finally
            {
                Debug.WriteLine(string.Format("Zakończenie obsługi urządzenia: {0}", this));
                Running = false;

                StopReceivingThread();

                if (_driver != null && _driver.IsOpen)
                {
                    try
                    {
                        // zatrzymanie skanowania wejść
                        WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.STOP_SCAN });
                    }
                    catch { }

                    try
                    {
                        // wyczyszczenie wyświetlaczy
                        WriteToDevice(_driver, new byte[3] { _activeDisplayID, 1, (byte)simOUTLEDsCommand.ALL_OFF });
                        WriteToDevice(_driver, new byte[3] { _standbyDisplayID, 1, (byte)simOUTLEDsCommand.ALL_OFF });
                    }
                    catch { }

                    try
                    {
                        _driver.Close();
                    }
                    catch { }
                }

                ((MultiradioV1Configuration)Configuration).DisplayBrightness = _displayBrightness;
                ((MultiradioV1Configuration)Configuration).BacklightBrightness = _backlightBrightness;
            }
        }

        private void CodeText(string text, ref Dictionary<char, byte> dictionary, ref byte[] data)
        {
            Array.Clear(data, 0, data.Length);
            if (text == null || text.Length == 0)
            {
                return;
            }
            byte dotSegments = dictionary['.'];
            List<byte> result = new List<byte>();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '.' || c == ',')
                {
                    // kropka
                    if (result.Count == 0)
                    {
                        result.Add(dotSegments);
                        continue;
                    }
                    result[result.Count - 1] |= dotSegments;
                    continue;
                }
                byte b = 0;
                if (dictionary.TryGetValue(c, out b))
                {
                    result.Add(b);
                    continue;
                }
                result.Add(0);
            }
            result.Reverse();
            for (int i = 0; i < result.Count && i < data.Length; i++)
            {
                data[i] = result[i];
                //data[data.Length - i - 1] = result[i];
            }
        }

        private void StartReceivingThread()
        {
            StopReceivingThread();

            _receivingThread = new Thread(ReceivedData);
            _receivingThread.Start();
        }

        private void StopReceivingThread()
        {
            if (_receivingThread != null)
            {
                try
                {
                    _receivingThread.Abort();
                }
                catch
                { }
                _receivingThread = null;
            }
        }

        private byte[] _keys = new byte[5];
        private FS.ControlVariable _controlVariable = new simPROJECT.FS.ControlVariable();
        private FS.FSVariable[] _controlVariables = null;

        private FS.SimpleVariable _comTransmitVariable = new simPROJECT.FS.SimpleVariable(0x3122)
        {
            ForWriting = true,
            ValueSize = 1,
            Value1 = 0
        };
        private FS.FSVariable[] _comTransmitVariables = null;

        private FS.SimpleVariable _xpdrModeVariable = new simPROJECT.FS.SimpleVariable(0x7b91)
        {
            ForWriting = true,
            ValueSize = 1,
            Value1 = 0
        };
        private FS.FSVariable[] _xpdrModeVariables = null;

        private void SetCOMTransmit()
        {
            // włączenie/wyłączenie odbioru i transmisji com1 lub com2
            /*
                t- transmit , o - odsluch
                tryb com1  t-com1 o-com1
                tryb com2 t-com2 0-com2
                nav1 t - com1 o - com1
                nav2 t - com1 o - com1
                i tak dalej
             */
            _comTransmitVariable.ForWriting = false;
            FS.FSUIPC.FS.Process(_comTransmitVariables);
            switch (_currentMode)
            {
                case Mode.COM2:
                    // wyłączenie com1 transmit
                    _comTransmitVariable.Value1 &= 127;

                    // włączenie com2 transmit
                    _comTransmitVariable.Value1 |= 64;
                    break;

                default:
                    // włączenie com1 transmit
                    _comTransmitVariable.Value1 |= 128;

                    // wyłączenie com2 transmit
                    _comTransmitVariable.Value1 &= 191;
                    break;
            }
            _comTransmitVariable.ForWriting = true;
            FS.FSUIPC.FS.Process(_comTransmitVariables);
        }

        private void ReceivedData()
        {
            try
            {
                Debug.WriteLine(string.Format("Start wątka odczytującego stan wejść urządzenia: {0}", this));
                bool transmitButton = false;
                bool encoderButton = false;
                bool xpdrStandBy = false;
                bool leftEncoderLeft = false;
                bool leftEncoderRight = false;
                bool rightEncoderLeft = false;
                bool rightEndocerRight = false;
                uint available = 0;
                byte[] data = new byte[3];
                bool xpdrFirst = true;

                FS.SimpleVariable xpdrIDENTVariable = new simPROJECT.FS.SimpleVariable(0x7b93)
                {
                    ForWriting = true,
                    ValueSize = 1,
                    Value1 = 1
                };
                FS.FSVariable[] xpdrIDENTVariables = new simPROJECT.FS.FSVariable[1] { xpdrIDENTVariable };

                FS.SimpleVariable nav1Value = new simPROJECT.FS.SimpleVariable(0x311e)
                {
                    ForWriting = false,
                    ValueSize = 2
                };

                FS.SimpleVariable nav2Value = new simPROJECT.FS.SimpleVariable(0x3120)
                {
                    ForWriting = false,
                    ValueSize = 2
                };

                FS.SimpleVariable qnhVariable = new simPROJECT.FS.SimpleVariable(0x0330)
                {
                    ForWriting = false,
                    ValueSize = 2
                };

                byte lastBacklightBrightness = _backlightBrightness;
                byte lastDisplayBrightness = _displayBrightness;

                if (_xpdrModeC)
                {
                    _xpdrModeVariables[0].Value1 = 0;
                    FS.FSUIPC.FS.Process(_xpdrModeVariables);
                }

                while (!NeedStop)
                {
                    _receivedEvent.WaitOne();
                    while (_driver.GetRxBytesAvailable(ref available) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && available > 2)
                    {
                        uint red = 0;
                        if (_driver.Read(data, 3, ref red) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && red == 3)
                        {
#if DEBUG
                            Debug.WriteLine(string.Format("Odebrano dane z urządzenia ({0}): 0x{1} 0x{2} 0x{3}", this.ToString(), data[0].ToString("X2"), data[1].ToString("X2"), data[2].ToString("X2")));
#endif

                            int type = (data[1] >> 4) & 0x0f;
                            int index = data[1] & 0x0f;
                            if (type == 1 && index < _keys.Length)
                            {
                                _keys[index] = data[2];
                                bool signal = false;
                                long command = 0;

                                _test = GetTestState(ref _keys);// (_keys[0] & 0x04) == 0x04;

                                Mode newMode = GetMode(ref _keys, _currentMode);
                                if (_currentMode != Mode.QNH && _currentMode != Mode.CRS1 && _currentMode != Mode.CRS2 && _currentMode != newMode)// (Mode)_keys[4])
                                {
                                    _currentMode = newMode;
                                    _currentModeChanged = true;
                                    signal = true;

                                    // wyzerowanie zmiennej - sygnał, że trzeba zmienić tryb nadawania/odbioru
                                    _comTransmitVariable.Value1 = 0;
                                }

                                if (_comTransmitVariable.Value1 == 0)
                                {
                                    SetCOMTransmit();
                                    _comTransmitVariable.Value1 = 255;
                                }

                                bool state = GetTransmitState(ref _keys);// (_keys[0] & 0x01) > 0;
                                if (transmitButton != state)
                                {
                                    transmitButton = state;

                                    if (transmitButton)
                                    {
                                        // przełączenie częstotliwości ACTIVE<->STANDBY
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, przycisk TRF", this));

                                        switch (_currentMode)
                                        {
                                            case Mode.COM1:
                                                command = FS.ControlVariable.CMD_TFR_COM1;
                                                signal = true;
                                                break;

                                            case Mode.COM2:
                                                command = FS.ControlVariable.CMD_TFR_COM2;
                                                signal = true;
                                                break;

                                            case Mode.NAV1:
                                                command = FS.ControlVariable.CMD_TFR_NAV1;
                                                signal = true;
                                                break;

                                            case Mode.NAV2:
                                                command = FS.ControlVariable.CMD_TFR_NAV2;
                                                signal = true;
                                                break;

                                            case Mode.QNH:
                                                // ustawienie standardowego ciśnienia
                                                // wartość 16212
                                                qnhVariable.ForWriting = true;
                                                qnhVariable.Value2 = (short)(16212);
                                                FS.FSUIPC.FS.Process(qnhVariable);
                                                signal = true;
                                                break;
                                        }
                                    }
                                }

                                state = GetXPDRStandByState(ref _keys);
                                //state = (_keys[0] & 0x02) > 0;
                                if (xpdrStandBy != state || xpdrFirst)
                                {
                                    xpdrFirst = false;
                                    xpdrStandBy = state;
                                    _xpdrModeC = !xpdrStandBy;

                                    if (!xpdrStandBy)
                                    {
                                        // włączenie MODE C
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, przełączenie XPDR na tryb MODE C", this));

                                        // zapisanie wartości do offsetu
                                        _xpdrModeVariables[0].Value1 = 0;
                                    }
                                    else
                                    {
                                        // wyłączenie MODE C
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, przełączenie XPDR na tryb STANDBY", this));

                                        // zapisanie wartości do offsetu
                                        _xpdrModeVariables[0].Value1 = 1;
                                    }

                                    FS.FSUIPC.FS.Process(_xpdrModeVariables);
                                }

                                _xpdrIDENT = _currentMode == Mode.XPDR && !xpdrStandBy && _test;

                                state = GetEncoderButton(ref _keys);// (_keys[0] & 0x08) > 0;
                                if (encoderButton != state)
                                {
                                    encoderButton = state;
                                    Debug.WriteLine(string.Format("Urządzenie: {0}, przycisk enkodera: {1}", this, encoderButton));

                                    if (encoderButton)
                                    {
                                        if (_currentMode == Mode.QNH)
                                        {
                                            _currentMode = newMode;
                                            _currentModeChanged = true;
                                            signal = true;
                                        }
                                        else
                                        {
                                            switch (_currentMode)
                                            {
                                                case Mode.CRS1:
                                                    _currentMode = Mode.NAV1;
                                                    break;

                                                case Mode.CRS2:
                                                    _currentMode = Mode.NAV2;
                                                    break;

                                                case Mode.NAV1:
                                                    _currentMode = Mode.CRS1;
                                                    break;

                                                case Mode.NAV2:
                                                    _currentMode = Mode.CRS2;
                                                    break;

                                                default:
                                                    _currentMode = Mode.QNH;
                                                    break;
                                            }
                                            _currentModeChanged = true;
                                            signal = true;
                                        }
                                    }
                                }

                                state = GetRightEncoderLeft(ref _keys);// (_keys[0] & 0x10) > 0;
                                if (rightEncoderLeft != state)
                                {
                                    rightEncoderLeft = state;
                                    if (rightEncoderLeft)
                                    {
                                        // zmniejszenie części dziesiętnych
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, zmniejszenie części dziesiętnych", this));
                                        if (_test)
                                        {
                                            if (_backlightBrightness > 0)
                                            {
                                                _backlightBrightness--;
                                            }
                                        }
                                        else
                                        {
                                            switch (_currentMode)
                                            {
                                                case Mode.COM1:
                                                    //command = FS.ControlVariable.CMD_COM1_FRAC_DEC;
                                                    _com1StbyWrite.COMFractDec();                                                    
                                                    signal = true;
                                                    break;

                                                case Mode.COM2:
                                                    //command = FS.ControlVariable.CMD_COM2_FRAC_DEC;
                                                    _com2StbyWrite.COMFractDec();
                                                    signal = true;
                                                    break;

                                                case Mode.NAV1:
                                                    // sprawdzenie czy wartość FRAC == 00
                                                    nav1Value.ForWriting = false;
                                                    if (FS.FSUIPC.FS.Process(nav1Value) && ((nav1Value.Value2 & 0xff) == 0))
                                                    {
                                                        // ustawienie wartości xx95
                                                        nav1Value.Value2 |= 0x95;

                                                        nav1Value.ForWriting = true;
                                                        FS.FSUIPC.FS.Process(nav1Value);
                                                    }
                                                    else
                                                    {
                                                        command = FS.ControlVariable.CMD_NAV1_FRAC_DEC;
                                                        signal = true;
                                                    }
                                                    break;

                                                case Mode.NAV2:
                                                    // sprawdzenie czy wartość FRAC == 00
                                                    nav2Value.ForWriting = false;
                                                    if (FS.FSUIPC.FS.Process(nav2Value) && ((nav2Value.Value2 & 0xff) == 0))
                                                    {
                                                        // ustawienie wartości xx95
                                                        nav2Value.Value2 |= 0x95;

                                                        nav2Value.ForWriting = true;
                                                        FS.FSUIPC.FS.Process(nav2Value);
                                                    }
                                                    else
                                                    {
                                                        command = FS.ControlVariable.CMD_NAV2_FRAC_DEC;
                                                        signal = true;
                                                    }
                                                    break;

                                                case Mode.CRS1:
                                                    FS.FSUIPC.FS.Read(_nav1CrsWrite);
                                                    _nav1CrsWrite.Value2 -= 1;
                                                    if (_nav1CrsWrite.Value2 <= 0)
                                                    {
                                                        _nav1CrsWrite.Value2 = 359;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav1CrsWrite);
                                                    break;

                                                case Mode.CRS2:
                                                    FS.FSUIPC.FS.Read(_nav2CrsWrite);
                                                    _nav2CrsWrite.Value2 -= 1;
                                                    if (_nav2CrsWrite.Value2 <= 0)
                                                    {
                                                        _nav2CrsWrite.Value2 = 359;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav2CrsWrite);
                                                    break;

                                                case Mode.DME:
                                                    _dme2Mode = DecDMEMode(_dme2Mode);
                                                    signal = true;
                                                    break;

                                                case Mode.ADF1:
                                                    _adf1DigitEditing++;
                                                    if (_adf1DigitEditing > 3)
                                                    {
                                                        _adf1DigitEditing = 0;
                                                    }
                                                    signal = true;
                                                    break;

                                                case Mode.ADF2:
                                                    _adf2DigitEditing++;
                                                    if (_adf2DigitEditing > 3)
                                                    {
                                                        _adf2DigitEditing = 0;
                                                    }
                                                    signal = true;
                                                    break;

                                                case Mode.XPDR:
                                                    _xpdrDigitEditing++;
                                                    if (_xpdrDigitEditing > 3)
                                                    {
                                                        _xpdrDigitEditing = 0;
                                                    }
                                                    signal = true;
                                                    break;
                                            }
                                        }
                                    }
                                }

                                state = GetRightEndocerRight(ref _keys);// (_keys[0] & 0x20) > 0;
                                if (rightEndocerRight != state)
                                {
                                    rightEndocerRight = state;
                                    if (rightEndocerRight)
                                    {
                                        // zwiększenie części dziesiętnych
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, zwiększenie części dziesiętnych", this));

                                        if (_test)
                                        {
                                            if (_backlightBrightness < 11)
                                            {
                                                _backlightBrightness++;
                                            }
                                        }
                                        else
                                        {
                                            switch (_currentMode)
                                            {
                                                case Mode.COM1:
                                                    //command = FS.ControlVariable.CMD_COM1_FRAC_INC;
                                                    _com1StbyWrite.COMFractInc();
                                                    signal = true;
                                                    break;

                                                case Mode.COM2:
                                                    //command = FS.ControlVariable.CMD_COM2_FRAC_INC;
                                                    _com2StbyWrite.COMFractInc();
                                                    signal = true;
                                                    break;

                                                case Mode.NAV1:
                                                    command = FS.ControlVariable.CMD_NAV1_FRAC_INC;
                                                    signal = true;
                                                    break;

                                                case Mode.NAV2:
                                                    command = FS.ControlVariable.CMD_NAV2_FRAC_INC;
                                                    signal = true;
                                                    break;

                                                case Mode.CRS1:
                                                    FS.FSUIPC.FS.Read(_nav1CrsWrite);
                                                    _nav1CrsWrite.Value2 += 1;
                                                    if (_nav1CrsWrite.Value2 >= 360)
                                                    {
                                                        _nav1CrsWrite.Value2 = 0;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav1CrsWrite);
                                                    break;

                                                case Mode.CRS2:
                                                    FS.FSUIPC.FS.Read(_nav2CrsWrite);
                                                    _nav2CrsWrite.Value2 += 1;
                                                    if (_nav2CrsWrite.Value2 >= 360)
                                                    {
                                                        _nav2CrsWrite.Value2 = 0;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav2CrsWrite);
                                                    break;

                                                case Mode.DME:
                                                    _dme2Mode = IncDMEMode(_dme2Mode);
                                                    signal = true;
                                                    break;

                                                case Mode.ADF1:
                                                    _adf1DigitEditing--;
                                                    if (_adf1DigitEditing < 0)
                                                    {
                                                        _adf1DigitEditing = 3;
                                                    }
                                                    signal = true;
                                                    break;

                                                case Mode.ADF2:
                                                    _adf2DigitEditing--;
                                                    if (_adf2DigitEditing < 0)
                                                    {
                                                        _adf2DigitEditing = 3;
                                                    }
                                                    signal = true;
                                                    break;

                                                case Mode.XPDR:
                                                    _xpdrDigitEditing--;
                                                    if (_xpdrDigitEditing < 0)
                                                    {
                                                        _xpdrDigitEditing = 3;
                                                    }
                                                    signal = true;
                                                    break;
                                            }
                                        }
                                    }
                                }

                                state = GetLeftEncoderRight(ref _keys);// (_keys[0] & 0x40) > 0;
                                if (leftEncoderRight != state)
                                {
                                    leftEncoderRight = state;
                                    if (leftEncoderRight)
                                    {
                                        // zwiększenie części całkowitej
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, zwiększenie części całktowitej", this));

                                        if (_test)
                                        {
                                            if (_displayBrightness < 10)
                                            {
                                                _displayBrightness++;
                                            }
                                        }
                                        else
                                        {
                                            switch (_currentMode)
                                            {
                                                case Mode.COM1:
                                                    //command = FS.ControlVariable.CMD_COM1_WHOLE_INC;
                                                    _com1StbyWrite.COMWholeInc();
                                                    signal = true;
                                                    break;

                                                case Mode.COM2:
                                                    //command = FS.ControlVariable.CMD_COM2_WHOLE_INC;
                                                    _com2StbyWrite.COMWholeInc();
                                                    signal = true;
                                                    break;

                                                case Mode.NAV1:
                                                    command = FS.ControlVariable.CMD_NAV1_WHOLE_INC;
                                                    signal = true;
                                                    break;

                                                case Mode.NAV2:
                                                    command = FS.ControlVariable.CMD_NAV2_WHOLE_INC;
                                                    signal = true;
                                                    break;

                                                case Mode.CRS1:
                                                    FS.FSUIPC.FS.Read(_nav1CrsWrite);
                                                    _nav1CrsWrite.Value2 += 10;
                                                    if (_nav1CrsWrite.Value2 >= 360)
                                                    {
                                                        _nav1CrsWrite.Value2 -= 360;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav1CrsWrite);
                                                    break;

                                                case Mode.CRS2:
                                                    FS.FSUIPC.FS.Read(_nav2CrsWrite);
                                                    _nav2CrsWrite.Value2 += 10;
                                                    if (_nav2CrsWrite.Value2 >= 360)
                                                    {
                                                        _nav2CrsWrite.Value2 -= 360;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav2CrsWrite);
                                                    break;

                                                case Mode.DME:
                                                    _dme1Mode = IncDMEMode(_dme1Mode);
                                                    signal = true;
                                                    break;

                                                case Mode.ADF1:
                                                    switch (_adf1DigitEditing)
                                                    {
                                                        case 0: // 0000.X
                                                            command = FS.ControlVariable.CMD_ADF1_FRAC_INC;
                                                            signal = true;
                                                            break;

                                                        case 1: // 000X.0
                                                            command = FS.ControlVariable.CMD_ADF1_1_INC;
                                                            signal = true;
                                                            break;

                                                        case 2: // 00X0.0
                                                            command = FS.ControlVariable.CMD_ADF1_10_INC;
                                                            signal = true;
                                                            break;

                                                        case 3: // 0X00.0
                                                            command = FS.ControlVariable.CMD_ADF1_100_INC;
                                                            signal = true;
                                                            break;
                                                    }
                                                    break;

                                                case Mode.ADF2:
                                                    switch (_adf2DigitEditing)
                                                    {
                                                        case 0: // 0000.X
                                                            command = FS.ControlVariable.CMD_ADF2_FRAC_INC;
                                                            signal = true;
                                                            break;

                                                        case 1: // 000X.0
                                                            command = FS.ControlVariable.CMD_ADF2_1_INC;
                                                            signal = true;
                                                            break;

                                                        case 2: // 00X0.0
                                                            command = FS.ControlVariable.CMD_ADF2_10_INC;
                                                            signal = true;
                                                            break;

                                                        case 3: // 0X00.0
                                                            command = FS.ControlVariable.CMD_ADF2_100_INC;
                                                            signal = true;
                                                            break;
                                                    }
                                                    break;

                                                case Mode.XPDR:
                                                    switch (_xpdrDigitEditing)
                                                    {
                                                        case 0: // 000X
                                                            command = FS.ControlVariable.CMD_XPDR_1_INC;
                                                            signal = true;
                                                            break;

                                                        case 1: // 00X0
                                                            command = FS.ControlVariable.CMD_XPDR_10_INC;
                                                            signal = true;
                                                            break;

                                                        case 2: // 0X00
                                                            command = FS.ControlVariable.CMD_XPDR_100_INC;
                                                            signal = true;
                                                            break;

                                                        case 3: // X000
                                                            command = FS.ControlVariable.CMD_XPDR_1000_INC;
                                                            signal = true;
                                                            break;
                                                    }
                                                    break;

                                                case Mode.QNH:
                                                    // odczytanie qnh
                                                    qnhVariable.ForWriting = false;
                                                    if (FS.FSUIPC.FS.Process(qnhVariable))
                                                    {
                                                        int qnh = qnhVariable.Value2 / 16;

                                                        // zwiększenie
                                                        qnh++;

                                                        // zapisanie większej wartości
                                                        qnhVariable.ForWriting = true;
                                                        qnhVariable.Value2 = (short)(qnh * 16);
                                                        FS.FSUIPC.FS.Process(qnhVariable);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }

                                state = GetLeftEncoderLeft(ref _keys);// (_keys[0] & 0x80) > 0;
                                if (leftEncoderLeft != state)
                                {
                                    leftEncoderLeft = state;
                                    if (leftEncoderLeft)
                                    {
                                        // zmniejszenie części całkowitej
                                        Debug.WriteLine(string.Format("Urządzenie: {0}, zmniejszenie części całktowitej", this));

                                        if (_test)
                                        {
                                            if (_displayBrightness > 0)
                                            {
                                                _displayBrightness--;
                                            }
                                        }
                                        else
                                        {
                                            switch (_currentMode)
                                            {
                                                case Mode.COM1:
                                                    //command = FS.ControlVariable.CMD_COM1_WHOLE_DEC;
                                                    _com1StbyWrite.COMWholeDec();
                                                    signal = true;
                                                    break;

                                                case Mode.COM2:
                                                    //command = FS.ControlVariable.CMD_COM2_WHOLE_DEC;
                                                    _com2StbyWrite.COMWholeDec();
                                                    signal = true;
                                                    break;

                                                case Mode.NAV1:
                                                    command = FS.ControlVariable.CMD_NAV1_WHOLE_DEC;
                                                    signal = true;
                                                    break;

                                                case Mode.NAV2:
                                                    command = FS.ControlVariable.CMD_NAV2_WHOLE_DEC;
                                                    signal = true;
                                                    break;

                                                case Mode.CRS1:
                                                    FS.FSUIPC.FS.Read(_nav1CrsWrite);
                                                    _nav1CrsWrite.Value2 -= 10;
                                                    if (_nav1CrsWrite.Value2 <= 0)
                                                    {
                                                        _nav1CrsWrite.Value2 += 360;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav1CrsWrite);
                                                    break;

                                                case Mode.CRS2:
                                                    FS.FSUIPC.FS.Read(_nav2CrsWrite);
                                                    _nav2CrsWrite.Value2 -= 10;
                                                    if (_nav2CrsWrite.Value2 <= 0)
                                                    {
                                                        _nav2CrsWrite.Value2 += 360;
                                                    }
                                                    FS.FSUIPC.FS.Write(_nav2CrsWrite);
                                                    break;

                                                case Mode.DME:
                                                    _dme1Mode = DecDMEMode(_dme1Mode);
                                                    signal = true;
                                                    break;

                                                case Mode.ADF1:
                                                    switch (_adf1DigitEditing)
                                                    {
                                                        case 0: // 0000.X
                                                            command = FS.ControlVariable.CMD_ADF1_FRAC_DEC;
                                                            signal = true;
                                                            break;

                                                        case 1: // 000X.0
                                                            command = FS.ControlVariable.CMD_ADF1_1_DEC;
                                                            signal = true;
                                                            break;

                                                        case 2: // 00X0.0
                                                            command = FS.ControlVariable.CMD_ADF1_10_DEC;
                                                            signal = true;
                                                            break;

                                                        case 3: // 0X00.0
                                                            command = FS.ControlVariable.CMD_ADF1_100_DEC;
                                                            signal = true;
                                                            break;
                                                    }
                                                    break;

                                                case Mode.ADF2:
                                                    switch (_adf2DigitEditing)
                                                    {
                                                        case 0: // 0000.X
                                                            command = FS.ControlVariable.CMD_ADF2_FRAC_DEC;
                                                            signal = true;
                                                            break;

                                                        case 1: // 000X.0
                                                            command = FS.ControlVariable.CMD_ADF2_1_DEC;
                                                            signal = true;
                                                            break;

                                                        case 2: // 00X0.0
                                                            command = FS.ControlVariable.CMD_ADF2_10_DEC;
                                                            signal = true;
                                                            break;

                                                        case 3: // 0X00.0
                                                            command = FS.ControlVariable.CMD_ADF2_100_DEC;
                                                            signal = true;
                                                            break;
                                                    }
                                                    break;

                                                case Mode.XPDR:
                                                    switch (_xpdrDigitEditing)
                                                    {
                                                        case 0: // 000X
                                                            command = FS.ControlVariable.CMD_XPDR_1_DEC;
                                                            signal = true;
                                                            break;

                                                        case 1: // 00X0
                                                            command = FS.ControlVariable.CMD_XPDR_10_DEC;
                                                            signal = true;
                                                            break;

                                                        case 2: // 0X00
                                                            command = FS.ControlVariable.CMD_XPDR_100_DEC;
                                                            signal = true;
                                                            break;

                                                        case 3: // X000
                                                            command = FS.ControlVariable.CMD_XPDR_1000_DEC;
                                                            signal = true;
                                                            break;
                                                    }
                                                    break;

                                                case Mode.QNH:
                                                    // odczytanie qnh
                                                    qnhVariable.ForWriting = false;
                                                    if (FS.FSUIPC.FS.Process(qnhVariable))
                                                    {
                                                        int qnh = qnhVariable.Value2 / 16;

                                                        // zmniejszenie
                                                        qnh--;

                                                        // zapisanie większej wartości
                                                        qnhVariable.ForWriting = true;
                                                        qnhVariable.Value2 = (short)(qnh * 16);
                                                        FS.FSUIPC.FS.Process(qnhVariable);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }

                                // przetworzenie danych
                                if (_test)
                                {
                                    if (_xpdrIDENT)
                                    {
                                        // wysłanie IDENT
                                        xpdrIDENTVariable.Value1 = 1;
                                        FS.FSUIPC.FS.Process(xpdrIDENTVariables);
                                        signal = true;
                                    }
                                    else
                                    {

                                    }
                                    //else
                                    {
                                        if (lastBacklightBrightness != _backlightBrightness)
                                        {
                                            // ustawienie nowego poziomu podświetlenia
                                            if (_backlightBrightness == 0)
                                            {
                                                // wyłączenie podświetlenia
                                                WriteToDevice(_driver, new byte[5] { _standbyDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 5, 0 });
                                                //WriteToDevice(_driver, new byte[5] { _activeDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 4, 0 });
                                            }
                                            else
                                            {
                                                // wysłanie 
                                                //WriteToDevice(_driver, new byte[4] { _activeDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | (_backlightBrightness - 1)) });
                                                WriteToDevice(_driver, new byte[4] { _standbyDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | (_backlightBrightness - 1)) });

                                                // włączenie podświetlenia
                                                WriteToDevice(_driver, new byte[5] { _standbyDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 5, 255 });
                                                //WriteToDevice(_driver, new byte[5] { _activeDisplayID, 3, (byte)simOUTLEDsCommand.LEDS, 4, ModeToLED(_currentMode) });
                                            }
                                            lastBacklightBrightness = _backlightBrightness;
                                        }

                                        if (lastDisplayBrightness != _displayBrightness)
                                        {
                                            // ustawienie nowego poziomu jasności wyświetlaczy
                                            WriteToDevice(_driver, new byte[4] { _activeDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displayBrightness });
                                            WriteToDevice(_driver, new byte[4] { _standbyDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displayBrightness });

                                            // ustawienie poziomu podświetlania napisów 
                                            WriteToDevice(_driver, new byte[4] { _activeDisplayID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | (_displayBrightness)) });

                                            lastDisplayBrightness = _displayBrightness;
                                        }
                                    }
                                }
                                else
                                {
                                    //if (xpdrIDENTVariable.Value1 == 1)
                                    //{
                                    //    xpdrIDENTVariable.Value1 = 0;
                                    //    FS.FSUIPC.FS.Process(xpdrIDENTVariables);
                                    //    signal = true;
                                    //}
                                }

                                if (command != 0)
                                {
                                    _controlVariable.Value8 = command;
                                    FS.FSUIPC.FS.Process(_controlVariables);
                                }

                                if (signal)
                                {
                                    _waitForData.Set();
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Błąd wątka odczytującego stan wejść urządzenia: {0}, błąd: {1}", this, ex));
            }
            finally
            {
                Debug.WriteLine(string.Format("Zakończenie wątka odczytującego stan wejść urządzenia: {0}", this));
            }
        }

        private void WriteToDisplayIfNeeded(int index, byte displayIndex, ref byte[] oldData, ref byte[] newData, ref byte[] data)
        {
            if (oldData[index] != newData[index])
            {                
                data[3] = displayIndex;
                data[4] = oldData[index] = newData[index];
                WriteToDevice(_driver, data);
                oldData[index] = newData[index];
            }
        }

        protected override void Disable()
        {
            // otwarcie urządzenia

            // wyłączenie raportowania

            // wyłączenie wyświetlaczy

        }

        protected virtual byte ModeToLED(Mode mode)
        {
            switch (mode)
            {
                case Mode.NAV1:
                case Mode.CRS1:
                    return 0x80;

                case Mode.XPDR:
                    return 0x01;

                case Mode.COM2:
                    return 0x20;

                case Mode.ADF2:
                    return 0x04;

                case Mode.COM1:
                    return 0x08;

                case Mode.ADF1:
                    return 0x10;

                case Mode.NAV2:
                case Mode.CRS2:
                    return 0x40;

                case Mode.DME:
                    return 0x02;

                default:
                    return 0x00;
            }
        }

        #region Metody do dziedzieczenia

        protected virtual void ResetStates()
        {

        }

        protected virtual byte[] GetEncoders()
        {
            return new byte[2] { 2, 3 };
        }

        protected virtual bool GetXPDRStandByState(ref byte[] keys)
        {
            return (keys[0] & 0x02) > 0;
        }

        protected virtual Mode GetInitialMode(ref byte[] keys)
        {
            return (Mode)keys[4];
        }

        protected virtual Mode GetMode(ref byte[] keys, Mode currentMode)
        {
            return (Mode)keys[4];
        }

        protected virtual bool GetTestState(ref byte[] keys)
        {
            return (keys[0] & 0x04) == 0x04;
        }

        protected virtual bool GetTransmitState(ref byte[] keys)
        {
            return (keys[0] & 0x01) > 0;
        }

        protected virtual bool GetRightEncoderLeft(ref byte[] keys)
        {
            return (keys[0] & 0x10) > 0;
        }

        protected virtual bool GetRightEndocerRight(ref byte[] keys)
        {
            return (keys[0] & 0x20) > 0;
        }

        protected virtual bool GetLeftEncoderRight(ref byte[] keys)
        {
            return (keys[0] & 0x40) > 0;
        }

        protected virtual bool GetLeftEncoderLeft(ref byte[] keys)
        {
            return (keys[0] & 0x80) > 0;
        }

        protected virtual bool GetEncoderButton(ref byte[] keys)
        {
            return (keys[0] & 0x08) > 0;
        }

        #endregion

        internal override void SetPlane(simPROJECT.Planes.IPlane plane)
        {
            //throw new NotImplementedException();
        }

        public override void StartIdentify()
        {
            throw new NotImplementedException();
        }

        public override void StopIdentify()
        {
            throw new NotImplementedException();
        }
    }
}
