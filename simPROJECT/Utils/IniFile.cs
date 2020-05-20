using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace simPROJECT.Utils
{
    class IniFile
    {
        public string Path
        {
            get;
            private set;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string INIPath)
        {
            Path = INIPath;
        }

        public void WriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, Path);
        }

        public string ReadValue(string section, string key, string def)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, def, temp, 255, Path);
            return temp.ToString();
        }
    }
}
