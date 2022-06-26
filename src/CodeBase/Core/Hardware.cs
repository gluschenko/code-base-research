using System.Management;

namespace CodeBase.Core
{
    public class Hardware
    {
        public static string GetID()
        {
            var result = "";

            using var bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");

            var biosCollection = bios.Get();
            foreach (var item in biosCollection)
            {
                result += item["SerialNumber"].ToString();
                break;
            }

            return result;
        }
    }
}
