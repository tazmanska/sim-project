using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace simPROJECT
{
    class AppTrayIcon : IDisposable
    {
        private NotifyIcon tray = null;

        public AppTrayIcon(IContainer container, ContextMenu menu)
        {
            tray = new NotifyIcon(container)
            {
                ContextMenu = menu,
                Icon = global::simPROJECT.Properties.Resources.icon,
                Text = string.Format("{0} {1}", Program.APP_NAME, Program.APP_VERSION)
            };
        }

        public bool Visible
        {
            get { return tray.Visible; }
            set { tray.Visible = value; }
        }

        public void ShowInfo(string info)
        {
            tray.ShowBalloonTip(3000, "Information", info, ToolTipIcon.Info);
        }

        #region IDisposable Members

        public void Dispose()
        {
            tray.Dispose();
        }

        #endregion
    }
}
