using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KynetLib
{
    public class Settings
    {
        public static string Address = "92.109.120.224";
        public static string Port = "20523";
        public static string ServiceName = "Kynet";
        public static string FileServiceName = "Kynet_Files";

        //Fileservice endpoint settings
        public static int MaxBufferSize = int.MaxValue;
        public static int MaxReceivedMessageSize = int.MaxValue;
        public static TimeSpan SendTimeout = TimeSpan.MaxValue;
        public static TimeSpan ReceiveTimeout = TimeSpan.MaxValue;
    }
}
