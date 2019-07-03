using System;
using System.IO;
using Microsoft.Win32;
using System.Security.Permissions;

namespace CodeBase
{
    public class Autorun
    {
        const string RegSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\";

        private string AppName {
            get {
                return Path.GetFileNameWithoutExtension(AppPath).ToUpper();
            }
        }

        private string AppPath {
            get {
                return System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
        }

        public RegistryKey GetSubKey()
        {
            RegistryPermission Permission = new RegistryPermission(RegistryPermissionAccess.AllAccess, RegSubKey);
            
            return Registry.CurrentUser.OpenSubKey(RegSubKey, true);
        }

        public bool GetState()
        {
            RegistryKey subkey = GetSubKey();
            if (subkey != null)
            {
                string val = (string)subkey.GetValue(AppName, "");
                subkey.Close();
                return val != "";
            }

            return false;
        }

        public bool SetState(bool state, Action<Exception> onError)
        {
            try
            {
                RegistryKey subkey = GetSubKey();
                if (state)
                {
                    subkey.SetValue(AppName, AppPath);
                    subkey.Close();
                }
                else
                {
                    subkey.DeleteValue(AppName, false);
                    subkey.Close();
                }
            }
            catch(Exception ex) {
                onError?.Invoke(ex);
            }

            return GetState();
        }
    }
}
