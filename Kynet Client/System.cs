using KynetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KynetClient
{
    public class System
    {
        private static ClientSystemInfo _SystemInfo;

        public static ClientSystemInfo SystemInfo
        {
            get
            {
                if (_SystemInfo == null)
                    UpdateSystemInfo();

                return _SystemInfo;
            }
            set { _SystemInfo = value; }
        }

        public static void UpdateSystemInfo()
        {
            _SystemInfo = new ClientSystemInfo()
            {
                Username = Inspector.GetUsername(),
                Machinename = Inspector.GetMachineName(),
                Domain = Inspector.GetDomain(),
                OperatingSystem = Inspector.GetOperatingSystem(),
                Country = Inspector.GetCountry(),
                SystemType = Inspector.GetSystemType(),
                MonitorCount = Inspector.GetMonitorCount(),
                GPU = Inspector.GetGPU(),
                CPU = Inspector.GetCPU(),
                RAM = Inspector.GetRAM(),
                x64_Bit = Inspector.GetOSType(),
                CPUID = Inspector.GetCPUID(),
                DRIVEID = Inspector.GetDriveID()
            };
        }
    }
}
