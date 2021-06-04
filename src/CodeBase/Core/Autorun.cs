using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace CodeBase
{
    public class Autorun
    {
        const string RegSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\";

        private string AppName => nameof(CodeBase);
        private string AppPath => Process.GetCurrentProcess().MainModule.FileName;

        public RegistryKey GetSubKey()
        {
            //var permission = new RegistryPermission(RegistryPermissionAccess.AllAccess, RegSubKey);

            return Registry.CurrentUser.OpenSubKey(RegSubKey, true);
        }

        public bool GetState()
        {
            var subkey = GetSubKey();
            if (subkey != null)
            {
                var val = (string)subkey.GetValue(AppName, "");
                subkey.Close();
                return val != "";
            }

            return false;
        }

        public bool SetState(bool state, Action<Exception> onError)
        {
            try
            {
                var subkey = GetSubKey();
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
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }

            return GetState();
        }
    }
}
