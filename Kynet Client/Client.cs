using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;
using KynetLib;

namespace KynetClient
{
    [CallbackBehavior(IncludeExceptionDetailInFaults = true)]
    class Client : CallbackContractFunctions
    {
        public DuplexChannelFactory<IContract> serviceClient;
        public static ChannelFactory<IFileTransferContract> FileserviceClient;
        public static IContract channel;

        public Client()
        {
            try
            {
                NetTcpBinding clientTcpBinding = new NetTcpBinding(SecurityMode.None);
                NetTcpBinding fileTcpbinding = new NetTcpBinding(SecurityMode.None);
                Settings.SetConfigs(fileTcpbinding, clientTcpBinding);

                serviceClient = new DuplexChannelFactory<IContract>(
                   new InstanceContext(this),
                   clientTcpBinding,
                   new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}", Settings.Address, Settings.Port, Settings.ServiceName)));

                FileserviceClient = new ChannelFactory<IFileTransferContract>(
                   fileTcpbinding,
                   new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}", Settings.Address, Settings.Port, Settings.FileServiceName)));
            }
            catch(Exception ex)
            {
                Diagnostics.WriteError(ex);
            }
        }

        public void Connect()
        {
            channel = serviceClient.CreateChannel();
            System.SystemInfo.Fingerprint = channel.Connect(System.SystemInfo);
            Diagnostics.WriteLog("Succesfully connected");
        }     
    }
}
