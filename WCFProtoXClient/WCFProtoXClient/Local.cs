using KynetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KynetClient
{
    public class Local
    {
        private static UserClient _UserInfo;

        public static UserClient UserInfo
        {
            get
            {
                if (_UserInfo == null)
                    UpdateUserInfo();

                return _UserInfo;
            }
            set { _UserInfo = value; }
        }


        public static void UpdateUserInfo()
        {
            _UserInfo = new UserClient()
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
                Fingerprint = Inspector.GetFingerprint()
            };

        }
    }
}
