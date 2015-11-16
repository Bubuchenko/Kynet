using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KynetLib
{
    public class Settings
    {
        //        public static string Address = "92.109.120.224";
        public static string Address = "localhost";
        public static string Port = "20523";
        public static string ServiceName = "Kynet";
        public static string FileServiceName = "Kynet_Files";

        //Endpoint settings
        public static int MaxBufferSize = int.MaxValue;
        public static int MaxReceivedMessageSize = int.MaxValue;
        public static TimeSpan SendTimeout = TimeSpan.FromSeconds(60);
        public static TimeSpan ReceiveTimeout = TimeSpan.MaxValue;
        public static TimeSpan CloseTimeout = TimeSpan.FromSeconds(30);
        public static int MaxConnections = 10;
        public static TimeSpan OpenTimeout = TimeSpan.FromSeconds(30);

        public static void SetConfigs(NetTcpBinding fileTcpbinding, NetTcpBinding clientTcpBinding)
        {
            fileTcpbinding.CloseTimeout = CloseTimeout;
            fileTcpbinding.MaxBufferSize = MaxBufferSize;
            //fileTcpbinding.MaxConnections = MaxConnections;
            fileTcpbinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
            fileTcpbinding.OpenTimeout = OpenTimeout;
            fileTcpbinding.ReceiveTimeout = ReceiveTimeout;
            fileTcpbinding.SendTimeout = SendTimeout;
            fileTcpbinding.TransferMode = TransferMode.Streamed;

            clientTcpBinding.CloseTimeout = CloseTimeout;
            clientTcpBinding.MaxBufferSize = MaxBufferSize;
            //clientTcpBinding.MaxConnections = MaxConnections;
            clientTcpBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
            clientTcpBinding.OpenTimeout = OpenTimeout;
            clientTcpBinding.ReceiveTimeout = ReceiveTimeout;
            clientTcpBinding.SendTimeout = SendTimeout;
        }
    }
}
