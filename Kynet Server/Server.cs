using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using KynetLib;

namespace KynetServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    class Server : ContractFunctions
    {
        public static List<UserClient> ConnectedClients = new List<UserClient>();
        public static List<FileTransfer> FileTransfers = new List<FileTransfer>();

        public ServiceHost serviceHost =
            new ServiceHost(typeof(Server),
            new Uri(string.Format("net.tcp://{0}:{1}", "localhost", Settings.Port)));

        public Server()
        {
            NetTcpBinding clientTcpBinding = new NetTcpBinding(SecurityMode.None);
            NetTcpBinding fileTcpbinding = new NetTcpBinding(SecurityMode.None);
            Settings.SetConfigs(fileTcpbinding, clientTcpBinding);


            serviceHost.AddServiceEndpoint(typeof(IFileTransferContract), fileTcpbinding, Settings.FileServiceName);
            serviceHost.AddServiceEndpoint(typeof(IContract), clientTcpBinding, Settings.ServiceName);
        }


        public void Open()
        {
            try
            {
                serviceHost.Open();
                Console.WriteLine("Successfullly opened on port {0}", Settings.Port);
            }
            catch (Exception ex)
            {
                Diagnostics.WriteError(ex);
            }
        }
    }
}
