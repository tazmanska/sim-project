using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using simPROJECT.Planes;
using System.Diagnostics;
using simPROJECT.Configuration;
using simPROJECT.UI;

namespace simPROJECT.Devices
{
    class MCP737 : Device
    {
        public const string SN_PREFIX = "S3";
        public const DeviceType TYPE = DeviceType.MCP737;

        public MCP737(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo)
            : base(deviceInfo, "MCP 737", TYPE)
        {
            // utworzenie drivera
            _driver = new FTD2XX_NET.FTDI();

            _displaysDictionary.Add('.', 128);
            _displaysDictionary.Add('0', 123);
            _displaysDictionary.Add('1', 80);
            _displaysDictionary.Add('2', 55);
            _displaysDictionary.Add('3', 118);
            _displaysDictionary.Add('4', 92);
            _displaysDictionary.Add('5', 110);
            _displaysDictionary.Add('6', 111);
            _displaysDictionary.Add('7', 112);
            _displaysDictionary.Add('8', 127);
            _displaysDictionary.Add('9', 126);

            _displaysDictionary.Add('H', 93);
            _displaysDictionary.Add('E', 47);
            _displaysDictionary.Add('L', 11);
            _displaysDictionary.Add('O', 123);
            _displaysDictionary.Add('r', 5);
            _displaysDictionary.Add('A', 124);
            _displaysDictionary.Add('-', 4);
            _displaysDictionary.Add('n', 69);
            _displaysDictionary.Add('o', 71);
            _displaysDictionary.Add('F', 45);
            _displaysDictionary.Add('S', 110);
        }

        private Dictionary<char, byte> _displaysDictionary = new Dictionary<char, byte>();

        private FTD2XX_NET.FTDI _driver = null;

        private byte _keysDeviceID = 161;
        private byte _displaysCRS_HDG_ID = 21;
        private byte _displaysIAS_ID = 22;
        private byte _displaysALT_ID = 23;
        private byte _displaysVS_ID = 24;

        private AutoResetEvent _receivedEvent = new AutoResetEvent(false);
        private AutoResetEvent _waitForData = new AutoResetEvent(false);

        private Thread _receivingThread = null;

        private byte _displaysBrightness = 0;
        private byte _ledsBrightness = 0;
        private byte _backlightBrightness = 0;

        protected override void WorkingThread()
        {
            Debug.WriteLine(string.Format("Wystartowanie wątka obsługującego urządzenie: {0}", this.ToString()));

            // wystartowanie współpracy z urządzeniem         
            byte displaysBrightness = ((MCP737Configuration)Configuration).DisplayBrightness;
            byte ledsBrightness = ((MCP737Configuration)Configuration).LEDBrightness;
            byte backlightBrightness = ((MCP737Configuration)Configuration).BacklightBrightness;
            Array.Clear(_keys, 0, _keys.Length);

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
                            _driver = OpenDevice(true);
                            if (_driver != null)
                            {
                                Debug.WriteLine(string.Format("Inicjalizacja urządzenia: {0}", this));

                                // wysłanie rozkazu zatrzymania skanowania wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.STOP_SCAN });

                                // wysłanie rozkazu wyczyszczeniu konfiguracji wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.CLEAR_ENCODERS });

                                // enkoder CRS
                                WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, 19, 0 });

