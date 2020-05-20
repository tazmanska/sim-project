using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.FlightSimulator.SimConnect;

namespace simPROJECT.Planes.PMDG737NGX
{
    class MCP : BaseMCP
    {
        private SimConnect _simconnect = null;
        private readonly EventWaitHandle _simConnectEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly EventWaitHandle _simConnectProcessEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        private Thread _simConnectThread = null;
        private volatile bool _working = false;

        public override void Startup()
        {
            base.Startup();
            _working = true;
            OpenSimConnect();
        }

        public override void Close()
        {
            base.Close();
            _working = false;
            CloseSimConnect();
        }

        public override void UpdateOutputs()
        {
            base.UpdateOutputs();
        }

        public override void ResetDevice()
        {
            base.ResetDevice();
            Startup();
        }

        private void CloseSimConnect()
        {
            if (_simConnectThread != null)
            {
                try
                {
                    _simConnectThread.Abort();
                }
                catch { }
                _simConnectThread = null;
            }
            if (_simconnect != null)
            {
                _simconnect.Dispose();
                _simconnect = null;
            }
        }

        private void OpenSimConnect()
        {
            CloseSimConnect();
            _simconnect = new SimConnect("simPROJECT", (IntPtr)0, 0, _simConnectEvent, 0);
            _simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(_simconnect_OnRecvOpen);
            _simconnect.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(_simconnect_OnRecvClientData);
            _simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(_simconnect_OnRecvSimobjectData);
            _simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(_simconnect_OnRecvException);
            _simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(_simconnect_OnRecvQuit);
            _simConnectThread = new Thread(new ThreadStart(SimConnectThreadProc));
            _simConnectThread.Start();
        }

        void _simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
        }

        void _simconnect_OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            if (data.dwRequestID == (uint) PMDGSDK.PMDGEnum.DATA_REQUEST)
            {
                PMDGSDK.PMDG_NGX_Data pmdg = (PMDGSDK.PMDG_NGX_Data) data.dwData[0];

                
            }
        }

        private void _simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            _working = false;
        }

        private void _simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            throw new Exception(data.dwException.ToString());
        }

        private void SimConnectThreadProc()
        {
            Thread t = new Thread(new ThreadStart(ProcessData));
            try
            {
                _dataQueue.Clear();
                t.Start();
                while (_working)
                {
                    _simConnectEvent.WaitOne();
                    //System.Diagnostics.Debug.WriteLine("Sygnał od SimConnect.");
                    _simconnect.ReceiveMessage();
                }
            }
            catch (Exception)
            { }
            finally
            {
                t.Abort();
            }
        }

        private Queue<SIMCONNECT_RECV_SIMOBJECT_DATA> _dataQueue = new Queue<SIMCONNECT_RECV_SIMOBJECT_DATA>();

        private void ProcessData()
        {
            try
            {
                List<SIMCONNECT_RECV_SIMOBJECT_DATA> data = new List<SIMCONNECT_RECV_SIMOBJECT_DATA>();
                while (_working)
                {
                    _simConnectProcessEvent.WaitOne();
                    _simConnectProcessEvent.Reset();
                    lock (_dataQueue)
                    {
                        while (_dataQueue.Count > 0)
                        {
                            data.Add(_dataQueue.Dequeue());
                        }
                    }
                    while (data.Count > 0)
                    {
                        SIMCONNECT_RECV_SIMOBJECT_DATA d = data[0];
                        data.RemoveAt(0);
                    }
                }
            }
            catch (ThreadAbortException)
            { }
        }

        private void _simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            _simConnectEvent.Reset();
            lock (_dataQueue)
            {
                _dataQueue.Enqueue(data);
            }
            _simConnectProcessEvent.Set();
        }
    }
}
