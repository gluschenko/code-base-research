using System;
using System.Threading.Tasks;
using System.Management;

namespace CodeBase
{
    public class Hardware
    {
        public static async Task<string> GetID()
        {
            string s = "";
            Task task = Task.Run(() =>
            {
                var bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                var bios_Collection = bios.Get();
                foreach (var obj in bios_Collection)
                {
                    s += obj["SerialNumber"].ToString();
                    break; //break just to get the first found object data only
                }
            });
            Task.WaitAll(task);

            return await Task.FromResult(s);
        }
    }
}