                                // enkoder IAS
                                WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, 16, 0 });

                                // enkoder HDG
                                WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, 0, 0 });

                                // enkoder ALT
                                WriteToDevice(_driver, new byte[5] { _keysDeviceID, 3, (byte)simINKeysCommand.SET_ENCODER, 4, 0 });

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
                                _leds1 = 0xff;
                                _leds2 = 0xff;
                                _leds3 = 0xff;
                                _crsDisplayText = "_";
                                _crsDisplayDataOld = new byte[] { 0xff, 0xff, 0xff };
                                _iasDisplayText = "_";
                                _iasDisplayDataOld = new byte[] { 0xff, 0xff, 0xff, 0xff };
                                _hdgDisplayText = "_";
                                _hdgDisplayDataOld = new byte[] { 0xff, 0xff, 0xff };
                                _altDisplayText = "_";
                                _altDisplayDataOld = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff };
                                _vsDisplayText = "_";
                                _vsDisplayDataOld = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff };

                                // ustawienie flagi identyfikacji
                                _identify = false;
                                
                                ClearDevice();

                                // wysłanie rozkazu raportu wejść
                                WriteToDevice(_driver, new byte[3] { _keysDeviceID, 1, (byte)simINKeysCommand.GET_KEYS });

                                Thread.Sleep(50);

                                // odczytanie raportów wejść
                                ReadKeysState(_driver, ref _keys);

                                // ustawienie poziomu jasności
                                SetDisplayBrightness(displaysBrightness);
                                SetLEDBrightness(ledsBrightness);
                                SetBacklightBrightness(backlightBrightness);

                                // animacja
                                if (((MCP737Configuration)Configuration).WelcomeAnimation)
                                {
                                    string hello = "     HELLO      ";
                                    int max = hello.Length - 6;
                                    for (int i = 0; i < max; i++)
                                    {
                                        hello = hello.Substring(1);
                                        SetVSDisplay(hello.Substring(0, 5));
                                        Thread.Sleep(200);
                                    }
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

                    if (_mcp != null)
                    {
                        // aktualizacja pól
                        _mcp.UpdateOutputs();



                        if (_mcp.HasData && !_identify)
                        {
                            // ustawienie wyświetlaczy
                            SetCRSDisplay(_mcp.GetCRS());
                            SetIASDisplay(_mcp.GetIAS());
                            SetHDGDisplay(_mcp.GetHDG());
                            SetALTDisplay(_mcp.GetALT());
                            SetVSDisplay(_mcp.GetVS());

                            // ustawienie diod
                            SetLEDMA(_mcp.GetLEDMA());
                            SetLEDAT(_mcp.GetLEDAT());
                            SetLEDN1(_mcp.GetLEDN1());
                            SetLEDSPEED(_mcp.GetLEDSPEED());
                            SetLEDLVLCHG(_mcp.GetLEDLVLCHG());
                            SetLEDVNAV(_mcp.GetLEDVNAV());
                            SetLEDHDGSEL(_mcp.GetLEDHDGSEL());
                            SetLEDLNAV(_mcp.GetLEDLNAV());
                            SetLEDVORLOC(_mcp.GetLEDVORLOC());
                            SetLEDAPP(_mcp.GetLEDAPP());
                            SetLEDALTHLD(_mcp.GetLEDALTHLD());
                            SetLEDVS(_mcp.GetLEDVS());
                            SetLEDCMDA(_mcp.GetLEDCMDA());
                            SetLEDCMDB(_mcp.GetLEDCMDB());
                            SetLEDCWSA(_mcp.GetLEDCWSA());
                            SetLEDCWSB(_mcp.GetLEDCWSB());
                        }
                        else
                        {
                            // wyczyszczenie wyświetlaczy
                            SetCRSDisplay("");
                            SetIASDisplay("");
                            SetHDGDisplay("");

                            if (_identify)
                            {
                                SetALTDisplay("");

                                // wyświetlenie błędu
                                SetVSDisplay("HELLO");
                            }
                            else
                            {
                                SetALTDisplay("Error");

                                // wyświetlenie błędu
                                SetVSDisplay(_mcp.Error);
                            }

                            // wyłączenie diod
                            SetLEDMA(false);
                            SetLEDAT(false);
                            SetLEDN1(false);
                            SetLEDSPEED(false);
                            SetLEDLVLCHG(false);
                            SetLEDVNAV(false);
                            SetLEDHDGSEL(false);
                            SetLEDLNAV(false);
                            SetLEDVORLOC(false);
                            SetLEDAPP(false);
                            SetLEDALTHLD(false);
                            SetLEDVS(false);
                            SetLEDCMDA(false);
                            SetLEDCMDB(false);
                            SetLEDCWSA(false);
                            SetLEDCWSB(false);
                        }
                    }
                    else
                    {
                        ClearDevice();
                    }

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
                        ClearDevice();
                    }
                    catch { }

                    try
                    {
                        _driver.Close();
                    }
                    catch { }
                }

                ((MCP737Configuration)Configuration).DisplayBrightness = _displaysBrightness;
                ((MCP737Configuration)Configuration).LEDBrightness = _ledsBrightness;
                ((MCP737Configuration)Configuration).BacklightBrightness = _backlightBrightness;
            }
        }

        public override void ClearDevice()
        {
            base.ClearDevice();

            // wyłączenie jasności
            byte tmp = _backlightBrightness;
            _backlightBrightness = 1;
            SetBacklightBrightness(0);
            _backlightBrightness = tmp;

            tmp = _displaysBrightness;
            _displaysBrightness = 1;
            SetDisplayBrightness(0);
            _displaysBrightness = tmp;

            tmp = _ledsBrightness;
            _ledsBrightness = 1;
            SetLEDBrightness(0);
            _ledsBrightness = tmp;

            // wyczyszczenie wyświetlaczy
            SetALTDisplay("");
            SetCRSDisplay("");
            SetHDGDisplay("");
            SetIASDisplay("");
            SetVSDisplay("");

            // wyłączenie wszystkich diod
            SetLEDMA(false);
            SetLEDAT(false);
            SetLEDN1(false);
            SetLEDSPEED(false);
            SetLEDLVLCHG(false);
            SetLEDVNAV(false);
            SetLEDHDGSEL(false);
            SetLEDLNAV(false);
            SetLEDVORLOC(false);
            SetLEDAPP(false);
            SetLEDALTHLD(false);
            SetLEDVS(false);
            SetLEDCMDA(false);
            SetLEDCMDB(false);
            SetLEDCWSA(false);
            SetLEDCWSB(false);
        }

        private void ChangeDisplayBrightness(bool increase)
        {
            int level = _displaysBrightness;
            if (increase)
            {
                level += 1;
            }
            else
            {
                level -= 1;
            }
            if (level < 0)
            {
                level = 0;
            }
            if (level > 10)
            {
                level = 10;
            }
            SetDisplayBrightness((byte)level);
        }

        private void SetDisplayBrightness(byte level)
        {
            if (_displaysBrightness != level)
            {
                _displaysBrightness = level;
                WriteToDevice(_driver, new byte[4] { _displaysCRS_HDG_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS, _displaysBrightness });
                WriteToDevice(_driver, new byte[4] { _displaysIAS_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displaysBrightness });
                WriteToDevice(_driver, new byte[4] { _displaysALT_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displaysBrightness });
                WriteToDevice(_driver, new byte[4] { _displaysVS_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, _displaysBrightness });
            }
        }

        private void ChangeLEDBrightness(bool increase)
        {
            int level = _ledsBrightness;
            if (increase)
            {
                level += 1;
            }
            else
            {
                level -= 1;
            }
            if (level < 0)
            {
                level = 0;
            }
            if (level > 10)
            {
                level = 10;
            }
            SetLEDBrightness((byte)level);
        }

        private void SetLEDBrightness(byte level)
        {
            if (_ledsBrightness != level)
            {
                _ledsBrightness = level;
                WriteToDevice(_driver, new byte[4] { _displaysIAS_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | _ledsBrightness) });
                WriteToDevice(_driver, new byte[4] { _displaysALT_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | _ledsBrightness) });
            }
        }

        private void ChangeBacklightBrightness(bool increase)
        {
            int level = _backlightBrightness;
            if (increase)
            {
                level += 1;
            }
            else
            {
                level -= 1;
            }
            if (level < 0)
            {
                level = 0;
            }
            if (level > 10)
            {
                level = 10;
            }
            SetBacklightBrightness((byte)level);
        }

        private void SetBacklightBrightness(byte level)
        {
            if (_backlightBrightness != level)
            {
                _backlightBrightness = level;
                WriteToDevice(_driver, new byte[4] { _displaysVS_ID, 2, (byte)simOUTLEDsCommand.BRIGHTNESS2, (byte)(0xf0 | _backlightBrightness) });

                byte data = (byte)(level == 0 ? 0x00 : 0xff);
                WriteToDevice(_driver, new byte[] { _displaysVS_ID, 3, (byte)simOUTLEDsCommand.LEDS, 5, data });
            }
        }

        private string _crsDisplayText = null;
        private byte[] _crsDisplayData = new byte[] { 0, 0, 0 };
        private byte[] _crsDisplayDataOld = new byte[] { 0, 0, 0 };
        
        private void SetCRSDisplay(string text)
        {
            if (_crsDisplayText != text)
            {
                _crsDisplayText = text;
                CodeText(_crsDisplayText, ref _displaysDictionary, ref _crsDisplayData);
                WriteToDisplayIfNeeded(_displaysCRS_HDG_ID, 0, 5, ref _crsDisplayDataOld, ref _crsDisplayData);
                WriteToDisplayIfNeeded(_displaysCRS_HDG_ID, 1, 3, ref _crsDisplayDataOld, ref _crsDisplayData);
                WriteToDisplayIfNeeded(_displaysCRS_HDG_ID, 2, 4, ref _crsDisplayDataOld, ref _crsDisplayData);
            }
        }

        private string _iasDisplayText = null;
        private byte[] _iasDisplayData = new byte[] { 0, 0, 0, 0 };
        private byte[] _iasDisplayDataOld = new byte[] { 0, 0, 0, 0 };

        private void SetIASDisplay(string text)
        {
            if (_iasDisplayText != text)
            {
                _iasDisplayText = text;
                CodeText(_iasDisplayText, ref _displaysDictionary, ref _iasDisplayData);
                WriteToDisplayIfNeeded(_displaysIAS_ID, 0, 3, ref _iasDisplayDataOld, ref _iasDisplayData);
                WriteToDisplayIfNeeded(_displaysIAS_ID, 1, 2, ref _iasDisplayDataOld, ref _iasDisplayData);
                WriteToDisplayIfNeeded(_displaysIAS_ID, 2, 1, ref _iasDisplayDataOld, ref _iasDisplayData);
                WriteToDisplayIfNeeded(_displaysIAS_ID, 3, 0, ref _iasDisplayDataOld, ref _iasDisplayData);
            }
        }

        private string _hdgDisplayText = null;
        private byte[] _hdgDisplayData = new byte[] { 0, 0, 0 };
        private byte[] _hdgDisplayDataOld = new byte[] { 0, 0, 0 };

        private void SetHDGDisplay(string text)
        {
            if (_hdgDisplayText != text)
            {
                _hdgDisplayText = text;
                CodeText(_hdgDisplayText, ref _displaysDictionary, ref _hdgDisplayData);
                WriteToDisplayIfNeeded(_displaysCRS_HDG_ID, 0, 0, ref _hdgDisplayDataOld, ref _hdgDisplayData);
                WriteToDisplayIfNeeded(_displaysCRS_HDG_ID, 1, 1, ref _hdgDisplayDataOld, ref _hdgDisplayData);
                WriteToDisplayIfNeeded(_displaysCRS_HDG_ID, 2, 2, ref _hdgDisplayDataOld, ref _hdgDisplayData);
            }
        }

        private string _altDisplayText = null;
        private byte[] _altDisplayData = new byte[] { 0, 0, 0, 0, 0 };
        private byte[] _altDisplayDataOld = new byte[] { 0, 0, 0, 0, 0 };

        private void SetALTDisplay(string text)
        {
            if (_altDisplayText != text)
            {
                _altDisplayText = text;
                CodeText(_altDisplayText, ref _displaysDictionary, ref _altDisplayData);
                WriteToDisplayIfNeeded(_displaysALT_ID, 0, 3, ref _altDisplayDataOld, ref _altDisplayData);
                WriteToDisplayIfNeeded(_displaysALT_ID, 1, 2, ref _altDisplayDataOld, ref _altDisplayData);
                WriteToDisplayIfNeeded(_displaysALT_ID, 2, 1, ref _altDisplayDataOld, ref _altDisplayData);
                WriteToDisplayIfNeeded(_displaysALT_ID, 3, 0, ref _altDisplayDataOld, ref _altDisplayData);
                WriteToDisplayIfNeeded(_displaysALT_ID, 4, 4, ref _altDisplayDataOld, ref _altDisplayData);
            }
        }

        private string _vsDisplayText = null;
        private byte[] _vsDisplayData = new byte[] { 0, 0, 0, 0, 0 };
        private byte[] _vsDisplayDataOld = new byte[] { 0, 0, 0, 0, 0 };

        private void SetVSDisplay(string text)
        {
            if (_vsDisplayText != text)
            {
                _vsDisplayText = text;
                CodeText(_vsDisplayText, ref _displaysDictionary, ref _vsDisplayData);
                WriteToDisplayIfNeeded(_displaysVS_ID, 0, 3, ref _vsDisplayDataOld, ref _vsDisplayData);
                WriteToDisplayIfNeeded(_displaysVS_ID, 1, 2, ref _vsDisplayDataOld, ref _vsDisplayData);
                WriteToDisplayIfNeeded(_displaysVS_ID, 2, 1, ref _vsDisplayDataOld, ref _vsDisplayData);
                WriteToDisplayIfNeeded(_displaysVS_ID, 3, 0, ref _vsDisplayDataOld, ref _vsDisplayData);
                WriteToDisplayIfNeeded(_displaysVS_ID, 4, 4, ref _vsDisplayDataOld, ref _vsDisplayData);
            }
        }

        private void SetLEDMA(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 2, state);
        }

        private void SetLEDAT(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 4, state);
        }

        private void SetLEDN1(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 1, state);
        }

        private void SetLEDSPEED(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 3, state);
        }

        private void SetLEDLVLCHG(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 5, state);
        }

        private void SetLEDVNAV(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds1, 5, 3, state);
        }

        private void SetLEDHDGSEL(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 7, state);
        }

        private void SetLEDLNAV(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds1, 5, 5, state);
        }

        private void SetLEDVORLOC(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds1, 5, 1, state);
        }

        private void SetLEDAPP(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds2, 4, 6, state);
        }

        private void SetLEDALTHLD(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds1, 5, 0, state);
        }

        private void SetLEDVS(bool state)
        {
            SetLEDIfNeeded(_displaysIAS_ID, ref _leds1, 5, 2, state);
        }

        private void SetLEDCMDA(bool state)
        {
            SetLEDIfNeeded(_displaysALT_ID, ref _leds3, 5, 6, state);
        }

        private void SetLEDCMDB(bool state)
        {
            SetLEDIfNeeded(_displaysALT_ID, ref _leds3, 5, 4, state);
        }

        private void SetLEDCWSA(bool state)
        {
            SetLEDIfNeeded(_displaysALT_ID, ref _leds3, 5, 0, state);
        }

        private void SetLEDCWSB(bool state) 
        {
            SetLEDIfNeeded(_displaysALT_ID, ref _leds3, 5, 2, state);
        }

        private byte _leds1 = 0;
        private byte _leds2 = 0;
        private byte _leds3 = 0;

        private void SetLEDIfNeeded(byte deviceId, ref byte data, byte index, int bit, bool state)
        {
            bool currentState = (data & (1 << bit)) > 0;
            if (currentState != state)
            {
                if (state)
                {
                    data |= (byte)(1 << bit);
                }
                else
                {
                    data &= (byte)~(1 << bit);
                }
                _writeToDisplayData[0] = deviceId;
                _writeToDisplayData[3] = index;
                _writeToDisplayData[4] = data;
                WriteToDevice(_driver, _writeToDisplayData);
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

        private void ReceivedData()
        {
            try
            {
                Debug.WriteLine(string.Format("Start wątka odczytującego stan wejść urządzenia: {0}", this));

                bool crsDec = false;
                bool crsInc = false;
                bool iasDec = false;
                bool iasInc = false;
                bool altDec = false;
                bool altInc = false;
                bool hdgDec = false;
                bool hdgInc = false;
                bool vsup = false;
                bool vsdown = false;

                bool at = false;
                bool co = false;
                bool spdintv = false;
                bool vnav = false;
                bool altintv = false;
                bool lnav = false;
                bool vorloc = false;
                bool hdgsel = false;
                bool hdgb = false;
                bool fd = false;
                bool lvlchg = false;
                bool speed = false;
                bool n1 = false;
                bool vs = false;
                bool althld = false;
                bool app = false;
                bool disengage = false;
                bool cwsa = false;
                bool cwsb = false;
                bool cmda = false;
                bool cmdb = false;

                DateTime incCRSTime = DateTime.Now;
                DateTime decCRSTime = DateTime.Now;
                DateTime incIASTime = DateTime.Now;
                DateTime decIASTime = DateTime.Now;
                DateTime incHDGTime = DateTime.Now;
                DateTime decHDGTime = DateTime.Now;
                DateTime incALTTime = DateTime.Now;
                DateTime decALTTime = DateTime.Now;
                DateTime incVSTime = DateTime.Now;
                DateTime decVSTime = DateTime.Now;

                uint available = 0;
                byte[] data = new byte[3];
                bool first = true;

                // pierwsze odczytanie
                // fd 4
                if (CheckKey(0, 0x10, ref fd) || first)
                {
                    _mcp.SetSwitchFD(!fd);
                }

                // at 37
                if (CheckKey(4, 0x20, ref at) || first)
                {
                    _mcp.SetSwitchAT(at);
                }

                while (!NeedStop)
                {
                    _receivedEvent.WaitOne(500, false);

                    // fd 0 4
                    CheckKey(0, 0x10, ref fd);
                    _mcp.SetSwitchFD(!fd);

                    // at 4 37
                    CheckKey(4, 0x20, ref at);
                    _mcp.SetSwitchAT(at);

                    while (_driver.GetRxBytesAvailable(ref available) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && available > 2)
                    {
                        uint red = 0;
                        if (_driver.Read(data, 3, ref red) == FTD2XX_NET.FTDI.FT_STATUS.FT_OK && red == 3)
                        {
#if DEBUG
                            Debug.WriteLine(string.Format("Odebrano dane z urządzenia ({0}): 0x{1} 0x{2} 0x{3}", this.ToString(), data[0].ToString("X2"), data[1].ToString("X2"), data[2].ToString("X2")));
#endif
                            if (_mcp == null)
                            {
                                continue;
                            }

                            if (_planeChanged)
                            {
                                first = true;
                                _planeChanged = false;
                            }

                            _mcp.ResetUpdate();

                            int type = (data[1] >> 4) & 0x0f;
                            int index = data[1] & 0x0f;
                            if (type == 1 && index < _keys.Length)
                            {
                                _keys[index] = data[2];
                               
                                switch (index)
                                {
                                    case 0:
                                        // hdg 0, 1
                                        if (CheckKey(index, 0x02, ref hdgInc) || first)
                                        {
                                            if (hdgInc)
                                            {
                                                if (_mcp.IsTestMode)
                                                {
                                                    ChangeLEDBrightness(true);
                                                }
                                                else
                                                {
                                                    _mcp.IncHDG(DetectFastRotate(ref incHDGTime));
                                                }
                                            }
                                        }
                                        if (CheckKey(index, 0x01, ref hdgDec) || first)
                                        {
                                            if (hdgDec)
                                            {
                                                if (_mcp.IsTestMode)
                                                {
                                                    ChangeLEDBrightness(false);
                                                }
                                                else
                                                {
                                                    _mcp.DecHDG(DetectFastRotate(ref decHDGTime));
                                                }
                                            }
                                        }

                                        // hdgsel 2
                                        if (CheckKey(index, 0x04, ref hdgsel) || first)
                                        {
                                            if (hdgsel)
                                            {
                                                _mcp.PressHdgSEL();
                                            }
                                        }

                                        // hdgb 3
                                        if (CheckKey(index, 0x08, ref hdgb) || first)
                                        {
                                            if (hdgb)
                                            {
                                                _mcp.PressHdgButton();
                                            }
                                        }

                                        // fd 4
                                        if (CheckKey(index, 0x10, ref fd) || first)
                                        {
                                            _mcp.SetSwitchFD(!fd);
                                        }

                                        // lvlchg 5
                                        if (CheckKey(index, 0x20, ref lvlchg) || first)
                                        {
                                            if (lvlchg)
                                            {
                                                _mcp.PressLvlCHG();
                                            }
                                        }

                                        // speed 6
                                        if (CheckKey(index, 0x40, ref speed) || first)
                                        {
                                            if (speed)
                                            {
                                                _mcp.PressSPEED();
                                            }
                                        }

                                        // n1 7
                                        if (CheckKey(index, 0x80, ref n1) || first)
                                        {
                                            if (n1)
                                            {
                                                _mcp.PressN1();
                                            }
                                        }

                                        break;

                                    case 1:
                                        // alt 8, 9
                                        if (CheckKey(index, 0x02, ref altInc) || first)
                                        {
                                            if (altInc)
                                            {
                                                _mcp.IncALT(DetectFastRotate(ref incALTTime));
                                            }
                                        }
                                        if (CheckKey(index, 0x01, ref altDec) || first)
                                        {
                                            if (altDec)
                                            {
                                                _mcp.DecALT(DetectFastRotate(ref decALTTime));
                                            }
                                        }

                                        // altintv 10
                                        if (CheckKey(index, 0x04, ref altintv) || first)
                                        {
                                            if (altintv)
                                            {
                                                _mcp.PressAltINTV();
                                            }
                                        }

                                        // lnav 11
                                        if (CheckKey(index, 0x08, ref lnav) || first)
                                        {
                                            if (lnav)
                                            {
                                                _mcp.PressLNAV();
                                            }
                                        }

                                        // vorloc 12
                                        if (CheckKey(index, 0x10, ref vorloc) || first)
                                        {
                                            if (vorloc)
                                            {
                                                _mcp.PressVORLOC();
                                            }
                                        }

                                        // vs 13
                                        if (CheckKey(index, 0x20, ref vs) || first)
                                        {
                                            if (vs)
                                            {
                                                _mcp.PressVS();
                                            }
                                        }

                                        // althld 14
                                        if (CheckKey(index, 0x40, ref althld) || first)
                                        {
                                            if (althld)
                                            {
                                                _mcp.PressAltHLD();
                                            }
                                        }

                                        // app 15
                                        if (CheckKey(index, 0x80, ref app) || first)
                                        {
                                            if (app)
                                            {
                                                _mcp.PressAPP();
                                            }
                                        }

                                        break;

                                    case 2:
                                        // disengage 16
                                        if (CheckKey(index, 0x01, ref disengage) || first)
                                        {
                                            if (disengage)
                                            {
                                                _mcp.PressDisengage();
                                            }
                                        }

                                        // cwsb 17
                                        if (CheckKey(index, 0x02, ref cwsb) || first)
                                        {
                                            if (cwsb)
                                            {
                                                _mcp.PressCwsB();
                                            }
                                        }

                                        // cwsa 18
                                        if (CheckKey(index, 0x04, ref cwsa) || first)
                                        {
                                            if (cwsa)
                                            {
                                                _mcp.PressCwsA();
                                            }
                                        }

                                        // cmdb 19
                                        if (CheckKey(index, 0x08, ref cmdb) || first)
                                        {
                                            if (cmdb)
                                            {
                                                _mcp.PressCmdB();
                                            }
                                        }

                                        // cmda 20
                                        if (CheckKey(index, 0x10, ref cmda) || first)
                                        {
                                            if (cmda)
                                            {
                                                _mcp.PressCmdA();
                                            }
                                        }

                                        // vs 21, 22
                                        if (CheckKey(index, 0x40, ref vsup) || first)
                                        {
                                            if (vsup)
                                            {
                                                _mcp.IncVS(DetectFastRotate(ref incVSTime));
                                            }
                                        }
                                        if (CheckKey(index, 0x20, ref vsdown) || first)
                                        {
                                            if (vsdown)
                                            {
                                                _mcp.DecVS(DetectFastRotate(ref decVSTime));
                                            }
                                        }

                                        break;

                                    case 4:
                                        // ias 32, 33
                                        if (CheckKey(index, 0x01, ref iasInc) || first)
                                        {
                                            if (iasInc)
                                            {
                                                if (_mcp.IsTestMode)
                                                {
                                                    ChangeDisplayBrightness(true);
                                                }
                                                else
                                                {
                                                    _mcp.IncIAS(DetectFastRotate(ref incIASTime));
                                                }
                                            }
                                        }
                                        if (CheckKey(index, 0x02, ref iasDec) || first)
                                        {
                                            if (iasDec)
                                            {
                                                if (_mcp.IsTestMode)
                                                {
                                                    ChangeDisplayBrightness(false);
                                                }
                                                else
                                                {
                                                    _mcp.DecIAS(DetectFastRotate(ref decIASTime));
                                                }
                                            }
                                        }

                                        // spdintv 34
                                        if (CheckKey(index, 0x04, ref spdintv) || first)
                                        {
                                            if (spdintv)
                                            {
                                                _mcp.PressSpdINTV();
                                            }
                                        }

                                        // vnav 35
                                        if (CheckKey(index, 0x08, ref vnav) || first)
                                        {
                                            if (vnav)
                                            {
                                                _mcp.PressVNAV();
                                            }
                                        }

                                        // co 36
                                        if (CheckKey(index, 0x10, ref co) || first)
                                        {
                                            if (co)
                                            {
                                                _mcp.PressCO();
                                            }
                                        }

                                        // at 37
                                        if (CheckKey(index, 0x20, ref at) || first)
                                        {
                                            _mcp.SetSwitchAT(at);
                                        }

                                        // crs 38, 39
                                        if (CheckKey(index, 0x80, ref crsInc) || first)
                                        {
                                            if (crsInc)
                                            {
                                                if (_mcp.IsTestMode)
                                                {
                                                    ChangeBacklightBrightness(true);
                                                }
                                                else
                                                {
                                                    _mcp.IncCRS(DetectFastRotate(ref incCRSTime));
                                                }
                                            }
                                        }
                                        if (CheckKey(index, 0x40, ref crsDec) || first)
                                        {
                                            if (crsDec)
                                            {
                                                if (_mcp.IsTestMode)
                                                {
                                                    ChangeBacklightBrightness(false);
                                                }
                                                else
                                                {
                                                    _mcp.DecCRS(DetectFastRotate(ref decCRSTime));
                                                }
                                            }
                                        }

                                        break;
                                }

                                if (_mcp.NeedUpdate || first)
                                {
                                    _waitForData.Set();
                                }

                                first = false;
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

        private TimeSpan _fastTime = new TimeSpan(0, 0, 0, 0, 6);

        private bool DetectFastRotate(ref DateTime time)
        {
            bool result = false;
            result = DateTime.Now - time < _fastTime;
            time = DateTime.Now;
            return result;
        }

        private bool CheckKey(int index, int bit, ref bool currentState)
        {
            bool state = (_keys[index] & bit) > 0;
            if (currentState != state)
            {
                currentState = state;
                return true;
            }
            return false;
        }

        private byte[] _writeToDisplayData = new byte[] { /* ID */ 0, /* długość */ 3, /* rozkaz */ (byte)simOUTLEDsCommand.LEDS, /* index */ 0, /* data */ 0 };

        private void WriteToDisplayIfNeeded(byte deviceId, int charIndex, byte displayIndex, ref byte[] oldData, ref byte[] newData)
        {
            if (oldData[charIndex] != newData[charIndex])
            {
                _writeToDisplayData[0] = deviceId;
                _writeToDisplayData[3] = displayIndex;
                _writeToDisplayData[4] = oldData[charIndex] = newData[charIndex];
                WriteToDevice(_driver, _writeToDisplayData);
                oldData[charIndex] = newData[charIndex];
            }
        }

        protected override void Disable()
        {
            // otwarcie urządzenia

            // wyłączenie raportowania

            // wyłączenie wyświetlaczy

        }

        private IMCP _mcp = null;
        private volatile bool _planeChanged = false;

        internal override void SetPlane(simPROJECT.Planes.IPlane plane)
        {
            if (_mcp != null)
            {
                _mcp.Close();
            }
            _mcp = plane.GetMCP();
            _mcp.DeviceConfiguration = Configuration;
            _mcp.Startup();
            _planeChanged = true;
        }

        private volatile bool _identify = false;

        public override void StartIdentify()
        {
            _identify = true;
            _waitForData.Set();
        }

        public override void StopIdentify()
        {
            _identify = false;
            _waitForData.Set();
        }

        public override simPROJECT.UI.PanelConfigurationDialogBase CreateSettingsDialog()
        {
            return new MCP737ConfigurationDialog(this);
        }
    }
}