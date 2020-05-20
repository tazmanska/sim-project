using System;
using System.Collections.Generic;
using System.Text;

namespace simPROJECT.Devices
{
    class MultiradioV1WithEncoder : MultiradioV1
    {
        public new const string SN_PREFIX = "S2";
        public new const DeviceType TYPE = DeviceType.MultiradioV1;

        public MultiradioV1WithEncoder(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE deviceInfo)
            : base(deviceInfo)
        {
            ChangeName("MultiRadio (Encoder)");
        }

        private bool _lastEncoderModeLeft = false;
        private bool _lastEncoderModeRight = false;

        private Mode _lastMode = Mode.COM1;

        protected override void ResetStates()
        {
            _lastEncoderModeLeft = false;
            _lastEncoderModeRight = false;
            _lastMode = Mode.COM1;
        }

        protected override byte[] GetEncoders()
        {
            return new byte[3] { 6, 9, 10 };
        }

        protected override bool GetXPDRStandByState(ref byte[] keys)
        {
            return (keys[1] & 0x40) > 0;
        }

        protected override Mode GetInitialMode(ref byte[] keys)
        {
            return Mode.COM1;
        }

        protected override Mode GetMode(ref byte[] keys, Mode currentMode)
        {
            if (currentMode == Mode.QNH || currentMode == Mode.CRS1 || currentMode == Mode.CRS2)
            {
                return _lastMode;
                //return currentMode;
            }

            if (currentMode == Mode.None)
            {
                return _lastMode;
                //return GetInitialMode(ref keys);
            }

            Mode result = currentMode;

            // stan enkodera (lewo)
            bool state = (keys[1] & 0x20) > 0;
            if (state != _lastEncoderModeLeft)
            {
                _lastEncoderModeLeft = state;

                if (_lastEncoderModeLeft && currentMode != Mode.XPDR)
                {
                    // zmiana trybu (w lewo)
                    result = (Mode)((((int)currentMode) << 1) & 0xff);
                }
            }

            // stan enkodera (w prawo)
            state = (keys[1] & 0x10) > 0;
            if (state != _lastEncoderModeRight)
            {
                _lastEncoderModeRight = state;

                if (_lastEncoderModeRight && currentMode != Mode.DME)
                {
                    // zmiana trybu (w prawo)
                    result = (Mode)((((int)currentMode) >> 1) & 0xff);
                }
            }

            _lastMode = result;

            return result;
        }

        protected override bool GetTestState(ref byte[] keys)
        {
            return (keys[2] & 0x01) > 0;
        }

        protected override bool GetTransmitState(ref byte[] keys)
        {
            return (keys[2] & 0x02) > 0;
        }

        protected override bool GetRightEncoderLeft(ref byte[] keys)
        {
            return (keys[2] & 0x20) > 0;
        }

        protected override bool GetRightEndocerRight(ref byte[] keys)
        {
            return (keys[2] & 0x10) > 0;
        }

        protected override bool GetLeftEncoderRight(ref byte[] keys)
        {
            return (keys[2] & 0x08) > 0;
        }

        protected override bool GetLeftEncoderLeft(ref byte[] keys)
        {
            return (keys[2] & 0x04) > 0;
        }

        protected override bool GetEncoderButton(ref byte[] keys)
        {
            return (keys[2] & 0x40) > 0;
        }

        protected override byte ModeToLED(Mode mode)
        {
            switch (mode)
            {
                case Mode.NAV1:
                    return 0x80;

                case Mode.XPDR:
                    return 0x40;

                case Mode.COM2:
                    return 0x20;

                case Mode.ADF2:
                    return 0x10;

                case Mode.COM1:
                    return 0x08;

                case Mode.ADF1:
                    return 0x04;

                case Mode.NAV2:
                    return 0x01;

                case Mode.DME:
                    return 0x02;

                default:
                    return 0x00;
            }
        }
    }
}
