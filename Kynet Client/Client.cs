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
    class Client : CallbackContractFunctions
    {
        public DuplexChannelFactory<IContract> serviceClient;
        public static ChannelFactory<IFileTransferContract> FileserviceClient;
        public IContract channel;

        public Client()
        {
            try
            {
                serviceClient = new DuplexChannelFactory<IContract>(
                   new InstanceContext(this),
                   new NetTcpBinding(SecurityMode.None),
                   new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}", Settings.Address, Settings.Port, Settings.ServiceName)));

                NetTcpBinding fileTcp = new NetTcpBinding(SecurityMode.None);
                fileTcp.TransferMode = TransferMode.Streamed;
                fileTcp.MaxBufferSize = Settings.MaxBufferSize;
                fileTcp.MaxReceivedMessageSize = Settings.MaxReceivedMessageSize;
                fileTcp.ReceiveTimeout = Settings.ReceiveTimeout;
                fileTcp.SendTimeout = Settings.SendTimeout;

                FileserviceClient = new ChannelFactory<IFileTransferContract>(
                   fileTcp,
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
            channel.Connect(Local.UserInfo);
            Diagnostics.WriteLog("Succesfully connected");
        }     
    }
}
