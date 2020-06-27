using System.Management;

namespace CodeBase
{
    public class Hardware
    {
        public static string GetID()
        {
            string result = "";

            using (var bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
            {
                var bios_Collection = bios.Get();
                foreach (var obj in bios_Collection)
                {
                    result += obj["SerialNumber"].ToString();
                    break; //break just to get the first found object data only
                }
            }

            return result;
        }
    }
}
