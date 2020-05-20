using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace simPROJECT
{
    static class Program
    {
        internal static readonly string APP_NAME = "simPROJECT";
        internal static string APP_VERSION = "0.2";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // wczytanie wersji Assemlby
            APP_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Debug.Listeners.Add(new TextWriterTraceListener("DEBUG.log"));
            Debug.AutoFlush = true;
            Debug.WriteLine(string.Format("{0}: Start aplikacji {1} {2}", DateTime.Now, APP_NAME, APP_VERSION));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            Application.AddMessageFilter(new Mess());



            __app = new AppService();
            __app.Run();
            Application.Run();
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (__app != null)
            {
                __app.Exit();
            }
        }

        private static AppService __app = null;

        private static void Dispose()
        {
            try
            {
                if (__app != null)
                {
                    __app.Dispose();
                    __app = null;
                }
            }
            catch { }
        }

        private class Mess : IMessageFilter
        {

            #region IMessageFilter Members

            public bool PreFilterMessage(ref Message m)
            {
                // reagowanie na komunikat o zamykaniu windows/wylogowaniu użytkownika
                // END_SESSION
                // http://msdn.microsoft.com/en-us/library/aa376889%28v=vs.85%29.aspx

                //MessageBox.Show("MESS");

                if (m.Msg == /* WM_QUERYENDSESSION */ 0x11)
                {
                    try
                    {
                        Program.__app.Exit();
                        Application.Exit();
                    }
                    catch
                    {
                    }
                }
                return false;
            }

            #endregion
        }
    }
}